using Moriyama.Runtime.Console.Application.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Console.Interfaces
{
    public interface IUmbracoContentExportSerialiser
    {

        ExportContentModel Serialise(ExportableContent content);
    }
}
