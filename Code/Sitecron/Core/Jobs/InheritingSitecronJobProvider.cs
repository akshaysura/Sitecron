using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Jobs
{
    public class InheritingSitecronJobProvider : DatabaseJobProvider
    {
        public InheritingSitecronJobProvider(ISitecronJobValidator jobValidator) : base(jobValidator)
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