using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
using Sitecore.Data.Events;

namespace Sitecron.Core.Events
{
    public class SitecronSavingHandler
    {
        public void OnItemSaving(object sender, EventArgs args)
        {
            Item savingItem = null;
            ItemSavedRemoteEventArgs remoteArgs = args as ItemSavedRemoteEventArgs;

            //Thank you Mike Edwards!
            if (remoteArgs != null)
            {
                savingItem = remoteArgs.Item;
            }
            else
            {
                savingItem = Event.ExtractParameter(args, 0) as Item;
            }

            if (savingItem != null && SitecronConstants.Templates.SitecronJobTemplateID == savingItem.TemplateID) //matched Sitecron job template
            {
                string appendText = "";
                string icon = "";
                if (savingItem.Fields[SitecronConstants.FieldNames.Disable].Value == "1")
                {
                    appendText = " _DISABLED_";
                    icon = "Applications/32x32/gears_stop.png";
                }
                else
                    icon = "Applications/32x32/gears.png";

                savingItem.Appearance.Icon = icon;
                savingItem.Appearance.DisplayName = string.Concat(savingItem.Name, appendText);
            }
        }
    }
}