using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sitecore.sitecore.admin;
using Sitecron.SitecronSettings;
using System;
using System.Collections.Generic;

namespace Sitecron.Core.Admin
{
    public class SiteCronAdminPage : AdminPage
    {
        protected List<QuartzJobDetail> QuartzJobs;
        protected List<QuartzExecutingJobDetail> QuartzExecutingJobs;

        static SiteCronAdminPage()
        {

        }
        public SiteCronAdminPage()
        {
            QuartzJobs = new List<QuartzJobDetail>();
            QuartzExecutingJobs = new List<QuartzExecutingJobDetail>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.CheckSecurity();

            GetJobs();
            //GetExecutingJobs();
        }

        //private void GetExecutingJobs()
        //{
        //    var scheduler = StdSchedulerFactory.GetDefaultScheduler();

        //    var executingJobs = scheduler.GetCurrentlyExecutingJobs();
        //    foreach (var job in executingJobs)
        //    {
        //        var newJob = new QuartzExecutingJobDetail();
        //        newJob.JobName = job.JobDetail.JobDataMap.Get(SitecronConstants.ParamNames.Name).ToString();
        //        newJob.RunTime = (DateTime.Now.ToUniversalTime() - ((DateTimeOffset)job.FireTimeUtc).DateTime).TotalMinutes.ToString();

        //        this.QuartzExecutingJobs.Add(newJob);
        //    }

        //}
        private void GetJobs()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            IList<string> jobGroups = scheduler.GetJobGroupNames();

            foreach (string group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = scheduler.GetJobKeys(groupMatcher);
                foreach (var jobKey in jobKeys)
                {
                    var detail = scheduler.GetJobDetail(jobKey);
                    var triggers = scheduler.GetTriggersOfJob(jobKey);
                    foreach (ITrigger trigger in triggers)
                    {
                        var newJob = new QuartzJobDetail();
                        newJob.Group = group;
                        newJob.JobName = detail.JobDataMap.Get(SitecronConstants.ParamNames.Name).ToString();
                        newJob.Type = detail.JobType.ToString();
                        newJob.State = scheduler.GetTriggerState(trigger.Key).ToString();
                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            newJob.NextFireTimeUtc = nextFireTime.Value.LocalDateTime.ToString();
                        }
                        QuartzJobs.Add(newJob);
                    }
                }
            }
        }
    }

    public class QuartzExecutingJobDetail
    {
        public string JobName { get; set; }
        public string RunTime { get; set; }
    }
    public class QuartzJobDetail
    {
        public string Group { get; set; }
        public string JobName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string NextFireTimeUtc { get; set; }
    }
}