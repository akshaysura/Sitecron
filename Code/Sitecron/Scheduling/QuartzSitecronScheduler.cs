using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sitecore.Diagnostics;
using Sitecron.Jobs;
using Sitecron.Listeners;
using Sitecron.SitecronSettings;

namespace Sitecron.Scheduling
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
                Log.Info($"Sitecron - Job Loaded - {job.Name} using CronExpression: {job.CronExpression}",
                    this);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.ItemId)
                    .WithCronSchedule(job.CronExpression)
                    .ForJob(jobDetail)
                    .Build();
                Scheduler.ScheduleJob(jobDetail, trigger);
            }

            if (job.ExecuteExactlyAtDateTime.HasValue)
            {
                Log.Info(
                    $"Sitecron - Job Loaded - {job.Name} using ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime}",
                    this);
                var startDateTime =
                    new DateTimeOffset(job.ExecuteExactlyAtDateTime.Value.ToUniversalTime());
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.ItemId)
                    .StartAt(startDateTime)
                    .ForJob(jobDetail)
                    .Build();
                Scheduler.ScheduleJob(jobDetail, trigger);
            }

            Log.Info(
                $"Sitecron - Loaded Job: {job.Name} Type: {job.JobTypeSignature} Cron Expression: {job.CronExpression} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime} Parameters: {job.Parameters}",
                this);
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
            jobDetail.JobDataMap.Add(SitecronConstants.ParamNames.SitecronJob, job);

            return jobDetail;
        }
    }
}