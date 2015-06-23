using Quartz;
using Sitecore.Diagnostics;

namespace Sitecron.Samples
{
    public class SampleLogJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Sample Log Job - Add Log Entry.", this);
        }
    }
}