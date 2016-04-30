using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Application.Domain
{
    public class Exportable : IExportable
    {
        public ExportableType Type { get; set; }
        public IUmbracoEntity Entity { get; set; }
    }
}
