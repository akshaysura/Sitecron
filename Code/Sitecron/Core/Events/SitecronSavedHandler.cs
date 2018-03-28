using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecron.SitecronSettings;
using System;
using Sitecore.Data.Events;
using System.Reflection;
using Sitecron.Extend;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using static Sitecron.SitecronSettings.SitecronConstants;
using Sitecore.Data.Managers;

namespace Sitecron.Core.Events
{
    public class SitecronSavedHandler
    {
        private readonly IScheduleManager _scheduleManager;

        public SitecronSavedHandler(IScheduleManager scheduleManager)
        {
            Assert.ArgumentNotNull(scheduleManager, nameof(scheduleManager));
            _scheduleManager = scheduleManager;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            Item savedItem = null;
            ItemSavedRemoteEventArgs remoteArgs = args as ItemSavedRemoteEventArgs;
            ItemChanges savedItemChanges = Event.ExtractParameter(args, 1) as ItemChanges;

            //Thank you Mike Edwards!
            if (remoteArgs != null)
            {
                savedItem = remoteArgs.Item;
            }
            else
            {
                savedItem = Event.ExtractParameter(args, 0) as Item;
            }

            if (savedItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, savedItem))
            {
                if (savedItemChanges != null && savedItemChanges.FieldChanges.ContainsAnyOf(SiteCronFieldIds.LastRunUTC, SiteCronFieldIds.NextRunUTC, SiteCronFieldIds.ExecutionTime, SiteCronFieldIds.LastRunLog))
                {
                    Log.Info("Sitecron - Ignoring Saved Handler due to stats update.", this);
                }
                else
                {
                    Log.Info("Sitecron based Item Saved/Created, reloading Jobs.", this);
                    _scheduleManager.ScheduleAllJobs();
                }
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
                    Log.Error("Sitecron OnItemSaved Custom Type ERROR: " + ex.Message, ex, this);
                }
            }
        }
    }
}