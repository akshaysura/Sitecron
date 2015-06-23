using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecron.Events
{
    public class SitecronDeletedHandler
    {
        public void OnItemDeleted(object sender, EventArgs args)
        {
            Item deletedItem = Event.ExtractParameter(args, 0) as Item;

            if (deletedItem != null && deletedItem.Database.Name.ToLower() == "master")
            {
                if (deletedItem.TemplateID == ID.Parse("{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}")) //matched Sitecron job template
                {
                    ScheduleHelper scheduler = new ScheduleHelper();
                    scheduler.InitializeScheduler();
                }
            }
        }
    }
}