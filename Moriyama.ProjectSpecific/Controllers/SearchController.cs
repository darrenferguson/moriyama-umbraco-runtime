﻿using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Moriyama.Blog.Project.Models;
using Moriyama.Runtime;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        public ActionResult  Search(string query)
        {
            var modelContent = RuntimeContext.Instance.ContentService.GetContent(System.Web.HttpContext.Current);
            var newModel = Mapper.Map<SearchResultsModel>(modelContent);

            //((SearchService) RuntimeContext.Instance.SearchService).FlushToDisc();

            var results = RuntimeContext.Instance.SearchService.Search(query);
            
            var searchResults = new List<SearchResultModel>();

            foreach (var result in results)
            {
                var content = RuntimeContext.Instance.ContentService.GetContent(result.Url);

                if (content != null && content.Type != "BlogComment")
                {
                    result.Content = content;
                    searchResults.Add(result);
                }
            }

            newModel.Query = query;
            newModel.SearchResults = searchResults;

            return View(newModel);
        }
    }
}