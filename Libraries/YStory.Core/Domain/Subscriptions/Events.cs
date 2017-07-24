namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Subscription paid event
    /// </summary>
    public class SubscriptionPaidEvent
    {
        public SubscriptionPaidEvent(Subscription subscription)
        {
            this.Subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public Subscription Subscription { get; private set; }
    }

    /// <summary>
    /// Subscription placed event
    /// </summary>
    public class SubscriptionPlacedEvent
    {
        public SubscriptionPlacedEvent(Subscription subscription)
        {
            this.Subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public Subscription Subscription { get; private set; }
    }

    /// <summary>
    /// Subscription cancelled event
    /// </summary>
    public class SubscriptionCancelledEvent
    {
        public SubscriptionCancelledEvent(Subscription subscription)
        {
            this.Subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public Subscription Subscription { get; private set; }
    }

    /// <summary>
    /// Subscription refunded event
    /// </summary>
    public class SubscriptionRefundedEvent
    {
        public SubscriptionRefundedEvent(Subscription subscription, decimal amount)
        {
            this.Subscription = subscription;
            this.Amount = amount;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public Subscription Subscription { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }
    }

}