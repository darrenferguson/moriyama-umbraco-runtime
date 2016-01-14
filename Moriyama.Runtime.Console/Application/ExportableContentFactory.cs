using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Runtime.Console.Application.Domain;
using Moriyama.Runtime.Console.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Runtime.Console.Application
{
    public class ExportableContentFactory : IExportableContentFactory
    {
        public IEnumerable<ExportableContent> GetExportableContent(IEnumerable<IContent> contents)
        {
            var exportable = new List<ExportableContent>();

            foreach (var content in contents)
            {

                var path = DiscPath(content, contents);
                System.Console.WriteLine(path);

                exportable.Add(new ExportableContent {Content = content, Path = path});
            }

            return exportable;
        }

        private static string DiscPath(IContent content, IEnumerable<IContent> allContent)
        {
            var pathComponents = content.Path.Split(',').Select(int.Parse).ToList();

            var pathArray = new List<string>();

            foreach (var pathComponentId in pathComponents.Where(x => x > -1))
            {
                var pathComponentContent = allContent.FirstOrDefault(x => x.Id == pathComponentId);

                if (pathComponentContent != null)
                {
                    var name = pathComponentContent.Name;
                    string invalid = new string(Path.GetInvalidPathChars());

                    foreach (char c in invalid)
                        name = name.Replace(c.ToString(), "-");
                    
                    pathArray.Add(name);

                }
            }

            return "/" + string.Join("/", pathArray);
        }
    }
}
