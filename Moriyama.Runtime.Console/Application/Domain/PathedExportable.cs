using Moriyama.Content.Export.Interfaces.Domain;

namespace Moriyama.Content.Export.Application.Domain
{
    public class PathedExportable : Exportable, IPathedExportable
    {
        public string Path { get; set; }
    }
}
