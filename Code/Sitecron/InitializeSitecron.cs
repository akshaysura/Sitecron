using Sitecore.Pipelines;

namespace Sitecron
{
    public class InitializeSitecron
    {
        public virtual void Process(PipelineArgs args)
        {
            ScheduleHelper scheduler = new ScheduleHelper();
            scheduler.InitializeScheduler();
        }
    }
}