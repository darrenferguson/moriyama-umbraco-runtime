using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Content;
using Moriyama.Content.Export.Application.Media;
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

        public void ExportContext(string path)
        {
            var exportableContent = new ExportableContentFactory().GetExportableContent(new UmbracoContentFinder(_applicationContext.Services.ContentService).FindAllContent().ToArray()).ToArray();
            var exportableMedia = new ExportableMediaFactory().GetExportableContent(new UmbracoMediaFinder(_applicationContext.Services.MediaService).FindAllContent().ToArray()).ToArray();

            var parsers = new List<IExportContentParser>
            {
                new NullValueExportContentParser(),
                new CommaDelimitedIntExportContentParser(exportableContent, exportableMedia),
                new IntExportContentParser(exportableContent, exportableMedia),
                new LocalLinkExportContentParser(exportableContent, exportableMedia)
            };

            var contentFileSystem = new FileSystem(Path.Combine(path, "content"));
            var contentExportSerialiser = new UmbracoContentExportSerialiser(parsers);
            foreach (var export in exportableContent)
            {
                var json = JsonConvert.SerializeObject(contentExportSerialiser.Serialise(export), Formatting.Indented);
                contentFileSystem.Write(export.Path + ".json", json);
            }

            var mediaFileSystem = new FileSystem(Path.Combine(path, "media"));
            var mediaExportSerialiser = new UmbracoMediaExportSerialiser(parsers);
            foreach (var export in exportableMedia)
            {
                var json = JsonConvert.SerializeObject(mediaExportSerialiser.Serialise(export), Formatting.Indented);
                mediaFileSystem.Write(export.Path + ".json", json);
            }
        }
    }
}
