using System;
using System.Web.Mvc;
using Moriyama.Runtime.Models;

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
                : View("~/Views/404.cshtml", Build404Model(ctx.Request.Url));
        }

        private RuntimeContentModel Build404Model(Uri url)
        {
            var homeUrl = url.Scheme + "://" + url.Host + ":" + url.Port + "/";
            var content = RuntimeContext.Instance.ContentService.GetContent(homeUrl);

            if (content == null)
            {
                content = new RuntimeContentModel();
                content.Name = "404 Not Found";
            }

            return content;
        }
      
    }
}