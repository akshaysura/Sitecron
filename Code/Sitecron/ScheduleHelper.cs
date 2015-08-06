using Quartz;
using Quartz.Impl;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
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

            if (!string.IsNullOrEmpty(publishingInstance) && !string.IsNullOrEmpty(instanceName) && publishingInstance != instanceName)
            {
                Log.Info(string.Format("Sitecron - Exit without initialization, this server is not the primary in the load balanced environment. {0}=={1}", publishingInstance, instanceName), this);
                return;
            }
            else
            {
                Log.Info("Initialize Sitecron", this);

                try
                {
                    //currently we are restricting this module to run using Master
                    Database masterDb = Factory.GetDatabase("master");
                    if (masterDb != null)
                    {
                        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                        scheduler.Start();

                        //get a list of all items in Sitecron folder and iterate through them
                        //add them to the schedule
                        string queryRetriveJobs = "fast:/sitecore/system/Modules/Sitecron//*[@@templateid = '{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}']";

                        scheduler.Clear();

                        Item[] sitecronJobs = masterDb.SelectItems(queryRetriveJobs);

                        Log.Info("Loading Sitecron Jobs", this);

                        if (sitecronJobs != null && sitecronJobs.Count() > 0)
                        {
                            foreach (Item i in sitecronJobs)
                            {
                                Log.Info("Loading Sitecron Job:" + i.Name, this);

                                if (!string.IsNullOrEmpty(i[FieldNames.Type]) && !string.IsNullOrEmpty(i[FieldNames.CronExpression]))
                                {
                                    Type jobType = Type.GetType(i[FieldNames.Type]);
                                    if (jobType != null)
                                    {
                                        IJobDetail jobDetail = JobBuilder.Create(jobType).Build();
                                        jobDetail.JobDataMap.Add(FieldNames.Parameters, i[FieldNames.Parameters]);

                                        if (!string.IsNullOrEmpty(i[FieldNames.Items]))
                                        {
                                            jobDetail.JobDataMap.Add(FieldNames.Items, i[FieldNames.Items]);
                                        }

                                        ITrigger trigger = TriggerBuilder.Create()
                                            .WithIdentity(i.ID.ToString())
                                            .WithCronSchedule(i[FieldNames.CronExpression])
                                            .ForJob(jobDetail)
                                            .Build();
                                        scheduler.ScheduleJob(jobDetail, trigger);

                                        Log.Info(string.Format("Sitecron - Loaded Job: {0} Type: {1} Cron Expression: {2} Parameters: {3}", i.Name, i[FieldNames.Type], i[FieldNames.CronExpression], i[FieldNames.Parameters]), this);
                                    }
                                    else
                                        Log.Info(string.Format("Sitecron - Job Not Loaded - Could not load Type: {0} Type: {1} Cron Expression: {2}", i.Name, i[FieldNames.Type], i[FieldNames.CronExpression]), this);
                                }
                                else
                                    Log.Info(string.Format("Sitecron - Job Not Loaded Invalid Type or Cron Expression: {0} Type: {1} Cron Expression: {2}", i.Name, i[FieldNames.Type], i[FieldNames.CronExpression]), this);
                            }
                        }
                    }
                    else
                        Log.Info("Sitecron - Exit, Master db not found.", this);
                }
                catch (Exception ex)
                {
                    Log.Error("Sitecron ERROR: " + ex.Message, this);
                }
            }
        }
    }
}