using System;
using System.Collections.Generic;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class ShipmentModel : BaseYStoryEntityModel
    {
        public ShipmentModel()
        {
            this.ShipmentStatusEvents = new List<ShipmentStatusEventModel>();
            this.Items = new List<ShipmentItemModel>();
        }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.ID")]
        public override int Id { get; set; }
        public int SubscriptionId { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.CustomSubscriptionNumber")]
        public string CustomSubscriptionNumber { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }
        public string TrackingNumberUrl { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }
        public bool CanShip { get; set; }
        public DateTime? ShippedDateUtc { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.AdminComment")]
        public string AdminComment { get; set; }

        public List<ShipmentItemModel> Items { get; set; }

        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }

        #region Nested classes

        public partial class ShipmentItemModel : BaseYStoryEntityModel
        {
            public ShipmentItemModel()
            {
                AvailableWarehouses = new List<WarehouseInfo>();
            }

            public int SubscriptionItemId { get; set; }
            public int ArticleId { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.Articles.ArticleName")]
            public string ArticleName { get; set; }
            public string Sku { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public bool ShipSeparately { get; set; }

            //weight of one item (article)
            [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.Articles.ItemWeight")]
            public string ItemWeight { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.Articles.ItemDimensions")]
            public string ItemDimensions { get; set; }

            public int QuantityToAdd { get; set; }
            public int QuantitySubscriptioned { get; set; }
            [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.Articles.QtyShipped")]
            public int QuantityInThisShipment { get; set; }
            public int QuantityInAllShipments { get; set; }

            public string ShippedFromWarehouse { get; set; }
            public bool AllowToChooseWarehouse { get; set; }
            //used before a shipment is created
            public List<WarehouseInfo> AvailableWarehouses { get; set; }

            #region Nested Classes
            public class WarehouseInfo : BaseYStoryModel
            {
                public int WarehouseId { get; set; }
                public string WarehouseName { get; set; }
                public int StockQuantity { get; set; }
                public int ReservedQuantity { get; set; }
                public int PlannedQuantity { get; set; }
            }
            #endregion
        }

        public partial class ShipmentStatusEventModel : BaseYStoryModel
        {
            public string EventName { get; set; }
            public string Location { get; set; }
            public string Country { get; set; }
            public DateTime? Date { get; set; }
        }

        #endregion
    }
}