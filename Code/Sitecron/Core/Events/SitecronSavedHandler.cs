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
using Sitecore.Data;
using System.Collections.Generic;

namespace Sitecron.Core.Events
{
    public class SitecronSavedHandler
    {
        private readonly IScheduleManager _scheduleManager;
        private static readonly SynchronizedCollection<ID> _inProcess = new SynchronizedCollection<ID>();

        public SitecronSavedHandler(IScheduleManager scheduleManager)
        {
            Assert.ArgumentNotNull(scheduleManager, nameof(scheduleManager));
            _scheduleManager = scheduleManager;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            Item savedItem = null;
            ItemSavedRemoteEventArgs remoteArgs = args as ItemSavedRemoteEventArgs;
            ItemChanges savedItemChanges = null;

            //Thank you Mike Edwards!
            if (remoteArgs != null)
            {
                savedItem = remoteArgs.Item;
                savedItemChanges = remoteArgs.Changes;
            }
            else
            {
                savedItem = Event.ExtractParameter(args, 0) as Item;
                savedItemChanges = Event.ExtractParameter(args, 1) as ItemChanges;
            }

            if (savedItem != null && TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, savedItem) && !StandardValuesManager.IsStandardValuesHolder(savedItem))
            {
                if (savedItemChanges != null && !savedItemChanges.FieldChanges.ContainsAnyOf(SiteCronFieldIds.LastRunUTC, SiteCronFieldIds.NextRunUTC, SiteCronFieldIds.ExecutionTime, SiteCronFieldIds.LastRunLog) && !_inProcess.Contains(savedItem.ID))
                {
                    _inProcess.Add(savedItem.ID);
                    Log.Info($"SiteCron based Item Saved/Created, reloading Jobs. {savedItem.Name} - {savedItem.ID.ToString()}", this);
                    _scheduleManager.ScheduleAllJobs();
                    _inProcess.Remove(savedItem.ID);
                }
                else
                {
                    Log.Info("SiteCron - Ignoring Saved Handler due to stats update.", this);
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
                    Log.Error("SiteCron OnItemSaved Custom Type ERROR: " + ex.Message, ex, this);
                }
            }
        }
    }
}