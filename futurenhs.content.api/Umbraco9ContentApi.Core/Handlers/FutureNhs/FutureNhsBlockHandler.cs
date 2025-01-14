﻿namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Requests;
    using Umbraco9ContentApi.Core.Models.Response;

    /// <summary>
    /// The handler that handles block methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsBlockHandler" />
    public sealed class FutureNhsBlockHandler : IFutureNhsBlockHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IFutureNhsBlockService _futureNhsBlockService;

        public FutureNhsBlockHandler(IConfiguration config, IFutureNhsContentService futureNhsContentService, IFutureNhsBlockService futureNhsBlockService)
        {
            _config = config;
            _futureNhsContentService = futureNhsContentService;
            _futureNhsBlockService = futureNhsBlockService;
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<ContentModelData>> GetAllBlocks(CancellationToken cancellationToken)
        {
            var blocksFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:PlaceholderBlocks");

            if (blocksFolderGuid == Guid.Empty)
            {
                throw new NullReferenceException($"Placeholder blocks folder guid cannot be null.");
            };

            var placeholderBlocksFolder = _futureNhsContentService.GetPublishedContent(blocksFolderGuid, cancellationToken);

            var contentModelList = new List<ContentModelData>();

            if (placeholderBlocksFolder.Children is not null && placeholderBlocksFolder.Children.Any())
            {
                foreach (var block in placeholderBlocksFolder.Children)
                {
                    contentModelList.Add(_futureNhsContentService.ResolvePublishedContent(block));
                }
            }

            return new ApiResponse<IEnumerable<ContentModelData>>().Success(contentModelList, "Success.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<string?>> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            var blockPlaceholderValues = _futureNhsBlockService.GetBlockPlaceholderValues(blockId, propertyGroupAlias, cancellationToken);
            return new ApiResponse<IEnumerable<string?>>().Success(blockPlaceholderValues, "Success.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<string>> GetBlockContent(Guid blockId, CancellationToken cancellationToken)
        {
            var block = _futureNhsContentService.GetPublishedContent(blockId, cancellationToken);
            return new ApiResponse<IEnumerable<string>>().Success(_futureNhsContentService.ResolvePublishedContent(block).Content.Keys, "Retrieved block content successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<IEnumerable<string>> GetBlockLabels(Guid blockId, CancellationToken cancellationToken)
        {
            var block = _futureNhsContentService.GetPublishedContent(blockId, cancellationToken);
            return new ApiResponse<IEnumerable<string>>().Success(_futureNhsContentService.ResolvePublishedContent(block, "labels").Content.Keys, "Retrieved block labels successfully.");
        }

        /// <inheritdoc />
        public ApiResponse<string> CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken)
        {
            var createdBlock = _futureNhsBlockService.CreateBlock(createRequest, cancellationToken);
            return new ApiResponse<string>().Success(createdBlock.Key.ToString(), "Block successfully created.");
        }
    }
}

