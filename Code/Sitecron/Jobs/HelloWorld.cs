using Quartz;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;
using System.Threading.Tasks;

namespace Sitecron.Jobs
{
    public class HelloWorld : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            Log.Info("SiteCron - Hello World", this);
            context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, "Sitecron - Hello World");
            return Task.FromResult<object>(null);
        }
    }
}