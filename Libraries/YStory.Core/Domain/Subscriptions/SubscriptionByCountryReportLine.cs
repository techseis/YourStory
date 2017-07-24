namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents an "subscription by country" report line
    /// </summary>
    public partial class OrderByCountryReportLine
    {
        /// <summary>
        /// Country identifier; null for unknow country
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the number of subscriptions
        /// </summary>
        public int TotalSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the subscription total summary
        /// </summary>
        public decimal SumSubscriptions { get; set; }
    }
}
