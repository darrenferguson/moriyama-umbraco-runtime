using System.Web.Mvc;
using Moriyama.Runtime.Controllers;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Moriyama.Runtime.Umbraco.Controllers
{
    public class RuntimeUmbracoController : RenderMvcController
    {
        private readonly RuntimeController _runtimeController;

        public RuntimeUmbracoController()
        {
            _runtimeController = new RuntimeController();
        }

        [OutputCache(CacheProfile = "Standard")]
        public override ActionResult Index(RenderModel model)
        {
            return _runtimeController.Index();
        }
    }
}