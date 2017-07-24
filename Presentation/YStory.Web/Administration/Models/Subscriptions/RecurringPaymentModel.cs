using System;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class RecurringPaymentModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.ID")]
        public override int Id { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.CycleLength")]
        public int CycleLength { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public int CyclePeriodId { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public string CyclePeriodStr { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.TotalCycles")]
        public int TotalCycles { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.StartDate")]
        public string StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.IsActive")]
        public bool IsActive { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.NextPaymentDate")]
        public string NextPaymentDate { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.CyclesRemaining")]
        public int CyclesRemaining { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.InitialSubscription")]
        public int InitialSubscriptionId { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.Customer")]
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.Customer")]
        public string CustomerEmail { get; set; }

        [YStoryResourceDisplayName("Admin.RecurringPayments.Fields.PaymentType")]
        public string PaymentType { get; set; }
        
        public bool CanCancelRecurringPayment { get; set; }

        public bool LastPaymentFailed { get; set; }

        #region Nested classes


        public partial class RecurringPaymentHistoryModel : BaseYStoryEntityModel
        {
            public int SubscriptionId { get; set; }

            [YStoryResourceDisplayName("Admin.RecurringPayments.History.CustomSubscriptionNumber")]
            public string CustomSubscriptionNumber { get; set; }

            public int RecurringPaymentId { get; set; }

            [YStoryResourceDisplayName("Admin.RecurringPayments.History.SubscriptionStatus")]
            public string SubscriptionStatus { get; set; }

            [YStoryResourceDisplayName("Admin.RecurringPayments.History.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [YStoryResourceDisplayName("Admin.RecurringPayments.History.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [YStoryResourceDisplayName("Admin.RecurringPayments.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}