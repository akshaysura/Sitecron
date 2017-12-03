using Quartz;
using Sitecore.Diagnostics;

namespace Sitecron.Jobs
{
    public class HelloWorld : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Sitecron - Hello World", this);
        }
    }
}