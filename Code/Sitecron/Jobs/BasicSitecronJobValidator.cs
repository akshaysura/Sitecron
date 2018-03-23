using System;
using Sitecore.Diagnostics;

namespace Sitecron.Jobs
{
    public class BasicSitecronJobValidator : ISitecronJobValidator
    {
        public bool IsValid(SitecronJob job)
        {
            if (job.JobType == null)
            {
                Log.Info($"Sitecron - Job Not Loaded - Could not load type '{job.JobTypeSignature}' for job {job.Name} - {job.ItemId}", this);
                return false;
            }

            if (string.IsNullOrEmpty(job.CronExpression) && !job.ExecuteExactlyAtDateTime.HasValue)
            {
                Log.Info($"Sitecron - Job Not Loaded - Invalid ExecuteExactlyAtDateTime or Cron Expression: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime} Cron Expression: {job.CronExpression}", this);
                return false;
            }

            if (!string.IsNullOrEmpty(job.CronExpression) && job.ExecuteExactlyAtDateTime.HasValue)
            {
                Log.Info($"Sitecron - Job Not Loaded - Both ExecuteExactlyAtDateTime and Cron Expression specified: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime} Cron Expression: {job.CronExpression}", this);
                return false;
            }

            if (job.ExecuteExactlyAtDateTime.HasValue && job.ExecuteExactlyAtDateTime.Value < DateTime.Now)
            {
                Log.Info($"Sitecron - Job Not Loaded - ExecuteExactlyAtDateTime is in the past: {job.Name} ExecuteExactlyAtDateTime: {job.ExecuteExactlyAtDateTime} Cron Expression: {job.CronExpression}", this);
                return false;
            }

            if (job.Disable)
            {
                Log.Info($"Sitecron - Job Not Loaded - Job Disabled: {job.Name} Type: {job.JobTypeSignature} Cron Expression: {job.CronExpression}", this);
                return false;
            }

            return true;
        }
    }
}