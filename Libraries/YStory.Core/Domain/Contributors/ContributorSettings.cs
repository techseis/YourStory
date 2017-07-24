using YStory.Core.Configuration;

namespace YStory.Core.Domain.Contributors
{
    /// <summary>
    /// Contributor settings
    /// </summary>
    public class ContributorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the default value to use for Contributor page size options (for new contributors)
        /// </summary>
        public string DefaultContributorPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many contributors to display in contributors block
        /// </summary>
        public int ContributorsBlockItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display contributor name on the article details page
        /// </summary>
        public bool ShowContributorOnArticleDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can contact contributors
        /// </summary>
        public bool AllowCustomersToContactContributors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users can fill a form to become a new contributor
        /// </summary>
        public bool AllowCustomersToApplyForContributorAccount { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether it is possible to carry out advanced search in the store by contributor
        /// </summary>
        public bool AllowSearchByContributor { get; set; }

        /// <summary>
        /// Get or sets a value indicating whether contributor can edit information about itself (public store)
        /// </summary>
        public bool AllowContributorsToEditInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the store owner is notified that the contributor information has been changed
        /// </summary>
        public bool NotifyStoreOwnerAboutContributorInformationChange { get; set; }

        /// <summary>
        /// Gets or sets a maximum number of articles per contributor
        /// </summary>
        public int MaximumArticleNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contributors are allowed to import articles
        /// </summary>
        public bool AllowContributorsToImportArticles { get; set; }
    }
}
