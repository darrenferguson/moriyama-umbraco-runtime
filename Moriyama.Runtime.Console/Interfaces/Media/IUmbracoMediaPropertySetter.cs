using System.Collections.Generic;
using Moriyama.Content.Export.Application.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Interfaces.Media
{
    public interface IUmbracoMediaPropertySetter
    {
        IMedia SetProperties(IMedia content, ExportMediaModel model, IEnumerable<IExportContentParser> parsers);
    }
}