using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Blog.Project.Models.Email;
using Moriyama.Runtime;
using Moriyama.Runtime.Extension;

namespace Moriyama.Blog.Project.Controllers
{
    public class PostController : Controller
    {
        [HttpGet]
        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);
            var model = Mapper.Map<CommentModel>(modelContent);

            return View(modelContent.View(), model);
        }

        [HttpPost]
        public ActionResult Submit(CommentModel model)
        {
            var contentService = RuntimeContext.Instance.ContentService;

            var modelContent = contentService.GetContent(System.Web.HttpContext.Current);

            var newModel = Mapper.Map<CommentModel>(modelContent);
            newModel = Mapper.Map(newModel, model);

            if (ModelState.IsValid)
            {
                // TODO: Create the comment here
                var commentId = Guid.NewGuid().ToString();
                var contentUrl = contentService.GetContentUrl(System.Web.HttpContext.Current);

                commentId = contentUrl + "/" + commentId + "/";

                var properties = new Dictionary<string, object>
                {
                    {"name", model.CommentName},
                    {"email", model.CommentEmail},
                    {"bodyText", model.CommentMessage}
                };

                var ugc = contentService.CreateContent(commentId, properties);

                ugc.Name = model.CommentEmail;
                ugc.Type = "BlogComment";
                ugc.Level = modelContent.Level + 1;
                ugc.SortOrder = modelContent.Children().Count();

                contentService.AddContent(ugc);

                // Remove the post and the homepage from the output cache.
                HttpResponse.RemoveOutputCacheItem(modelContent.RelativeUrl);
                HttpResponse.RemoveOutputCacheItem(modelContent.Home().RelativeUrl);

                var email = new CommentEmail
                {
                    Email = model.CommentEmail,
                    Name = model.CommentName,
                    Comment = model.CommentMessage
                };

                email.Send();

            }

            return View(modelContent.View(), newModel);
        }
    }
}