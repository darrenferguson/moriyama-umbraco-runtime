using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
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

        public ExportContentModel ParseContent(ExportContentModel model)
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
                newContent.Add(property.Key, newValue);
                model.Meta.Add(property.Key, Name);
            }
            model.Content = newContent;
            return model;
        }

        public ExportContentModel ParseForImport(ExportContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (model.Meta.ContainsKey(property.Key) && (model.Meta[property.Key] == Name))
                {
                    var v = (string) property.Value;
                    var values = v.Split(',');

                    var ids = new List<int>();

                    foreach (var item in values)
                    {
                        var content = _allContent.FirstOrDefault(x => x.Path == item);

                        if(content != null)
                            ids.Add(content.Content.Id);

                    }

                    newContent[property.Key] = string.Join(",", ids);

                }
            }
            model.Content = newContent;
            return model;
        }

        private string AllPaths(IEnumerable<int> ints)
        {
            var paths = new List<string>();
            foreach (var id in ints)
            {
                var content = _allContent.FirstOrDefault(x => x.Content.Id == id);

                if(content != null)
                    paths.Add(content.Path);
            }

            return string.Join(",", paths);
        }
    }
}
