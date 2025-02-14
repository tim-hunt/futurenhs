﻿using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface ICommentCommand
    {
        Task<CommentData> GetCommentAsync(Guid commentId, CancellationToken cancellationToken);
        Task<Guid> CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken = default);
        Task UpdateCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task DeleteCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken);
    }
}
