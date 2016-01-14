using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Console.Interfaces
{
    public interface IUmbracoContentFinder
    {
        IEnumerable<IContent> FindAllContent();

    }
}
