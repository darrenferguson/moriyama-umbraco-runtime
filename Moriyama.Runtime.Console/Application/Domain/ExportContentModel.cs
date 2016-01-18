using Moriyama.Content.Export.Application.Domain.Abstract;

namespace Moriyama.Content.Export.Application.Domain
{
    public class ExportContentModel : BaseExportModel
    {
        public bool Published { get; set; }
        public string Template { get; set; }

    }
}
