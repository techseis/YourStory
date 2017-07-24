using System;
using System.Collections.Generic;
using YStory.Core.Domain.Subscriptions;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Subscription
{
    public partial class CustomerSubscriptionListModel : BaseYStoryModel
    {
        public CustomerSubscriptionListModel()
        {
            Subscriptions = new List<SubscriptionDetailsModel>();
            RecurringSubscriptions = new List<RecurringSubscriptionModel>();
            RecurringPaymentErrors = new List<string>();
        }

        public IList<SubscriptionDetailsModel> Subscriptions { get; set; }
        public IList<RecurringSubscriptionModel> RecurringSubscriptions { get; set; }
        public IList<string> RecurringPaymentErrors { get; set; }


        #region Nested classes

        public partial class SubscriptionDetailsModel : BaseYStoryEntityModel
        {
            public string CustomSubscriptionNumber { get; set; }
            public string SubscriptionTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public SubscriptionStatus SubscriptionStatusEnum { get; set; }
            public string SubscriptionStatus { get; set; }
            public string PaymentStatus { get; set; }
            public string ShippingStatus { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial class RecurringSubscriptionModel : BaseYStoryEntityModel
        {
            public string StartDate { get; set; }
            public string CycleInfo { get; set; }
            public string NextPayment { get; set; }
            public int TotalCycles { get; set; }
            public int CyclesRemaining { get; set; }
            public int InitialSubscriptionId { get; set; }
            public bool CanRetryLastPayment { get; set; }
            public string InitialSubscriptionNumber { get; set; }
            public bool CanCancel { get; set; }
        }

        #endregion
    }
}