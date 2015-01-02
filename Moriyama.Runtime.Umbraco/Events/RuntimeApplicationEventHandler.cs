using System.Reflection;
using System.Web;
using log4net;
using Moriyama.Runtime.Umbraco.Controllers;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Moriyama.Runtime.Umbraco.Events
{
    public class RuntimeApplicationEventHandler : IApplicationEventHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DefaultRenderMvcControllerResolver.Current.SetDefaultControllerType(typeof(RuntimeUmbracoController));
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Logger.Info(string.Format("OnApplicationStarted {0}", GetType().Name));

            var helper = new UmbracoHelper(UmbracoContext.Current);
            RuntimeUmbracoContext.Instance.Init(umbracoApplication.Server.MapPath("/"), helper);

            ContentService.Published += ContentServicePublished;
            ContentService.Trashing += ContentServiceTrashing;

            ContentService.UnPublishing += ContentServiceUnPublishing;

            RuntimeContext.Instance.Initialise(HttpContext.Current);
        }

        void ContentServiceUnPublishing(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            Logger.Info("Received unoublish event");

            foreach (var publishedEntity in e.PublishedEntities)
                RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(publishedEntity);
            
        }

        static void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            Logger.Info("Received trash event");
            RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(e.Entity);
        }

        static void ContentServicePublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            Logger.Info("Received publish event");
            foreach (var publishedEntity in e.PublishedEntities)
            {
                Logger.Info(string.Format("Sending Seralisation request for {0} ({1})", publishedEntity.Name, publishedEntity.Id));
                RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(publishedEntity);
            }
        }
    }
}
