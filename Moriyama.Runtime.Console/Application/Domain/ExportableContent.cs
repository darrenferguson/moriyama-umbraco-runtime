using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Domain
{
    public class ExportableContent
    {
        public string Path { get; set; }
        public IContent Content { get; set; }
    }
}
