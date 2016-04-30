using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Application
{
    public class ExportablePather : IExportablePather
    {
        public IEnumerable<IPathedExportable> Path(IEnumerable<IExportable> exportables)
        {
            var content = exportables.Where(x => x.Type == ExportableType.Content).Select(x=> x.Entity).ToArray();
            var media = exportables.Where(x => x.Type == ExportableType.Media).Select(x => x.Entity).ToArray();

            var patheds = new List<PathedExportable>();

            foreach (var exportable in exportables)
            {
                var pathed = new PathedExportable();
                pathed.Entity = exportable.Entity;
                pathed.Type = exportable.Type;

                if (exportable.Type == ExportableType.Media)
                    pathed.Path = GetPath(exportable.Entity, media);
                else
                    pathed.Path = GetPath(exportable.Entity, content);

                patheds.Add(pathed);
            }

            return patheds;
        }

        private string GetPath(IUmbracoEntity content, IEnumerable<IUmbracoEntity> allContent)
        {
            var pathComponents = content.Path.Split(',').Select(int.Parse).ToList();

            var pathArray = new List<string>();

            foreach (var pathComponentId in pathComponents.Where(x => x > -1 && x != content.Id))
            {
                var pathComponentContent = allContent.FirstOrDefault(x => x.Id == pathComponentId);

                if (pathComponentContent != null)
                {
                    var name = pathComponentContent.Name;
                    string invalid = new string(System.IO.Path.GetInvalidPathChars());

                    foreach (char c in invalid)
                        name = name.Replace(c.ToString(), "-");

                    pathArray.Add(name);

                }
            }

            pathArray.Add(content.Name);

            return "/" + string.Join("/", pathArray);
        }
    }
}
