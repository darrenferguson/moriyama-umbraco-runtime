namespace Moriyama.Content.Export.Interfaces
{
    public interface IUmbracoContentExportSerialiser<out T, in T2>
    {
        T Serialise(T2 content);
    }
}
