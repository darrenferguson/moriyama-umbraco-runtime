using Moriyama.Runtime.Models;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Umbraco.Interfaces
{
    public interface IUmbracoContentSerialiser
    {
        RuntimeContentModel Remove(IContent content);
        RuntimeContentModel Serialise(IContent content);
    }
}
