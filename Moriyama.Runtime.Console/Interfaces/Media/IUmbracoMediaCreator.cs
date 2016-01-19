using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Result;

namespace Moriyama.Content.Export.Interfaces.Media
{
    public interface IUmbracoMediaCreator
    {
        IEnumerable<ExportableMedia> GetAllContent();
        MediaCreateResult Create(ExportMediaModel content);
    }
}
