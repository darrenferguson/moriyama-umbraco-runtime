using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moriyama.Runtime;

namespace Moriyama.Blog.Site
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            RuntimeContext.Instance.Initialise(HttpContext.Current);
            

        }
    }
}