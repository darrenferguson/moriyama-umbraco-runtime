﻿using System.Web.Mvc;
using Moriyama.Content.Export.Application;
using Moriyama.Content.Export.Application.Media;
using Umbraco.Web.Mvc;

namespace Moriyama.Content.Export.Umbraco6.Controllers
{
    public class ExportController : SurfaceController
    {

        [HttpGet]  
        public ActionResult Index()
        {
            var path = Server.MapPath("~/App_Data/Moriyama");

            var u = new UmbracoContentExporter(ApplicationContext);

            var result = u.ExportContext(path);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Import()
        {
            var path = Server.MapPath("~/App_Data/Moriyama");

            var u = new UmbracoContentImporter(ApplicationContext, new UmbracoContentPropertySetter(), new UmbracoMediaPropertySetter());
            var result = u.Import(path);

            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

    }
}