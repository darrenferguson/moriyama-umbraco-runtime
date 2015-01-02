using System.Web.Mvc;
using Moriyama.Runtime.Application;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Controllers
{
    public class ContentController : Controller
    {
        [ChildActionOnly]
        [CustomOutputCache(CacheProfile = "Partials")]
        public ActionResult TopNavigation(RuntimeContentModel viewModel)
        {
            return PartialView("~/Views/Partial/TopNavigation.cshtml", viewModel);
        }
    }
}