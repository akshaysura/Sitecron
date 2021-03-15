using Quartz;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecron.Core.Jobs;
using Sitecron.SitecronSettings;
using System;
using System.Threading;

namespace Sitecron.Jobs.SitecoreJob
{
    public class RunAsSitecoreJob : IJob //Inherit from IJob interface from Quartz
    {
        public void Execute(IJobExecutionContext context)
        {
            //simple type and method call without params
            try
            {
                //get job parameters
                JobDataMap dataMap = context.JobDetail.JobDataMap; //get the datamap from the Quartz job 
                var job = (SitecronJob)dataMap[SitecronConstants.ParamNames.SitecronJob]; //get the SitecronJob from the job definition
                if (job == null)
                    throw new Exception("Unable to load SiteCron Job Definition");

                Type type = Type.GetType(job.SitecoreJobType);
                if (type == null)
                    throw new Exception("Unable to resolve the Sitecore Job Type specified: " + job.SitecoreJobType);

                object instance = Activator.CreateInstance(type);
                if (instance == null)
                    throw new Exception("Unable to instantiate the Sitecore Job Type specified: " + job.SitecoreJobType);

                DefaultJobOptions options = new DefaultJobOptions(job.SitecoreJobName, job.SitecoreJobCategory, job.SitecoreJobSiteName, instance, job.SitecoreJobMethod);

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
                Log.Error("SiteCron: RunAsSitecoreJob: ERROR something went wrong - " + ex.Message, ex, this);
                context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "Sitecron: RunAsSitecoreJob: ERROR something went wrong - " + ex.Message);
            }
        }
    }
}