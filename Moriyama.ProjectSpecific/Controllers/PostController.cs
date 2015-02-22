using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using log4net;
using Moriyama.Blog.Project.Application;
using Moriyama.Blog.Project.Models;
using Moriyama.Blog.Project.Models.Email;
using Moriyama.Library.Html;
using Moriyama.Runtime;
using Moriyama.Runtime.Extension;
using Moriyama.Library.Extension;
using Segment;
using Segment.Model;

namespace Moriyama.Blog.Project.Controllers
{
    public class PostController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [OutputCache(CacheProfile = "Standard")]
        public ActionResult Index()
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);
            var model = Mapper.Map<CommentModel>(modelContent);

            return View(modelContent.View(), model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Submit(CommentModel model)
        {
            var contentService = RuntimeContext.Instance.ContentService;

            var modelContent = contentService.GetContent(System.Web.HttpContext.Current);

            var newModel = Mapper.Map<CommentModel>(modelContent);
            newModel = Mapper.Map(newModel, model);

            if (ModelState.IsValid)
            {
                var akismet = new Akismet("740317862571", "http://blog.darren-ferguson.com/", "Joel.Net's Akismet API/1.0");

                var isSpam = true;

                try
                {
                    var verifyKey = akismet.VerifyKey();

                    if (verifyKey)
                    {
                        var akismetComment = new AkismetComment();
                        akismetComment.Blog = "http://blog.darren-ferguson.com/";
                        akismetComment.UserIp = Request.ServerVariables["REMOTE_ADDR"];
                        akismetComment.UserAgent = Request.ServerVariables["HTTP_USER_AGENT"];
                        akismetComment.CommentAuthor = model.CommentName;
                        akismetComment.CommentAuthorEmail = model.CommentEmail;
                        akismetComment.CommentAuthorUrl = "";
                        akismetComment.CommentType = "comment";
                        akismetComment.CommentContent = model.CommentMessage;

                        isSpam = akismet.CommentCheck(akismetComment);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    isSpam = true;
                }

                if (!string.IsNullOrEmpty(model.Moriyama))
                {
                    isSpam = true;
                }

                if (!isSpam)
                {
                    var stripper = new HtmlTagStripper(new[] { "p", "em", "b", "i", "u" });
                    model.CommentMessage = stripper.RemoveUnwantedTags(model.CommentMessage);
                    model.CommentMessage = model.CommentMessage.Replace(Environment.NewLine, "<br/>");

                    model.CommentName = model.CommentName.RemoveNonAlphaNumeric();

                    var commentId = Guid.NewGuid().ToString();
                    var contentUrl = contentService.GetContentUrl(System.Web.HttpContext.Current);

                    commentId = contentUrl + "/" + commentId + "/";

                    var properties = new Dictionary<string, object>
                    {
                        {"name", model.CommentName},
                        {"email", model.CommentEmail},
                        {"bodyText", model.CommentMessage}
                    };

                    try
                    {
                        var email = new CommentEmail
                        {
                            Email = model.CommentEmail,
                            Name = model.CommentName,
                            Comment = model.CommentMessage
                        };

                        email.Send();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        ModelState.AddModelError("", "Your comment can't be added at this time.");
                        return View(modelContent.View(), newModel);
                    }

                    var ugc = contentService.CreateContent(commentId, properties);

                    ugc.Name = model.CommentEmail;
                    ugc.Type = "BlogComment";
                    ugc.Level = modelContent.Level + 1;
                    ugc.SortOrder = modelContent.Children().Count();

                    contentService.AddContent(ugc);

                    // Remove the post and the homepage from the output cache.
                    HttpResponse.RemoveOutputCacheItem(modelContent.RelativeUrl);
                    HttpResponse.RemoveOutputCacheItem(modelContent.Home().RelativeUrl);

                    Analytics.Client.Identify(model.CommentEmail, new Traits() {
                        { "name", model.CommentName },
                        { "email", model.CommentEmail }
                    });

                }
                else
                {
                    ModelState.AddModelError("", "Your comment can't be added at this time.");
                }
            }

            return View(modelContent.View(), newModel);
        }
    }
}