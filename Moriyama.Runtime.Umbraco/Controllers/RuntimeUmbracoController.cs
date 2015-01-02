using System.Web.Mvc;
using Moriyama.Runtime.Controllers;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Moriyama.Runtime.Umbraco.Controllers
{
    public class RuntimeUmbracoController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var controller = new RuntimeController();
            return controller.Index(Request);
        }
    }
}