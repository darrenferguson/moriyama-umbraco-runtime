using System.Collections;
using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentCreator
    {

        IEnumerable<ExportableContent> GetAllContent();

        ContentCreateResult Create(ExportContentModel content);
    }
}
