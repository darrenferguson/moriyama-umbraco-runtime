namespace Moriyama.Content.Export.Interfaces.Domain
{
    public interface IPathedExportable : IExportable
    {
        string Path { get;  }
    }
}
