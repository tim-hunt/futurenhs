﻿namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Umbraco9ContentApi.Core.Services.FutureNhs.Interface;
    using UmbracoContentApi.Core.Models;


    /// <summary>
    /// The handler that handles template methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsTemplateHandler" />
    public sealed class FutureNhsTemplateHandler : IFutureNhsTemplateHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IFutureNhsTemplateHandler"/> class.
        /// </summary>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        public FutureNhsTemplateHandler(IFutureNhsContentService futureNhsContentService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<ContentModel> GetTemplateAsync(Guid id)
        {
            var template = await _futureNhsContentService.GetPublishedAsync(id);
            return await _futureNhsContentService.ResolveAsync(template);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetAllTemplatesAsync()
        {
            var contentModels = new List<ContentModel>();
            var templatesFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Templates");
            var templates = await _futureNhsContentService.GetPublishedChildrenAsync(templatesFolderGuid);

            foreach (var template in templates)
            {
                contentModels.Add(await _futureNhsContentService.ResolveAsync(template));
            }

            return contentModels;
        }
    }
}