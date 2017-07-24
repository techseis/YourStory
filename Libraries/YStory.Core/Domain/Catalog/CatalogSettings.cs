using System.Collections.Generic;
using YStory.Core.Configuration;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Catalog settings
    /// </summary>
    public class CatalogSettings : ISettings
    {
        public CatalogSettings()
        {
            ArticleSortingEnumDisabled = new List<int>();
            ArticleSortingEnumDisplaySubscription= new Dictionary<int, int>();
        }

        /// <summary>
        /// Gets or sets a value indicating details pages of unpublished article details pages could be open (for SEO optimization)
        /// </summary>
        public bool AllowViewUnpublishedArticlePage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating customers should see "discontinued" message when visibting details pages of unpublished articles (if "AllowViewUnpublishedArticlePage" is "true)
        /// </summary>
        public bool DisplayDiscontinuedMessageForUnpublishedArticles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Published" or "Disable buy/wishlist buttons" flags should be updated after subscription cancellation (deletion).
        /// Of course, when qty > configured minimum stock level
        /// </summary>
        public bool PublishBackArticleWhenCancellingSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display article SKU on the article details page
        /// </summary>
        public bool ShowSkuOnArticleDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display article SKU on catalog pages
        /// </summary>
        public bool ShowSkuOnCatalogPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display publisher part number of a article
        /// </summary>
        public bool ShowPublisherPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display GTIN of a article
        /// </summary>
        public bool ShowGtin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Free shipping" icon should be displayed for articles
        /// </summary>
        public bool ShowFreeShippingNotification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether article sorting is enabled
        /// </summary>
        public bool AllowArticleSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change article view mode
        /// </summary>
        public bool AllowArticleViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change article view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a category details page should include articles from subcategories
        /// </summary>
        public bool ShowArticlesFromSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of articles should be displayed beside each category
        /// </summary>
        public bool ShowCategoryArticleNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryArticleNumber' is 'true')
        /// </summary>
        public bool ShowCategoryArticleNumberIncludingSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category breadcrumb is enabled
        /// </summary>
        public bool CategoryBreadcrumbEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating article reviews must be approved
        /// </summary>
        public bool ArticleReviewsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default rating value of the article reviews
        /// </summary>
        public int DefaultArticleRatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write article reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewArticle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether article can be reviewed only by customer who have already subscriptioned it
        /// </summary>
        public bool ArticleReviewPossibleOnlyAfterPurchasing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notification of a store owner about new article reviews is enabled
        /// </summary>
        public bool NotifyStoreOwnerAboutNewArticleReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article reviews will be filtered per store
        /// </summary>
        public bool ShowArticleReviewsPerStore { get; set; }

        /// <summary>
        /// Gets or sets a show article reviews tab on account page
        /// </summary>
        public bool ShowArticleReviewsTabOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets the page size for article reviews in account page
        /// </summary>
        public int ArticleReviewsPageSizeOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether article 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed articles"
        /// </summary>
        public int RecentlyViewedArticlesNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed articles" feature is enabled
        /// </summary>
        public bool RecentlyViewedArticlesEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of articles on the "New articles" page
        /// </summary>
        public int NewArticlesNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "New articles" page is enabled
        /// </summary>
        public bool NewArticlesEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare articles" feature is enabled
        /// </summary>
        public bool CompareArticlesEnabled { get; set; }
        /// <summary>
        /// Gets or sets an allowed number of articles to be compared
        /// </summary>
        public int CompareArticlesNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether autocomplete is enabled
        /// </summary>
        public bool ArticleSearchAutoCompleteEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of articles to return when using "autocomplete" feature
        /// </summary>
        public int ArticleSearchAutoCompleteNumberOfArticles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show article images in the auto complete search
        /// </summary>
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }
        /// <summary>
        /// Gets or sets a minimum search term length
        /// </summary>
        public int ArticleSearchTermMinimumLength { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to show bestsellers on home page
        /// </summary>
        public bool ShowBestsellersOnHomepage { get; set; }
        /// <summary>
        /// Gets or sets a number of bestsellers on home page
        /// </summary>
        public int NumberOfBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of articles per page on the search articles page
        /// </summary>
        public int SearchPageArticlesPerPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size on the search articles page
        /// </summary>
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        /// <summary>
        /// Gets or sets the available customer selectable page size options on the search articles page
        /// </summary>
        public string SearchPagePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets "List of articles purchased by other customers who purchased the above" option is enable
        /// </summary>
        public bool ArticlesAlsoPurchasedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of articles also purchased by other customers to display
        /// </summary>
        public int ArticlesAlsoPurchasedNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should process attribute change using AJAX. It's used for dynamical attribute change, SKU/GTIN update of combinations, conditional attributes
        /// </summary>
        public bool AjaxProcessAttributeChange { get; set; }
        
        /// <summary>
        /// Gets or sets a number of article tags that appear in the tag cloud
        /// </summary>
        public int NumberOfArticleTags { get; set; }

        /// <summary>
        /// Gets or sets a number of articles per page on 'articles by tag' page
        /// </summary>
        public int ArticlesByTagPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size for 'articles by tag'
        /// </summary>
        public bool ArticlesByTagAllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options for 'articles by tag'
        /// </summary>
        public string ArticlesByTagPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include "Short description" in compare articles
        /// </summary>
        public bool IncludeShortDescriptionInCompareArticles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to include "Full description" in compare articles
        /// </summary>
        public bool IncludeFullDescriptionInCompareArticles { get; set; }
        /// <summary>
        /// An option indicating whether articles on category and publisher pages should include featured articles as well
        /// </summary>
        public bool IncludeFeaturedArticlesInNormalLists { get; set; }
        
         
        /// <summary>
        /// Gets or sets a value indicating whether to ignore featured articles (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreFeaturedArticles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to cache article prices. It can significantly improve performance when enabled.
        /// </summary>
        public bool CacheArticlePrices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of 'back in stock' subscription
        /// </summary>
        public int MaximumBackInStockSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many publishers to display in publishers block
        /// </summary>
        public int PublishersBlockItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in the footer (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoFooter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on article details pages (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoArticleDetailsPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in article boxes (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoArticleBoxes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on shopping cart page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on wishlist page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on subscription details page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoSubscriptionDetailsPage { get; set; }
        
        /// <summary>
        /// Gets or sets the default value to use for Category page size options (for new categories)
        /// </summary>
        public string DefaultCategoryPageSizeOptions { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Category page size (for new categories)
        /// </summary>
        public int DefaultCategoryPageSize { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Publisher page size options (for new publishers)
        /// </summary>
        public string DefaultPublisherPageSizeOptions { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Publisher page size (for new publishers)
        /// </summary>
        public int DefaultPublisherPageSize { get; set; }

        /// <summary>
        /// Gets or sets a list of disabled values of ArticleSortingEnum
        /// </summary>
        public List<int> ArticleSortingEnumDisabled { get; set; }

        /// <summary>
        /// Gets or sets a display subscription of ArticleSortingEnum values 
        /// </summary>
        public Dictionary<int, int> ArticleSortingEnumDisplaySubscription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the articles need to be exported/imported with their attributes
        /// </summary>
        public bool ExportImportArticleAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool ExportImportUseDropdownlistsForAssociatedEntities { get; set; }
    }
}