using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project
{
    public class ProjectContext
    {
        private static ProjectContext _instance;

        private ProjectContext() { }

        public static ProjectContext Instance
        {
            get { return _instance ?? (_instance = new ProjectContext()); }
        }


        public void Init(RouteCollection routes)
        {
            Mapper.CreateMap<RuntimeContentModel, ContactModel>();

            //routes.MapRoute(
            //    "contact",
            //    "Contact/{action}/{id}",
            //    new
            //    {
            //        controller = "Contact",
            //        action = "Index",
            //        id = UrlParameter.Optional
            //    },
            //    new[] { "Moriyama.ProjectSpecific.Controllers" });

            routes.MapRoute(
                "customContent",
                "Content/{action}/{id}",
                new
                {
                    controller = "Content",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new[] { "Moriyama.ProjectSpecific.Controllers" });
        }

        
    }
}