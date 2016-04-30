using System.Collections.Generic;
using Moriyama.Content.Export.Interfaces.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportableFinder
    {
        IEnumerable<IExportable> FindExportable();
    }
}
