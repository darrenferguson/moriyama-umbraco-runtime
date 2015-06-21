using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using ByteSizeLib;
using log4net;
using Moriyama.Runtime.Application;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Services;
using Moriyama.Runtime.Services.Search;

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
            var startTime = DateTime.Now;
            Logger.Info("Startup: " + startTime);

            SearchService = new SearchService();

            var contentPath = context.Server.MapPath("/");

            contentPath = Path.Combine(contentPath, "App_Data", "Moriyama", "content");
            var contentPathMapper = new ContentPathMapper(contentPath);

            var cache = ConfigurationManager.AppSettings["Moriyama.Runtime.Cached"];
            Logger.Info("Starting with cache " + cache + " (" + ByteSize.FromBytes(Process.GetCurrentProcess().WorkingSet64).MegaBytes + " memory MB)");

            if (string.IsNullOrEmpty(cache) || Convert.ToBoolean(cache) == false)
                ContentService = new CacheLessRuntimeContentService(contentPathMapper, SearchService);
            else
                ContentService = new LuceneQueryingContentService(contentPathMapper, SearchService);

            ContentService.RefreshUrls();

            var indexOnStart = ConfigurationManager.AppSettings["Moriyama.Runtime.Search"];

            if (!string.IsNullOrEmpty(indexOnStart) && Convert.ToBoolean(indexOnStart))
                SearchService.IndexAll(ContentService);

            Logger.Info("Startup complete " + "(" + ByteSize.FromBytes(Process.GetCurrentProcess().WorkingSet64).MegaBytes + " memory MB)");
            Logger.Info("Startup time " + DateTime.Now.Subtract(startTime).TotalSeconds);
        }
    }
}