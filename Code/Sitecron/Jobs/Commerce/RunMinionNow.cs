using Quartz;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecron.Jobs.Commerce
{
    public class RunMinionNow : IJob //Inherit from IJob interface from Quartz
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var container = EngineConnectUtility.GetShopsContainer(string.Empty, storefrontContext.CurrentStorefront.ShopName, storefrontContext.CurrentStorefront.Context.User.GetId().ToString());
                Proxy.DoCommand(container.RunMinionNow("Sitecore.Commerce.Plugin.Orders.PendingOrdersMinionBoss, Sitecore.Commerce.Plugin.Orders", "HabitatMinions"));
            }
            catch (Exception ex)
            {
                Log.Error("Sitecron: Commerce.RunMinionNow: ERROR something went wrong - " + ex.Message, ex, this);
            }
        }
    }
}