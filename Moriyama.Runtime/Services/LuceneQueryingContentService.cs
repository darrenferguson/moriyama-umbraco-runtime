using System.Collections.Generic;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Services
{
    public class LuceneQueryingContentService : CachedRuntimeContentService
    {
        private ISearchService _searchService;
        
        public LuceneQueryingContentService(IContentPathMapper pathMapper, ISearchService searchService)
            : base(pathMapper)
        {
            _searchService = searchService;
        }

        public override IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model,
            IDictionary<string, string> filters)
        {
            var items = _searchService.Search(filters);
            var content = new List<RuntimeContentModel>();

            foreach (var url in items)
                content.Add(GetCachedContent(url));

            return content;
        }
    }
}
