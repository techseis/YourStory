namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents an subscription average report line summary
    /// </summary>
    public partial class SubscriptionAverageReportLineSummary
    {
        /// <summary>
        /// Gets or sets the subscription status
        /// </summary>
        public SubscriptionStatus SubscriptionStatus { get; set; }

        /// <summary>
        /// Gets or sets the sum today total
        /// </summary>
        public decimal SumTodaySubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the today count
        /// </summary>
        public int CountTodaySubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the sum this week total
        /// </summary>
        public decimal SumThisWeekSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the this week count
        /// </summary>
        public int CountThisWeekSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the sum this month total
        /// </summary>
        public decimal SumThisMonthSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the this month count
        /// </summary>
        public int CountThisMonthSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the sum this year total
        /// </summary>
        public decimal SumThisYearSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the this year count
        /// </summary>
        public int CountThisYearSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the sum all time total
        /// </summary>
        public decimal SumAllTimeSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the all time count
        /// </summary>
        public int CountAllTimeSubscriptions { get; set; }
    }
}
