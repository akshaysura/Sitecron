using Quartz;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecron.SitecronSettings;
using System.Collections.Specialized;

namespace Sitecron.Jobs.Publishing
{
    public class SmartPublish : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("SitePublishJob Execute - Start", this);

            //get job parameters
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string rawParameters = dataMap.GetString(SitecronConstants.FieldNames.Parameters);
            NameValueCollection parameters = Sitecore.Web.WebUtil.ParseUrlParameters(rawParameters);

            string targetParam = parameters["Target"];
            if (!string.IsNullOrEmpty(targetParam))
            {
                Database master = Sitecore.Configuration.Factory.GetDatabase(SitecronConstants.SitecoreDatabases.Master);

                Database[] targetDBs = new Database[] { Sitecore.Configuration.Factory.GetDatabase(targetParam) }; 
                Language[] languages = languages = LanguageManager.GetLanguages(master).ToArray();
                Sitecore.Publishing.PublishManager.PublishSmart(master, targetDBs, languages);
            }
            else
                Log.Warn("SitePublishJob Execute - Target parameter missing", this);



            Log.Info("SitePublishJob Execute - End", this);
        }
    }
}