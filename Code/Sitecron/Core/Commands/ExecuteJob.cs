using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecron.SitecronSettings;
using System;
namespace Sitecron.Core.Commands
{
    public class ExecuteJob : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.IsNotNull(context, "context");
            Assert.IsNotNull(context.Parameters["id"], "id");

            string contextDbName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master");
            Database contextDb = Factory.GetDatabase(contextDbName);

            Item scriptItem = contextDb.GetItem(new ID(context.Parameters["id"]));
            if (scriptItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, scriptItem))
            {
                string newItemName = ItemUtil.ProposeValidItemName(string.Concat("Execute Now ", scriptItem.Name, DateTime.Now.ToString(" yyyyMMddHHmmss")));

                Item autoFolderItem = contextDb.GetItem(SitecronConstants.ItemIds.AutoFolderID);
                if (autoFolderItem != null)
                {
                    Item newScriptItem = scriptItem.CopyTo(autoFolderItem, newItemName);

                    double addExecutionSeconds = 20;
                    if (!Double.TryParse(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronExecuteNowSeconds), out addExecutionSeconds))
                        addExecutionSeconds = 20;

                    using (new EditContext(newScriptItem, Sitecore.SecurityModel.SecurityCheck.Disable))
                    {
                        DateTime executeTime = DateTime.Now.AddSeconds(addExecutionSeconds);
                        newScriptItem[SitecronConstants.FieldNames.CronExpression] = string.Format("{0} {1} {2} 1/1 * ? * ", executeTime.ToString("ss"), executeTime.ToString("mm"), executeTime.ToString("HH"));
                        newScriptItem[SitecronConstants.FieldNames.ArchiveAfterExecution] = "1";
                        newScriptItem[SitecronConstants.FieldNames.ExecuteExactlyAtDateTime] = "";
                        newScriptItem[SitecronConstants.FieldNames.Disable] = "0";
                    }
                    var newIndexItem = (SitecoreIndexableItem)newScriptItem;
                    ContentSearchManager.GetIndex(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronGetItemsIndex, "sitecore_master_index").Trim()).Refresh(newIndexItem);
                }
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length != 1)
                return CommandState.Hidden;

            var publishingInstance = Settings.Publishing.PublishingInstance;
            var instanceName = Settings.InstanceName.ToLower();

            if (!string.IsNullOrEmpty(publishingInstance) &&
            !string.IsNullOrEmpty(instanceName) &&
            !publishingInstance.Equals(instanceName, StringComparison.OrdinalIgnoreCase))
            {
                Log.Info($"SiteCron - Hide execute now, this server is not the primary in the load balanced environment. PublishingInstance: {publishingInstance} != InstanceName: {instanceName}", this);
                return CommandState.Hidden;
            }

            Item currentItem = context.Items[0];
            if (currentItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, currentItem))
            {
                return CommandState.Enabled;
            }

            return CommandState.Hidden;
        }
    }
}