using Quartz;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Archiving;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecron.SitecronSettings;
using System;

namespace Sitecron.Core.Quartz.Listeners
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

            if (string.IsNullOrEmpty(itemID))
                return;

            Log.Info(string.Format("Sitecron - Job {0} in group {1} was executed in {4}. (ItemID: {2} Archive:{3})", context.JobDetail.Key.Name, context.JobDetail.Key.Group, itemID, archiveItem.ToString(), context.JobRunTime.TotalSeconds.ToString()), this);

            string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master");
            if (contextDbName != SitecronConstants.SitecoreDatabases.Master)
            {
                Database contextDb = Factory.GetDatabase(contextDbName);

                if (contextDb != null)
                {
                    SetItemStats(contextDb, itemID, context.FireTimeUtc.Value.DateTime.ToString(), context.NextFireTimeUtc.Value.DateTime.ToString(), context.JobRunTime.TotalSeconds.ToString());
                    if (archiveItem)
                        ArchiveItem(contextDb, itemID);
                }
            }
            Database masterDb = Factory.GetDatabase(SitecronConstants.SitecoreDatabases.Master);
            if (masterDb != null)
            {
                SetItemStats(masterDb, itemID, context.FireTimeUtc.Value.DateTime.ToString(), context.NextFireTimeUtc.Value.DateTime.ToString(), context.JobRunTime.TotalSeconds.ToString());

                //Only do it on master.
                CreateExecutionReport(dataMap.GetString(SitecronConstants.ParamNames.Name), itemID, dataMap.GetString(SitecronConstants.ParamNames.SitecronJobLogData), context.FireTimeUtc.Value.DateTime.ToString());

                if (archiveItem)
                    ArchiveItem(masterDb, itemID);
            }
        }

        private void SetItemStats(Database db, string itemID, string lastRunTime, string nextRunTime, string executionTime)
        {
            Item jobItem = db.GetItem(new ID(itemID));
            if (jobItem != null)
            {
                using (new SecurityDisabler())
                {
                    jobItem.Editing.BeginEdit();
                    {
                        jobItem[SitecronConstants.FieldNames.LastRunUTC] = lastRunTime;
                        jobItem[SitecronConstants.FieldNames.NextRunUTC] = nextRunTime;
                        jobItem[SitecronConstants.FieldNames.ExecutionTime] = executionTime;
                    }
                    jobItem.Editing.EndEdit();
                }
            }
        }

        private void CreateExecutionReport(string jobName, string itemID, string logData, string lastRunTime)
        {
            try
            {
                string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master");
                Database contextDb = Factory.GetDatabase(contextDbName);

                //The bucket is not publishable so it will stay only on the master. This way it will not cause any publishing delays.
                Item executionReportFolderItem = contextDb.GetItem(new ID(SitecronConstants.ItemIds.SiteCronExecutionReportsFolderID));
                if (executionReportFolderItem != null)
                {
                    string newItemName = ItemUtil.ProposeValidItemName(string.Concat(jobName, DateTime.Now.ToString(" yyyyMMddHHmmss")));

                    using (new SecurityDisabler())
                    {
                        Item executionReport = executionReportFolderItem.Add(newItemName, new TemplateID(SitecronConstants.Templates.SiteCronExecutionReportTemplateID));
                        if (executionReport != null)
                        {
                            executionReport.Editing.BeginEdit();
                            {
                                executionReport[SitecronConstants.FieldNames.LastRunUTC] = lastRunTime;
                                executionReport[SitecronConstants.FieldNames.Log] = logData;
                                executionReport[SitecronConstants.FieldNames.SitecronJob] = itemID;
                            }
                            executionReport.Editing.EndEdit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitecron ERROR creating Execution report: " + ex.Message, ex, this);
            }
        }
        private void ArchiveItem(Database db, string itemID)
        {
            Item jobItem = db.GetItem(new ID(itemID));
            if (jobItem != null)
            {
                using (new SecurityDisabler())
                {
                    Archive archive = ArchiveManager.GetArchive("archive", jobItem.Database);
                    archive.ArchiveItem(jobItem);
                    Log.Info(string.Format("Sitecron - Item Archived. (ItemID: {0} DB: {1})", itemID, db.Name), this);
                }
            }
        }
    }
}