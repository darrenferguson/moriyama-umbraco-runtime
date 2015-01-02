using Moriyama.Runtime.Models;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Umbraco.Interfaces
{
    public interface IUmbracoContentSerialiser
    {
        void Remove(IContent content);
        void Serialise(IContent content);
    }
}
