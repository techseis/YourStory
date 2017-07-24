using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Media;

namespace YStory.Web.Models.Catalog
{
    public partial class ArticleDetailsModel : BaseYStoryEntityModel
    {
        public ArticleDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            ArticlePrice = new ArticlePriceModel();
            AddToCart = new AddToCartModel();
            ArticleAttributes = new List<ArticleAttributeModel>();
            AssociatedArticles = new List<ArticleDetailsModel>();
            ContributorModel = new ContributorBriefInfoModel();
            Breadcrumb = new ArticleBreadcrumbModel();
            ArticleTags = new List<ArticleTagModel>();
            ArticleSpecifications= new List<ArticleSpecificationModel>();
            ArticlePublishers = new List<PublisherBriefInfoModel>();
            ArticleReviewOverview = new ArticleReviewOverviewModel();
        }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public ArticleType ArticleType { get; set; }

        public bool ShowSku { get; set; }
        public string Sku { get; set; }

        public bool ShowPublisherPartNumber { get; set; }
        public string PublisherPartNumber { get; set; }

        public bool ShowGtin { get; set; }
        public string Gtin { get; set; }

        public bool ShowContributor { get; set; }
        public ContributorBriefInfoModel ContributorModel { get; set; }

   

       
        public string DeliveryDate { get; set; }


        public bool IsRental { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

       

        public bool EmailAFriendEnabled { get; set; }
        public bool CompareArticlesEnabled { get; set; }

        public string PageShareCode { get; set; }

        public ArticlePriceModel ArticlePrice { get; set; }

        public AddToCartModel AddToCart { get; set; }

        public ArticleBreadcrumbModel Breadcrumb { get; set; }

        public IList<ArticleTagModel> ArticleTags { get; set; }

        public IList<ArticleAttributeModel> ArticleAttributes { get; set; }

        public IList<ArticleSpecificationModel> ArticleSpecifications { get; set; }

        public IList<PublisherBriefInfoModel> ArticlePublishers { get; set; }

        public ArticleReviewOverviewModel ArticleReviewOverview { get; set; }

        

        //a list of associated articles. For example, "Grouped" articles could have several child "simple" articles
        public IList<ArticleDetailsModel> AssociatedArticles { get; set; }

        public bool DisplayDiscontinuedMessage { get; set; }

        public string CurrentStoreName { get; set; }

        #region Nested Classes

        public partial class ArticleBreadcrumbModel : BaseYStoryModel
        {
            public ArticleBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial class AddToCartModel : BaseYStoryModel
        {
            public AddToCartModel()
            {
                this.AllowedQuantities = new List<SelectListItem>();
            }
            public int ArticleId { get; set; }

            //qty
            [YStoryResourceDisplayName("Articles.Qty")]
            public int EnteredQuantity { get; set; }
            public string MinimumQuantityNotification { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            //price entered by customers
            [YStoryResourceDisplayName("Articles.EnterArticlePrice")]
            public bool CustomerEntersPrice { get; set; }
            [YStoryResourceDisplayName("Articles.EnterArticlePrice")]
            public decimal CustomerEnteredPrice { get; set; }
            public String CustomerEnteredPriceRange { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }

            //rental
            public bool IsRental { get; set; }

            //pre-subscription
            public bool AvailableForPreSubscription { get; set; }
            public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

            //updating existing shopping cart or wishlist item?
            public int UpdatedShoppingCartItemId { get; set; }
            public ShoppingCartType? UpdateShoppingCartItemType { get; set; }
        }

        public partial class ArticlePriceModel : BaseYStoryModel
        {
            /// <summary>
            /// The currency (in 3-letter ISO 4217 format) of the offer price 
            /// </summary>
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }

            public string Price { get; set; }
            public string PriceWithDiscount { get; set; }
            public decimal PriceValue { get; set; }

            public bool CustomerEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public int ArticleId { get; set; }

            public bool HidePrices { get; set; }

            //rental
            public bool IsRental { get; set; }
            public string RentalPrice { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }
        }

        public partial class GiftCardModel : BaseYStoryModel
        {
            public bool IsGiftCard { get; set; }

            [YStoryResourceDisplayName("Articles.GiftCard.RecipientName")]
            [AllowHtml]
            public string RecipientName { get; set; }
            [YStoryResourceDisplayName("Articles.GiftCard.RecipientEmail")]
            [AllowHtml]
            public string RecipientEmail { get; set; }
            [YStoryResourceDisplayName("Articles.GiftCard.SenderName")]
            [AllowHtml]
            public string SenderName { get; set; }
            [YStoryResourceDisplayName("Articles.GiftCard.SenderEmail")]
            [AllowHtml]
            public string SenderEmail { get; set; }
            [YStoryResourceDisplayName("Articles.GiftCard.Message")]
            [AllowHtml]
            public string Message { get; set; }

            
        }

       

        public partial class ArticleAttributeModel : BaseYStoryEntityModel
        {
            public ArticleAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<ArticleAttributeValueModel>();
            }

            public int ArticleId { get; set; }

            public int ArticleAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }
            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ArticleAttributeValueModel> Values { get; set; }

        }

        public partial class ArticleAttributeValueModel : BaseYStoryEntityModel
        {
            public ArticleAttributeValueModel()
            {
                ImageSquaresPictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            //picture model is used with "image square" attribute type
            public PictureModel ImageSquaresPictureModel { get; set; }

            public string PriceAdjustment { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //article picture ID (associated to this value)
            public int PictureId { get; set; }

            public bool CustomerEntersQty { get; set; }

            public int Quantity { get; set; }
        }

		#endregion
    }
}