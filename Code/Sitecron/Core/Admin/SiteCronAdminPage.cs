using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sitecore.sitecore.admin;
using Sitecron.SitecronSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecron.Core.Admin
{
    public class SiteCronAdminPage : AdminPage
    {
        protected Repeater JobRepeater;

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckSecurity();

            if (IsPostBack)
            {
                return;
            }

            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                JobRepeater.DataSource = await GetJobs();
                JobRepeater.DataBind();
            }));
        }

        protected void JobRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Execute")
            {
                return;
            }

            var parts = e.CommandArgument.ToString().Split('.');
            var jobKey = JobKey.Create(parts[1], parts[0]);

            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.TriggerJob(jobKey);

                Response.Redirect(Request.Url.PathAndQuery, false);
                Context.ApplicationInstance.CompleteRequest();
            }));
        }

        private static async Task<List<QuartzJobDetail>> GetJobs()
        {
            var outputJobs = new List<QuartzJobDetail>();

            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            var jobGroups = await scheduler.GetJobGroupNames();
            foreach (string group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeysInGroup = await scheduler.GetJobKeys(groupMatcher);
                foreach (JobKey jobKey in jobKeysInGroup)
                {
                    var jobDetail = await scheduler.GetJobDetail(jobKey);
                    var jobTriggers = await scheduler.GetTriggersOfJob(jobKey);
                    foreach (ITrigger jobTrigger in jobTriggers)
                    {
                        QuartzJobDetail quartzJobDetail = new QuartzJobDetail
                        {
                            Group = group,
                            JobName = jobDetail.JobDataMap.Get(SitecronConstants.ParamNames.Name).ToString(),
                            Type = jobDetail.JobType.ToString(),
                            State = (await scheduler.GetTriggerState(jobTrigger.Key)).ToString(),
                            Key = jobKey
                        };

                        DateTimeOffset? nextFireTimeUtc = jobTrigger.GetNextFireTimeUtc();
                        if (nextFireTimeUtc.HasValue)
                        {
                            quartzJobDetail.NextFireTimeUtc = nextFireTimeUtc.Value.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
                        }

                        outputJobs.Add(quartzJobDetail);
                    }
                }
            }

            return outputJobs;
        }
    }

    public class QuartzJobDetail
    {
        public JobKey Key { get; set; }
        public string Group { get; set; }
        public string JobName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string NextFireTimeUtc { get; set; }
    }
}