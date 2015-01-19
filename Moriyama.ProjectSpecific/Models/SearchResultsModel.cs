using System.Collections.Generic;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Models
{
    public class SearchResultsModel : RuntimeContentModel
    {
        public string Query { get; set; }
        public IEnumerable<SearchResultModel> SearchResults { get; set; }
    }
}