using System.Web.Mvc;
using Moriyama.Runtime.Controllers;

namespace Moriyama.Blog.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly RuntimeController _controller;

        public HomeController()
        {
            _controller = new RuntimeController();
        }

        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            return _controller.Index();
        }
    }
}