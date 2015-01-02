using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using AutoMapper;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Services
{
    public class USyncContentService : IContentService
    {
        private string ContentPath { get; set; }
        
        public USyncContentService(string path)
        {
            ContentPath = path;
            
        }

        public void InitialiseFromDisc()
        {
            foreach (var file in Directory.EnumerateFiles(ContentPath, "*.content", SearchOption.AllDirectories))
            {
                var model = LoadUSyncContentFromDisc(file);
                
                model.FromCache = false;
                
                HttpRuntime.Cache.Insert(model.Url, model);
            }
        }

        public T GetContent<T>(string path) where T : RuntimeContentModel, new()
        {
            Mapper.CreateMap<RuntimeContentModel, T>();
            var model = GetContent(path);
            return Mapper.Map<T>(model);
        }

        public T GetContent<T>(HttpRequestBase request) where T : RuntimeContentModel, new()
        {
            Mapper.CreateMap<RuntimeContentModel, T>();
            var model = GetContent(request);
            return Mapper.Map<T>(model);
        }

        public RuntimeContentModel GetContent(HttpRequestBase request)
        {
            var path = request.Url.AbsolutePath;
            return GetContent(path);
        }

        public RuntimeContentModel GetContent(string path)
        {

            if (path != "/" && path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            if (HttpRuntime.Cache[path] != null)
            {
                var content = (RuntimeContentModel) HttpRuntime.Cache[path];
                if (content != null)
                {
                    content.FromCache = true;
                    return content;
                }
            }

            var model = LoadUSyncContentFromDisc(FilePathFromUrlPath(path));
            model.FromCache = false;

            HttpRuntime.Cache.Insert(model.Url, model);

            return model;
        }

        private RuntimeContentModel LoadUSyncContentFromDisc(string path)
        {

            var d = new XmlDocument();
            d.Load(path);

            var model = new RuntimeContentModel();

            //model.RawContent = d;
            model.ContentProperties = ExtractProperties(d.DocumentElement);

            model.Template = model.ContentProperties.ContainsKey("templateAlias")
                ? (string) model.ContentProperties["templateAlias"]
                : string.Empty;


            model.SortOrder = model.ContentProperties.ContainsKey("sortOrder")
                ? Convert.ToInt32(model.ContentProperties["sortOrder"])
                : 0;

            model.BuildTime = DateTime.Now;

            var url = path.Replace(ContentPath, "");
            url = url.Replace('\\', '/');
            if (url == "Home.content")
            {
                url = "/";
            }
            else
            {
                if (url.EndsWith(".content"))
                    url = url.Substring(0, url.Length - 8);

                if (url.StartsWith("Home/"))
                    url = url.Substring(4);
            }

            var altUrl = Regex.Replace(url, @"(?<!_)([A-Z])", "-$1");
            altUrl = altUrl.ToLower();
            altUrl = altUrl.Replace("/-", "/");

            model.Url = altUrl;
            model.Level = url.Split('/').Length;

            return model;
        }

        private IDictionary<string, object> ExtractProperties(XmlNode n)
        {
            var properties = new Dictionary<string, object>();

            if (n.Attributes != null)
            {
                foreach (XmlAttribute attribute in n.Attributes)
                {
                    if(!properties.ContainsKey(attribute.Name))
                        properties.Add(attribute.Name, attribute.Value);
                }
            }

            foreach (XmlNode child in n.ChildNodes)
            {
                var name = child.Name;
                if (properties.ContainsKey(name)) continue;

                var value = string.Empty;

                if (child.HasChildNodes && child.FirstChild is XmlCDataSection)
                {
                    value = child.FirstChild.InnerText;
                }
                else
                {
                    value = child.InnerText;
                }
                properties.Add(name, value);
            }

            return properties;
        }

        private string FilePathFromUrlPath(string urlPath)
        {
            urlPath = urlPath.Replace('/', '\\');

            urlPath = urlPath.Substring(1);

            var basePath = Path.Combine(ContentPath, urlPath);

            string contentFile;

            if (string.IsNullOrEmpty(urlPath))
            {
                contentFile = Path.Combine(basePath, "Home.content");
            }
            else
            {
                urlPath = urlPath.Replace(@"\", "");
                urlPath = urlPath.Replace("-", "");

                contentFile = Path.Combine(ContentPath, "Home", urlPath + ".content");
            }
            
            return contentFile;
        }
    }
}
