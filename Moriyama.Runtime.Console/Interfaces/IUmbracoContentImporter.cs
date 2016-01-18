using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentImporter
    {
        IEnumerable<ContentCreateResult> Import(IFileSystem fs);
    }
}
