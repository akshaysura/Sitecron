using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecron.Listeners;
using Sitecron.SitecronSettings;
using System;
using System.Linq;

namespace Sitecron
{
    public class ScheduleHelper
    {
        public void InitializeScheduler()
        {
            string publishingInstance = Settings.GetSetting("Publishing.PublishingInstance").ToLower();
            string instanceName = Settings.InstanceName.ToLower();
            string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB);

            if (!string.IsNullOrEmpty(publishingInstance) && !string.IsNullOrEmpty(instanceName) && publishingInstance != instanceName)
            {
                Log.Info(string.Format("Sitecron - Exit without initialization, this server is not the primary in the load balanced environment. PublishingInstance: {0} != InstanceName: {1}", publishingInstance, instanceName), this);
                return;
            }
            else
            {
                Log.Info("Initialize Sitecron", this);

                try
                {
                    //currently we are restricting this module to run using Master
                    Database contextDb = Factory.GetDatabase(contextDbName);

                    if (contextDb != null)
                    {
                        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                        scheduler.Start();

                        scheduler.Clear();

                        //attach Job listener to pickup status on all jobs in all groups
                        scheduler.ListenerManager.AddJobListener(new CustomJobListener(), GroupMatcher<JobKey>.AnyGroup());

                        //get a list of all items in Sitecron folder and iterate through them
                        //add them to the schedule

                        Item[] sitecronJobs = contextDb.SelectItems(SitecronConstants.Queries.QueryRetriveJobs);

                        Log.Info("Loading Sitecron Jobs", this);

                        if (sitecronJobs != null && sitecronJobs.Count() > 0)
                        {
                            foreach (Item i in sitecronJobs)
                            {
                                if (string.IsNullOrEmpty(i[SitecronConstants.FieldNames.Type]))
                                {
                                    Log.Info(string.Format("Sitecron - Job Not Loaded - {0} Invalid Type: {1}", i.Name, i[SitecronConstants.FieldNames.Type]), this);
                                    continue;
                                }

                                if (string.IsNullOrEmpty(i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]) && string.IsNullOrEmpty(i[SitecronConstants.FieldNames.CronExpression]))
                                {
                                    Log.Info(string.Format("Sitecron - Job Not Loaded - Invalid ExecuteExactlyAtDateTime or Cron Expression: {0} ExecuteExactlyAtDateTime: {1} Cron Expression: {2}", i.Name, i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime], i[SitecronConstants.FieldNames.CronExpression]), this);
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]) && !string.IsNullOrEmpty(i[SitecronConstants.FieldNames.CronExpression]))
                                {
                                    Log.Info(string.Format("Sitecron - Job Not Loaded - Both ExecuteExactlyAtDateTime and Cron Expression specified: {0} ExecuteExactlyAtDateTime: {1} Cron Expression: {2}", i.Name, i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime], i[SitecronConstants.FieldNames.CronExpression]), this);
                                    continue;
                                }

                                Type jobType = Type.GetType(i[SitecronConstants.FieldNames.Type]);
                                if (jobType == null)
                                {
                                    Log.Info(string.Format("Sitecron - Job Not Loaded - Could not load Type: {0} Type: {1} Cron Expression: {2}", i.Name, i[SitecronConstants.FieldNames.Type], i[SitecronConstants.FieldNames.CronExpression]), this);
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(i[SitecronConstants.FieldNames.Disable]) && i[SitecronConstants.FieldNames.Disable] == "1")
                                {
                                    Log.Info(string.Format("Sitecron - Job Not Loaded - Job Disabled: {0} Type: {1} Cron Expression: {2}", i.Name, i[SitecronConstants.FieldNames.Type], i[SitecronConstants.FieldNames.CronExpression]), this);
                                    continue;
                                }

                                IJobDetail jobDetail = JobBuilder.Create(jobType).Build();
                                string jobParams = i[SitecronConstants.FieldNames.Parameters];
                                if (string.IsNullOrEmpty(jobParams))
                                    jobParams = string.Concat(SitecronConstants.ParamNames.zSiteCronItemID, "=", i.ID.ToString());
                                else
                                    jobParams += string.Concat("&", SitecronConstants.ParamNames.zSiteCronItemID, "=" , i.ID.ToString());

                                jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.Parameters, jobParams);

                                if (!string.IsNullOrEmpty(i[SitecronConstants.FieldNames.Items]))
                                {
                                    jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.Items, i[SitecronConstants.FieldNames.Items]);
                                }

                                jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.ArchiveAfterExecution, i[SitecronConstants.FieldNames.ArchiveAfterExecution]);
                                jobDetail.JobDataMap.Add(SitecronConstants.FieldNames.ItemID, i.ID.ToString());

                                if (!string.IsNullOrEmpty(i[SitecronConstants.FieldNames.CronExpression]))
                                {
                                    Log.Info(string.Format("Sitecron - Job Loaded - {0} using CronExpression: {1}", i.Name, i[SitecronConstants.FieldNames.CronExpression]), this);
                                    ITrigger trigger = TriggerBuilder.Create()
                                        .WithIdentity(i.ID.ToString())
                                        .WithCronSchedule(i[SitecronConstants.FieldNames.CronExpression])
                                        .ForJob(jobDetail)
                                        .Build();
                                    scheduler.ScheduleJob(jobDetail, trigger);
                                }

                                if (!string.IsNullOrEmpty(i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]))
                                {
                                    var startDateField = (DateField)i.Fields[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime];
                                    if (startDateField != null)
                                    {
                                        Log.Info(string.Format("Sitecron - Job Loaded - {0} using ExecuteExactlyAtDateTime: {1}", i.Name, i[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]), this);
                                        DateTimeOffset startDateTime = new DateTimeOffset(startDateField.DateTime.ToUniversalTime());
                                        ITrigger trigger = TriggerBuilder.Create()
                                            .WithIdentity(i.ID.ToString())
                                            .StartAt(startDateTime)
                                            .ForJob(jobDetail)
                                            .Build();
                                        scheduler.ScheduleJob(jobDetail, trigger);
                                    }
                                }


                                Log.Info(string.Format("Sitecron - Loaded Job: {0} Type: {1} Cron Expression: {2} ExecuteExactlyAtDateTime: {3} Parameters: {4}", i.Name, i[SitecronConstants.FieldNames.Type], i[SitecronConstants.FieldNames.CronExpression], i[SitecronConstants.FieldNames.ArchiveAfterExecution], i[SitecronConstants.FieldNames.Parameters]), this);
                            }
                        }
                    }
                    else
                        Log.Warn("Sitecron - Exit, context db not found.", this);
                }
                catch (Exception ex)
                {
                    Log.Error("Sitecron ERROR: " + ex.Message, ex, this);
                }
            }
        }
    }
}