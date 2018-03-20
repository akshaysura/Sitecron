namespace Sitecron.Jobs
{
    public interface ISitecronJobValidator
    {
        bool IsValid(SitecronJob job);
    }
}
