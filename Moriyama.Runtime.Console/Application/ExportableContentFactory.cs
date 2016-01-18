using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application
{
    public class ExportableContentFactory : IExportableContentFactory
    {
        public IEnumerable<ExportableContent> GetExportableContent(IEnumerable<IContent> contents)
        {
            var exportable = new List<ExportableContent>();

            foreach (var content in contents)
            {

                var path = GetPath(content, contents);
                System.Console.WriteLine(path);

                exportable.Add(new ExportableContent {Content = content, Path = path});
            }

            return exportable;
        }
        
        public string GetPath(IContent content, IEnumerable<IContent> allContent)
        {
            var pathComponents = content.Path.Split(',').Select(int.Parse).ToList();

            var pathArray = new List<string>();
            
            foreach (var pathComponentId in pathComponents.Where(x => x > -1 && x != content.Id))
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

            pathArray.Add(content.Name);

            return "/" + string.Join("/", pathArray);
        }
    }
}
