using System;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Jobs
{
    public class SitecronJobValidator : ISitecronJobValidator
    {
        public bool IsValid(SitecronJob job)
        {
            if (job.JobType == null)
            {
                Log.Info($"Sitecron - Job Not Loaded - Could not load type '{job.JobTypeSignature}' for job {job.Name} - {job.ItemId}", SitecronConstants.ParamNames.Log4NetLogger);
                return false;
            }

            if (string.IsNullOrEmpty(job.CronExpression) &&  job.ExecuteExactlyAtDateTime.Value == DateTime.MinValue)
            {
                Log.Info($"Sitecron - Job Not Loaded - Invalid ExecuteExactlyAtDateTime or Cron Expression: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime.Value} Cron Expression: {job.CronExpression}", SitecronConstants.ParamNames.Log4NetLogger);
                return false;
            }

            if (!string.IsNullOrEmpty(job.CronExpression) && job.ExecuteExactlyAtDateTime.Value != DateTime.MinValue)
            {
                Log.Info($"Sitecron - Job Not Loaded - Both ExecuteExactlyAtDateTime and Cron Expression specified: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime.Value} Cron Expression: {job.CronExpression}", SitecronConstants.ParamNames.Log4NetLogger);
                return false;
            }

            if (job.ExecuteExactlyAtDateTime.Value != DateTime.MinValue && job.ExecuteExactlyAtDateTime.Value.ToUniversalTime() < DateTime.Now.ToUniversalTime())
            {
                Log.Info($"Sitecron - Job Not Loaded - ExecuteExactlyAtDateTime is in the past: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime.Value} Cron Expression: {job.CronExpression}", SitecronConstants.ParamNames.Log4NetLogger);
                return false;
            }

            if (job.Disable)
            {
                Log.Info($"Sitecron - Job Not Loaded - Job Disabled: {job.Name} Type: {job.JobTypeSignature} Cron Expression: {job.CronExpression}", SitecronConstants.ParamNames.Log4NetLogger);
                return false;
            }

            return true;
        }
    }
}