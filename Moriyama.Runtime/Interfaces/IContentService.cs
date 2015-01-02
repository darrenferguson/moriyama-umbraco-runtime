using System.Collections.Generic;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Interfaces
{
    public interface IContentService
    {
        
        void AddContent(RuntimeContentModel model);
        void RemoveContent(string url);

        IEnumerable<string> GetUrlList();
            
        RuntimeContentModel GetContent(string url);
        RuntimeContentModel Home(RuntimeContentModel model);

        IEnumerable<RuntimeContentModel> TopNavigation(RuntimeContentModel model);
        IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model);

        IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model);
    }
}
