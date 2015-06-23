using Quartz;
using Quartz.Impl;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                Log.Info(string.Format("Sitecron - Exit without initialization, this server is not the primary in the load balanced environment. {0}=={1}",publishingInstance, instanceName), this);
                return;
            }
            else
            {
                Log.Info("Initialize Sitecron", this);

                try
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();

                    //get a list of all items in Sitecron folder and iterate through them
                    //add them to the schedule

                    string queryRetriveJobs = "fast:/sitecore/system/Modules/Sitecron//*[@@templateid = '{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}']";
                    string typeField = "Type";
                    string cronExpressionField = "CronExpression";

                    scheduler.Clear();

                    Database masterDb = Factory.GetDatabase("master");
                    Item[] sitecronJobs = masterDb.SelectItems(queryRetriveJobs);
                    if (sitecronJobs != null && sitecronJobs.Count() > 0)
                    {
                        foreach (Item i in sitecronJobs)
                        {
                            if (!string.IsNullOrEmpty(i[typeField]) && !string.IsNullOrEmpty(i[cronExpressionField]))
                            {
                                Type jobType = Type.GetType(i[typeField]);
                                if (jobType != null)
                                {
                                    IJobDetail jobDetail = JobBuilder.Create(jobType).Build();

                                    ITrigger trigger = TriggerBuilder.Create()
                                        .WithIdentity(i.Name)
                                        .WithCronSchedule(i[cronExpressionField])
                                        .ForJob(jobDetail)
                                        .Build();
                                    scheduler.ScheduleJob(jobDetail, trigger);

                                    Log.Info(string.Format("Sitecron - Loading Job: {0} Type: {1} Cron Expression: {2}", i.Name, i[typeField], i[cronExpressionField]), this);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Sitecron ERROR: " + ex.Message, this);
                }
            }
        }
    }
}