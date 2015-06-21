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
using Quartz;
using Quartz.Impl;

namespace Moriyama.Runtime.Services
{
    public class CachedRuntimeContentService : CacheLessRuntimeContentService
    {

        [DisallowConcurrentExecution]
        private class CacheRefresherJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                Logger.Info(GetType().Name + " scheduled task..");

                var contentService = (CachedRuntimeContentService) RuntimeContext.Instance.ContentService;
                contentService.SanitiseCache();
            }
        }
        
        public override event ContentRemovedHandler Removed;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ObjectCache _customCache;
        private readonly CacheItemPolicy _policy; 

        public CachedRuntimeContentService(IContentPathMapper pathMapper, ISearchService searchService) : base(pathMapper, searchService)
        {
            _customCache = MemoryCache.Default;
            _policy = new CacheItemPolicy {SlidingExpiration = TimeSpan.FromHours(1)};

            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<CacheRefresherJob>()
                .WithIdentity("moriyamaCacheRefresherJob", "cacheRefresherJob")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("cacheRefresherJobTrigger", "cacheRefresherJobGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60)
                .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
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

                SearchService.Delete(url);

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

            if(Logger.IsDebugEnabled)
                Logger.Debug("Caching: " + url);

            content.CacheTime = DateTime.Now;
            _customCache.Set(url, content, _policy);
        }

        public override RuntimeContentModel GetContent(string url)
        {
            url = ProcessUrlAliases(url);
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
                if(Logger.IsDebugEnabled)
                    Logger.Debug("Attempt " + url + " from cache"); 
 
                content = _customCache.Get(url) as RuntimeContentModel;
            }

            if (content != null)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("Got " + content.Name + " from cache " + content.Url);  

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
