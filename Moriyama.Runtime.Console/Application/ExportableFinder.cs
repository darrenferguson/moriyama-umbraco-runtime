using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Services;

namespace Moriyama.Content.Export.Application
{
    public class ExportableFinder : IExportableFinder
    {
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;

        public ExportableFinder(IContentService contentService, IMediaService mediaService)
        {
            _contentService = contentService;
            _mediaService = mediaService;
        }

        public IEnumerable<IExportable> FindExportable()
        {
            var umbracoEntities = new List<IUmbracoEntity>();

            var rootContent = _contentService.GetRootContent();
            var rootMedia = _mediaService.GetRootMedia();

            umbracoEntities.AddRange(FindContent(rootContent));
            umbracoEntities.AddRange(FindContent(rootMedia));

            var exportables = new List<IExportable>();

            foreach (var umbracoEntity in umbracoEntities)
            {
                var exportable = new Exportable
                {
                    Entity = umbracoEntity,
                    Type = umbracoEntity is IContent ? ExportableType.Content : ExportableType.Media
                };
                exportables.Add(exportable);

            }

            return exportables;
        }

        private IEnumerable<IUmbracoEntity> FindContent(IEnumerable<IUmbracoEntity> contents)
        {
            var allContent = new List<IUmbracoEntity>();

            foreach (var content in contents)
            {
                allContent.Add(content);

                if(content is IContent)
                    allContent.AddRange(FindContent(((IContent)content).Children()));

                if (content is IMedia)
                    allContent.AddRange(FindContent(((IMedia)content).Children()));

            }
            return allContent;
        }
    }
}
