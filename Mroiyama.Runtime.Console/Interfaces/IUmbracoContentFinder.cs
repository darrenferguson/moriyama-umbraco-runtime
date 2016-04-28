using System.Collections.Generic;

namespace Mroiyama.Runtime.Console.Interfaces
{
    interface IUmbracoContentFinder
    {
        IEnumerable<int> GetAllUmbracoContentIds();
    }
}
