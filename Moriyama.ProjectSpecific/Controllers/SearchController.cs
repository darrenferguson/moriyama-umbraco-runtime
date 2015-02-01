using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Application;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public ActionResult  Search(string query, int page = 1)
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);
            var newModel = Mapper.Map<SearchResultsModel>(modelContent);
            
            var results = RuntimeContext.Instance.SearchService.Search(query);
            
            var searchResults = new List<SearchResultModel>();

            foreach (var result in results)
            {
                var content = RuntimeContext.Instance.ContentService.GetContent(result.Url);

                if (content == null || content.Type == "BlogComment") continue;

                result.Content = content;
                searchResults.Add(result);
            }

            var paged = new PagedList<SearchResultModel>(searchResults.AsQueryable(), page, 10);

            newModel.Query = query;
            newModel.SearchResults = paged;

            return View(newModel);
        }
    }
}