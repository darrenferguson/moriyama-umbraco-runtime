using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime.Controllers;
using Moriyama.Runtime;

namespace Moriyama.Blog.Site.Controllers
{
    public class HomeController : Controller
    {
        

        //[OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var ctx = System.Web.HttpContext.Current;
            var model = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());

            if(model == null)
                return View("~/Views/404.cshtml");

            if (model.Type != "BlogPost")
                return View("~/Views/" + model.Template + ".cshtml", model);

            var newModel = Mapper.Map<CommentModel>(model);
            return View("~/Views/" + model.Template + ".cshtml", newModel);
        }
    }
}