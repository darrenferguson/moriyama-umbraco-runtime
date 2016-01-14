using System;
using System.Collections.Generic;
using System.IO;
using Moriyama.Runtime.Console.Interfaces;

namespace Moriyama.Runtime.Console.Application
{
    public class FileSystem : IFileSystem
    {
        private readonly string _basePath;

        public FileSystem(string basePath)
        {
            _basePath = basePath;
        }

        public void Write(string path, string contents)
        {
            path = path.Replace("/", @"\");

            if (path.StartsWith(@"\"))
                path = path.Substring(1);

            string invalid = new string(Path.GetInvalidFileNameChars()).Replace(@"\", "");

            foreach (char c in invalid)
            {
                path = path.Replace(c.ToString(), "-");
            }

            var filename = Path.Combine(_basePath, path);
            var info = new FileInfo(filename);

            if (!info.Directory.Exists)
                Directory.CreateDirectory(info.DirectoryName);

            File.WriteAllText(filename, contents);

        }

        public IEnumerable<string> List()
        {
            return Directory.EnumerateFiles(_basePath, "*.*", SearchOption.AllDirectories);
        }
    }
}
