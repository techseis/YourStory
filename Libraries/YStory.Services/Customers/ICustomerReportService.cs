using System;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
 

namespace YStory.Services.Customers
{
    /// <summary>
    /// Customer report service interface
    /// </summary>
    public partial interface ICustomerReportService
    {
        /// <summary>
        /// Get best customers
        /// </summary>
        /// <param name="createdFromUtc">Subscription created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Subscription created date to (UTC); null to load all records</param>
        /// <param name="os">Subscription status; null to load all records</param>
        /// <param name="ps">Subscription payment status; null to load all records</param>
        /// <param name="ss">Subscription shipment status; null to load all records</param>
        /// <param name="subscriptionBy">1 - subscription by subscription total, 2 - subscription by number of subscriptions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Report</returns>
        IPagedList<BestCustomerReportLine> GetBestCustomersReport(DateTime? createdFromUtc,
            DateTime? createdToUtc, SubscriptionStatus? os, PaymentStatus? ps,   int subscriptionBy,
            int pageIndex = 0, int pageSize = 214748364);
        
        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Number of registered customers</returns>
        int GetRegisteredCustomersReport(int days);
    }
}