using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime;
using Moriyama.Runtime.Extension;

namespace Moriyama.Blog.Project.Controllers
{
    public class PostController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);
            var model = Mapper.Map<CommentModel>(modelContent);

            return View(modelContent.View(), model);
        }

        [HttpPost]
        public ActionResult Submit(CommentModel model)
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);

            var newModel = Mapper.Map<CommentModel>(modelContent);
            newModel = Mapper.Map(newModel, model);

            if (ModelState.IsValid)
            {
                // TODO: Create the comment here
            }

            return View(modelContent.View(), newModel);
        }
    }
}