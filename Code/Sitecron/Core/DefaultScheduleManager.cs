using System;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecron.Core.Jobs;
using Sitecron.Core.Scheduling;
using Sitecron.SitecronSettings;
using System.Reflection;
using Sitecore.Data;
using System.Linq;
using Sitecore.Data.Managers;
using Sitecore.Data.Archiving;
using Sitecore.SecurityModel;
using Sitecore.Data.Items;

namespace Sitecron.Core
{
    public class DefaultScheduleManager : IScheduleManager
    {
        private readonly ISitecronJobProvider _jobProvider;
        private readonly ISitecronScheduler _scheduler;

        public DefaultScheduleManager(ISitecronJobProvider jobProvider, ISitecronScheduler scheduler)
        {
            Assert.ArgumentNotNull(jobProvider, nameof(jobProvider));
            Assert.ArgumentNotNull(scheduler, nameof(scheduler));

            _jobProvider = jobProvider;
            _scheduler = scheduler;
        }

        public void CleanUpExistingJobs()
        {
            string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master");
            Database contextDb = Factory.GetDatabase(contextDbName);

            var folder = contextDb?.GetItem(SitecronConstants.ItemIds.AutoFolderID);
            if (folder == null)
            {
                return;
            }

            var autoFolderItems = folder.Axes.GetDescendants().Where(i =>
                TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, i));
            if (autoFolderItems == null || (autoFolderItems != null && !autoFolderItems.Any()))
                return;

            Archive archive = ArchiveManager.GetArchive("archive", contextDb);

            foreach (var jobItem in autoFolderItems)
            {
                using (new SecurityDisabler())
                {
                    using (new EditContext(jobItem))
                    {
                        jobItem[SitecronConstants.FieldNames.LastRunLog] += "Archived at startup.";
                    }
                    archive.ArchiveItem(jobItem);
                    Log.Info(string.Format("SiteCron - Item Archived during startup. (ItemID: {0} DB: {1} Name:{2})", jobItem.ID.ToString(), contextDbName, jobItem.Name), this);
                }
            }
        }

        public void ScheduleAllJobs()
        {
            var publishingInstance = Settings.Publishing.PublishingInstance;
            var instanceName = Settings.InstanceName.ToLower();
            var usePublishingInstanceAsPrimaryServer = Settings.GetBoolSetting(SitecronConstants.SettingsNames.UsePublishingInstanceAsPrimaryServer, true);

            if ((!string.IsNullOrEmpty(publishingInstance) &&
                !string.IsNullOrEmpty(instanceName) &&
                !publishingInstance.Equals(instanceName, StringComparison.OrdinalIgnoreCase)) 
                && usePublishingInstanceAsPrimaryServer)
            {
                Log.Info($"SiteCron - Exit without initialization, this server is not the primary in the load balanced environment. PublishingInstance: {publishingInstance} != InstanceName: {instanceName}", this);
                return;
            }

            try
            {
                Log.Info("Initialize SiteCron: " + Assembly.GetExecutingAssembly().GetName().Version, this);
                _scheduler.ClearJobs();

                Log.Info("Loading SiteCron Jobs", this);

                foreach (var job in _jobProvider.GetJobs())
                {
                    _scheduler.ScheduleJob(job);
                }
            }
            catch (Exception ex)
            {
                Log.Error("SiteCron ERROR: " + ex.Message, ex, this);
            }
        }
    }
}