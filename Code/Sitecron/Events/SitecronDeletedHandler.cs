using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
using Sitecore.Diagnostics;
using Sitecron.Jobs;
using Sitecron.Scheduling;

namespace Sitecron.Events
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

            if (deletedItem != null && SitecronConstants.Templates.SitecronJobTemplateID == deletedItem.TemplateID) //matched Sitecron job template
            {
                _scheduleManager.ScheduleAllJobs();
            }
        }
    }
}