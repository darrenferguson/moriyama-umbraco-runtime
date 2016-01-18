using System.Collections.Generic;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Moriyama.Content.Export.Application.Content
{
    public class UmbracoContentFinder : IUmbracoContentFinder<IContent>
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
                allContent.Add(content);
                allContent.AddRange(FindContent(content.Children()));
            }
            return allContent;
        } 
    }
}
