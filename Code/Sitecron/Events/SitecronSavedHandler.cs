using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
using Sitecore.Data.Events;
using System.Reflection;
using Sitecron.Extend;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Sitecron.Events
{
    public class SitecronSavedHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            Item savedItem = null;
            ItemSavedRemoteEventArgs remoteArgs = args as ItemSavedRemoteEventArgs;

            //Thank you Mike Edwards!
            if (remoteArgs != null)
            {
                savedItem = remoteArgs.Item;
            }
            else
            {
                savedItem = Event.ExtractParameter(args, 0) as Item;
            }

            if (savedItem != null && SitecronConstants.Templates.SitecronJobTemplateID == savedItem.TemplateID) //matched Sitecron job template
            {
                ScheduleHelper scheduler = new ScheduleHelper();
                scheduler.InitializeScheduler();
            }
            else
            {
                try
                {
                    string typeName = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronSavedHandlerType);
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        Type type = Type.GetType(typeName);
                        if (type != null)
                        {
                            object instance = Activator.CreateInstance(type);
                            if (instance != null && instance is ISavedHandler)
                            {
                                MethodInfo method = type.GetMethod("OnItemSaved");
                                if (method != null)
                                {
                                    method.Invoke(instance, new object[] { sender, args });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Sitecron OnItemSaved Custom Type ERROR: " + ex.Message, this);
                }
            }
        }
    }
}