using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class NullValueExportContentParser : IExportContentParser
    {
        public string Name { get { return "Null"; } }

        public ExportContentModel ParseForImport(ExportContentModel model)
        {
            return model;
        }

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
