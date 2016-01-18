using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
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

        public ExportContentModel ParseForImport(ExportContentModel model)
        {
            return model;
        }

        public ExportContentModel ParseContent(ExportContentModel model)
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

                    if (content != null)
                    {                     
                        newContent.Remove(property.Key);
                        newContent.Add(property.Key, content.Path);
                        model.Meta.Add(property.Key, Name);
                    }
                }
            }

            model.Content = newContent;
            return model;
        }
    }
}
