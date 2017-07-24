
using YStory.Core.Configuration;

namespace YStory.Core.Domain.Catalog
{
    public class ArticleEditorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether 'ID' field is shown
        /// </summary>
        public bool Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article type' field is shown
        /// </summary>
        public bool ArticleType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Visible individually' field is shown
        /// </summary>
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article template' field is shown
        /// </summary>
        public bool ArticleTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Admin comment' feild is shown
        /// </summary>
        public bool AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Contributor' field is shown
        /// </summary>
        public bool Contributor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Stores' field is shown
        /// </summary>
        public bool Stores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'ACL' field is shown
        /// </summary>
        public bool ACL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Show on home page' field is shown
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Display subscription 'field is shown
        /// </summary>
        public bool DisplaySubscription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow customer reviews' field is shown
        /// </summary>
        public bool AllowCustomerReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article tags' field is shown
        /// </summary>
        public bool ArticleTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Publisher part number' field is shown
        /// </summary>
        public bool PublisherPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'GTIN' field is shown
        /// </summary>
        public bool GTIN { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article cost' field is shown
        /// </summary>
        public bool ArticleCost { get; set; }

       

        /// <summary>
        /// Gets or sets a value indicating whether 'Discounts' field is shown
        /// </summary>
        public bool Discounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Disable buy button' field is shown
        /// </summary>
        public bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Disable wishlist button' field is shown
        /// </summary>
        public bool DisableWishlistButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Available for pre-subscription' field is shown
        /// </summary>
        public bool AvailableForPreSubscription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Call for price' field is shown
        /// </summary>
        public bool CallForPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Old price' field is shown
        /// </summary>
        public bool OldPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Customer enters price' field is shown
        /// </summary>
        public bool CustomerEntersPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'PAngV' field is shown
        /// </summary>
        public bool PAngV { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Require other articles added to the cart' field is shown
        /// </summary>
        public bool RequireOtherArticlesAddedToTheCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Is gift card' field is shown
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Downloadable article' field is shown
        /// </summary>
        public bool DownloadableArticle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Recurring article' field is shown
        /// </summary>
        public bool RecurringArticle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Is rental' field is shown
        /// </summary>
        public bool IsRental { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Free shipping' field is shown
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Ship separately' field is shown
        /// </summary>
        public bool ShipSeparately { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Additional shipping charge' field is shown
        /// </summary>
        public bool AdditionalShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Delivery date' field is shown
        /// </summary>
        public bool DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Telecommunications, broadcasting and electronic services' field is shown
        /// </summary>
        public bool TelecommunicationsBroadcastingElectronicServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article availability range' field is shown
        /// </summary>
        public bool ArticleAvailabilityRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Use multiple warehouses' field is shown
        /// </summary>
        public bool UseMultipleWarehouses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Warehouse' field is shown
        /// </summary>
        public bool Warehouse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Display stock availability' field is shown
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Display stock quantity' field is shown
        /// </summary>
        public bool DisplayStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Minimum stock quantity' field is shown
        /// </summary>
        public bool MinimumStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Low stock activity' field is shown
        /// </summary>
        public bool LowStockActivity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Notify admin for quantity below' field is shown
        /// </summary>
        public bool NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Backsubscriptions' field is shown
        /// </summary>
        public bool Backsubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow back in stock subscriptions' field is shown
        /// </summary>
        public bool AllowBackInStockSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Minimum cart quantity' field is shown
        /// </summary>
        public bool MinimumCartQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Maximum cart quantity' field is shown
        /// </summary>
        public bool MaximumCartQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allowed quantities' field is shown
        /// </summary>
        public bool AllowedQuantities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Allow only existing attribute combinations' field is shown
        /// </summary>
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Not returnable' field is shown
        /// </summary>
        public bool NotReturnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Weight' field is shown
        /// </summary>
        public bool Weight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Dimension' fields (height, length, width) are shown
        /// </summary>
        public bool Dimensions { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether 'Available start date' field is shown
        /// </summary>
        public bool AvailableStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Available end date' field is shown
        /// </summary>
        public bool AvailableEndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new' field is shown
        /// </summary>
        public bool MarkAsNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new. Start date' field is shown
        /// </summary>
        public bool MarkAsNewStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Mark as new. End date' field is shown
        /// </summary>
        public bool MarkAsNewEndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Published' field is shown
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Created on' field is shown
        /// </summary>
        public bool CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Updated on' field is shown
        /// </summary>
        public bool UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Related articles' block is shown
        /// </summary>
        public bool RelatedArticles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Cross-sells articles' block is shown
        /// </summary>
        public bool CrossSellsArticles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'SEO' tab is shown
        /// </summary>
        public bool Seo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Purchased with subscriptions' tab is shown
        /// </summary>
        public bool PurchasedWithSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one column is used on the article details page
        /// </summary>
        public bool OneColumnArticlePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Article attributes' tab is shown
        /// </summary>
        public bool ArticleAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Specification attributes' tab is shown
        /// </summary>
        public bool SpecificationAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Publishers' field is shown
        /// </summary>
        public bool Publishers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Stock quantity history' tab is shown
        /// </summary>
        public bool StockQuantityHistory { get; set; }
    }
}