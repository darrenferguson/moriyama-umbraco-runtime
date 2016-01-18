using System.Collections.Generic;
using Moriyama.Content.Export.Application.Abstract;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Media
{
    public class ExportableMediaFactory : AbstractExportableContentFactory, IExportableContentFactory<ExportableMedia, IMedia>
    {
        public IEnumerable<ExportableMedia> GetExportableContent(IMedia[] contents)
        {
            var exportable = new List<ExportableMedia>();
            foreach (var content in contents)
            {
                var path = GetPath(content, contents);
                exportable.Add(new ExportableMedia { Content = content, Path = path });
            }
            return exportable;
        }
    }
}
