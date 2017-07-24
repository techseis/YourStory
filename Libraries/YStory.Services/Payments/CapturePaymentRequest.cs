using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Payments
{
    /// <summary>
    /// Represents a CapturePaymentRequest
    /// </summary>
    public partial class CapturePaymentRequest
    {
        /// <summary>
        /// Gets or sets an subscription
        /// </summary>
        public Subscription Subscription { get; set; }
    }
}
