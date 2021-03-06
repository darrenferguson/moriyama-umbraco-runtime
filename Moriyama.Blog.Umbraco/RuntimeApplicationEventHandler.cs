﻿using System.Configuration;
using System.Web.Routing;
using Moriyama.Blog.Project;
using Segment;
using Umbraco.Core;

namespace Moriyama.Blog.Umbraco
{
    public class RuntimeApplicationEventHandler : IApplicationEventHandler
    {
        public void OnApplicationStarted(
        UmbracoApplicationBase umbracoApplication,
        ApplicationContext applicationContext)
        {
            ProjectContext.Instance.Init(RouteTable.Routes);
            // Analytics.Initialize(ConfigurationManager.AppSettings["SegmentKey"]);
        }
        public void OnApplicationInitialized(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
        }
        public void OnApplicationStarting(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
        }
    }
}