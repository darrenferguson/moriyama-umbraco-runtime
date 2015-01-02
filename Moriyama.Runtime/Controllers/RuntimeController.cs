using System.Web.Mvc;

namespace Moriyama.Runtime.Controllers
{
    public class RuntimeController : Controller
    {
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