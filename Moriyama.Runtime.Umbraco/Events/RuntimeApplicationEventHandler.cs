using System.Reflection;
using System.Web;
using AutoMapper.Mappers;
using log4net;
using Moriyama.Runtime.Umbraco.Controllers;
using Moriyama.Runtime.Umbraco.Interfaces;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
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
            // hack - http://stackoverflow.com/questions/18447148/automapper-3-0-this-type-is-not-supported-on-this-platform-imapperregistry
            var useless = new ListSourceMapper();
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

            // ContentService.Published += ContentServicePublished;
            ContentService.Trashing += ContentServiceTrashing;

            ContentService.UnPublishing += ContentServiceUnPublishing;
            ContentService.Moving += ContentServiceMoving;
            
             // Re-instate this to work with - v6 earlier versions. see RuntimeContentCacheRefresher for a current implementation
             umbraco.content.AfterUpdateDocumentCache += ContentAfterUpdateDocumentCache;

            RuntimeContext.Instance.Initialise(HttpContext.Current);
        }

        void ContentServiceMoving(IContentService sender, MoveEventArgs<IContent> e)
        {
            Logger.Info("Received moved event");
            var runtimeContentModel = RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(e.Entity);
            foreach (var adapter in RuntimeUmbracoContext.Instance.DeploymentAdapters)
            {
                adapter.DeployContent(runtimeContentModel, DeploymentAction.Delete);
            }
        }

        void ContentAfterUpdateDocumentCache(Document sender, DocumentCacheEventArgs e)
        {

            Logger.Info("Received update cache event");

            var content = UmbracoContext.Current.Application.Services.ContentService.GetById(sender.Id);
            var runtimeContentModel = RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(content);

            foreach (var adapter in RuntimeUmbracoContext.Instance.DeploymentAdapters)
            {
                adapter.DeployContent(runtimeContentModel, DeploymentAction.Deploy);
            }
        }

        void ContentServiceUnPublishing(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            Logger.Info("Received unoublish event");

            foreach (var publishedEntity in e.PublishedEntities)
            {
                var runtimeContentModel = RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(publishedEntity);
                foreach (var adapter in RuntimeUmbracoContext.Instance.DeploymentAdapters)
                {
                    adapter.DeployContent(runtimeContentModel, DeploymentAction.Delete);
                }
            }
            
        }

        static void ContentServiceTrashing(IContentService sender, MoveEventArgs<IContent> e)
        {
            Logger.Info("Received trash event");
            var runtimeContentModel = RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(e.Entity);
            foreach (var adapter in RuntimeUmbracoContext.Instance.DeploymentAdapters)
            {
                adapter.DeployContent(runtimeContentModel, DeploymentAction.Delete);
            }
        }

        //static void ContentServicePublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        //{
        //    Logger.Info("Received publish event");
        //    foreach (var publishedEntity in e.PublishedEntities)
        //    {
        //        Logger.Info(string.Format("Sending Seralisation request for {0} ({1})", publishedEntity.Name, publishedEntity.Id));
        //        RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(publishedEntity);
        //    }
        //}
    }
}
