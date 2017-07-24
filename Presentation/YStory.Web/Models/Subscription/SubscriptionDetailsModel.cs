using System;
using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Subscription
{
    public partial class SubscriptionDetailsModel : BaseYStoryEntityModel
    {
        public SubscriptionDetailsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<SubscriptionItemModel>();
            SubscriptionNotes = new List<SubscriptionNote>();
            Shipments = new List<ShipmentBriefModel>();

            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            PickupAddress = new AddressModel();

            CustomValues = new Dictionary<string, object>();
        }

        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }

        public string CustomSubscriptionNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public string SubscriptionStatus { get; set; }

        public bool IsReSubscriptionAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }
        
        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        public AddressModel PickupAddress { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public IList<ShipmentBriefModel> Shipments { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentMethodStatus { get; set; }
        public bool CanRePostProcessPayment { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }

        public string SubscriptionSubtotal { get; set; }
        public string SubscriptionSubTotalDiscount { get; set; }
        public string SubscriptionShipping { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public string CheckoutAttributeInfo { get; set; }

        public bool PricesIncludeTax { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public string SubscriptionTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string SubscriptionTotal { get; set; }
        
        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public IList<SubscriptionItemModel> Items { get; set; }
        
        public IList<SubscriptionNote> SubscriptionNotes { get; set; }

		#region Nested Classes

        public partial class SubscriptionItemModel : BaseYStoryEntityModel
        {
            public Guid SubscriptionItemGuid { get; set; }
            public string Sku { get; set; }
            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public string UnitPrice { get; set; }
            public string SubTotal { get; set; }
            public int Quantity { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }

            //downloadable article properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
        }

        public partial class TaxRate : BaseYStoryModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseYStoryModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class SubscriptionNote : BaseYStoryEntityModel
        {
            public bool HasDownload { get; set; }
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial class ShipmentBriefModel : BaseYStoryEntityModel
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }
		#endregion
    }
}