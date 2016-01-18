using System.Collections.Generic;
using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentFinder<out T> where T : IUmbracoEntity
    {
        IEnumerable<T> FindAllContent();
    }
}
