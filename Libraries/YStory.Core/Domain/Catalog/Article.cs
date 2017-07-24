using System;
using System.Collections.Generic;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Security;
using YStory.Core.Domain.Seo;
using YStory.Core.Domain.Stores;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article
    /// </summary>
    public partial class Article : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported, IStoreMappingSupported
    {
        private ICollection<ArticleCategory> _articleCategories;
        private ICollection<ArticlePublisher> _articlePublishers;
        private ICollection<ArticlePicture> _articlePictures;
        private ICollection<ArticleReview> _articleReviews;
        private ICollection<ArticleSpecificationAttribute> _articleSpecificationAttributes;
        private ICollection<ArticleTag> _articleTags;
        private ICollection<ArticleAttributeMapping> _articleAttributeMappings;
        private ICollection<ArticleAttributeCombination> _articleAttributeCombinations;


        /// <summary>
        /// Gets or sets the article type identifier
        /// </summary>
        public int ArticleTypeId { get; set; }
        /// <summary>
        /// Gets or sets the parent article identifier. It's used to identify associated articles (only with "grouped" articles)
        /// </summary>
        public int ParentGroupedArticleId { get; set; }
        /// <summary>
        /// Gets or sets the values indicating whether this article is visible in catalog or search results.
        /// It's used when this article is associated to some "grouped" one
        /// This way associated articles could be accessed/added/etc only from a grouped article details page
        /// </summary>
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value of used article template identifier
        /// </summary>
        public int ArticleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets a contributor identifier
        /// </summary>
        public int ContributorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the article on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }
        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }
        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article allows customer reviews
        /// </summary>
        public bool AllowCustomerReviews { get; set; }
        /// <summary>
        /// Gets or sets the rating sum (approved reviews)
        /// </summary>
        public int ApprovedRatingSum { get; set; }
        /// <summary>
        /// Gets or sets the rating sum (not approved reviews)
        /// </summary>
        public int NotApprovedRatingSum { get; set; }
        /// <summary>
        /// Gets or sets the total rating votes (approved reviews)
        /// </summary>
        public int ApprovedTotalReviews { get; set; }
        /// <summary>
        /// Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        public int NotApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string Sku { get; set; }
        /// <summary>
        /// Gets or sets the publisher part number
        /// </summary>
        public string PublisherPartNumber { get; set; }
        /// <summary>
        /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>
        public string Gtin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is gift card
        /// </summary>
        public bool IsGiftCard { get; set; }
        /// <summary>
        /// Gets or sets the gift card type identifier
        /// </summary>
        public int GiftCardTypeId { get; set; }
        /// <summary>
        /// Gets or sets gift card amount that can be used after purchase. If not specified, then article price will be used.
        /// </summary>
        public decimal? OverriddenGiftCardAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article requires that other articles are added to the cart (Article X requires Article Y)
        /// </summary>
        public bool RequireOtherArticles { get; set; }
        /// <summary>
        /// Gets or sets a required article identifiers (comma separated)
        /// </summary>
        public string RequiredArticleIds { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether required articles are automatically added to the cart
        /// </summary>
        public bool AutomaticallyAddRequiredArticles { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether the article has user agreement
        /// </summary>
        public bool HasUserAgreement { get; set; }
        /// <summary>
        /// Gets or sets the text of license agreement
        /// </summary>
        public string UserAgreementText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is recurring
        /// </summary>
        public bool IsRecurring { get; set; }
        /// <summary>
        /// Gets or sets the cycle length
        /// </summary>
        public int RecurringCycleLength { get; set; }
        /// <summary>
        /// Gets or sets the cycle period
        /// </summary>
        public int RecurringCyclePeriodId { get; set; }
        /// <summary>
        /// Gets or sets the total cycles
        /// </summary>
        public int RecurringTotalCycles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is rental
        /// </summary>
        public bool IsRental { get; set; }
        /// <summary>
        /// Gets or sets the rental length for some period (price for this period)
        /// </summary>
        public int RentalPriceLength { get; set; }
        /// <summary>
        /// Gets or sets the rental period (price for this period)
        /// </summary>
        public int RentalPricePeriodId { get; set; }

        

        /// <summary>
        /// Gets or sets a value indicating whether the article is marked as tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }
        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether we allow adding to the cart/wishlist only attribute combinations that exist and have stock greater than zero.
        /// This option is used only when we have "manage inventory" set to "track inventory by article attributes"
        /// </summary>
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this article is returnable (a customer is allowed to submit return request with this article)
        /// </summary>
        public bool NotReturnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable buy (Add to cart) button
        /// </summary>
        public bool DisableBuyButton { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to disable "Add to wishlist" button
        /// </summary>
        public bool DisableWishlistButton { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this item is available for Pre-Subscription
        /// </summary>
        public bool AvailableForPreSubscription { get; set; }
        /// <summary>
        /// Gets or sets the start date and time of the article availability (for pre-subscription articles)
        /// </summary>
        public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show "Call for Pricing" or "Call for quote" instead of price
        /// </summary>
        public bool CallForPrice { get; set; }
        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }
        /// <summary>
        /// Gets or sets the article cost
        /// </summary>
        public decimal ArticleCost { get; set; }
       

        


        /// <summary>
        /// Gets or sets a value indicating whether this article is marked as new
        /// </summary>
        public bool MarkAsNew { get; set; }
        /// <summary>
        /// Gets or sets the start date and time of the new article (set article as "New" from date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the end date and time of the new article (set article as "New" to date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

 
 
 

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public DateTime? AvailableStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the available end date and time
        /// </summary>
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a display subscription.
        /// This value is used when sorting associated articles (used with "grouped" articles)
        /// This value is used when sorting home page articles
        /// </summary>
        public int DisplaySubscription { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of article creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of article update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }






        /// <summary>
        /// Gets or sets the article type
        /// </summary>
        public ArticleType ArticleType
        {
            get
            {
                return (ArticleType)this.ArticleTypeId;
            }
            set
            {
                this.ArticleTypeId = (int)value;
            }
        }
 
 

      

      
        /// <summary>
        /// Gets or sets the cycle period for recurring articles
        /// </summary>
        public RecurringArticleCyclePeriod RecurringCyclePeriod
        {
            get
            {
                return (RecurringArticleCyclePeriod)this.RecurringCyclePeriodId;
            }
            set
            {
                this.RecurringCyclePeriodId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the period for rental articles
        /// </summary>
        public RentalPricePeriod RentalPricePeriod
        {
            get
            {
                return (RentalPricePeriod)this.RentalPricePeriodId;
            }
            set
            {
                this.RentalPricePeriodId = (int)value;
            }
        }






        /// <summary>
        /// Gets or sets the collection of ArticleCategory
        /// </summary>
        public virtual ICollection<ArticleCategory> ArticleCategories
        {
            get { return _articleCategories ?? (_articleCategories = new List<ArticleCategory>()); }
            protected set { _articleCategories = value; }
        }

        /// <summary>
        /// Gets or sets the collection of ArticlePublisher
        /// </summary>
        public virtual ICollection<ArticlePublisher> ArticlePublishers
        {
            get { return _articlePublishers ?? (_articlePublishers = new List<ArticlePublisher>()); }
            protected set { _articlePublishers = value; }
        }

        /// <summary>
        /// Gets or sets the collection of ArticlePicture
        /// </summary>
        public virtual ICollection<ArticlePicture> ArticlePictures
        {
            get { return _articlePictures ?? (_articlePictures = new List<ArticlePicture>()); }
            protected set { _articlePictures = value; }
        }

        /// <summary>
        /// Gets or sets the collection of article reviews
        /// </summary>
        public virtual ICollection<ArticleReview> ArticleReviews
        {
            get { return _articleReviews ?? (_articleReviews = new List<ArticleReview>()); }
            protected set { _articleReviews = value; }
        }

        /// <summary>
        /// Gets or sets the article specification attribute
        /// </summary>
        public virtual ICollection<ArticleSpecificationAttribute> ArticleSpecificationAttributes
        {
            get { return _articleSpecificationAttributes ?? (_articleSpecificationAttributes = new List<ArticleSpecificationAttribute>()); }
            protected set { _articleSpecificationAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the article tags
        /// </summary>
        public virtual ICollection<ArticleTag> ArticleTags
        {
            get { return _articleTags ?? (_articleTags = new List<ArticleTag>()); }
            protected set { _articleTags = value; }
        }

        /// <summary>
        /// Gets or sets the article attribute mappings
        /// </summary>
        public virtual ICollection<ArticleAttributeMapping> ArticleAttributeMappings
        {
            get { return _articleAttributeMappings ?? (_articleAttributeMappings = new List<ArticleAttributeMapping>()); }
            protected set { _articleAttributeMappings = value; }
        }

        /// <summary>
        /// Gets or sets the article attribute combinations
        /// </summary>
        public virtual ICollection<ArticleAttributeCombination> ArticleAttributeCombinations
        {
            get { return _articleAttributeCombinations ?? (_articleAttributeCombinations = new List<ArticleAttributeCombination>()); }
            protected set { _articleAttributeCombinations = value; }
        }

        
 
        
        
    }
}