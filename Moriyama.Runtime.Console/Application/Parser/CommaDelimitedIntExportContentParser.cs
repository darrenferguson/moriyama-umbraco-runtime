using System;
using System.Collections.Generic;
using System.Linq;
using Moriyama.Runtime.Console.Application.Domain;
using Moriyama.Runtime.Console.Interfaces;

namespace Moriyama.Runtime.Console.Application.Parser
{
    class CommaDelimitedIntExportContentParser : IExportContentParser
    {
        private readonly IEnumerable<ExportableContent> _allContent;

        public CommaDelimitedIntExportContentParser(IEnumerable<ExportableContent> allContent)
        {
            _allContent = allContent;
        }

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
