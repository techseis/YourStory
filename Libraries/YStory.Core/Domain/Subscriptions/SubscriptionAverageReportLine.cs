namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents an subscription average report line
    /// </summary>
    public partial class SubscriptionAverageReportLine
    {
        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int CountSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the shipping summary (excluding tax)
        /// </summary>
        public decimal SumShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax summary
        /// </summary>
        public decimal SumTax { get; set; }

        /// <summary>
        /// Gets or sets the subscription total summary
        /// </summary>
        public decimal SumSubscriptions { get; set; }
    }
}
