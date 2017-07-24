using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Payments
{
    /// <summary>
    /// Represents a VoidPaymentResult
    /// </summary>
    public partial class VoidPaymentRequest
    {
        /// <summary>
        /// Gets or sets an subscription
        /// </summary>
        public Subscription Subscription { get; set; }
    }
}
