using Sitecore.Data;

namespace Sitecron.SitecronSettings
{
    public static class SitecronConstants
    {
        public class ItemIds
        {
            public const string RootFolderID = "{C9ADDAE6-2298-49F2-8C9E-920A24D3D6D9}";
            public const string AutoFolderID = "{462CF67D-13C6-4E28-B0AE-709E29E01A71}";
            public const string SiteCronExecutionReportsFolderID = "{E62862B1-03DF-4752-A290-9B505806C515}";
        }
        public class Templates
        {
            public static ID SitecronJobTemplateID = new ID("{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}");
            public static ID SiteCronExecutionReportTemplateID = new ID("{F8E58F37-9BF0-4B71-B562-4B21973CF6E7}");
        }

        public static class FieldNames
        {
            public const string Type = "Type";
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