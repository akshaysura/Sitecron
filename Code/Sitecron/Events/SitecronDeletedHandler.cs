using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
namespace Sitecron.Events
{
    public class SitecronDeletedHandler
    {
        public void OnItemDeleted(object sender, EventArgs args)
        {
			Item deletedItem = null;
            ItemSavedRemoteEventArgs remoteArgs = args as ItemSavedRemoteEventArgs;
            if (remoteArgs != null)
            {
                deletedItem = remoteArgs.Item;
            }
            else
            {
                deletedItem = Event.ExtractParameter(args, 0) as Item;
            }
            if (deletedItem != null && SitecronConstants.Templates.SitecronJobTemplateID == deletedItem.TemplateID) //matched Sitecron job template
            {
                ScheduleHelper scheduler = new ScheduleHelper();
                scheduler.InitializeScheduler();
            }
        }
    }
}
