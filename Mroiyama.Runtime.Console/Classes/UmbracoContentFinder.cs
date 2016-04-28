using Mroiyama.Runtime.Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Mroiyama.Runtime.Console.Classes
{
    class UmbracoContentFinder : IUmbracoContentFinder
    {
        private readonly ApplicationContext _applicationContext;

        public UmbracoContentFinder(ApplicationContext applicationContext)
        {
            this._applicationContext = applicationContext;
        }

        public IEnumerable<int> GetAllUmbracoContentIds()
        {
            var contentService = _applicationContext.Services.ContentService;

            var root = contentService.GetRootContent();
            return GetIds(root);
        }

        private IEnumerable<int> GetIds(IEnumerable<IContent> contents)
        {
            var ids = new List<int>();
            foreach (var content in contents)
            {
                System.Console.WriteLine(content.Id);
                ids.Add(content.Id);
                ids.AddRange(GetIds(content.Children()));
            }

            return ids;
        } 

    }
}
