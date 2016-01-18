using System.Collections.Generic;
using AutoMapper;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Content
{
    public class UmbracoContentExportSerialiser : IUmbracoContentExportSerialiser<ExportContentModel, ExportableContent>
    {

        private readonly IEnumerable<IExportContentParser> _exportContentParsers;

        public UmbracoContentExportSerialiser(IEnumerable<IExportContentParser> exportContentParsers)
        {
            _exportContentParsers = exportContentParsers;
            Mapper.CreateMap<IContent, ExportContentModel>();
        }

        public ExportContentModel Serialise(ExportableContent export)
        {
            var model = Mapper.Map<ExportContentModel>(export.Content);

            model.Path = export.Path;
            model.Type = export.Content.ContentType.Alias;
            model.Template = export.Content.Template == null ? string.Empty : export.Content.Template.Alias;

            model.Content = new Dictionary<string, object>();
            model.Meta = new Dictionary<string, string>();

            foreach (var property in export.Content.Properties)
            {
                if (!model.Content.ContainsKey(property.Alias))
                    model.Content.Add(property.Alias, property.Value);
            }

            foreach (var contentParser in _exportContentParsers)
            {
                model = contentParser.ParseContent(model);
            }

            return model;
        }
    }
}
