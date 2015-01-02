using System.Linq;
using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Interfaces;

namespace Moriyama.Runtime.Umbraco.Application.Parser
{
    public class NullValueUmbracoContentParser : IUmbracoContentParser
    {
        public RuntimeContentModel ParseContent(RuntimeContentModel model)
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
