using Moriyama.Content.Export.Interfaces.Domain;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportHelper<T>
    {
        void Export(IExportableItem<T> items, IFileSystem fileSystem);
    }
}
