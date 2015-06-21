using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;
using Newtonsoft.Json;

namespace Moriyama.Runtime.Services.Search
{
    public class DummySearchService : ISearchService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void IndexAll(IContentService contentService)
        {
        }

        public void Index(RuntimeContentModel model)
        {
            if(Logger.IsDebugEnabled)
                Logger.Debug(JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        public void Delete(string url)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("Delete " + url);
        }

        public IEnumerable<SearchResultModel> Search(string query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Search(IDictionary<string, string> matches)
        {
            throw new NotImplementedException();
        }
    }
}
