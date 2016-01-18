using System.Collections.Generic;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IFileSystem
    {

        void Write(string path, string contents);
        IEnumerable<string> List();

    }
}
