﻿using System.Web.Mvc;
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
            Mapper.CreateMap<RuntimeContentModel, CommentModel>();
            Mapper.CreateMap<RuntimeContentModel, SearchResultsModel>();

            routes.MapRoute(
                "post", // Route name
                "{year}/{month}/{day}/{title}", // Route Pattern
                new { controller = "Post", action = "Index" },
                new { year = @"\d+", month = @"\d+", day = @"\d+", httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Moriyama.Blog.Project.Controllers" }
            );

            routes.MapRoute(
                "postComment", // Route name
                "{year}/{month}/{day}/{title}", // Route Pattern
                new { controller = "Post", action = "Submit" },
                new { year = @"\d+", month = @"\d+", day = @"\d+", httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Moriyama.Blog.Project.Controllers" }
            );


            routes.MapRoute(
                "search",
                "Search/{action}/{id}",
                new
                {
                    controller = "Search",
                    action = "Search",
                    id = UrlParameter.Optional
                },
                new { httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Moriyama.Blog.Project.Controllers" });


            routes.MapRoute(
               "share",
               "Social/{action}",
               new
               {
                   controller = "Social",
                   action = "Share",
                   id = UrlParameter.Optional
               },
               new { httpMethod = new HttpMethodConstraint("GET") },
               new[] { "Moriyama.Blog.Project.Controllers" });

            routes.MapRoute(
                "customContent",
                "Content/{action}/{id}",
                new
                {
                    controller = "Content",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new[] { "Moriyama.Blog.Project.Controllers" });
        }
    }
}