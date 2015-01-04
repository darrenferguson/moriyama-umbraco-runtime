using System.Collections;
using System.Collections.Generic;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Interfaces
{
    public interface ISearchService
    {
        void Index(RuntimeContentModel model);
        void Delete(string url);
        
        IEnumerable<string> Search(string query);
    }
}
