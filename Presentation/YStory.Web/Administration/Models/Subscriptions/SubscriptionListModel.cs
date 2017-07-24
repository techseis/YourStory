using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class SubscriptionListModel : BaseYStoryModel
    {
        public SubscriptionListModel()
        {
            SubscriptionStatusIds = new List<int>();
            PaymentStatusIds = new List<int>();
            ShippingStatusIds = new List<int>();
            AvailableSubscriptionStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableContributors = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailablePaymentMethods = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.SubscriptionStatus")]
        [UIHint("MultiSelect")]
        public List<int> SubscriptionStatusIds { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.PaymentStatus")]
        [UIHint("MultiSelect")]
        public List<int> PaymentStatusIds { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.ShippingStatus")]
        [UIHint("MultiSelect")]
        public List<int> ShippingStatusIds { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.PaymentMethod")]
        public string PaymentMethodSystemName { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.Store")]
        public int StoreId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.Contributor")]
        public int ContributorId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.Warehouse")]
        public int WarehouseId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.Article")]
        public int ArticleId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.BillingEmail")]
        [AllowHtml]
        public string BillingEmail { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.BillingLastName")]
        [AllowHtml]
        public string BillingLastName { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.BillingCountry")]
        public int BillingCountryId { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.SubscriptionNotes")]
        [AllowHtml]
        public string SubscriptionNotes { get; set; }

        [YStoryResourceDisplayName("Admin.Subscriptions.List.GoDirectlyToNumber")]
        public string GoDirectlyToCustomSubscriptionNumber { get; set; }

        public bool IsLoggedInAsContributor { get; set; }


        public IList<SelectListItem> AvailableSubscriptionStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailablePaymentMethods { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
    }
}