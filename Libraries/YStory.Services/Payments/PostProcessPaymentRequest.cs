using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an subscription. Used when subscription is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public Subscription Subscription { get; set; }
    }
}
