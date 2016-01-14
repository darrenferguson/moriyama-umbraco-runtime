using System.Linq;
using Moriyama.Runtime.Console.Application.Domain;
using Moriyama.Runtime.Console.Interfaces;

namespace Moriyama.Runtime.Console.Application.Parser
{
    public class NullValueExportContentParser : IExportContentParser
    {
        public ExportContentModel ParseContent(ExportContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);
            foreach (var property in model.Content)
            {
                if (property.Value != null)
                    continue;

                newContent.Remove(property.Key);
                newContent.Add(property.Key, string.Empty);
            }
            model.Content = newContent;
            return model;
        }
    }
}
