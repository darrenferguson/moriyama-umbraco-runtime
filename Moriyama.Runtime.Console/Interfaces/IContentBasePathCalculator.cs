using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IContentBasePathCalculator
    {

        string GetPath(ContentBase content, IEnumerable<ContentBase> allContent);
    }
}
