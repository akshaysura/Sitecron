using Quartz;
using Quartz.Impl;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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