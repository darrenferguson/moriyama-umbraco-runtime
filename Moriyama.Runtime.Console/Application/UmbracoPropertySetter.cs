using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application
{
    public class UmbracoPropertySetter : IUmbracoPropertySetter
    {
        private readonly ApplicationContext _applicationContext;

        public UmbracoPropertySetter(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IContent SetProperties(IContent content, ExportContentModel model, IEnumerable<IExportContentParser> parsers)
        {
            if (model == null)
                return content;

            foreach (var parser in parsers)
            {
                model = parser.ParseForImport(model);
            }

            foreach (var property in model.Content)
            {
                if(content.HasProperty(property.Key))
                    content.SetValue(property.Key, property.Value);
            }

            return content;
        }
    }
}
