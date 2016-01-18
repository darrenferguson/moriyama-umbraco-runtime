using Moriyama.Content.Export.Application.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportContentParser
    {

        string Name { get; }

        ExportContentModel ParseContent(ExportContentModel model);
        ExportContentModel ParseForImport(ExportContentModel model);

    }
}
