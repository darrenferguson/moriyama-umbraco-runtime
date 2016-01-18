using System.Collections.Generic;
using Moriyama.Content.Export.Application.Abstract;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Content
{
    public class ExportableContentFactory : AbstractExportableContentFactory, IExportableContentFactory<ExportableContent, IContent>
    {
        public IEnumerable<ExportableContent> GetExportableContent(IContent[] contents)
        {
            var exportable = new List<ExportableContent>();
            foreach (var content in contents)
            {
                var path = GetPath(content, contents);
                exportable.Add(new ExportableContent {Content = content, Path = path});
            }
            return exportable;
        }
    }
}
