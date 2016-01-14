using Moriyama.Runtime.Console.Application.Domain;

namespace Moriyama.Runtime.Console.Interfaces
{
    public interface IExportContentParser
    {
        ExportContentModel ParseContent(ExportContentModel model);
    }
}
