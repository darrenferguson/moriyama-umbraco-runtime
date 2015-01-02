using System;
using System.Linq;
using System.Text.RegularExpressions;
using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Interfaces;
using Umbraco.Web;

namespace Moriyama.Runtime.Umbraco.Application.Parser
{
    public class LocalLinkUmbracoContentParser : IUmbracoContentParser
    {
        private readonly UmbracoHelper _umbracoHelper;

        public LocalLinkUmbracoContentParser(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public RuntimeContentModel ParseContent(RuntimeContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Value is string && ((string)property.Value).Contains("{localLink:"))
                {
                    var localLinkRegex = new Regex(@"\/\{localLink\:(.*?)\}", RegexOptions.Compiled | RegexOptions.Multiline);
                    var value = (string) property.Value;

                    foreach (Match match in localLinkRegex.Matches(value))
                    {
                        var replacement = match.Value;
                        var docId = match.Groups[1].Value;

                        value = value.Replace(replacement, UrlForLocalLink(docId));
                    }

                    newContent[property.Key] = value;
                }
            }
            model.Content = newContent;
            return model;
        }

        private string UrlForLocalLink(string id)
        {
            try
            {
                var intId = Convert.ToInt32(id);
                var content = _umbracoHelper.TypedContent(intId);
                return content.Url;
            }
            catch (Exception ex)
            {
                
            }
            return "#";
        }
    }
}
