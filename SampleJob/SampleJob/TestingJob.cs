using Quartz;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleJob
{
    public class TestingJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> LOG LOG LOG", this);
        }
    }
}
