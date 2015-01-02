using System.Web.Mvc;

namespace Moriyama.ProjectSpecific.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}