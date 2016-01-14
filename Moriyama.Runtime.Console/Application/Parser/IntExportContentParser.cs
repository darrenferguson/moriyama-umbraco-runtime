using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moriyama.Runtime.Console.Application.Domain;
using Moriyama.Runtime.Console.Interfaces;

namespace Moriyama.Runtime.Console.Application.Parser
{
    public class IntExportContentParser : IExportContentParser
    {
        private readonly IEnumerable<ExportableContent> _allContent;

        public IntExportContentParser(IEnumerable<ExportableContent> allContent)
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

                int x = -1;
                if (int.TryParse(property.Value.ToString(), out x))
                {
                    var content = _allContent.FirstOrDefault(v => v.Content.Id == x);

                    if (content != null)
                    {
                        

                        newContent.Remove(property.Key);
                        newContent.Add(property.Key, content.Path);
                    }

                }

            }

            model.Content = newContent;
            return model;
        }
    }
}
