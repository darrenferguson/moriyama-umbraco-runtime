using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using log4net;
using Moriyama.Runtime.Application;
using Moriyama.Runtime.Models;
using Moriyama.Runtime.Umbraco.Application;
using Moriyama.Runtime.Umbraco.Application.Parser;
using Moriyama.Runtime.Umbraco.Interfaces;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Moriyama.Runtime.Umbraco
{
    public class RuntimeUmbracoContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static RuntimeUmbracoContext _instance;

        private RuntimeUmbracoContext() { }

        public IList<IDeploymentAdapter> DeploymentAdapters { get; private set; }

        public static RuntimeUmbracoContext Instance
        {
            get { return _instance ?? (_instance = new RuntimeUmbracoContext()); }
        }

        public IUmbracoContentSerialiser UmbracoContentSerialiser { get; private set; }

        public void Init(string path, UmbracoHelper helper)
        {
            Logger.Info(string.Format("Init {0} with {1}", GetType().Name, path));

            path = Path.Combine(path, "App_Data", "Moriyama", "content");

            var contentPathMapper = new ContentPathMapper(path);

            var parsers = new List<IUmbracoContentParser>
            {
                new NaviHideUmbracoContentParser(), 
                new NullValueUmbracoContentParser(),
                new LocalLinkUmbracoContentParser(helper)
            };

            UmbracoContentSerialiser = new UmbracoContentSerialiser(helper, parsers);
            Mapper.CreateMap<IPublishedContent, RuntimeContentModel>();
        }

        public void AddDeploymentAdapter(IDeploymentAdapter adapter)
        {
            if (DeploymentAdapters == null)
                DeploymentAdapters = new List<IDeploymentAdapter>();

            LogHelper.Info<RuntimeUmbracoContext>("Adding deployment adapter " + adapter.GetType().Name);

            DeploymentAdapters.Add(adapter);
        }

    }
}