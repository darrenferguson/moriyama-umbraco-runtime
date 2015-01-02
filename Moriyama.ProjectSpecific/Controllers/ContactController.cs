using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime;

namespace Moriyama.Blog.Project.Controllers
{
    public class ContactController : Controller
    {
        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var ctx = System.Web.HttpContext.Current;
            
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());
            var model = Mapper.Map<ContactModel>(modelContent);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ContactModel model)
        {
            var ctx = System.Web.HttpContext.Current;

            var modelContent = RuntimeContext.Instance.ContentService.GetContent(ctx.Request.Url.ToString());

            var newModel = Mapper.Map<ContactModel>(modelContent);
            newModel = Mapper.Map(newModel, model);

            if(!ModelState.IsValid)
                ModelState.AddModelError("", "Bad wolf!");

            return View(newModel);
        } 
    }
}