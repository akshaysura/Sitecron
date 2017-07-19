using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;

namespace Sitecron.Events
{
    public class SitecronSavedHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            Item savedItem = Event.ExtractParameter(args, 0) as Item;

            if (savedItem != null && SitecronConstants.Templates.SitecronJobTemplateID == savedItem.TemplateID) //matched Sitecron job template
            {
                    ScheduleHelper scheduler = new ScheduleHelper();
                    scheduler.InitializeScheduler();
            }
        }

    }
}