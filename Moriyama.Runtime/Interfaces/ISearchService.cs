using System.Collections.Generic;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Interfaces
{
    public interface ISearchService
    {
        void Index(RuntimeContentModel model);
        void Delete(string url);

        IEnumerable<SearchResultModel> Search(string query);

        IEnumerable<string> Search(IDictionary<string, string> matches);

    }
}
