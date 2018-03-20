using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecron.Jobs;
using Sitecron.Scheduling;

namespace Sitecron.Pipelines.Initialize
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
            _scheduleManager.ScheduleAllJobs();
        }
    }
}