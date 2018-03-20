using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecron.Jobs;

namespace Sitecron.Scheduling
{
    public interface ISitecronScheduler
    {
        void ClearJobs();

        void ScheduleJob(SitecronJob job);
    }
}
