﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco9ContentApi.Core.Models.Requests;
using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
using UmbracoContentApi.Core.Builder;
using ContentModelData = Umbraco9ContentApi.Core.Models.Content.ContentModelData;

namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    public class FutureNhsBlockService : IFutureNhsBlockService
    {
        private readonly IConfiguration _config;
        private readonly ConverterCollection _converters;
        private readonly IFutureNhsContentService _futureNhsContentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;

        public FutureNhsBlockService(IConfiguration config, ConverterCollection converters, IFutureNhsContentService futureNhsContentService, IContentTypeService contentTypeService, IContentService contentService)
        {
            _config = config;
            _converters = converters;
            _futureNhsContentService = futureNhsContentService;
            _contentTypeService = contentTypeService;
            _contentService = contentService;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken)
        {
            var block = _futureNhsContentService.GetPublishedContent(blockId, cancellationToken);
            var contentType = _contentTypeService.Get(block.ContentType.Alias);
            var propertyGroup = contentType.PropertyGroups.FirstOrDefault(x => x.Alias == propertyGroupAlias);
            var groupProperties = propertyGroup?.PropertyTypes.Select(x => x.Alias);

            return (from property in block.Properties.Where(x => groupProperties != null && groupProperties.Contains(x.Alias))
                    let converter = _converters.FirstOrDefault(x => x.EditorAlias.Equals(property.PropertyType.EditorAlias))
                    where converter != null
                    select property.GetValue().ToString()).ToList();
        }

        /// <inheritdoc />
        public IContent CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken)
        {
            // Get parent content
            var parentContent = _contentService.GetById(createRequest.parentId);
            // Create new block as child of parent content.
            // Use a random Guid to generate temporary name
            var content = _contentService.Create(createRequest.ContentType + $":{Guid.NewGuid()}", parentContent.Id, createRequest.ContentType);
            // Once the content item has been created, update the name by replacing the random Guid with the Guid generated by umbraco
            content.Name = createRequest.ContentType + $":{content.Key}";
            _contentService.SaveAndPublish(content);
            return content;
        }

        /// <inheritdoc />
        public IContent UpdateBlock(ContentModelData block, int sortOrder, CancellationToken cancellationToken)
        {
            // Get block draft to update
            var blockToUpdate = _futureNhsContentService.GetDraftContent(block.Item.Id, cancellationToken);

            // Get block content type
            var contentType = _contentTypeService.Get(block.Item?.ContentType);

            // Apply sort order
            blockToUpdate.SortOrder = sortOrder;

            // Loop through the content type properties
            foreach (var property in contentType.PropertyTypes)
            {
                // Find the new value for the property we're updating
                var updateValue = block.Content.Where(p => p.Key == property.Alias).Select(x => x.Value).FirstOrDefault();
                blockToUpdate = _futureNhsContentService.SetContentPropertyValue(blockToUpdate, property.Alias, updateValue, cancellationToken);
            }

            _futureNhsContentService.SaveContent(blockToUpdate, cancellationToken);
            return _contentService.GetById(block.Item.Id);
        }

        /// <inheritdoc />
        public IEnumerable<ContentModelData> GetAllDescendentBlockIds(IEnumerable<ContentModelData> blocks, CancellationToken cancellationToken)
        {
            List<ContentModelData> contentModels = new();

            // Add context blocks
            contentModels.AddRange(blocks);

            // Add all block children if applicable
            foreach (var child in blocks.Where(x => x.Content != null))
            {
                foreach (var content in child.Content.Where(x => x.Key == "blocks"))
                {
                    var childBlockObjects = JsonConvert.SerializeObject(content.Value);
                    var childBlocks = JsonConvert.DeserializeObject<List<ContentModelData>>(childBlockObjects);
                    if (childBlocks is not null && childBlocks.Any())
                    {
                        contentModels.AddRange(childBlocks.Where(x => x.Item != null));
                    }
                }
            }

            return contentModels;
        }
    }
}
