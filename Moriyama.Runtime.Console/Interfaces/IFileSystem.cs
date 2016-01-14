using System.Collections;
using System.Collections.Generic;

namespace Moriyama.Runtime.Console.Interfaces
{
    public interface IFileSystem
    {

        void Write(string path, string contents);
        IEnumerable<string> List();

    }
}
