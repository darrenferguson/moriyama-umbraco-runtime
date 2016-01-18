using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportableContentFactory
    {
        IEnumerable<ExportableContent> GetExportableContent(IEnumerable<IContent> content);
        string GetPath(IContent content, IEnumerable<IContent> allContent);
    }
}
