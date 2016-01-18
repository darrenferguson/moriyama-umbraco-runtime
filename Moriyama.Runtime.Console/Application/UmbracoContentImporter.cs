using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Content;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Application.Media;
using Moriyama.Content.Export.Application.Parser;
using Moriyama.Content.Export.Interfaces;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Moriyama.Content.Export.Application
{
    public class UmbracoContentImporter : IUmbracoContentImporter
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IUmbracoPropertySetter _umbracoPropertySetter;

        public UmbracoContentImporter(ApplicationContext applicationContext, IUmbracoPropertySetter umbracoPropertySetter)
        {
            _applicationContext = applicationContext;
            _umbracoPropertySetter = umbracoPropertySetter;
        }

        public IEnumerable<ContentCreateResult> Import(IFileSystem fileSystem)
        {


            var allContent = new UmbracoContentFinder(_applicationContext.Services.ContentService).FindAllContent().ToArray();
            var contentFactory = new ExportableContentFactory();
            var exportable = contentFactory.GetExportableContent(allContent).ToArray();

            var allExportedContent = new List<ExportContentModel>();


            var allMedia = new UmbracoMediaFinder(_applicationContext.Services.MediaService).FindAllContent().ToArray();
            var exportableMedia = new ExportableMediaFactory().GetExportableContent(allMedia).ToArray();

            var creator = new UmbracoContentCreator(exportable, _applicationContext.Services.ContentService, contentFactory);

            foreach (var file in fileSystem.List())
            {
                var item = JsonConvert.DeserializeObject<ExportContentModel>(System.IO.File.ReadAllText(file));
                allExportedContent.Add(item);
            }

            var status = new List<ContentCreateResult>();

            // Pass 1 - Create the content
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
            {
                status.Add(creator.Create(contentItem));
            }

            exportable = creator.GetAllContent().ToArray();

            var parsers = new List<IExportContentParser>
            {
                new NullValueExportContentParser(),
                new CommaDelimitedIntExportContentParser(exportable, exportableMedia),
                new IntExportContentParser(exportable,exportableMedia),
                new LocalLinkExportContentParser(exportable,exportableMedia)
            };

            // Pass 2 - Set the properties
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder)) { 
                var content = creator.Create(contentItem);

                if (content.Content != null)
                {
                    var setContent = _umbracoPropertySetter.SetProperties(content.Content, contentItem, parsers);
                    _applicationContext.Services.ContentService.Save(setContent);
                }
            }

            // Pass 3 - Publishing
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
            {
                if (contentItem.Published)
                {
                    var content = creator.Create(contentItem);

                    if(content.Content != null)
                        _applicationContext.Services.ContentService.Publish(content.Content);
                }
            }

            return status;
        }
    }
}
