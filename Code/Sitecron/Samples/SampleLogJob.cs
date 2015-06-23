using Quartz;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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