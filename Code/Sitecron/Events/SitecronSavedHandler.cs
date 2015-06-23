using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecron.Events
{
    public class SitecronSavedHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            Item savedItem = Event.ExtractParameter(args, 0) as Item;

            if (savedItem != null && savedItem.Database.Name.ToLower() == "master")
            {
                if (savedItem.TemplateID == ID.Parse("{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}")) //matched Sitecron job template
                {
                    ScheduleHelper scheduler = new ScheduleHelper();
                    scheduler.InitializeScheduler();
                }
            }
        }

    }
}