using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Abstract;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class NullValueExportContentParser : IExportContentParser
    {
        public string Name { get { return "Null"; } }

        public ParseResult ParseForImport(BaseExportModel model)
        {

            return new ParseResult
            {
                Meta = model.Meta,
                Content = model.Content
            };
        }

        public ParseResult ParseContent(BaseExportModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Value != null)
                    continue;

                newContent.Remove(property.Key);
                newContent.Add(property.Key, string.Empty);
            }
            
            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }
    }
}
