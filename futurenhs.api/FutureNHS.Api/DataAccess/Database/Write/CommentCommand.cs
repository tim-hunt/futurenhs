﻿using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public sealed class CommentCommand : ICommentCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<CommentCommand> _logger;

        public CommentCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<CommentCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommentData> GetCommentAsync(Guid commentId, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(CommentData.Id)}]                  = comment.Id,
                                [{nameof(CommentData.Content)}]             = comment.Content,          
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(comment.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = comment.CreatedBy,
                                [{nameof(CommentData.RowVersion)}]          = comment.RowVersion

                    FROM            Entity_Comment comment	
					WHERE           comment.Id = @commentId
                    AND             comment.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var commentData = await dbConnection.QuerySingleAsync<CommentData>(query, new
            {
                commentId
            });

            if (commentData is null)
            {
                _logger.LogError($"Not Found: Comment:{0} not found", commentId);
                throw new NotFoundException("Not Found: Comment not found");
            }

            return commentData;
        }

        public async Task CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken)
        {
            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            using (var connection = new SqlConnection(dbConnection.ConnectionString))
            {
                const string insertComment =
                @"  
	                INSERT INTO  [dbo].[Entity_Comment]
                                 ([Content]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy]
                                 ,[ModifiedAtUTC]
                                 ,[FlaggedAsSpam]
                                 ,[InReplyTo]
                                 ,[Entity_Id]
                                 ,[ThreadId]
                                 ,[IsDeleted])
                    VALUES
                                 (@Content
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy
                                 ,@ModifiedAtUTC
                                 ,@FlaggedAsSpam
                                 ,@InReplyTo
                                 ,@EntityId
                                 ,@ThreadId
                                 ,@IsDeleted)
	              ";

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommentResult = connection.Execute(insertComment, new
                    {
                        Content = comment.Content,
                        CreatedBy = comment.CreatedBy,
                        CreatedAtUTC = comment.CreatedAtUTC,
                        ModifiedBy = comment.ModifiedBy,
                        ModifiedAtUTC = comment.ModifiedAtUTC,
                        FlaggedAsSpam = comment.FlaggedAsSpam,
                        InReplyTo = comment.InReplyTo,
                        EntityId = comment.EntityId,
                        ThreadId = comment.ThreadId,
                        IsDeleted = comment.IsDeleted,
                    }, transaction: transaction);

                    if (insertCommentResult != 1)
                    {
                        _logger.LogError("Error: User request to create was not successful.", insertComment);
                        throw new DataException("Error: User request to create was not successful.");

                    }

                    transaction.Commit();
                }
            }
        }

        public async Task UpdateCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =

                @"  
                    BEGIN TRAN
                    BEGIN TRY

	                UPDATE        [dbo].[Entity_Comment]
                    SET 
                                  [Content] = @Content
                                 ,[ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                    
                    WHERE 
                                 [Id] = @CommentId
                    AND          [RowVersion] = @RowVersion


                    COMMIT TRAN;

                    END TRY
                    BEGIN CATCH
	                    PRINT ERROR_MESSAGE();
	                    ROLLBACK TRAN;
                    END CATCH";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = comment.Id,
                Content = comment.Content,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to edit a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to edit a comment was not successful");
            }
        }

        public async Task DeleteCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            const string query =
                @"  
                    BEGIN TRAN
                    BEGIN TRY

	                UPDATE        [dbo].[Entity_Comment]
                    SET                                   
                                  [ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                                 ,[IsDeleted] = 1
                    
                    WHERE 
                                 [Id] = @CommentId
                    AND          [RowVersion] = @RowVersion


                    COMMIT TRAN;

                    END TRY
                    BEGIN CATCH
	                    PRINT ERROR_MESSAGE();
	                    ROLLBACK TRAN;
                    END CATCH";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = comment.Id,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to delete a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to delete a comment was not successful");
            }
        }

        public async Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken)
        {
            // Work up the chain using the InReplyTo column to find the original comment to find the threadId
            const string query =
                @" WITH            Comments AS 
                    (
                    SELECT      
                                    Id,
                                    InReplyTo

                    FROM            Entity_Comment
                    WHERE           Entity_Id = @CommentId
                    UNION ALL
                    SELECT
                                    comment.Entity_Id AS PK,
                                    comment.InReplyTo AS ParentFK

                    FROM            Entity_Comment comment
                    INNER JOIN      Comments comments 
                    ON              Comments.InReplyTo = comment.Entity_Id)

                    SELECT
                                    Entity_Id  
                    FROM            Comments 
                    WHERE           InReplyTo 
                    IS              NULL;";

            var queryDefinition = new CommandDefinition(query, new
            {
                Entity_Id = commentId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstOrDefaultAsync<Guid?>(queryDefinition);

            return result;
        }
    }
}
