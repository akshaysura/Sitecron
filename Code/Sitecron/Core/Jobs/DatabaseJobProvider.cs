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
            return new SitecronJob
            {
                ItemId = jobItem.ID.ToString(),
                TemplateId = jobItem.TemplateID,
                Name = jobItem.Name,
                JobSource = "DATABASE",
                JobTypeSignature = jobItem[SitecronConstants.FieldNames.Type],
                CronExpression = jobItem[SitecronConstants.FieldNames.CronExpression],
                Parameters = jobItem[SitecronConstants.FieldNames.Parameters],
                Items = jobItem[SitecronConstants.FieldNames.Items],
                Disable = jobItem[SitecronConstants.FieldNames.Disable] == "1",
                ArchiveAfterExecution = jobItem[SitecronConstants.FieldNames.ArchiveAfterExecution] == "1",
                ExecuteExactlyAtDateTime = new DateField(jobItem.Fields[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime]).DateTime,
                LastRunUTC = jobItem[SitecronConstants.FieldNames.LastRunUTC],
                NextRunUTC = jobItem[SitecronConstants.FieldNames.NextRunUTC],
                ExecutionTime = jobItem[SitecronConstants.FieldNames.ExecutionTime],
                LastRunLog = jobItem[SitecronConstants.FieldNames.LastRunUTC],
                SitecoreJobType = jobItem[SitecronConstants.FieldNames.SitecoreJobType],
                SitecoreJobMethod = jobItem[SitecronConstants.FieldNames.SitecoreJobMethod],
                SitecoreJobName = jobItem[SitecronConstants.FieldNames.SitecoreJobName],
                SitecoreJobCategory = jobItem[SitecronConstants.FieldNames.SitecoreJobCategory],
                SitecoreJobSiteName = jobItem[SitecronConstants.FieldNames.SitecoreJobSiteName],
                SitecoreJobPriority = jobItem[SitecronConstants.FieldNames.SitecoreJobPriority],
                SitecoreScheduleJob = jobItem[SitecronConstants.FieldNames.SitecoreScheduleJob],
                MinionFullName = jobItem[SitecronConstants.FieldNames.MinionFullName],
                EnvironmentName = jobItem[SitecronConstants.FieldNames.EnvironmentName]
            };
        }

        protected abstract IEnumerable<Item> GetJobItems();
    }
}