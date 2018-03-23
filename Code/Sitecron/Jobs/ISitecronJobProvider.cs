using System.Collections.Generic;

namespace Sitecron.Jobs
{
    public interface ISitecronJobProvider
    {
        IEnumerable<SitecronJob> GetJobs();
    }
}
