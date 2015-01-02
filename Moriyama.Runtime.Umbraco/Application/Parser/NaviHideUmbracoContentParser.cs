using System.Linq;
using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Interfaces;

namespace Moriyama.Runtime.Umbraco.Application.Parser
{
    public class NaviHideUmbracoContentParser : IUmbracoContentParser
    {
        public RuntimeContentModel ParseContent(RuntimeContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Key != "umbracoNaviHide") continue;

                var v = property.Value;
                newContent.Remove(property.Key);

                var newValue = v.ToString() != "0";
                newContent.Add("HideInNavigation", newValue);
            }

            model.Content = newContent;
            return model;
        }
    }
}
