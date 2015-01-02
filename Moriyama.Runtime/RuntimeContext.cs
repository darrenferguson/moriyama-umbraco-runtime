﻿using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using log4net;
using Moriyama.Runtime.Application;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Services;

namespace Moriyama.Runtime
{
    public class RuntimeContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IContentService ContentService { get; set; }

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

            if (string.IsNullOrEmpty(cache) || Convert.ToBoolean(cache) == false)
            {
                ContentService = new CacheLessRuntimeContentService(contentPathMapper);
            }
            else
            {
                ContentService = new CachedRuntimeContentService(contentPathMapper);
            }
        } 
    }
}