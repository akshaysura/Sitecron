using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;

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
            Item deletedItem = Event.ExtractParameter(args, 0) as Item;

            if (deletedItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, deletedItem))
            {
                Log.Info("Sitecron based Item Deleted/Archived, reloading Jobs.", this);
                _scheduleManager.ScheduleAllJobs();
            }
        }
    }
}