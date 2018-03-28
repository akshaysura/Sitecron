using System.Collections.Generic;

namespace Sitecron.Core.Jobs
{
    public interface ISitecronJobProvider
    {
        IEnumerable<SitecronJob> GetJobs();
    }
}
