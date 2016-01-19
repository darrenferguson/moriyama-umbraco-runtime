using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain.Result;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentImporter
    {
        IEnumerable<ContentCreateResult> Import(string path);
    }
}
