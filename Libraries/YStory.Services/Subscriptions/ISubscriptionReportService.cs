using System;
using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
 

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription report service interface
    /// </summary>
    public partial interface ISubscriptionReportService
    {
        /// <summary>
        /// Get "subscription by country" report
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="os">Subscription status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        IList<OrderByCountryReportLine> GetCountryReport(int storeId = 0, SubscriptionStatus? os = null,
            PaymentStatus? ps = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null);

        /// <summary>
        /// Get subscription average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="contributorId">Contributor identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all subscriptions</param>
        /// <param name="subscriptionId">Subscription identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="osIds">Subscription status identifiers</param>
        /// <param name="psIds">Payment status identifiers</param>
        /// <param name="ssIds">Shipping status identifiers</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="subscriptionNotes">Search in subscription notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        SubscriptionAverageReportLine GetSubscriptionAverageReportLine(int storeId = 0, int contributorId = 0,
            int billingCountryId = 0, int subscriptionId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string subscriptionNotes = null);
        
        /// <summary>
        /// Get subscription average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Subscription status</param>
        /// <returns>Result</returns>
        SubscriptionAverageReportLineSummary SubscriptionAverageReport(int storeId, SubscriptionStatus os);

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier (subscriptions placed in a specific store); 0 to load all records</param>
        /// <param name="contributorId">Contributor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="publisherId">Publisher identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Subscription created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Subscription created date to (UTC); null to load all records</param>
        /// <param name="os">Subscription status; null to load all records</param>
        /// <param name="ps">Subscription payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="subscriptionBy">1 - subscription by quantity, 2 - subscription by total amount</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Result</returns>
        IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int publisherId = 0, 
            int storeId = 0, int contributorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionStatus? os = null, PaymentStatus? ps = null,
            int billingCountryId = 0,
            int subscriptionBy = 1,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = false);

        /// <summary>
        /// Gets a list of articles (identifiers) purchased by other customers who purchased a specified article
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="articleId">Article identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only articles marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Articles</returns>
        int[] GetAlsoPurchasedArticlesIds(int storeId, int articleId,
            int recordsToReturn = 5, bool visibleIndividuallyOnly = true, bool showHidden = false);

        /// <summary>
        /// Gets a list of articles that were never sold
        /// </summary>
        /// <param name="contributorId">Contributor identifier (filter articles by a specific contributor); 0 to load all records</param>
        /// <param name="storeId">Store identifier (filter articles by a specific store); 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="publisherId">Publisher identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Subscription created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Subscription created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Articles</returns>
        IPagedList<Article> ArticlesNeverSold(int contributorId = 0, int storeId = 0,
            int categoryId = 0, int publisherId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="contributorId">Contributor identifier; pass 0 to ignore this parameter</param>
        /// <param name="subscriptionId">Subscription identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all subscriptions</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="osIds">Subscription status identifiers; null to load all records</param>
        /// <param name="psIds">Payment status identifiers; null to load all records</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all records</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="subscriptionNotes">Search in subscription notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        decimal ProfitReport(int storeId = 0, int contributorId = 0,
            int billingCountryId = 0, int subscriptionId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string subscriptionNotes = null);
    }
}
