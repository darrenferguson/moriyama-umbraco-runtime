using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Parser;
using Moriyama.Content.Export.Interfaces;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Moriyama.Content.Export.Application
{
    public class UmbracoContentExporter : IUmbracoContentExporter
    {
        private readonly ApplicationContext _applicationContext;

        public UmbracoContentExporter(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void ExportContext(IFileSystem fileSystem)
        {
            var contentService = _applicationContext.Services.ContentService;
            
            var finder = new UmbracoContentFinder(contentService);

            var allContent = finder.FindAllContent().ToArray();
            var contentFactory = new ExportableContentFactory();

            var exportable = contentFactory.GetExportableContent(allContent).ToArray();

            var parsers = new List<IExportContentParser>
            {
                new NullValueExportContentParser(),
                new CommaDelimitedIntExportContentParser(exportable),
                new IntExportContentParser(exportable),
                new LocalLinkExportContentParser(exportable)
            };

            var serialiser = new UmbracoContentExportSerialiser(parsers);
                     
            foreach (var export in exportable)
            {
                var json = JsonConvert.SerializeObject(serialiser.Serialise(export), Formatting.Indented);
                fileSystem.Write(export.Path + ".json", json);
            }
        }
    }
}
