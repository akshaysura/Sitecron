using Quartz;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecron.SitecronSettings;
using System.Collections.Specialized;
using System.Text;

namespace Sitecron.Jobs.Publishing
{
    public class SmartPublish : IJob //Inherit from IJob interface from Quartz
    {
        public void Execute(IJobExecutionContext context) //Implement the Execute method
        {
            Log.Info("SitePublishJob Execute - Start", this);

            //get job parameters
            JobDataMap dataMap = context.JobDetail.JobDataMap; //get the datamap from the Quartz job 
            string rawParameters = dataMap.GetString(SitecronConstants.FieldNames.Parameters); //Get the Parameters field in Quartz JobDataMap which maps to the Parameters field in the SiteCron Job item.
            NameValueCollection parameters = Sitecore.Web.WebUtil.ParseUrlParameters(rawParameters); //Use Sitecore WebUtil to parse the parameters

            string targetParam = parameters["Target"]; //Get the target parameter
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
            context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "SitePublishJob Execute - End"); 
        }
    }
}
