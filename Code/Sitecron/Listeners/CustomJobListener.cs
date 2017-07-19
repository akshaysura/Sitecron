using Quartz;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Archiving;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Listeners
{
    public class CustomJobListener : IJobListener
    {
        public string Name
        {
            get
            {
                return "CustomJobListener"; 
            }
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            // executes before a job is run, we could stop job execution from starting by returning true
        }

        //runs before a job is executed 
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            Log.Info(string.Format("Sitecron - Job {0} in group {1} is about to be executed", context.JobDetail.Key.Name, context.JobDetail.Key.Group), this);
        }

        //runs after the job is executed
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string itemID = dataMap.GetString(SitecronConstants.FieldNames.ItemID);
            bool archiveItem = false;

            if (!string.IsNullOrEmpty(dataMap.GetString(SitecronConstants.FieldNames.ArchiveAfterExecution)) && dataMap.GetString(SitecronConstants.FieldNames.ArchiveAfterExecution) == "1")
                archiveItem = true;

            Log.Info(string.Format("Sitecron - Job {0} in group {1} was executed. (ItemID: {2} Archive:{3})", context.JobDetail.Key.Name, context.JobDetail.Key.Group, itemID, archiveItem.ToString()), this);

            if (archiveItem && !string.IsNullOrEmpty(itemID))
            {
                string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB);
                if (contextDbName != SitecronConstants.SitecoreDatabases.Master)
                {
                    Database contextDb = Factory.GetDatabase(contextDbName);

                    if (contextDb != null)
                    {
                        ArchiveItem(contextDb, itemID);
                    }
                }
                Database masterDb = Factory.GetDatabase(SitecronConstants.SitecoreDatabases.Master);
                if (masterDb != null)
                {
                    ArchiveItem(masterDb, itemID);
                }
            }
        }

        private void ArchiveItem(Database db, string itemID)
        {
            Item jobItem = db.GetItem(new ID(itemID));
            if (jobItem != null)
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    Archive archive = ArchiveManager.GetArchive("archive", jobItem.Database);
                    archive.ArchiveItem(jobItem);
                    Log.Info(string.Format("Sitecron - Item Archived. (ItemID: {0} Archive:{1} DB: {2})", itemID, db.Name), this);
                }
            }
        }
    }
}
