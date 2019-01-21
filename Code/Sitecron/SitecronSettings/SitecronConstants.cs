using Sitecore.Data;

namespace Sitecron.SitecronSettings
{
    public static class SitecronConstants
    {
        public class ItemIds
        {
            public static ID RootFolderID = new ID("{C9ADDAE6-2298-49F2-8C9E-920A24D3D6D9}");
            public static ID AutoFolderID = new ID("{462CF67D-13C6-4E28-B0AE-709E29E01A71}");
            public const string SiteCronExecutionReportsFolderID = "{E62862B1-03DF-4752-A290-9B505806C515}";
        }
        public class Templates
        {
            public static ID SitecronJobTemplateID = new ID("{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}");
            public static ID SiteCronExecutionReportTemplateID = new ID("{F8E58F37-9BF0-4B71-B562-4B21973CF6E7}");
            public static ID SitecronRunAsSitecoreJobTemplateID = new ID("{49A27DC8-2A6A-48A2-A8EF-02A3DD0D3274}");
            public static ID SiteCronSitecoreScheduleCommandJobTemplateID = new ID("{B9B437B2-789C-48F6-AAE2-B49C4AA2A4BB}");
        }

        public static class FieldNames
        {
            public const string Type = "Type";
            public const string Method = "Method";
            public const string CronExpression = "CronExpression";
            public const string Parameters = "Parameters";
            public const string Items = "Items";
            public const string Disable = "Disable";
            public const string ArchiveAfterExecution = "ArchiveAfterExecution";
            public const string ExecuteExactlyAtDateTime = "ExecuteExactlyAtDateTime";
            public const string LastRunUTC = "LastRunUTC";
            public const string NextRunUTC = "NextRunUTC";
            public const string ExecutionTime = "ExecutionTime";
            public const string LastRunLog = "LastRunLog";
            public const string Log = "log";
            public const string SitecronJob = "SitecronJob";
            public const string NeverPublish = "__Never publish";

            public const string SitecoreJobType = "SitecoreJobType";
            public const string SitecoreJobMethod = "SitecoreJobMethod";
            public const string SitecoreJobName = "SitecoreJobName";
            public const string SitecoreJobCategory = "SitecoreJobCategory";
            public const string SitecoreJobSiteName = "SitecoreJobSiteName";
            public const string SitecoreJobPriority = "SitecoreJobPriority";
            public const string SitecoreScheduleJob = "SitecoreScheduleJob";

            public const string MinionFullName = "MinionFullName";
            public const string EnvironmentName = "EnvironmentName";

            //non sitecore related, used for internal storage
            public const string ItemID = "ItemID";
        }

        public static class SiteCronFieldIds
        {
            public static ID CronExpression = new ID("{8BDF639E-21DE-48A7-9F93-CA85B0F695A3}");
            public static ID LastRunUTC = new ID("{A363FA42-810F-48E8-87D8-ABE5991D9B61}");
            public static ID NextRunUTC = new ID("{0571F1A5-E0BC-4256-8096-8F1738AF1071}");
            public static ID ExecutionTime = new ID("{0D5EDDE7-08A6-424F-B751-CC2C1D48EBBA}");
            public static ID LastRunLog = new ID("{63959259-4721-4370-BFB9-0A93E43E8993}");
        }
        public static class ParamNames
        {
            public const string zSiteCronItemID = "zSiteCronItemID";
            public const string SitecronJob = "SitecronJob";
            public const string Log4NetLogger = "SitecronLogger";
            public const string SitecronJobLogData = "SitecronJobLogData";
            public const string PSSitecronExecutionLog = "sitecronExecutionLog";
            public const string Name = "Name";
        }
        public static class SettingsNames
        {
            public const string SiteCronContextDB = "SiteCronContextDB";
            public const string SiteCronExecuteNowSeconds = "SiteCronExecuteNowSeconds";
            public const string SiteCronSavedHandlerType = "SiteCron.SavedHandlerType";
            public const string SiteCronValidTemplates = "SiteCron.ValidTemplates";
            public const string SiteCronGetItemsIndex = "SiteCron.GetItemIndex";
        }
        public static class Queries
        {
            public const string QueryRetriveJobs = ("/sitecore/system/Modules/Sitecron//*[@@templateid='{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}']");
        }

        public static class SitecoreDatabases
        {
            public const string Master = "master";
        }
    }
}