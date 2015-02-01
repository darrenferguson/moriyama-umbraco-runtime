using Moriyama.Blog.Project.Application;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Models
{
    public class SearchResultsModel : RuntimeContentModel
    {
        public string Query { get; set; }
        public PagedList<SearchResultModel> SearchResults { get; set; }
    }
}