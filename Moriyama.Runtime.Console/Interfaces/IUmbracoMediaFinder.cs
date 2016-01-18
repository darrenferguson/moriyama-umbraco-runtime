using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoMediaFinder
    {
        IEnumerable<IMedia> FindAllMedia();
    }
}
