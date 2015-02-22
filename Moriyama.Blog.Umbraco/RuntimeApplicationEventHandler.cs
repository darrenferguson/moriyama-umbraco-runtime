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
            Analytics.Initialize("QgDmGvCjQv4rLIIfAQ0WTfbe8CAUfUtO");
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