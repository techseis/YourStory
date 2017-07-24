using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class ContributorSettingsModel : BaseYStoryModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.ContributorsBlockItemsToDisplay")]
        public int ContributorsBlockItemsToDisplay { get; set; }
        public bool ContributorsBlockItemsToDisplay_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.ShowContributorOnArticleDetailsPage")]
        public bool ShowContributorOnArticleDetailsPage { get; set; }
        public bool ShowContributorOnArticleDetailsPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.AllowCustomersToContactContributors")]
        public bool AllowCustomersToContactContributors { get; set; }
        public bool AllowCustomersToContactContributors_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.AllowCustomersToApplyForContributorAccount")]
        public bool AllowCustomersToApplyForContributorAccount { get; set; }
        public bool AllowCustomersToApplyForContributorAccount_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.AllowSearchByContributor")]
        public bool AllowSearchByContributor { get; set; }
        public bool AllowSearchByContributor_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.AllowContributorsToEditInfo")]
        public bool AllowContributorsToEditInfo { get; set; }
        public bool AllowContributorsToEditInfo_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.NotifyStoreOwnerAboutContributorInformationChange")]
        public bool NotifyStoreOwnerAboutContributorInformationChange { get; set; }
        public bool NotifyStoreOwnerAboutContributorInformationChange_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.MaximumArticleNumber")]
        public int MaximumArticleNumber { get; set; }
        public bool MaximumArticleNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Contributor.AllowContributorsToImportArticles")]
        public bool AllowContributorsToImportArticles { get; set; }
        public bool AllowContributorsToImportArticles_OverrideForStore { get; set; }
    }
}