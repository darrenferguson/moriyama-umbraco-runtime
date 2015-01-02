using System.Web.Mvc;
using System.Web.Routing;

namespace Moriyama.ProjectSpecific
{
    public class ProjectContext
    {
        private static ProjectContext _instance;

        private ProjectContext() { }

        public static ProjectContext Instance
        {
            get { return _instance ?? (_instance = new ProjectContext()); }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "contact",
                "Contact/{action}/{id}",
                new
                {
                    controller = "Contact",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new[] { "Moriyama.ProjectSpecific.Controllers" });
        }
    }
}