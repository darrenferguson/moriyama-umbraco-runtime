using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Content;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Application.Media;
using Moriyama.Content.Export.Application.Parser;
using Moriyama.Content.Export.Interfaces;
using Moriyama.Content.Export.Interfaces.Content;
using Moriyama.Content.Export.Interfaces.Media;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Moriyama.Content.Export.Application
{
    public class UmbracoContentImporter : IUmbracoContentImporter
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IUmbracoContentPropertySetter _umbracoContentPropertySetter;
        private readonly IUmbracoMediaPropertySetter _umbracoMediaPropertySetter;

        public UmbracoContentImporter(ApplicationContext applicationContext, IUmbracoContentPropertySetter umbracoContentPropertySetter, IUmbracoMediaPropertySetter umbracoMediaPropertySetter)
        {
            _applicationContext = applicationContext;
            _umbracoContentPropertySetter = umbracoContentPropertySetter;
            _umbracoMediaPropertySetter = umbracoMediaPropertySetter;
        }

        public IEnumerable<ContentCreateResult> Import(string path)
        {
            var contentFactory = new ExportableContentFactory();
            var mediaFactory = new ExportableMediaFactory();

            var exportableContent = contentFactory.GetExportableContent(new UmbracoContentFinder(_applicationContext.Services.ContentService).FindAllContent().ToArray()).ToArray();
            var exportableMedia = mediaFactory.GetExportableContent(new UmbracoMediaFinder(_applicationContext.Services.MediaService).FindAllContent().ToArray()).ToArray();

            var allExportedContent = new List<ExportContentModel>();
            var allExportedMedia = new List<ExportMediaModel>();
  
            var contentFileSystem = new FileSystem(Path.Combine(path, "content"));
            var mediaFileSystem = new FileSystem(Path.Combine(path, "media"));
            
            foreach (var file in contentFileSystem.List())
            {
                var item = JsonConvert.DeserializeObject<ExportContentModel>(File.ReadAllText(file));
                allExportedContent.Add(item);
            }

            foreach (var file in mediaFileSystem.List())
            {
                var item = JsonConvert.DeserializeObject<ExportMediaModel>(File.ReadAllText(file));
                allExportedMedia.Add(item);
            }

            var status = new List<ContentCreateResult>();
            var contentCreator = new UmbracoContentCreator(exportableContent, _applicationContext.Services.ContentService, contentFactory);
            var mediaCreator = new UmbracoMediaCreator(exportableMedia, _applicationContext.Services.MediaService, mediaFactory);

            // Pass 1 - Create the media
            foreach (var contentItem in allExportedMedia.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
                mediaCreator.Create(contentItem);
                       
            // Pass 1 - Create the content
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
                status.Add(contentCreator.Create(contentItem));

            exportableContent = contentCreator.GetAllContent().ToArray();
            exportableMedia = mediaCreator.GetAllContent().ToArray();

            var parsers = new List<IExportContentParser>
            {
                new NullValueExportContentParser(),
                new CommaDelimitedIntExportContentParser(exportableContent, exportableMedia),
                new IntExportContentParser(exportableContent,exportableMedia),
                new LocalLinkExportContentParser(exportableContent,exportableMedia)
            };

            // Pass 2 media properties.
            foreach (var contentItem in allExportedMedia.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
            {
                var content = mediaCreator.Create(contentItem);

                if (content.Media != null)
                {
                    var setContent = _umbracoMediaPropertySetter.SetProperties(content.Media, contentItem, parsers);
                    _applicationContext.Services.MediaService.Save(setContent);

                }
            }

            // Pass 2 - Set the  content properties
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder)) { 
                var content = contentCreator.Create(contentItem);

                if (content.Content != null)
                {
                    var setContent = _umbracoContentPropertySetter.SetProperties(content.Content, contentItem, parsers);
                    _applicationContext.Services.ContentService.Save(setContent);
                }
            }

            // Pass 3 - Publishing
            foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
            {
                if (contentItem.Published)
                {
                    var content = contentCreator.Create(contentItem);

                    if(content.Content != null)
                        _applicationContext.Services.ContentService.Publish(content.Content);
                }
            }

            return status;
        }
    }
}
