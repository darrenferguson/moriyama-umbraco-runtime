using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Interfaces.Domain
{
    public enum ExportableType
    {
        Content, Media
    }

    public interface IExportable
    {
        ExportableType Type { get; }
        IUmbracoEntity Entity { get; set; }
    }
}
