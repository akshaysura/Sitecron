using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Jobs
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
                Name = jobItem.Name,
                JobTypeSignature = jobItem[SitecronConstants.FieldNames.Type],
                CronExpression = jobItem[SitecronConstants.FieldNames.CronExpression],
                Parameters = jobItem[SitecronConstants.FieldNames.Parameters],
                Items = jobItem[SitecronConstants.FieldNames.Items],
                Disable = jobItem[SitecronConstants.FieldNames.Disable] == "1",
                ArchiveAfterExecution = jobItem[SitecronConstants.FieldNames.ArchiveAfterExecution] == "1",
                ExecuteExactlyAtDateTime = new DateField(jobItem.Fields[SitecronConstants.SiteCronFieldIds.ExecutionTime]).DateTime,
                LastRunUTC = jobItem[SitecronConstants.FieldNames.LastRunUTC],
                NextRunUTC = jobItem[SitecronConstants.FieldNames.NextRunUTC],
                ExecutionTime = jobItem[SitecronConstants.FieldNames.ExecutionTime],
                LastRunLog = jobItem[SitecronConstants.FieldNames.LastRunUTC]
            };
        }

        protected abstract IEnumerable<Item> GetJobItems();
    }
}