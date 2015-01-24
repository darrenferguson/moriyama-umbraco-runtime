using System.Web.Mvc;
using Moriyama.Runtime.Controllers;
using Moriyama.Runtime;

namespace Moriyama.Blog.Site.Controllers
{
    public class HomeController : Controller
    {
        

        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var ctx = System.Web.HttpContext.Current;
            var model = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());
            
            return model != null
                ? View("~/Views/" + model.Template + ".cshtml", model)
                : View("~/Views/404.cshtml");
        }
    }
}