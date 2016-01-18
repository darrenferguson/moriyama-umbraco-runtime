using System.Collections.Generic;
using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportableContentFactory<out T1, in T2> where T2 : IUmbracoEntity
    {
        IEnumerable<T1> GetExportableContent(T2[] content);
        string GetPath(IUmbracoEntity content, IEnumerable<IUmbracoEntity> allContent);
    }
}
