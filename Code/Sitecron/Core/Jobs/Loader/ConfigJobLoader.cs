using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Sitecron.Core.Jobs.Loader
{
    public class ConfigJobLoader
    {
        public List<SitecronJob> SiteCronConfigJobs { get; private set; }

        public ConfigJobLoader()
        {
            this.SiteCronConfigJobs = new List<SitecronJob>();
        }

        public void LoadConfigJobs(XmlNode node)
        {
            this.SiteCronConfigJobs.Add(CreateSitecronJob(node));
        }

        private SitecronJob CreateSitecronJob(XmlNode node)
        {
            DateTime executeExactlyAtDateTime;

            if (!DateTime.TryParse(XmlUtil.GetChildValue("executeExactlyAtDateTime", node), out executeExactlyAtDateTime))
                executeExactlyAtDateTime = DateTime.MinValue;
            //Archive after execution set to false since this is not an item.
            return new SitecronJob
            {
                ItemId = "SiteCron Config Job " + Guid.NewGuid().ToString(),
                TemplateId = ID.Null,
                Name = Sitecore.Xml.XmlUtil.GetAttribute("name", node),
                JobSource = "CONFIG",
                JobTypeSignature = Sitecore.Xml.XmlUtil.GetChildValue("type", node),
                CronExpression = Sitecore.Xml.XmlUtil.GetChildValue("cronExpression", node),
                Parameters = Sitecore.Xml.XmlUtil.GetChildValue("parameters", node),
                Items = Sitecore.Xml.XmlUtil.GetChildValue("items", node),
                Disable = Sitecore.Xml.XmlUtil.GetChildValue("disable", node) == "1",
                ArchiveAfterExecution = false,
                ExecuteExactlyAtDateTime = executeExactlyAtDateTime,
                LastRunUTC = string.Empty,
                NextRunUTC = string.Empty,
                ExecutionTime = string.Empty,
                LastRunLog = string.Empty,
                SitecoreJobType = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobType", node),
                SitecoreJobMethod = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobMethod", node),
                SitecoreJobName = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobName", node),
                SitecoreJobCategory = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobCategory", node),
                SitecoreJobSiteName = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobSiteName", node),
                SitecoreJobPriority = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreJobPriority", node),
                SitecoreScheduleJob = Sitecore.Xml.XmlUtil.GetChildValue("sitecoreScheduleJob", node),
                MinionFullName = Sitecore.Xml.XmlUtil.GetChildValue("minionFullName", node),
                EnvironmentName = Sitecore.Xml.XmlUtil.GetChildValue("environmentName", node)
            };
        }
    }
}