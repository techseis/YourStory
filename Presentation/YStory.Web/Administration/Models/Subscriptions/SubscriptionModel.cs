using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Admin.Models.Common;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Tax;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class SubscriptionModel : BaseYStoryEntityModel
    {
        public SubscriptionModel()
        {
            CustomValues = new Dictionary<string, object>();
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<SubscriptionItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
            Warnings = new List<string>();
        }

        public bool IsLoggedInAsContributor { get; set; }

        //identifiers
        public override int Id { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionGuid")]
        public Guid SubscriptionGuid { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CustomSubscriptionNumber")]
        public string CustomSubscriptionNumber { get; set; }
        
        //store
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Store")]
        public string StoreName { get; set; }

        //customer info
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Customer")]
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Customer")]
        public string CustomerInfo { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CustomerIP")]
        public string CustomerIp { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CustomValues")]
        public Dictionary<string, object> CustomValues { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //Used discounts
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.UsedDiscounts")]
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }

        //totals
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        public TaxDisplayType TaxDisplayType { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionSubtotalInclTax")]
        public string SubscriptionSubtotalInclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionSubtotalExclTax")]
        public string SubscriptionSubtotalExclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionSubTotalDiscountInclTax")]
        public string SubscriptionSubTotalDiscountInclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionSubTotalDiscountExclTax")]
        public string SubscriptionSubTotalDiscountExclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionShippingInclTax")]
        public string SubscriptionShippingInclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionShippingExclTax")]
        public string SubscriptionShippingExclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PaymentMethodAdditionalFeeInclTax")]
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PaymentMethodAdditionalFeeExclTax")]
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionTotalDiscount")]
        public string SubscriptionTotalDiscount { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.RedeemedRewardPoints")]
        public int RedeemedRewardPoints { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.RedeemedRewardPoints")]
        public string RedeemedRewardPointsAmount { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionTotal")]
        public string SubscriptionTotal { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.RefundedAmount")]
        public string RefundedAmount { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Profit")]
        public string Profit { get; set; }

        //edit totals
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionSubtotal")]
        public decimal SubscriptionSubtotalInclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionSubtotal")]
        public decimal SubscriptionSubtotalExclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionSubTotalDiscount")]
        public decimal SubscriptionSubTotalDiscountInclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionSubTotalDiscount")]
        public decimal SubscriptionSubTotalDiscountExclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionShipping")]
        public decimal SubscriptionShippingInclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionShipping")]
        public decimal SubscriptionShippingExclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.Tax")]
        public decimal TaxValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.TaxRates")]
        public string TaxRatesValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionTotalDiscount")]
        public decimal SubscriptionTotalDiscountValue { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.Edit.SubscriptionTotal")]
        public decimal SubscriptionTotalValue { get; set; }

        //associated recurring payment id
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.RecurringPayment")]
        public int RecurringPaymentId { get; set; }

        //subscription status
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionStatus")]
        public string SubscriptionStatus { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionStatus")]
        public int SubscriptionStatusId { get; set; }

        //payment info
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        //credit card info
        public bool AllowStoringCreditCardNumber { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardType")]
        [AllowHtml]
        public string CardType { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardName")]
        [AllowHtml]
        public string CardName { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardNumber")]
        [AllowHtml]
        public string CardNumber { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardCVV2")]
        [AllowHtml]
        public string CardCvv2 { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardExpirationMonth")]
        [AllowHtml]
        public string CardExpirationMonth { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CardExpirationYear")]
        [AllowHtml]
        public string CardExpirationYear { get; set; }

        //misc payment info
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.AuthorizationTransactionID")]
        public string AuthorizationTransactionId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CaptureTransactionID")]
        public string CaptureTransactionId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.SubscriptionTransactionID")]
        public string SubscriptionTransactionId { get; set; }

        //shipping info
        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PickupAddress")]
        public AddressModel PickupAddress { get; set; }
        public string PickupAddressGoogleMapsUrl { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.ShippingStatus")]
        public int ShippingStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }
        public bool CanAddNewShipments { get; set; }

        //billing info
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.BillingAddress")]
        public AddressModel BillingAddress { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.VatNumber")]
        public string VatNumber { get; set; }
        
        //gift cards
        public IList<GiftCard> GiftCards { get; set; }

        //items
        public bool HasDownloadableArticles { get; set; }
        public IList<SubscriptionItemModel> Items { get; set; }

        //creation date
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //checkout attributes
        public string CheckoutAttributeInfo { get; set; }


        //subscription notes
        [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.DisplayToCustomer")]
        public bool AddSubscriptionNoteDisplayToCustomer { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.Note")]
        [AllowHtml]
        public string AddSubscriptionNoteMessage { get; set; }
        public bool AddSubscriptionNoteHasDownload { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.Download")]
        [UIHint("Download")]
        public int AddSubscriptionNoteDownloadId { get; set; }

        //refund info
        [YStoryResourceDisplayName("Admin.Subscriptions.Fields.PartialRefund.AmountToRefund")]
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        //workflow info
        public bool CanCancelSubscription { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkSubscriptionAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }

        //warnings
        public List<string> Warnings { get; set; }

        #region Nested Classes

        public partial class SubscriptionItemModel : BaseYStoryEntityModel
        {
            public SubscriptionItemModel()
            {
               
                ReturnRequests = new List<ReturnRequestBriefModel>();
            }
            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ContributorName { get; set; }
            public string Sku { get; set; }

            public string PictureThumbnailUrl { get; set; }

            public string UnitPriceInclTax { get; set; }
            public string UnitPriceExclTax { get; set; }
            public decimal UnitPriceInclTaxValue { get; set; }
            public decimal UnitPriceExclTaxValue { get; set; }

            public int Quantity { get; set; }

            public string DiscountInclTax { get; set; }
            public string DiscountExclTax { get; set; }
            public decimal DiscountInclTaxValue { get; set; }
            public decimal DiscountExclTaxValue { get; set; }

            public string SubTotalInclTax { get; set; }
            public string SubTotalExclTax { get; set; }
            public decimal SubTotalInclTaxValue { get; set; }
            public decimal SubTotalExclTaxValue { get; set; }

            public string AttributeInfo { get; set; }
            public string RecurringInfo { get; set; }
            public string RentalInfo { get; set; }
            public IList<ReturnRequestBriefModel> ReturnRequests { get; set; }


            #region Nested Classes

            public partial class ReturnRequestBriefModel : BaseYStoryEntityModel
            {
                public string CustomNumber { get; set; }
            }

            #endregion
        }

        public partial class TaxRate : BaseYStoryModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Subscriptions.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class SubscriptionNote : BaseYStoryEntityModel
        {
            public int SubscriptionId { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.DisplayToCustomer")]
            public bool DisplayToCustomer { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.Note")]
            public string Note { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.Download")]
            public int DownloadId { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.Download")]
            public Guid DownloadGuid { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.SubscriptionNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class UploadLicenseModel : BaseYStoryModel
        {
            public int SubscriptionId { get; set; }

            public int SubscriptionItemId { get; set; }

            [UIHint("Download")]
            public int LicenseDownloadId { get; set; }

        }

        public partial class AddSubscriptionArticleModel : BaseYStoryModel
        {
            public AddSubscriptionArticleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailablePublishers = new List<SelectListItem>();
                AvailableArticleTypes = new List<SelectListItem>();
            }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleName")]
            [AllowHtml]
            public string SearchArticleName { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchPublisher")]
            public int SearchPublisherId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleType")]
            public int SearchArticleTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailablePublishers { get; set; }
            public IList<SelectListItem> AvailableArticleTypes { get; set; }

            public int SubscriptionId { get; set; }

            #region Nested classes
            
            public partial class ArticleModel : BaseYStoryEntityModel
            {
                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.Name")]
                [AllowHtml]
                public string Name { get; set; }

                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.SKU")]
                [AllowHtml]
                public string Sku { get; set; }
            }

            public partial class ArticleDetailsModel : BaseYStoryModel
            {
                public ArticleDetailsModel()
                {
                    ArticleAttributes = new List<ArticleAttributeModel>();
                    GiftCard = new GiftCardModel();
                    Warnings = new List<string>();
                }

                public int ArticleId { get; set; }

                public int SubscriptionId { get; set; }

                public ArticleType ArticleType { get; set; }

                public string Name { get; set; }

                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.UnitPriceInclTax")]
                public decimal UnitPriceInclTax { get; set; }
                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.UnitPriceExclTax")]
                public decimal UnitPriceExclTax { get; set; }

                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.Quantity")]
                public int Quantity { get; set; }

                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.SubTotalInclTax")]
                public decimal SubTotalInclTax { get; set; }
                [YStoryResourceDisplayName("Admin.Subscriptions.Articles.AddNew.SubTotalExclTax")]
                public decimal SubTotalExclTax { get; set; }

                //article attributes
                public IList<ArticleAttributeModel> ArticleAttributes { get; set; }
                //gift card info
                public GiftCardModel GiftCard { get; set; }
                //rental
                public bool IsRental { get; set; }

                public List<string> Warnings { get; set; }

                /// <summary>
                /// A value indicating whether this attribute depends on some other attribute
                /// </summary>
                public bool HasCondition { get; set; }

                public bool AutoUpdateSubscriptionTotals { get; set; }
            }

            public partial class ArticleAttributeModel : BaseYStoryEntityModel
            {
                public ArticleAttributeModel()
                {
                    Values = new List<ArticleAttributeValueModel>();
                }

                public int ArticleAttributeId { get; set; }

                public string Name { get; set; }

                public string TextPrompt { get; set; }

                public bool IsRequired { get; set; }

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
                public string Name { get; set; }

                public bool IsPreSelected { get; set; }

                public string PriceAdjustment { get; set; }

                public decimal PriceAdjustmentValue { get; set; }

                public bool CustomerEntersQty { get; set; }

                public int Quantity { get; set; }
            }


            public partial class GiftCardModel : BaseYStoryModel
            {
                public bool IsGiftCard { get; set; }

                [YStoryResourceDisplayName("Admin.GiftCards.Fields.RecipientName")]
                [AllowHtml]
                public string RecipientName { get; set; }
                [YStoryResourceDisplayName("Admin.GiftCards.Fields.RecipientEmail")]
                [AllowHtml]
                public string RecipientEmail { get; set; }
                [YStoryResourceDisplayName("Admin.GiftCards.Fields.SenderName")]
                [AllowHtml]
                public string SenderName { get; set; }
                [YStoryResourceDisplayName("Admin.GiftCards.Fields.SenderEmail")]
                [AllowHtml]
                public string SenderEmail { get; set; }
                [YStoryResourceDisplayName("Admin.GiftCards.Fields.Message")]
                [AllowHtml]
                public string Message { get; set; }

                public GiftCardType GiftCardType { get; set; }
            }
            #endregion
        }

        public partial class UsedDiscountModel:BaseYStoryModel
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }


    public partial class SubscriptionAggreratorModel : BaseYStoryModel
    {
        //aggergator properties
        public string aggregatorprofit { get; set; }
        public string aggregatorshipping { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }
    }
}