using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Abstract;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class CommaDelimitedIntExportContentParser : IExportContentParser
    {
        private readonly IEnumerable<ExportableContent> _allContent;
        private readonly IEnumerable<ExportableMedia> _allMedia;

        public CommaDelimitedIntExportContentParser(IEnumerable<ExportableContent> allContent, IEnumerable<ExportableMedia> allMedia)
        {
            _allContent = allContent;
            _allMedia = allMedia;
        }

        public string Name { get { return "CsvPaths"; } }

        public ParseResult ParseContent(BaseExportModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Value == null)
                    continue;

                var v = property.Value.ToString().Trim().Replace(" ", "");

                var items = v.Split(',');

                var allInts = true;
                foreach (var item in items)
                {
                    var intValue = -1;
                    allInts = int.TryParse(item, out intValue);
                    allInts = allInts && intValue > 0;
                }

                allInts = allInts && items.Length > 1;

                if (!allInts)
                    continue;
                
                var newValue = AllPaths(items.Select(int.Parse));

                newContent.Remove(property.Key);
                newContent.Add(property.Key, newValue.Path);
                model.Meta.Add(property.Key, Name + " - " + newValue.Type);


            }
            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }

        public ParseResult ParseForImport(BaseExportModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (model.Meta.ContainsKey(property.Key) && (model.Meta[property.Key].StartsWith(Name)))
                {
                    var v = (string) property.Value;
                    var values = v.Split(',');

                    var ids = new List<int>();

                    foreach (var item in values)
                    {
                        if (model.Meta[property.Key].Contains(" - Content"))
                        {
                            var content = _allContent.FirstOrDefault(x => x.Path == item);

                            if (content != null)
                                ids.Add(content.Content.Id);
                        }
                        else
                        {
                            var content = _allMedia.FirstOrDefault(x => x.Path == item);

                            if (content != null)
                                ids.Add(content.Content.Id);
                        }
                    }

                    newContent[property.Key] = string.Join(",", ids);

                }
            }
            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }

        private class PathResult
        {
            public string Path { get; set; }
            public string Type { get; set; }
        }

        private PathResult AllPaths(IEnumerable<int> ints)
        {
            var paths = new List<string>();

            string type = "Content";

            foreach (var id in ints)
            {
                var content = _allContent.FirstOrDefault(x => x.Content.Id == id);
                var media = _allMedia.FirstOrDefault(x => x.Content.Id == id);

                if(content != null)
                    paths.Add(content.Path);

                if (media != null)
                {
                    paths.Add(media.Path);
                    type = "Media";
                }
            }

            var p = string.Join(",", paths);

            return new PathResult
            {
                Path = p,
                Type = type
            };
        }
    }
}
