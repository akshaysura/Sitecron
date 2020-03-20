using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecron.Core.Pipelines
{
    public class InitializeSitecron
    {
        private readonly IScheduleManager _scheduleManager;

        public InitializeSitecron(IScheduleManager scheduleManager)
        {
            Assert.ArgumentNotNull(scheduleManager, nameof(scheduleManager));
            _scheduleManager = scheduleManager;
        }

        public virtual void Process(PipelineArgs args)
        {
            _scheduleManager.CleanUpExistingJobs();
            _scheduleManager.ScheduleAllJobs();
        }
    }
}