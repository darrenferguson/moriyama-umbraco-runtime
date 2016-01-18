using System.Collections.Generic;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Moriyama.Content.Export.Application.Media
{
    public class UmbracoMediaFinder : IUmbracoContentFinder<IMedia>
    {
        private readonly IMediaService _mediaService;

        public UmbracoMediaFinder(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public IEnumerable<IMedia> FindAllContent()
        {
            return FindContent(_mediaService.GetRootMedia());
        }

        private IEnumerable<IMedia> FindContent(IEnumerable<IMedia> contents)
        {
            var allContent = new List<IMedia>();

            foreach (var content in contents)
            {
                allContent.Add(content);
                allContent.AddRange(FindContent(content.Children()));
            }
            return allContent;
        } 
    }
}
