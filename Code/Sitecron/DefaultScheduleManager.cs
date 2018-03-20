using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecron.Jobs;
using Sitecron.Scheduling;

namespace Sitecron
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
                Log.Info($"Sitecron - Exit without initialization, this server is not the primary in the load balanced environment. PublishingInstance: {publishingInstance} != InstanceName: {instanceName}", this);
                return;
            }

            try
            {
                Log.Info("Initialize Sitecron", this);
                _scheduler.ClearJobs();

                Log.Info("Loading Sitecron Jobs", this);

                foreach (var job in _jobProvider.GetJobs())
                {
                    _scheduler.ScheduleJob(job);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitecron ERROR: " + ex.Message, ex, this);
            }
        }
    }
}