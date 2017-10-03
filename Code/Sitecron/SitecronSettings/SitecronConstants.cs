using Sitecore.Data;

namespace Sitecron.SitecronSettings
{
    public static class SitecronConstants
    {
        public class ItemIds
        {
            public const string ExecuteNowFolderID = "{462CF67D-13C6-4E28-B0AE-709E29E01A71}";
        }
        public class Templates
        {
            public static ID SitecronJobTemplateID = new ID("{7F2C8881-6AE4-48CF-A499-7745CC4B2EB2}");
        }

        public static class FieldNames
        {
            public const string Type = "Type";
            public const string CronExpression = "CronExpression";
            public const string Parameters = "Parameters";
            public const string Items = "Items";
            public const string Disable = "Disable";
            public const string ArchiveAfterExecution = "ArchiveAfterExecution";

            //non sitecore related, used for internal storage
            public const string ItemID = "ItemID";
        }

        public static class SettingsNames
        {
            public const string SiteCronContextDB = "SiteCronContextDB";
            public const string SiteCronExecuteNowSeconds = "SiteCronExecuteNowSeconds";
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