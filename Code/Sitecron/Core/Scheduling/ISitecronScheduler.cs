using Sitecron.Core.Jobs;

namespace Sitecron.Core.Scheduling
{
    public interface ISitecronScheduler
    {
        void ClearJobs();

        void ScheduleJob(SitecronJob job);
    }
}
