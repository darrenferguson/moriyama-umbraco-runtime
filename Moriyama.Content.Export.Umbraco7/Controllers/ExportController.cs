﻿using System.Web.Mvc;
using Moriyama.Content.Export.Application;
using Umbraco.Web.Mvc;

namespace Moriyama.Content.Export.Umbraco7.Controllers
{
    public class ExportController : SurfaceController
    {

        [HttpGet]  
        public ActionResult Index()
        {
            var path = Server.MapPath("~/App_Data/Moriyama");

            var u = new UmbracoContentExporter(ApplicationContext);

            u.ExportContext(path);

            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Import()
        {
            var path = Server.MapPath("~/App_Data/Moriyama");

            var u = new UmbracoContentImporter(ApplicationContext, new UmbracoPropertySetter(ApplicationContext));
            var result = u.Import(new FileSystem(path));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}