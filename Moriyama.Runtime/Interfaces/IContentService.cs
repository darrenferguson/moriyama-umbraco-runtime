using System;
using System.Collections.Generic;
using System.Web;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Interfaces
{
    public delegate void ContentAddedHandler(RuntimeContentModel sender, EventArgs e);
    public delegate void ContentRemovedHandler(string sender, EventArgs e);

    public interface IContentService
    {
        event ContentAddedHandler Added;
        event ContentRemovedHandler Removed;

        void AddContent(RuntimeContentModel model);
        void RemoveContent(string url);

        IEnumerable<string> GetUrlList();

        RuntimeContentModel GetContent(HttpContext context);
        string GetContentUrl(HttpContext context);

        RuntimeContentModel GetContent(string url);
        RuntimeContentModel Home(RuntimeContentModel model);

        IEnumerable<RuntimeContentModel> TopNavigation(RuntimeContentModel model);
        IEnumerable<RuntimeContentModel> Children(RuntimeContentModel model);

        IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model);
        IEnumerable<RuntimeContentModel> Descendants(RuntimeContentModel model, IDictionary<string, string> filter);

        RuntimeContentModel CreateContent(string url, IDictionary<string, object> properties);
    }
}
