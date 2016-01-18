using System.Collections.Generic;
using AutoMapper;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Media
{
    public class UmbracoMediaExportSerialiser : IUmbracoContentExportSerialiser<ExportMediaModel, ExportableMedia>
    {

        private readonly IEnumerable<IExportContentParser> _exportContentParsers;

        public UmbracoMediaExportSerialiser(IEnumerable<IExportContentParser> exportContentParsers)
        {
            _exportContentParsers = exportContentParsers;
            Mapper.CreateMap<IMedia, ExportMediaModel>();
        }

        public ExportMediaModel Serialise(ExportableMedia export)
        {
            var model = Mapper.Map<ExportMediaModel>(export.Content);

            model.Path = export.Path;
            model.Type = export.Content.ContentType.Alias;
            
            model.Content = new Dictionary<string, object>();
            model.Meta = new Dictionary<string, string>();

            foreach (var property in export.Content.Properties)
            {
                if (!model.Content.ContainsKey(property.Alias))
                    model.Content.Add(property.Alias, property.Value);
            }

            foreach (var contentParser in _exportContentParsers)
            {
                // TODO: Sort this
                // model = contentParser.ParseContent(model);
            }

            return model;
        }
    }
}
