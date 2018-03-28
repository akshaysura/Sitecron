using System;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Quartz
{
    [Obsolete("ScheduleHelper has been deprecated. Use DefaultScheduleManager or inject an IScheduleManager instead.")]
    public class ScheduleHelper
    {
        public void InitializeScheduler()
        {
            var manager = ServiceLocator.ServiceProvider.GetService(typeof(IScheduleManager)) as IScheduleManager;
            if (manager == null)
            {
                Log.Error(
                    "Could not resolve instance of Sitecron.Core.Scheduling.IScheduleManager. Check service registration configuration.", this);
            }
            else
            {
                manager.ScheduleAllJobs();
            }
        }
    }
}