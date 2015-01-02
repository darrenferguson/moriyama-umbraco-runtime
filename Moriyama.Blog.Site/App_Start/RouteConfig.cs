using System.Web.Mvc;
using System.Web.Routing;
using Moriyama.Blog.Project;

namespace Moriyama.Blog.Site
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            ProjectContext.Instance.Init(routes);

            routes.MapRoute(
                "NotFound",
                "{*url}",
                new { controller = "Home", action = "Index" },
                new[] { "Moriyama.Blog.Site.Controllers" }
            );
        }
    }
}
