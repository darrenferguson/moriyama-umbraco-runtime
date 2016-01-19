using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Abstract;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class IntExportContentParser : IExportContentParser
    {
        private readonly IEnumerable<ExportableContent> _allContent;
        private readonly IEnumerable<ExportableMedia> _allMedia;

        public IntExportContentParser(IEnumerable<ExportableContent> allContent, IEnumerable<ExportableMedia> allMedia)
        {
            _allContent = allContent;
            _allMedia = allMedia;
        }

        public string Name { get { return "Path"; } }

        public ParseResult ParseForImport(BaseExportModel model)
        {

             var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (model.Meta.ContainsKey(property.Key) && (model.Meta[property.Key].StartsWith(Name)))
                {
                    var value = (string)property.Value;

                    if (model.Meta[property.Key].Contains("- Media"))
                    {
                        var content = _allMedia.FirstOrDefault(x => x.Path == value);
                        if (content != null)
                            newContent[property.Key] = content.Content.Id;

                    }
                    else
                    {
                        var content = _allContent.FirstOrDefault(x => x.Path == value);
                        if(content != null)
                            newContent[property.Key] = content.Content.Id;
                    }
                    
                }
            }

            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }

        public ParseResult ParseContent(BaseExportModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Value == null)
                    continue;

                int x = -1;
                if (int.TryParse(property.Value.ToString(), out x))
                {
                    var content = _allContent.FirstOrDefault(v => v.Content.Id == x);
                    var media = _allMedia.FirstOrDefault(v => v.Content.Id == x);

                    if (content != null || media != null)
                    {                     
                        newContent.Remove(property.Key);

                        if (content != null)
                        {
                            newContent.Add(property.Key, content.Path);
                            model.Meta.Add(property.Key, Name + " - Content");
                        }

                        if (media != null)
                        {
                            newContent.Add(property.Key, media.Path);
                            model.Meta.Add(property.Key, Name + " - Media");
                        }
                    }
                }
            }

            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }
    }
}
