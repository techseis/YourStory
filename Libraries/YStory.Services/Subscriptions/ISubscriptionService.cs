using System;
using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription service interface
    /// </summary>
    public partial interface ISubscriptionService
    {
        #region Subscriptions

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>Subscription</returns>
        Subscription GetOrderById(int subscriptionId);

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="customSubscriptionNumber">The custom subscription number</param>
        /// <returns>Subscription</returns>
        Subscription GetOrderByCustomSubscriptionNumber(string customSubscriptionNumber);

        /// <summary>
        /// Get subscriptions by identifiers
        /// </summary>
        /// <param name="subscriptionIds">Subscription identifiers</param>
        /// <returns>Subscription</returns>
        IList<Subscription> GetSubscriptionsByIds(int[] subscriptionIds);

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="subscriptionGuid">The subscription identifier</param>
        /// <returns>Subscription</returns>
        Subscription GetOrderByGuid(Guid subscriptionGuid);

        /// <summary>
        /// Deletes an subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        void DeleteSubscription(Subscription subscription);

        /// <summary>
        /// Search subscriptions
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all subscriptions</param>
        /// <param name="contributorId">Contributor identifier; null to load all subscriptions</param>
        /// <param name="customerId">Customer identifier; null to load all subscriptions</param>
        /// <param name="articleId">Article identifier which was purchased in an subscription; 0 to load all subscriptions</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all subscriptions</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all subscriptions</param>
        /// <param name="warehouseId">Warehouse identifier, only subscriptions with articles from a specified warehouse will be loaded; 0 to load all subscriptions</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Subscription status identifiers; null to load all subscriptions</param>
        /// <param name="psIds">Payment status identifiers; null to load all subscriptions</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all subscriptions</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="subscriptionNotes">Search in subscription notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Subscriptions</returns>
        IPagedList<Subscription> SearchSubscriptions(int storeId = 0,
            int contributorId = 0, int customerId = 0,
            int articleId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingEmail = null, string billingLastName = "", 
            string subscriptionNotes = null, int pageIndex = 0, int pageSize = int.MaxValue);
        
        /// <summary>
        /// Inserts an subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void InsertSubscription(Subscription subscription);

        /// <summary>
        /// Updates the subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        void UpdateSubscription(Subscription subscription);

        /// <summary>
        /// Get an subscription by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Subscription</returns>
        Subscription GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName);

        #endregion

        #region Subscriptions items

        /// <summary>
        /// Gets an subscription item
        /// </summary>
        /// <param name="subscriptionItemId">Subscription item identifier</param>
        /// <returns>Subscription item</returns>
        SubscriptionItem GetSubscriptionItemById(int subscriptionItemId);

        /// <summary>
        /// Gets an subscription item
        /// </summary>
        /// <param name="subscriptionItemGuid">Subscription item identifier</param>
        /// <returns>Subscription item</returns>
        SubscriptionItem GetSubscriptionItemByGuid(Guid subscriptionItemGuid);

        /// <summary>
        /// Gets all downloadable subscription items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>Subscription items</returns>
        IList<SubscriptionItem> GetDownloadableSubscriptionItems(int customerId);

        /// <summary>
        /// Delete an subscription item
        /// </summary>
        /// <param name="subscriptionItem">The subscription item</param>
        void DeleteSubscriptionItem(SubscriptionItem subscriptionItem);

        #endregion

        #region Subscription notes

        /// <summary>
        /// Gets an subscription note
        /// </summary>
        /// <param name="subscriptionNoteId">The subscription note identifier</param>
        /// <returns>Subscription note</returns>
        SubscriptionNote GetSubscriptionNoteById(int subscriptionNoteId);

        /// <summary>
        /// Deletes an subscription note
        /// </summary>
        /// <param name="subscriptionNote">The subscription note</param>
        void DeleteSubscriptionNote(SubscriptionNote subscriptionNote);

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void DeleteRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        RecurringPayment GetRecurringPaymentById(int recurringPaymentId);

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void InsertRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void UpdateRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialSubscriptionId">The initial subscription identifier; 0 to load all records</param>
        /// <param name="initialSubscriptionStatus">Initial subscription status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payments</returns>
        IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialSubscriptionId = 0, SubscriptionStatus? initialSubscriptionStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        #endregion
    }
}
