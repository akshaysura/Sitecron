using Sitecore.Diagnostics;

namespace Sitecron.Demo.Jobs
{
    public class DummyJob
    {
        public void SomeWork()
        {
            Log.Info("DummyJob.SomeWork - Start", this);
            System.Threading.Thread.Sleep(5000);
            Log.Info("DummyJob.SomeWork - End", this);
        }
    }
}