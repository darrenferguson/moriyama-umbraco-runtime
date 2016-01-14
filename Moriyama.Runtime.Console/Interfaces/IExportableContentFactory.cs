using System.Collections;
using System.Collections.Generic;
using Moriyama.Runtime.Console.Application.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Console.Interfaces
{
    public interface IExportableContentFactory
    {
        IEnumerable<ExportableContent> GetExportableContent(IEnumerable<IContent> content);

    }
}
