using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Moriyama.Content.Export.Interfaces.Content;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application
{
    public class UmbracoContentPropertySetter : IUmbracoContentPropertySetter
    {

        public IContent SetProperties(IContent content, ExportContentModel model, IEnumerable<IExportContentParser> parsers)
        {
            if (model == null)
                return content;

            foreach (var parser in parsers)
            {
                var result = parser.ParseForImport(model);
                model.Content = result.Content;
                model.Meta = result.Meta;
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
