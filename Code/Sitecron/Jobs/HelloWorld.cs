using Quartz;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace Sitecron.Jobs
{
    public class HelloWorld : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("SiteCron - Hello World", this);
            context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "Sitecron - Hello World");
        }
    }
}