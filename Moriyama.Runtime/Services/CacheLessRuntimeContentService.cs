using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Moriyama.Runtime.Extension;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;
using Newtonsoft.Json;
using log4net;

namespace Moriyama.Runtime.Services
{
    public class CacheLessRuntimeContentService : IContentService
    {
        public event ContentAddedHandler Added;
        public virtual event ContentRemovedHandler Removed;


        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IContentPathMapper PathMapper;
        private readonly object _lock;

        protected List<string> Urls;

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

        public void RefreshUrls()
        {
            var urls = new List<string>();
            var files = Directory.GetFiles(PathMapper.ContentRootFolder("/"), "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Contains("sitemap-urls")) continue;

                var content = FromFile(file);
                urls.Add(content.Url);
            }

            Urls = urls;
            FlushUrls();
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

            if (Added != null)
                Added(model, new EventArgs());
      
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

            if (Removed != null)
                Removed(url, new EventArgs());
        }

        public IEnumerable<string> GetUrlList()
        {
            return Urls;
        }

        public RuntimeContentModel GetContent(HttpContext context)
        {
            return GetContent(GetContentUrl(context));
        }

        public string GetContentUrl(HttpContext context)
        {
            var url = context.Request.Url.Scheme + "://" + context.Request.Url.Host + context.Request.Url.AbsolutePath;
            return url;
        }

        protected void FlushUrls()
        {
            // TODO: Configure flush interval
            // if (_lastUrlFlush >= DateTime.Now.AddSeconds(-30)) return;
            lock (_lock)
            {
                _lastUrlFlush = DateTime.Now;
                var urls = JsonConvert.SerializeObject(Urls, Formatting.Indented);
                File.WriteAllText(_urlPath, urls);
            }
        }


        protected string ProcessUrlAliases(string url)
        {
            if (ConfigurationManager.AppSettings["Moriyama.Runtime.Domains"] == null) return url;

            foreach (var domain in ConfigurationManager.AppSettings["Moriyama.Runtime.Domains"].Split(','))
            {
                url = url.Replace("/" + domain + "/", "/localhost/");
            }

            return url;
        }

        public virtual RuntimeContentModel GetContent(string url)
        {
            url = ProcessUrlAliases(url);

            if(Logger.IsDebugEnabled)
                Logger.Debug("Got from disk " + url);

            var contentFile = PathMapper.PathForUrl(url, false);

            if (!File.Exists(contentFile))
            {
                if (Removed != null)
                    Removed(url, new EventArgs());

                return null;
            }

            var content = FromFile(contentFile);

            if (Added != null)
                Added(content, new EventArgs());
            
            return content;
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

        public virtual IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model, IDictionary<string, string> filters)
        {
            var descendants = Descendants(model);
            var filteredDescendants = new List<RuntimeContentModel>();

            foreach (var descendant in descendants)
            {
                var include = true;

                foreach (var filter in filters)
                {
                    var key = filter.Key;
                    var value = filter.Value;

                    if (
                        (descendant.Content.ContainsKey(key) && descendant.Content[key] != value)
                        ||
                        (HasProperty(descendant, key) && GetPropertyValue(descendant, key) != value)
                        )
                    {
                        include = false;
                    }
                }

                if (include)
                {
                    filteredDescendants.Add(descendant);
                }
            }
            return filteredDescendants;
        }

        private bool HasProperty(object o, string propertyName)
        {
            var property = o.GetType().GetProperty(propertyName);
            return property != null;
        }

        private string GetPropertyValue(object o, string propertyName)
        {
            var property = o.GetType().GetProperty(propertyName);
            return property.GetValue(o).ToString();
        }

        public RuntimeContentModel CreateContent(string url, IDictionary<string, object> properties)
        {
            url = ProcessUrlAliases(url);

            var content = new RuntimeContentModel();

            content.Url = url;
            
            content.CreateDate = DateTime.Now;
            content.UpdateDate = DateTime.Now;
            
            content.CreatorName = "User Generated";
            content.WriterName = "User Generated";

            content.RelativeUrl = new Uri(url).AbsolutePath;

            content.Type = "UserGeneratedContent";
            content.Template = string.Empty;

            content.Content = properties;

            return content;
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
