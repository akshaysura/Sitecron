using Quartz;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleJob.Publishing
{
    public class IncrementalSitePublishJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("IncrementalSitePublishJob Execute - Start", this);

            Database master = Sitecore.Configuration.Factory.GetDatabase("master");

            Database[] targetDBs = new Database[] { Sitecore.Configuration.Factory.GetDatabase("web") }; //you can get this via config if you need customize targets
            Language[] languages = languages = LanguageManager.GetLanguages(master).ToArray();

            Sitecore.Publishing.PublishManager.PublishIncremental(master, targetDBs, languages);

            Log.Info("IncrementalSitePublishJob Execute - End", this);
        }
    }
}
