using Umbraco.Core.Models;

namespace Moriyama.Runtime.Console.Application.Domain
{
    public class ExportableContent
    {
        public string Path { get; set; }
        public IContent Content { get; set; }
    }
}
