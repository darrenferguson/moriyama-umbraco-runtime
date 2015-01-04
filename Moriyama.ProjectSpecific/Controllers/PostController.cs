using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime;

namespace Moriyama.Blog.Project.Controllers
{
    public class PostController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var ctx = System.Web.HttpContext.Current;

            var modelContent = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());
            var model = Mapper.Map<CommentModel>(modelContent);

            return View("~/Views/" + model.Template + ".cshtml", model);
        }

        [HttpPost]
        public ActionResult Submit()
        {
            var ctx = System.Web.HttpContext.Current;

            var modelContent = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());
            var model = Mapper.Map<CommentModel>(modelContent);

            return View("~/Views/" + model.Template + ".cshtml", model);
        }
    }
}