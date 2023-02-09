using System;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Events
{
    public class SitecronDeletedHandler
    {
        private readonly IScheduleManager _scheduleManager;

        public SitecronDeletedHandler(IScheduleManager scheduleManager)
        {
            Assert.ArgumentNotNull(scheduleManager, nameof(scheduleManager));
            _scheduleManager = scheduleManager;
        }
        public void OnItemDeleted(object sender, EventArgs args)
        {
            Item deletedItem = null;
            ID parentId = null;
            ItemDeletedRemoteEventArgs remoteArgs = args as ItemDeletedRemoteEventArgs;
            if (remoteArgs != null)
            {
                deletedItem = remoteArgs.Item;
                parentId = remoteArgs.ParentId;
            }
            else
            {
                deletedItem = Event.ExtractParameter(args, 0) as Item;
                parentId = Event.ExtractParameter(args, 1) as ID;
            }

            if (deletedItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, deletedItem) && !StandardValuesManager.IsStandardValuesHolder(deletedItem) && parentId != SitecronConstants.ItemIds.AutoFolderID)
            {
                Log.Info("SiteCron based Item Archived/Deleted, reloading Jobs. (Execute now jobs excluded)", this);
                _scheduleManager.ScheduleAllJobs();
            }
        }
    }
}