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
using Sitecore.Data.Managers;

namespace Sitecron.Core.Jobs
{
    public class IndexJobProvider : DatabaseJobProvider
    {
        public IndexJobProvider(ISitecronJobValidator jobValidator) : base(jobValidator)
        {
        }

        protected override IEnumerable<Item> GetJobItems()
        {
            //get index items and auto folder items from db since they might not be in the index yet
            return GetJobItems(SitecronConstants.ItemIds.AutoFolderID);
        }

        protected override IEnumerable<Item> GetJobItems(ID rootFolderId)
        {
            List<Item> siteCronItems = new List<Item>();

            var folder = ContextDatabase?.GetItem(rootFolderId);
            if (folder == null)
            {
                return Enumerable.Empty<Item>();
            }

            var autoFolderItems = folder.Axes.GetDescendants().Where(i =>
                TemplateManager.IsFieldPartOfTemplate(SitecronConstants.SiteCronFieldIds.CronExpression, i));
            if (autoFolderItems != null && autoFolderItems.Any())
                siteCronItems.AddRange(autoFolderItems);

            var solrItems = GetSolrJobs();
            if (!solrItems.Any())
            {
                //ERROR: There is no method 'GetResults' on type 'Sitecore.ContentSearch.Linq.QueryableExtensions'
                Log.Warn("SiteCron IndexJobProvider got no results. Trying again assuming its Solr Initialization issue.", this);
                //might be caused due to solr not initializing
                System.Threading.Thread.Sleep(3000);
                solrItems = GetSolrJobs();
            }

            if (solrItems != null && solrItems.Any())
                siteCronItems.AddRange(solrItems.Where(i=> !siteCronItems.Contains(i)));
           
            return siteCronItems; 
        }

        private List<Item> GetSolrJobs()
        {
            List<Item> siteCronItems = new List<Item>();
            try
            {
                var folder = ContextDatabase?.GetItem(SitecronConstants.ItemIds.RootFolderID);
                if (folder == null)
                {
                    return siteCronItems;
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
                Log.Error("SiteCron IndexJobProvider ERROR: " + ex.Message, ex, this);
            }
            return siteCronItems;
        }
    }
}