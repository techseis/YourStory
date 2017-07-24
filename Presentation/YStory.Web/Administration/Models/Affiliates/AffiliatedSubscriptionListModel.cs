using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Affiliates
{
    public partial class AffiliatedSubscriptionListModel : BaseYStoryModel
    {
        public AffiliatedSubscriptionListModel()
        {
            AvailableSubscriptionStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        public int AffliateId { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.SubscriptionStatus")]
        public int SubscriptionStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableSubscriptionStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
    }
}