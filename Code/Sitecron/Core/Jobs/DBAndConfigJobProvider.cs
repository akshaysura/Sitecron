using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;

namespace Sitecron.Core.Jobs
{
    public class DBAndConfigJobProvider : ISitecronJobProvider
    {
        private readonly ISitecronJobValidator _jobValidator;
        public DBAndConfigJobProvider(ISitecronJobValidator jobValidator)
        {
            Assert.ArgumentNotNull(jobValidator, nameof(jobValidator));
            _jobValidator = jobValidator;
        }
        public IEnumerable<SitecronJob> GetJobs()
        {
            InheritingSitecronJobProvider dbProvider = new InheritingSitecronJobProvider(_jobValidator);
            ConfigJobProvider configProvider = new ConfigJobProvider(_jobValidator);
            List<SitecronJob> jobs = new List<SitecronJob>();
            jobs.AddRange(dbProvider.GetJobs());
            jobs.AddRange(configProvider.GetJobs());

            return jobs;
        }
    }
}