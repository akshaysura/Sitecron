using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecron.SitecronSettings;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Linq;
using Sitecore.Configuration;
using System;
using Sitecore.Diagnostics;

namespace Sitecron.Core.Jobs
{
    public class InheritingSitecronJobProvider : DatabaseJobProvider
    {
        public InheritingSitecronJobProvider(ISitecronJobValidator jobValidator) : base(jobValidator)
        {
        }

        protected override IEnumerable<Item> GetJobItems()
        {
            List<Item> siteCronItems = new List<Item>();
            try
            {
                var folder = ContextDatabase?.GetItem(new ID(SitecronConstants.ItemIds.RootFolderID));
                if (folder == null)
                {
                    return Enumerable.Empty<Item>();
                }
                List<ID> validTemplates = new List<ID>();
                string[] configSiteCronTemplates = Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronValidTemplates, "").Trim().Replace(" ", "").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string templateId in configSiteCronTemplates)
                {
                    ID testId;
                    if (ID.TryParse(templateId, out testId))
                    {
                        validTemplates.Add(testId);
                    }
                }

                if (validTemplates.Any())
                {
                    var index = ContentSearchManager.GetIndex(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronGetItemsIndex, "sitecore_master_index").Trim());
                    if (index != null)
                    {
                        using (var context = index.CreateSearchContext())
                        {
                            var query = PredicateBuilder.True<SearchResultItem>();
                            query = query.And(i => i.Paths.Contains(folder.ID));
                            query = query.And(i => validTemplates.Contains(i.TemplateId));

                            var results = context.GetQueryable<SearchResultItem>().Where(query).GetResults();
                            siteCronItems.AddRange(results.Select(i => i.Document.GetItem()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitecron InheritingSitecronJobProvider ERROR: " + ex.Message, ex, this);
            }
            return siteCronItems;
        }
    }
}