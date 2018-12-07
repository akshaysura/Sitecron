using System;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecron.SitecronSettings;
using Sitecore.Data;

namespace Sitecron.Core.Jobs
{
    public class SitecronJob
    {
        private string _typeSignature;
        private Type _type;

        public virtual string JobSource { get; set; }
        public virtual string ItemId { get; set; }
        public virtual ID TemplateId { get; set; }
        public virtual string Name { get; set; }

        public virtual string JobTypeSignature
        {
            get { return _typeSignature; }
            set
            {
                _typeSignature = value;
                _type = null;
            }
        }

        public Type JobType
        {
            get
            {
                if (_type == null && !string.IsNullOrWhiteSpace(JobTypeSignature))
                {
                    _type = Type.GetType(JobTypeSignature);
                }

                return _type;
            }
        }

        public virtual string CronExpression { get; set; }

        public virtual string Parameters { get; set; }

        public virtual string Items { get; set; }

        public virtual bool Disable { get; set; }

        public virtual bool ArchiveAfterExecution { get; set; }

        public virtual DateTime? ExecuteExactlyAtDateTime { get; set; }

        public virtual string LastRunUTC { get; set; }

        public virtual string NextRunUTC { get; set; }

        public virtual string ExecutionTime { get; set; }

        public virtual string LastRunLog { get; set; }

        public Item GetItem()
        {
            return Factory.GetDatabase(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronContextDB, "master"))
                ?.GetItem(ItemId);
        }

        //Run as a Sitecore Job
        public virtual string SitecoreJobType { get; set; }
        public virtual string SitecoreJobMethod { get; set; }
        public virtual string SitecoreJobName { get; set; }
        public virtual string SitecoreJobCategory { get; set; }
        public virtual string SitecoreJobSiteName { get; set; }
        public virtual string SitecoreJobPriority { get; set; }

        //Run a Sitecore Scheduled Job Command
        public virtual string SitecoreScheduleJob { get; set; }

        //Run Minion
        public virtual string MinionFullName { get; set; }
        public virtual string EnvironmentName { get; set; }
        

    }
}