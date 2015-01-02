using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Umbraco.Interfaces
{
    public interface IUmbracoContentParser
    {
        RuntimeContentModel ParseContent(RuntimeContentModel model);
    }
}
