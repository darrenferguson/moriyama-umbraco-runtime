using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using Moriyama.Runtime;
using Segment;

namespace Moriyama.Blog.Site
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            Logger.Info("I take your brain to another dimension");
   
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            RuntimeContext.Instance.Initialise(HttpContext.Current);

            Analytics.Initialize(ConfigurationManager.AppSettings["SegmentKey"]);
            Logger.Info("Pay close attention");
   

        }
    }
}