﻿namespace Umbraco9ContentApi.Core.Services.FutureNhs
{
    using System;
    using Umbraco.Cms.Core.Services;
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;

    public class FutureNhsValidationService : IFutureNhsValidationService
    {
        private readonly IContentTypeService _contentTypeService;

        public FutureNhsValidationService(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
        }

        public void ValidateContentModelData(ContentModelData contentModel)
        {
            var contentType = _contentTypeService.Get(contentModel.Item?.ContentType);

            if (contentType is null)
                throw new KeyNotFoundException($"No content type for {contentModel.Item?.ContentType} found.");

            var expectedValues = contentType.PropertyTypes.Select(x => x.Alias).ToList();
            var blockValues = contentModel.Content?.Select(x => x.Key.ToString()).Where(x => x != "blocks").ToList(); // ignore 'blocks' as they're not set a contentType properties but are used to identify contentModel children in Umbraco.

            if (blockValues is not null)
            {
                foreach (var value in blockValues)
                {
                    if (!expectedValues.Contains(value))
                        throw new ArgumentOutOfRangeException($"{contentType.Name}");
                }
            }
        }
    }
}
