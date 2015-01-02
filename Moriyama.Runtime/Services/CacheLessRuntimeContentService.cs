using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moriyama.Runtime.Extension;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;
using Newtonsoft.Json;

namespace Moriyama.Runtime.Services
{
    public class CacheLessRuntimeContentService : IContentService
    {
        private readonly IContentPathMapper _pathMapper;
        private readonly object _lock;

        private readonly List<string> _urls;

        private DateTime _lastUrlFlush;
        private readonly string _urlPath;

        public CacheLessRuntimeContentService(IContentPathMapper pathMapper)
        {
            _pathMapper = pathMapper;
            _urls = new List<string>();
            _lock = new object();

            _lastUrlFlush = DateTime.Now;

            _urlPath = Path.Combine(_pathMapper.ContentRootFolder("/"), "sitemap-urls.json");

            if (!File.Exists(_urlPath)) return;

            var urls = File.ReadAllText(_urlPath);
            _urls = JsonConvert.DeserializeObject<List<string>>(urls);
        }

        public void AddContent(RuntimeContentModel model)
        {
            var targetPath = _pathMapper.PathForUrl(model.Url, true);
            var serialisedContent = JsonConvert.SerializeObject(model, Formatting.Indented);

            lock (_lock)
            {
                File.WriteAllText(targetPath, serialisedContent);

                if (!_urls.Contains(model.Url))
                    _urls.Add(model.Url);

            }
            FlushUrls();
        }

        public void RemoveContent(string url)
        {
            var targetPath = _pathMapper.PathForUrl(url, true);

            if (!File.Exists(targetPath))
                return;

            var fileInfo = new FileInfo(targetPath);
            // var directoryInfo = fileInfo.Directory;

            lock (_lock)
            {
                File.Delete(targetPath);
                
                if (_urls.Contains(url))
                    _urls.Remove(url);

                // CleanEmptyDirectory(directoryInfo.FullName);
            }
            FlushUrls();
        }

        public IEnumerable<string> GetUrlList()
        {
            return _urls;
        }

        private void FlushUrls()
        {
            // TODO: Configure flush interval
            if (_lastUrlFlush >= DateTime.Now.AddSeconds(-30)) return;
            lock (_lock)
            {
                _lastUrlFlush = DateTime.Now;
                var urls = JsonConvert.SerializeObject(_urls);
                File.WriteAllText(_urlPath, urls);
            }
        }

        public virtual RuntimeContentModel GetContent(string url)
        {
            var contentFile = _pathMapper.PathForUrl(url, false);
            return !File.Exists(contentFile) ? null : FromFile(contentFile);
        }

        private RuntimeContentModel FromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RuntimeContentModel>(json);
        }

        protected string HomeUrl(RuntimeContentModel model)
        {
            var a = _urls.Where(x => model.Url.StartsWith(x)).OrderBy(x => x.Length);
            return a.First();
        }

        public virtual RuntimeContentModel Home(RuntimeContentModel model)
        {
            return GetContent(HomeUrl(model));
        }

        protected IEnumerable<string> TopNavigationUrls(RuntimeContentModel model)
        {
            return _urls.Where(x => x.Split('/').Length == 5 && x.StartsWith(model.Home().Url));
        }

        public virtual IEnumerable<RuntimeContentModel> TopNavigation(RuntimeContentModel model)
        {
            return FromUrls(TopNavigationUrls(model));   
        }

        protected IEnumerable<string> ChildrenUrls(RuntimeContentModel model)
        {
            return _urls.Where(x => x.StartsWith(model.Url) && x != model.Url && x.Split('/').Length == model.Url.Split('/').Length + 1);
        }

        public virtual IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model)
        {
            return FromUrls(ChildrenUrls(model));
        }

        protected IEnumerable<string> DescendantsUrls(RuntimeContentModel model)
        {
            return _urls.Where(x => x.StartsWith(model.Url) && x != model.Url);
        }

        public virtual IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model)
        {
            return FromUrls(DescendantsUrls(model));
        }

        private IEnumerable<RuntimeContentModel> FromUrls(IEnumerable<string> urls)
        {
            var content = new List<RuntimeContentModel>();
            foreach (var url in urls)
            {
                content.Add(GetContent(url));

                // var p = Path.Combine(_pathMapper.ContentFolder(path), _pathMapper.GetContentFileName());
                // content.Add(FromFile(p));
            }
            return content;
        }
        
        //private void CleanEmptyDirectory(string path)
        //{
        //    if (!Directory.EnumerateFileSystemEntries(path).Any())
        //        Directory.Delete(path);
        //}
    }
}
