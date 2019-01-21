using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Jobs
{
    public abstract class DatabaseJobProvider : ISitecronJobProvider
    {
        private readonly ISitecronJobValidator _jobValidator;

        protected virtual Database ContextDatabase =>
            Factory.GetDatabase(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master"));

        public DatabaseJobProvider(ISitecronJobValidator jobValidator)
        {
            Assert.ArgumentNotNull(jobValidator, nameof(jobValidator));
            _jobValidator = jobValidator;
        }

        public IEnumerable<SitecronJob> GetJobs()
        {
            return GetJobItems().Select(CreateSitecronJob).Where(_jobValidator.IsValid);
        }

        protected virtual SitecronJob CreateSitecronJob(Item jobItem)
        {
            if (jobItem == null)
                return null;

            var siteCronJob = new SitecronJob();
            siteCronJob.ItemId = jobItem.ID.ToString();
            siteCronJob.TemplateId = jobItem.TemplateID;
            siteCronJob.Name = jobItem.Name;
            siteCronJob.JobSource = "DATABASE";
            siteCronJob.JobTypeSignature = jobItem[SitecronConstants.FieldNames.Type];
            siteCronJob.CronExpression = jobItem[SitecronConstants.FieldNames.CronExpression];
            siteCronJob.Parameters = jobItem[SitecronConstants.FieldNames.Parameters];
            siteCronJob.Items = jobItem[SitecronConstants.FieldNames.Items];
            siteCronJob.Disable = jobItem[SitecronConstants.FieldNames.Disable] == "1";
            siteCronJob.ArchiveAfterExecution = jobItem[SitecronConstants.FieldNames.ArchiveAfterExecution] == "1";
            siteCronJob.ExecuteExactlyAtDateTime = new DateField(jobItem.Fields[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]).DateTime;
            siteCronJob.LastRunUTC = jobItem[SitecronConstants.FieldNames.LastRunUTC];
            siteCronJob.NextRunUTC = jobItem[SitecronConstants.FieldNames.NextRunUTC];
            siteCronJob.ExecutionTime = jobItem[SitecronConstants.FieldNames.ExecutionTime];
            siteCronJob.LastRunLog = jobItem[SitecronConstants.FieldNames.LastRunUTC];
            siteCronJob.SitecoreJobType = jobItem[SitecronConstants.FieldNames.SitecoreJobType];
            siteCronJob.SitecoreJobMethod = jobItem[SitecronConstants.FieldNames.SitecoreJobMethod];
            siteCronJob.SitecoreJobName = jobItem[SitecronConstants.FieldNames.SitecoreJobName];
            siteCronJob.SitecoreJobCategory = jobItem[SitecronConstants.FieldNames.SitecoreJobCategory];
            siteCronJob.SitecoreJobSiteName = jobItem[SitecronConstants.FieldNames.SitecoreJobSiteName];
            siteCronJob.SitecoreJobPriority = jobItem[SitecronConstants.FieldNames.SitecoreJobPriority];
            siteCronJob.SitecoreScheduleJob = jobItem[SitecronConstants.FieldNames.SitecoreScheduleJob];
            siteCronJob.MinionFullName = jobItem[SitecronConstants.FieldNames.MinionFullName];
            siteCronJob.EnvironmentName = jobItem[SitecronConstants.FieldNames.EnvironmentName];
            return siteCronJob;
        }

        protected abstract IEnumerable<Item> GetJobItems();
        protected abstract IEnumerable<Item> GetJobItems(ID rootFolderId);
    }
}