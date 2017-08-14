using Quartz;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleJob.Parameters
{
    public class TestParams : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string jobParams = dataMap.GetString("Parameters");

            Log.Info(string.Format("Instance {0} of TestParams Job - {4}Parameters: {1} {4}Fired at: {2} {4}Next Scheduled For:{3}", context.JobDetail.Key, jobParams, context.FireTimeUtc.Value.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"), Environment.NewLine), this);
        }
    }
}
