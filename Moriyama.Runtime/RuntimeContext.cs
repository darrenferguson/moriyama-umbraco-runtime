using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using log4net;
using Moriyama.Runtime.Application;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Services;
using Moriyama.Runtime.Services.Schedule;
using Quartz;
using Quartz.Impl;

namespace Moriyama.Runtime
{
    public class RuntimeContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IContentService ContentService { get; set; }
        public ISearchService SearchService { get; set; }

        private static RuntimeContext _instance;

        private RuntimeContext() { }

        public static RuntimeContext Instance
        {
            get { return _instance ?? (_instance = new RuntimeContext()); }
        }

        public void Initialise(HttpContext context)
        {
            var contentPath = context.Server.MapPath("/");

            contentPath = Path.Combine(contentPath, "App_Data", "Moriyama", "content");
            var contentPathMapper = new ContentPathMapper(contentPath);

            var cache = ConfigurationManager.AppSettings["Moriyama.Runtime.Cached"];
            
            Logger.Info("Starting with cache " + cache);

            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            if (string.IsNullOrEmpty(cache) || Convert.ToBoolean(cache) == false)
            {
                ContentService = new CacheLessRuntimeContentService(contentPathMapper);
            }
            else
            {
                ContentService = new LuceneQueryingContentService(contentPathMapper, Services.Search.SearchService.Instance);

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

            SearchService = Services.Search.SearchService.Instance;
            var search = ConfigurationManager.AppSettings["Moriyama.Runtime.Search"];


            Logger.Info("Begnning Indexing");
            var count = 0;

            if (!string.IsNullOrEmpty(search) && Convert.ToBoolean(search))
            {
                var urls = ContentService.GetUrlList();

                // Forces everything to be cached on startup... :(
                foreach (var url in urls)
                {
                    var content = ContentService.GetContent(url);

                    if (content != null)
                        SearchService.Index(content);

                    count++;
                }

                ContentService.Added += ContentServiceAdded;
                ContentService.Removed += ContentServiceRemoved;
                
            }

            Logger.Info("Indexed " + count + " documents");
            Logger.Info("Startup complete");

        }

        void ContentServiceRemoved(string sender, EventArgs e)
        {
            SearchService.Delete(sender);
        }

        void ContentServiceAdded(Models.RuntimeContentModel sender, EventArgs e)
        {
            SearchService.Index(sender);
        }
    }
}