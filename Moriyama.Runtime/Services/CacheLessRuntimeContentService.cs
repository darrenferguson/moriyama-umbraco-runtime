using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moriyama.Runtime.Extension;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;
using Newtonsoft.Json;
using log4net;

namespace Moriyama.Runtime.Services
{
    public class CacheLessRuntimeContentService : IContentService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IContentPathMapper PathMapper;
        private readonly object _lock;

        protected readonly List<string> Urls;

        private DateTime _lastUrlFlush;
        private readonly string _urlPath;

        public CacheLessRuntimeContentService(IContentPathMapper pathMapper)
        {
            PathMapper = pathMapper;
            Urls = new List<string>();
            _lock = new object();

            _lastUrlFlush = DateTime.Now;

            _urlPath = Path.Combine(PathMapper.ContentRootFolder("/"), "sitemap-urls.json");

            if (!File.Exists(_urlPath)) return;

            var urls = File.ReadAllText(_urlPath);
            Urls = JsonConvert.DeserializeObject<List<string>>(urls);
        }

        public void AddContent(RuntimeContentModel model)
        {
            var targetPath = PathMapper.PathForUrl(model.Url, true);
            var serialisedContent = JsonConvert.SerializeObject(model, Formatting.Indented);

            lock (_lock)
            {
                File.WriteAllText(targetPath, serialisedContent);

                if (!Urls.Contains(model.Url))
                    Urls.Add(model.Url);

            }
            FlushUrls();
        }

        public void RemoveContent(string url)
        {
            var targetPath = PathMapper.PathForUrl(url, true);

            if (!File.Exists(targetPath))
                return;

            var fileInfo = new FileInfo(targetPath);
            // var directoryInfo = fileInfo.Directory;

            lock (_lock)
            {
                File.Delete(targetPath);
                
                if (Urls.Contains(url))
                    Urls.Remove(url);

                // CleanEmptyDirectory(directoryInfo.FullName);
            }
            FlushUrls();
        }

        public IEnumerable<string> GetUrlList()
        {
            return Urls;
        }

        protected void FlushUrls()
        {
            // TODO: Configure flush interval
            // if (_lastUrlFlush >= DateTime.Now.AddSeconds(-30)) return;
            lock (_lock)
            {
                _lastUrlFlush = DateTime.Now;
                var urls = JsonConvert.SerializeObject(Urls);
                File.WriteAllText(_urlPath, urls);
            }
        }

        public virtual RuntimeContentModel GetContent(string url)
        {
            Logger.Info("Got from disk " + url);
            var contentFile = PathMapper.PathForUrl(url, false);
        
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
            var a = Urls.Where(x => model.Url.StartsWith(x)).OrderBy(x => x.Length);
            return a.First();
        }

        public virtual RuntimeContentModel Home(RuntimeContentModel model)
        {
            return GetContent(HomeUrl(model));
        }

        protected IEnumerable<string> TopNavigationUrls(RuntimeContentModel model)
        {
            return Urls.Where(x => x.Split('/').Length == 5 && x.StartsWith(model.Home().Url));
        }

        public virtual IEnumerable<RuntimeContentModel> TopNavigation(RuntimeContentModel model)
        {
            return FromUrls(TopNavigationUrls(model)).Where(x => x != null);   
        }

        protected IEnumerable<string> ChildrenUrls(RuntimeContentModel model)
        {
            return Urls.Where(x => x.StartsWith(model.Url) && x != model.Url && x.Split('/').Length == model.Url.Split('/').Length + 1);
        }

        public virtual IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model)
        {
            return FromUrls(ChildrenUrls(model)).Where(x => x != null);
        }

        protected IEnumerable<string> DescendantsUrls(RuntimeContentModel model)
        {
            return Urls.Where(x => x.StartsWith(model.Url) && x != model.Url);
        }

        public virtual IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model)
        {
            return FromUrls(DescendantsUrls(model)).Where(x => x != null);
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
