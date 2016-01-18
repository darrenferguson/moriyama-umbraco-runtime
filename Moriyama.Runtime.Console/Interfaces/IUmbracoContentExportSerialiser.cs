using Moriyama.Content.Export.Application.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentExportSerialiser
    {
        ExportContentModel Serialise(ExportableContent content);
    }
}
