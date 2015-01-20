using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using log4net;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Services
{
    public class CachedRuntimeContentService : CacheLessRuntimeContentService
    {
        public override event ContentRemovedHandler Removed;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ObjectCache _customCache;
        private readonly CacheItemPolicy _policy; 

        public CachedRuntimeContentService(IContentPathMapper pathMapper) : base(pathMapper)
        {
            _customCache = MemoryCache.Default;
            _policy = new CacheItemPolicy {SlidingExpiration = TimeSpan.FromHours(1)};
        }

        public void SanitiseCache()
        {
            var stale = new List<string>();
            var hasStale = false;

            foreach (var url in Urls)
            {
                 var file = PathMapper.PathForUrl(url, false);

                 if (!File.Exists(file))
                     stale.Add(url);
            }

            foreach (var url in stale)
            {
                Urls.Remove(url);

                if (Removed != null)
                    Removed(url, new EventArgs());

                hasStale = true;
            }

            if (hasStale)
                FlushUrls();

            foreach (var url in Urls)
            {
                var localUrl = RemovePortFromUrl(url);
                var file = PathMapper.PathForUrl(localUrl, false);
                
                if (!File.Exists(file))
                    continue;

                var lastModified = File.GetLastWriteTime(file);
                var inCache = _customCache.Contains(localUrl);

                if (inCache)
                {
                    var content = GetCachedContent(localUrl);
                    if (content.CacheTime != null && DateTime.Compare(content.CacheTime.Value, lastModified) < 0)
                    {
                        content = base.GetContent(localUrl);
                        PlaceInCache(localUrl, content);
                    }
                }
            }
        }

        void PlaceInCache(string url, RuntimeContentModel content)
        {
            if (content == null)
                return;

            Logger.Info("Caching: " + url);

            content.CacheTime = DateTime.Now;
            _customCache.Set(url, content, _policy);
        }

        public override RuntimeContentModel GetContent(string url)
        {
            return GetCachedContent(url);
        }

        public override RuntimeContentModel Home(RuntimeContentModel model)
        {
            return (GetCachedContent(HomeUrl(model)));
        }

        private string RemovePortFromUrl(string url)
        {
            var rgx = new Regex(@"\:\d+"); // get rid of any port from the URL

            url = rgx.Replace(url, "");
            return url;
        }

        protected RuntimeContentModel GetCachedContent(string url)
        {
            url = RemovePortFromUrl(url);

            RuntimeContentModel content = null;

            if (_customCache.Contains(url))
            {
                Logger.Info("Attempt " + url + " from cache");  
                content = _customCache.Get(url) as RuntimeContentModel;
            }

            if (content != null)
            {
                Logger.Info("Got " + content.Name + " from cache " + content.Url);  
                return content;
            }

            content = base.GetContent(url);
            PlaceInCache(url, content);

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
            return FromUrls(TopNavigationUrls(model)).Where(x => x!= null);
        }

        public override IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model)
        {
            return FromUrls(ChildrenUrls(model)).Where(x => x != null); 
        }

        public override IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model)
        {
            return FromUrls(DescendantsUrls(model)).Where(x => x != null); 
        }
    }
}
