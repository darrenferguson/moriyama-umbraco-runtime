using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moriyama.Runtime;
using Segment;

namespace Moriyama.Blog.Site
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            RuntimeContext.Instance.Initialise(HttpContext.Current);

            Analytics.Initialize(ConfigurationManager.AppSettings["SegmentKey"]);

        }
    }
}