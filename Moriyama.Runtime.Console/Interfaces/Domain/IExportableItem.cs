namespace Moriyama.Content.Export.Interfaces.Domain
{
    public interface IExportableItem<T>
    {
        string Path { get; set; }
        T Content { get; set; }
    }
}
