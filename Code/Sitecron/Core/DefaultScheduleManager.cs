using System;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecron.Core.Jobs;
using Sitecron.Core.Scheduling;
using Sitecron.SitecronSettings;

namespace Sitecron.Core
{
    public class DefaultScheduleManager : IScheduleManager
    {
        private readonly ISitecronJobProvider _jobProvider;
        private readonly ISitecronScheduler _scheduler;

        public DefaultScheduleManager(ISitecronJobProvider jobProvider, ISitecronScheduler scheduler)
        {
            Assert.ArgumentNotNull(jobProvider, nameof(jobProvider));
            Assert.ArgumentNotNull(scheduler, nameof(scheduler));

            _jobProvider = jobProvider;
            _scheduler = scheduler;
        }

        public void ScheduleAllJobs()
        {
            var publishingInstance = Settings.Publishing.PublishingInstance;
            var instanceName = Settings.InstanceName.ToLower();

            if (!string.IsNullOrEmpty(publishingInstance) &&
                !string.IsNullOrEmpty(instanceName) &&
                !publishingInstance.Equals(instanceName, StringComparison.OrdinalIgnoreCase))
            {
                Log.Info($"Sitecron - Exit without initialization, this server is not the primary in the load balanced environment. PublishingInstance: {publishingInstance} != InstanceName: {instanceName}", SitecronConstants.ParamNames.Log4NetLogger);
                return;
            }

            try
            {
                Log.Info("Initialize Sitecron", SitecronConstants.ParamNames.Log4NetLogger);
                _scheduler.ClearJobs();

                Log.Info("Loading Sitecron Jobs", SitecronConstants.ParamNames.Log4NetLogger);

                foreach (var job in _jobProvider.GetJobs())
                {
                    _scheduler.ScheduleJob(job);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitecron ERROR: " + ex.Message, ex, SitecronConstants.ParamNames.Log4NetLogger);
            }
        }
    }
}