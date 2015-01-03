using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoMapper;
using log4net;
using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Moriyama.Runtime.Umbraco.Application
{
    internal class UmbracoContentSerialiser : IUmbracoContentSerialiser
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UmbracoHelper _umbracoHelper;
        private readonly IEnumerable<IUmbracoContentParser> _contentParsers;
        
        public UmbracoContentSerialiser(UmbracoHelper umbracoHelper, IEnumerable<IUmbracoContentParser> contentParsers)
        {
            _umbracoHelper = umbracoHelper;
            _contentParsers = contentParsers;
        }

        public void Remove(IContent content)
        {
            var publishedContent = _umbracoHelper.TypedContent(content.Id);
            RuntimeContext.Instance.ContentService.RemoveContent(publishedContent.Url);
        }

        private string RemovePortFromUrl(string url)
        {
            var rgx = new Regex(@"\:\d+"); // get rid of any port from the URL

            url = rgx.Replace(url, "");
            return url;
        }

        public void Serialise(IContent content)
        {
            var publishedContent = _umbracoHelper.TypedContent(content.Id);

            if (publishedContent == null)
                return;

            var runtimeContent = Mapper.Map<RuntimeContentModel>(publishedContent);

            runtimeContent.Url = RemovePortFromUrl(publishedContent.UrlWithDomain());
            runtimeContent.RelativeUrl = publishedContent.Url;
            runtimeContent.CacheTime = null;

            runtimeContent.Type = publishedContent.DocumentTypeAlias;

            runtimeContent.Template = publishedContent.GetTemplateAlias();

            runtimeContent.Content = new Dictionary<string, object>();

            foreach (var property in content.Properties)
            {
                if (!runtimeContent.Content.ContainsKey(property.Alias))
                    runtimeContent.Content.Add(property.Alias, property.Value);
            }

            foreach (var contentParser in _contentParsers)
            {
                runtimeContent = contentParser.ParseContent(runtimeContent);
            }
            
            RuntimeContext.Instance.ContentService.AddContent(runtimeContent);
        }
    }
}
