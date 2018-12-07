using System;
using Sitecron.Extend;

namespace Sitecron.Custom.Publishing
{
    public class ProcessPublishRestrictions : ISavedHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            /*
             * 
             * string whyCantYouBeMyFriend = "";
             * 
             * 
             __   __  ___   _______  __   __  ___      __   __    _______  __   __  _______  _______  ______    ___   __   __  _______  __    _  _______  _______  ___     
            |  | |  ||   | |       ||  | |  ||   |    |  | |  |  |       ||  |_|  ||       ||       ||    _ |  |   | |  |_|  ||       ||  |  | ||       ||   _   ||   |    
            |  |_|  ||   | |    ___||  |_|  ||   |    |  |_|  |  |    ___||       ||    _  ||    ___||   | ||  |   | |       ||    ___||   |_| ||_     _||  |_|  ||   |    
            |       ||   | |   | __ |       ||   |    |       |  |   |___ |       ||   |_| ||   |___ |   |_||_ |   | |       ||   |___ |       |  |   |  |       ||   |    
            |       ||   | |   ||  ||       ||   |___ |_     _|  |    ___| |     | |    ___||    ___||    __  ||   | |       ||    ___||  _    |  |   |  |       ||   |___ 
            |   _   ||   | |   |_| ||   _   ||       |  |   |    |   |___ |   _   ||   |    |   |___ |   |  | ||   | | ||_|| ||   |___ | | |   |  |   |  |   _   ||       |
            |__| |__||___| |_______||__| |__||_______|  |___|    |_______||__| |__||___|    |_______||___|  |_||___| |_|   |_||_______||_|  |__|  |___|  |__| |__||_______|

            */
            //bool scheduleAutoPublish = false;
            //if (bool.TryParse(Settings.GetSetting(SitecronConstants.SettingsNames.SiteCronScheduleAutoPublish), out scheduleAutoPublish) && scheduleAutoPublish)
            //{
            //    ItemChanges savedItemChanges = Event.ExtractParameter(args, 1) as ItemChanges;

            //    // Ensure that a change was made to the valid to/from fields
            //    if (savedItem != null &&
            //        savedItemChanges != null &&
            //        savedItemChanges.FieldChanges.ContainsAnyOf(FieldIDs.ValidFrom, FieldIDs.ValidTo))
            //    {
            //        if (savedItem.Publishing.ValidTo != DateTime.MaxValue ||
            //        savedItem.Publishing.ValidFrom.ToString("MMddyyyyHHmmss") !=
            //        savedItem.Statistics.Created.ToString("MMddyyyyHHmmss"))
            //        {
            //            var validFromDateField = (DateField)savedItem.Fields["__valid from"];
            //            var validToDateField = (DateField)savedItem.Fields["__valid to"];

            //            if (validFromDateField != null && validToDateField != null)
            //            {
            //                var validFromDate = DateUtil.ToServerTime(validFromDateField.DateTime);
            //                var validToDate = DateUtil.ToServerTime(validToDateField.DateTime);
            //                Log.Info(string.Format("VERSION ItemID:{0} ItemName:{1} ItemVersion:{2} version Publishable From:{3}  version Publishable To:{4}  ", savedItem.ID.ToString(), savedItem.Name, savedItem.Version.Number.ToString(), validFromDate.ToString(), validToDate.ToString()), this);

            //                //need to add some checks and balances

            //            }
            //        }
            //    }

            //    // Ensure that a change was made to the valid to/from fields
            //    if (savedItem != null &&
            //        savedItemChanges != null &&
            //        savedItemChanges.FieldChanges.ContainsAnyOf(FieldIDs.UnpublishDate, FieldIDs.PublishDate))
            //    {
            //        // Ensure that the item is not brand new (such as when an author locks or clicks Edit)
            //        if (savedItem.Publishing.ValidTo != DateTime.MaxValue ||
            //            savedItem.Publishing.ValidFrom.ToString("MMddyyyyHHmmss") !=
            //            savedItem.Statistics.Created.ToString("MMddyyyyHHmmss"))
            //        {
            //            var publishDateField = (DateField)savedItem.Fields["__publish"];
            //            var unpublishDateField = (DateField)savedItem.Fields["__unpublish"];

            //            if (publishDateField != null && unpublishDateField != null)
            //            {
            //                var publishDate = DateUtil.ToServerTime(publishDateField.DateTime);
            //                var unpublishDate = DateUtil.ToServerTime(unpublishDateField.DateTime);

            //                Log.Info(string.Format("ITEM ItemID:{0} ItemName:{1} ItemVersion:{2} item Publishable From:{3}  item Publishable To:{4}", savedItem.ID.ToString(), savedItem.Name, savedItem.Version.Number.ToString(), publishDate.ToString(), unpublishDate.ToString()), this);

            //                //need to add some checks and balances
            //            }
            //        }
            //    }
            //}
        }
    }
}