using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Caching;
using log4net;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Services
{
    public class CachedRuntimeContentService : CacheLessRuntimeContentService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ObjectCache _customCache;
        private readonly CacheItemPolicy _policy; 

        public CachedRuntimeContentService(IContentPathMapper pathMapper) : base(pathMapper)
        {
            _customCache = MemoryCache.Default;
            _policy = new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60.0)};
        }

        public override RuntimeContentModel GetContent(string url)
        {
            return GetCachedContent(url);
        }

        public override RuntimeContentModel Home(RuntimeContentModel model)
        {
            return (GetCachedContent(HomeUrl(model)));
        }

        private RuntimeContentModel GetCachedContent(string url)
        {
            RuntimeContentModel content = null;

            if (_customCache.Contains(url))
                content = _customCache.Get(url) as RuntimeContentModel;

            if (content != null)
            {
                if(Logger.IsDebugEnabled)
                    Logger.Debug("Got " + content.Name + " from cache " + content.Url);

                return content;
            }

            content = base.GetContent(url);
            _customCache.Set(url, content, _policy);

            if (Logger.IsDebugEnabled)
                Logger.Debug("Cached " + content.Name + " to cache " + content.Url);

            return content;
        }

        private IEnumerable<RuntimeContentModel> FromUrls(IEnumerable<string> urls)
        {
            var contents = new List<RuntimeContentModel>();

            foreach (var url in urls)
            {
                contents.Add(GetCachedContent(url));
            }

            return contents;
        }

        public override IEnumerable<RuntimeContentModel> TopNavigation(RuntimeContentModel model)
        {
            return FromUrls(TopNavigationUrls(model));
        }

        public override IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model)
        {
            return FromUrls(ChildrenUrls(model));
        }

        public override IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model)
        {
            return FromUrls(DescendantsUrls(model));
        }
    }
}
