using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
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

            var finder = new UmbracoContentFinder(_applicationContext.Services.ContentService);

            var allContent = finder.FindAllContent().ToArray();
            var contentFactory = new ExportableContentFactory();

            var exportable = contentFactory.GetExportableContent(allContent).ToArray();

           
            var allExportedContent = new List<ExportContentModel>();
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
                new CommaDelimitedIntExportContentParser(exportable),
                new IntExportContentParser(exportable),
                new LocalLinkExportContentParser(exportable)
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
