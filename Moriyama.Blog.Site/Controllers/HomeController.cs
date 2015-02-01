using System;
using System.Web.Mvc;
using Moriyama.Runtime;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Site.Controllers
{
    public class HomeController : Controller
    {
        

        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var ctx = System.Web.HttpContext.Current;

            var url = ctx.Request.Url;
            var urlString = string.Format("{0}{1}{2}{3}", url.Scheme, Uri.SchemeDelimiter, url.Authority, url.AbsolutePath);

            var model = RuntimeContext.Instance.ContentService.GetContent(urlString);

            if (model != null)
            {
                return View("~/Views/" + model.Template + ".cshtml", model);
            }

            Response.StatusCode = 404;
            return View("~/Views/404.cshtml", Build404Model(ctx.Request.Url));
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