using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecron.SitecronSettings;

namespace Sitecron.Core.Jobs
{
    public class QueryJobProvider : DatabaseJobProvider
    {
        public QueryJobProvider(ISitecronJobValidator jobValidator) : base(jobValidator)
        {
        }

        protected override IEnumerable<Item> GetJobItems()
        {
            return ContextDatabase == null
                ? Enumerable.Empty<Item>()
                : ContextDatabase.SelectItems(SitecronConstants.Queries.QueryRetriveJobs);
        }
    }
}