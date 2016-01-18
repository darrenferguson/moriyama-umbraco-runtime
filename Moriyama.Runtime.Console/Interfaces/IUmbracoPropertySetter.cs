using System.Collections;
using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoPropertySetter
    {

        IContent SetProperties(IContent content, ExportContentModel model, IEnumerable<IExportContentParser> parsers);

    }
}
