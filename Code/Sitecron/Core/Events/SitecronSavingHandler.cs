using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
using Sitecore.Data.Events;
using Sitecore.Data.Managers;
using Sitecore.Data;

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

            if (savingItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, savingItem) && !StandardValuesManager.IsStandardValuesHolder(savingItem))
            {
                Item existingItem = savingItem.Database.GetItem(savingItem.ID, savingItem.Language, savingItem.Version);
                if (existingItem.Fields[SitecronConstants.FieldNames.Disable].Value != savingItem.Fields[SitecronConstants.FieldNames.Disable].Value)
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
}