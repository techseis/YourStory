using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Payments
{
    /// <summary>
    /// Represents a CancelRecurringPaymentResult
    /// </summary>
    public partial class CancelRecurringPaymentRequest
    {
        /// <summary>
        /// Gets or sets an subscription
        /// </summary>
        public Subscription Subscription { get; set; }
    }
}
