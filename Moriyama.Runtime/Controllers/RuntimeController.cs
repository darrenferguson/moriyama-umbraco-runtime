using System.Web;
using System.Web.Mvc;

namespace Moriyama.Runtime.Controllers
{
    public class RuntimeController : Controller
    {
        //[OutputCache(CacheProfile = "Standard", Location = OutputCacheLocation.Server)]
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult Index(HttpRequestBase request)
        {
            var model = RuntimeContext.Instance.ContentService.GetContent(request.Url.ToString());

            return model != null 
                ? View("~/Views/"+ model.Template + ".cshtml", model) 
                : View("~/Views/404.cshtml");
        }
    }
}