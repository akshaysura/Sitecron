using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;
using Sitecron.Core.Quartz.Listeners;
using Sitecron.Core.Jobs;
using Sitecore;

namespace Sitecron.Core.Scheduling
{
    public class QuartzSitecronScheduler : ISitecronScheduler
    {
        private IScheduler _scheduler;
        protected IScheduler Scheduler => _scheduler ?? (_scheduler = InitializeScheduler());

        protected virtual IScheduler InitializeScheduler()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            //attach Job listener to pickup status on all jobs in all groups
            scheduler.ListenerManager.AddJobListener(new CustomJobListener(), GroupMatcher<JobKey>.AnyGroup());

            return scheduler;
        }

        public void ClearJobs()
        {
            Scheduler.Clear();
        }

        public void ScheduleJob(SitecronJob job)
        {
            var jobDetail = CreateJobDetail(job);

            if (!string.IsNullOrEmpty(job.CronExpression))
            {
                if (CronExpression.IsValidExpression(job.CronExpression))
                {
                    Log.Info($"Sitecron - Job Loaded - {job.Name} Type: {job.JobTypeSignature} USING Cron Expression: {job.CronExpression} Parameters: {job.Parameters}", this);
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(job.ItemId)
                        .WithCronSchedule(job.CronExpression)
                        .ForJob(jobDetail)
                        .Build();
                    Scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                    Log.Info($"Sitecron - Job NOT Loaded - Invalid CRON Expression - {job.Name} Type: {job.JobTypeSignature} USING Cron Expression: {job.CronExpression} Parameters: {job.Parameters}", this);
            }

            if (job.ExecuteExactlyAtDateTime.Value != DateTime.MinValue)
            {
                Log.Info(
                    $"Sitecron - Job Loaded - {job.Name} Type: {job.JobTypeSignature} USING ExecuteExactlyAtDateTime: {DateUtil.ToServerTime(job.ExecuteExactlyAtDateTime.Value)} Parameters: {job.Parameters}", this);
                var startDateTime =
                    new DateTimeOffset(job.ExecuteExactlyAtDateTime.Value.ToUniversalTime());
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.ItemId)
                    .StartAt(startDateTime)
                    .ForJob(jobDetail)
                    .Build();
                Scheduler.ScheduleJob(jobDetail, trigger);
            }
        }

        protected IJobDetail CreateJobDetail(SitecronJob job)
        {
            var jobDetail = JobBuilder.Create(job.JobType).Build();
            var jobParams = string.IsNullOrEmpty(job.Parameters) ? job.Parameters + "&" : string.Empty;
            jobParams += $"{SitecronConstants.ParamNames.zSiteCronItemID}={job.ItemId}";
            jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.Parameters, jobParams);

            if (!string.IsNullOrEmpty(job.Items))
            {
                jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.Items, job.Items);
            }

            jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.ArchiveAfterExecution,
                job.ArchiveAfterExecution ? "1" : "0");
            jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.ItemID, job.ItemId);
            jobDetail.JobDataMap.Add(SitecronConstants.ParamNames.Name, job.Name);
            jobDetail.JobDataMap.Add(SitecronConstants.ParamNames.SitecronJob, job);
            jobDetail.JobDataMap.Add(SitecronConstants.ParamNames.SitecronJobLogData, ""); //Parameter used to log execution reports
            return jobDetail;
        }
    }
}