using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecron.SitecronSettings;

namespace Sitecron.Jobs
{
    public class InheritingDescendantsSitecronJobProvider : DatabaseJobProvider
    {
        public InheritingDescendantsSitecronJobProvider(ISitecronJobValidator jobValidator) : base(jobValidator)
        {
        }

        protected override IEnumerable<Item> GetJobItems()
        {
            var folder = ContextDatabase?.GetItem(new ID(SitecronConstants.ItemIds.RootFolderID));
            if (folder == null)
            {
                return Enumerable.Empty<Item>();
            }

            return folder.Axes.GetDescendants().Where(i =>
                TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, i));
        }
    }
}