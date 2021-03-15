using System;
using Quartz;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Configuration;
using Sitecron.SitecronSettings;
using Sitecron.Core.Jobs;
using System.Threading;

namespace Sitecron.Jobs.SitecoreJob
{
    public class SitecoreScheduleCommandJob : IJob //Inherit from IJob interface from Quartz
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //get job parameters
                JobDataMap dataMap = context.JobDetail.JobDataMap; //get the datamap from the Quartz job 

                var job = (SitecronJob)dataMap[SitecronConstants.ParamNames.SitecronJob]; //get the SitecronJob from the job definition
                if (job == null)
                    throw new Exception("Unable to load SiteCron Job Definition");

                string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master");
                Database contextDb = Factory.GetDatabase(contextDbName);

                Item jobItem = contextDb.GetItem(new ID(job.SitecoreScheduleJob));
                if (jobItem == null)
                    throw new Exception("Unable to load Sitecore Schedule Job Item: " + job.SitecoreScheduleJob);

                ScheduleItem scheduleItem = new ScheduleItem(jobItem);
                if (scheduleItem == null)
                    throw new Exception("Invalid Sitecore Job item specified: " + job.SitecoreJobType);

                Type type = Type.GetType(scheduleItem.CommandItem.InnerItem[SitecronConstants.FieldNames.Type]);
                if (type == null)
                    throw new Exception("Unable to resolve the Sitecore Job Type specified: " + job.SitecoreJobType);

                object instance = Activator.CreateInstance(type);
                if (instance == null)
                    throw new Exception("Unable to instantiate the Sitecore Job Type specified: " + job.SitecoreJobType);

                DefaultJobOptions options = new DefaultJobOptions(job.SitecoreJobName, job.SitecoreJobCategory, job.SitecoreJobSiteName, instance, scheduleItem.CommandItem.InnerItem[SitecronConstants.FieldNames.Method], new object[] { scheduleItem.Items, scheduleItem.CommandItem, scheduleItem });

                ThreadPriority jobPriority;
                if (Enum.TryParse<ThreadPriority>(job.SitecoreJobPriority, out jobPriority))
                    options.Priority = jobPriority;
                else
                    options.Priority = ThreadPriority.Normal;

                JobManager.Start(options);

                context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "Sitecron: RunAsSitecoreJob: Done");
            }
            catch (Exception ex)
            {
                Log.Error("SiteCron: SitecoreScheduleCommandJob: ERROR something went wrong - " + ex.Message, ex, this);
                context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "Sitecron: SitecoreScheduleCommandJob: ERROR something went wrong - " + ex.Message);
            }
        }
    }
}