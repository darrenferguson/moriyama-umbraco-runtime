using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Moriyama.Blog.Umbraco.Classes
{
    public class TestApplicationEventHandler : IApplicationEventHandler
    {
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //PageCacheRefresher.CacheUpdated += PageCacheRefresher_CacheUpdated;
            //var svc = UmbracoContext.Current.Application.Services.ContentService;

            //foreach (var rootContent in svc.GetRootContent())
            //{
            //    foreach (var item in rootContent.Descendants())
            //    {
            //        if (item.ContentType.Alias != "BlogFolder" && item.ContentType.Alias != "BlogPost")
            //            continue;

            //        var p = new PageCacheRefresher();
            //        p.Remove(item);

            //    }
            //}

        }

        void PageCacheRefresher_CacheUpdated(PageCacheRefresher sender, CacheRefresherEventArgs e)
        {
           // var a = (IContent) e.MessageObject;

           // if (a.ContentType.Alias != "BlogFolder" || a.ContentType.Alias != "BlogPost")
           //     return;

           //if(e.MessageType == MessageType.RefreshById || e.MessageType == MessageType.RefreshByInstance)
           //     sender.Remove(a);

        }

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}