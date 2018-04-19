using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecron.Core.Jobs.Loader;
using System.Collections.Generic;
using System.Linq;

namespace Sitecron.Core.Jobs
{
    public class ConfigJobProvider : ISitecronJobProvider
    {
        private readonly ISitecronJobValidator _jobValidator;

        public ConfigJobProvider(ISitecronJobValidator jobValidator)
        {
            Assert.ArgumentNotNull(jobValidator, nameof(jobValidator));
            _jobValidator = jobValidator;
        }
        public IEnumerable<SitecronJob> GetJobs()
        {
            var siteCronJobs = (Factory.CreateObject("sitecronJobLoader", false) as ConfigJobLoader).SiteCronConfigJobs;
            return siteCronJobs.Where(_jobValidator.IsValid);
        }
    }
}