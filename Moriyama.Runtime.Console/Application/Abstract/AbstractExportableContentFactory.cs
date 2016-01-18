using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models.EntityBase;

namespace Moriyama.Content.Export.Application.Abstract
{
    public abstract class AbstractExportableContentFactory 
    {

       

        public string GetPath(IUmbracoEntity content, IEnumerable<IUmbracoEntity> allContent)
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
