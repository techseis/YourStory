using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class ShipmentListModel : BaseYStoryModel
    {
        public ShipmentListModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.TrackingNumber")]
        [AllowHtml]
        public string TrackingNumber { get; set; }
        
        public IList<SelectListItem> AvailableCountries { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }
        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.StateProvince")]
        public int StateProvinceId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.City")]
        [AllowHtml]
        public string City { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.LoadNotShipped")]
        public bool LoadNotShipped { get; set; }


        [YStoryResourceDisplayName("Admin.Subscriptions.Shipments.List.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
    }
}