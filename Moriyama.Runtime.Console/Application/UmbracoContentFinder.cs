using System.Collections.Generic;
using Moriyama.Runtime.Console.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Moriyama.Runtime.Console.Application
{
    public class UmbracoContentFinder : IUmbracoContentFinder
    {

        private readonly IContentService _contentService;

        public UmbracoContentFinder(IContentService contentService)
        {
            _contentService = contentService;
        }

        public IEnumerable<IContent> FindAllContent()
        {
            return FindContent(_contentService.GetRootContent());
        }

        private IEnumerable<IContent> FindContent(IEnumerable<IContent> contents)
        {
            var allContent = new List<IContent>();

            foreach (var content in contents)
            {
                System.Console.WriteLine(content.Name);

                allContent.Add(content);
                allContent.AddRange(FindContent(content.Children()));
            }
            return allContent;
        } 
    }
}
