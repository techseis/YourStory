
namespace YStory.Core.Domain.Customers
{

    /// <summary>
    /// Represents a best customer report line
    /// </summary>
    public partial class BestCustomerReportLine
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the subscription total
        /// </summary>
        public decimal SubscriptionTotal { get; set; }

        /// <summary>
        /// Gets or sets the subscription count
        /// </summary>
        public int SubscriptionCount { get; set; }
    }
}
