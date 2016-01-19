using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain.Result;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentExporter
    {

        IEnumerable<ExportResult> ExportContext(string path);

    }
}
