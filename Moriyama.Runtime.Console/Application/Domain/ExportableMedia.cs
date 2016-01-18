using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Domain
{
    public class ExportableMedia : IExportableItem<IMedia>
    {
        public string Path { get; set; }
        public IMedia Content { get; set; }
    }
}
