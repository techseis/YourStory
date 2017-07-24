using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class CatalogSettingsModel : BaseYStoryModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowViewUnpublishedArticlePage")]
        public bool AllowViewUnpublishedArticlePage { get; set; }
        public bool AllowViewUnpublishedArticlePage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedArticles")]
        public bool DisplayDiscontinuedMessageForUnpublishedArticles { get; set; }
        public bool DisplayDiscontinuedMessageForUnpublishedArticles_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnArticleDetailsPage")]
        public bool ShowSkuOnArticleDetailsPage { get; set; }
        public bool ShowSkuOnArticleDetailsPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnCatalogPages")]
        public bool ShowSkuOnCatalogPages { get; set; }
        public bool ShowSkuOnCatalogPages_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPublisherPartNumber")]
        public bool ShowPublisherPartNumber { get; set; }
        public bool ShowPublisherPartNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowGtin")]
        public bool ShowGtin { get; set; }
        public bool ShowGtin_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowFreeShippingNotification")]
        public bool ShowFreeShippingNotification { get; set; }
        public bool ShowFreeShippingNotification_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowArticleSorting")]
        public bool AllowArticleSorting { get; set; }
        public bool AllowArticleSorting_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowArticleViewModeChanging")]
        public bool AllowArticleViewModeChanging { get; set; }
        public bool AllowArticleViewModeChanging_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticlesFromSubcategories")]
        public bool ShowArticlesFromSubcategories { get; set; }
        public bool ShowArticlesFromSubcategories_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryArticleNumber")]
        public bool ShowCategoryArticleNumber { get; set; }
        public bool ShowCategoryArticleNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryArticleNumberIncludingSubcategories")]
        public bool ShowCategoryArticleNumberIncludingSubcategories { get; set; }
        public bool ShowCategoryArticleNumberIncludingSubcategories_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.PageShareCode")]
        [AllowHtml]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleReviewsMustBeApproved")]
        public bool ArticleReviewsMustBeApproved { get; set; }
        public bool ArticleReviewsMustBeApproved_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewArticle")]
        public bool AllowAnonymousUsersToReviewArticle { get; set; }
        public bool AllowAnonymousUsersToReviewArticle_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleReviewPossibleOnlyAfterPurchasing")]
        public bool ArticleReviewPossibleOnlyAfterPurchasing { get; set; }
        public bool ArticleReviewPossibleOnlyAfterPurchasing_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyStoreOwnerAboutNewArticleReviews")]
        public bool NotifyStoreOwnerAboutNewArticleReviews { get; set; }
        public bool NotifyStoreOwnerAboutNewArticleReviews_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleReviewsPerStore")]
        public bool ShowArticleReviewsPerStore { get; set; }
        public bool ShowArticleReviewsPerStore_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleReviewsTabOnAccountPage")]
        public bool ShowArticleReviewsTabOnAccountPage { get; set; }
        public bool ShowArticleReviewsOnAccountPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleReviewsPageSizeOnAccountPage")]
        public int ArticleReviewsPageSizeOnAccountPage { get; set; }
        public bool ArticleReviewsPageSizeOnAccountPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedArticlesNumber")]
        public int RecentlyViewedArticlesNumber { get; set; }
        public bool RecentlyViewedArticlesNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedArticlesEnabled")]
        public bool RecentlyViewedArticlesEnabled { get; set; }
        public bool RecentlyViewedArticlesEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.NewArticlesNumber")]
        public int NewArticlesNumber { get; set; }
        public bool NewArticlesNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.NewArticlesEnabled")]
        public bool NewArticlesEnabled { get; set; }
        public bool NewArticlesEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.CompareArticlesEnabled")]
        public bool CompareArticlesEnabled { get; set; }
        public bool CompareArticlesEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }
        public bool ShowBestsellersOnHomepage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }
        public bool NumberOfBestsellersOnHomepage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageArticlesPerPage")]
        public int SearchPageArticlesPerPage { get; set; }
        public bool SearchPageArticlesPerPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize")]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        public bool SearchPageAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchAutoCompleteEnabled")]
        public bool ArticleSearchAutoCompleteEnabled { get; set; }
        public bool ArticleSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchAutoCompleteNumberOfArticles")]
        public int ArticleSearchAutoCompleteNumberOfArticles { get; set; }
        public bool ArticleSearchAutoCompleteNumberOfArticles_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowArticleImagesInSearchAutoComplete")]
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }
        public bool ShowArticleImagesInSearchAutoComplete_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticleSearchTermMinimumLength")]
        public int ArticleSearchTermMinimumLength { get; set; }
        public bool ArticleSearchTermMinimumLength_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesAlsoPurchasedEnabled")]
        public bool ArticlesAlsoPurchasedEnabled { get; set; }
        public bool ArticlesAlsoPurchasedEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesAlsoPurchasedNumber")]
        public int ArticlesAlsoPurchasedNumber { get; set; }
        public bool ArticlesAlsoPurchasedNumber_OverrideForStore { get; set; }
        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfArticleTags")]
        public int NumberOfArticleTags { get; set; }
        public bool NumberOfArticleTags_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagPageSize")]
        public int ArticlesByTagPageSize { get; set; }
        public bool ArticlesByTagPageSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagAllowCustomersToSelectPageSize")]
        public bool ArticlesByTagAllowCustomersToSelectPageSize { get; set; }
        public bool ArticlesByTagAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ArticlesByTagPageSizeOptions")]
        public string ArticlesByTagPageSizeOptions { get; set; }
        public bool ArticlesByTagPageSizeOptions_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareArticles")]
        public bool IncludeShortDescriptionInCompareArticles { get; set; }
        public bool IncludeShortDescriptionInCompareArticles_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareArticles")]
        public bool IncludeFullDescriptionInCompareArticles { get; set; }
        public bool IncludeFullDescriptionInCompareArticles_OverrideForStore { get; set; }
        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.PublishersBlockItemsToDisplay")]
        public int PublishersBlockItemsToDisplay { get; set; }
        public bool PublishersBlockItemsToDisplay_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter")]
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool DisplayTaxShippingInfoFooter_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoArticleDetailsPage")]
        public bool DisplayTaxShippingInfoArticleDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoArticleDetailsPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoArticleBoxes")]
        public bool DisplayTaxShippingInfoArticleBoxes { get; set; }
        public bool DisplayTaxShippingInfoArticleBoxes_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart")]
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        public bool DisplayTaxShippingInfoShoppingCart_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist")]
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        public bool DisplayTaxShippingInfoWishlist_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoSubscriptionDetailsPage")]
        public bool DisplayTaxShippingInfoSubscriptionDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoSubscriptionDetailsPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportArticleAttributes")]
        public bool ExportImportArticleAttributes { get; set; }
        public bool ExportImportArticleAttributes_OverrideForStore { get; set; }
        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreDiscounts")]
        public bool IgnoreDiscounts { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedArticles")]
        public bool IgnoreFeaturedArticles { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.CacheArticlePrices")]
        public bool CacheArticlePrices { get; set; }
    }
}