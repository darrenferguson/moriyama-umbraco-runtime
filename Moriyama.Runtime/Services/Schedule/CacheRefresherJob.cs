using System.Reflection;
using log4net;
using Quartz;

namespace Moriyama.Runtime.Services.Schedule
{
    [DisallowConcurrentExecution]
    public class CacheRefresherJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info(GetType().Name + " scheduled task..");

            var contentService = RuntimeContext.Instance.ContentService;

            if (contentService is CachedRuntimeContentService)
            {
                var cachedService = (CachedRuntimeContentService) contentService;
                cachedService.SanitiseCache();
            }
        }
    }
}
