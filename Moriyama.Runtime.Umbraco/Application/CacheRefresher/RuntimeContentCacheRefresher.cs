using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Moriyama.Runtime.Umbraco.Application.CacheRefresher
{
    public class RuntimeContentCacheRefresher : PageCacheRefresher
    {
        //public RuntimeContentCacheRefresher() : base()
        //{
            
        //}

        public override void Refresh(int id)
        {
            base.Refresh(id);
            var content = UmbracoContext.Current.Application.Services.ContentService.GetById(id);
            RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(content);
        }

        /// <summary>
        /// Removes the node with the specified id from the cache
        /// </summary>
        /// <param name="id">The id.</param>
        public override void Remove(int id)
        {
            base.Remove(id);
            var content = UmbracoContext.Current.Application.Services.ContentService.GetById(id);
            RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(content);
        }

        public override void Refresh(IContent instance)
        {
            
            base.Refresh(instance);
            RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(instance);
        }

        public override void Remove(IContent instance)
        {
            base.Remove(instance);
            RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Remove(instance);
        }
    }
}
