using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Events;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription service
    /// </summary>
    public partial class SubscriptionService : ISubscriptionService
    {
        #region Fields

        private readonly IRepository<Subscription> _subscriptionRepository;
        private readonly IRepository<SubscriptionItem> _subscriptionItemRepository;
        private readonly IRepository<SubscriptionNote> _subscriptionNoteRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionRepository">Subscription repository</param>
        /// <param name="subscriptionItemRepository">Subscription item repository</param>
        /// <param name="subscriptionNoteRepository">Subscription note repository</param>
        /// <param name="articleRepository">Article repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SubscriptionService(IRepository<Subscription> subscriptionRepository,
            IRepository<SubscriptionItem> subscriptionItemRepository,
            IRepository<SubscriptionNote> subscriptionNoteRepository,
            IRepository<Article> articleRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<Customer> customerRepository, 
            IEventPublisher eventPublisher)
        {
            this._subscriptionRepository = subscriptionRepository;
            this._subscriptionItemRepository = subscriptionItemRepository;
            this._subscriptionNoteRepository = subscriptionNoteRepository;
            this._articleRepository = articleRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Subscriptions

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>Subscription</returns>
        public virtual Subscription GetOrderById(int subscriptionId)
        {
            if (subscriptionId == 0)
                return null;

            return _subscriptionRepository.GetById(subscriptionId);
        }

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="customSubscriptionNumber">The custom subscription number</param>
        /// <returns>Subscription</returns>
        public virtual Subscription GetOrderByCustomSubscriptionNumber(string customSubscriptionNumber)
        {
            if (string.IsNullOrEmpty(customSubscriptionNumber))
                return null;
           
            return _subscriptionRepository.Table.FirstOrDefault(o => o.CustomSubscriptionNumber == customSubscriptionNumber);
        }

        /// <summary>
        /// Get subscriptions by identifiers
        /// </summary>
        /// <param name="subscriptionIds">Subscription identifiers</param>
        /// <returns>Subscription</returns>
        public virtual IList<Subscription> GetSubscriptionsByIds(int[] subscriptionIds)
        {
            if (subscriptionIds == null || subscriptionIds.Length == 0)
                return new List<Subscription>();

            var query = from o in _subscriptionRepository.Table
                        where subscriptionIds.Contains(o.Id) && !o.Deleted
                        select o;
            var subscriptions = query.ToList();
            //sort by passed identifiers
            var sortedSubscriptions = new List<Subscription>();
            foreach (int id in subscriptionIds)
            {
                var subscription = subscriptions.Find(x => x.Id == id);
                if (subscription != null)
                    sortedSubscriptions.Add(subscription);
            }
            return sortedSubscriptions;
        }

        /// <summary>
        /// Gets an subscription
        /// </summary>
        /// <param name="subscriptionGuid">The subscription identifier</param>
        /// <returns>Subscription</returns>
        public virtual Subscription GetOrderByGuid(Guid subscriptionGuid)
        {
            if (subscriptionGuid == Guid.Empty)
                return null;

            var query = from o in _subscriptionRepository.Table
                        where o.SubscriptionGuid == subscriptionGuid
                        select o;
            var subscription = query.FirstOrDefault();
            return subscription;
        }

        /// <summary>
        /// Deletes an subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        public virtual void DeleteSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            subscription.Deleted = true;
            UpdateSubscription(subscription);

            //event notification
            _eventPublisher.EntityDeleted(subscription);
        }

        /// <summary>
        /// Search subscriptions
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all subscriptions</param>
        /// <param name="contributorId">Contributor identifier; null to load all subscriptions</param>
        /// <param name="customerId">Customer identifier; 0 to load all subscriptions</param>
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
        public virtual IPagedList<Subscription> SearchSubscriptions(int storeId = 0,
            int contributorId = 0, int customerId = 0,
            int articleId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingEmail = null, string billingLastName = "",
            string subscriptionNotes = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _subscriptionRepository.Table;
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (contributorId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionItems
                    .Any(subscriptionItem => subscriptionItem.Article.ContributorId == contributorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (articleId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionItems
                    .Any(subscriptionItem => subscriptionItem.Article.Id == articleId));
            }
            
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.SubscriptionStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(subscriptionNotes))
                query = query.Where(o => o.SubscriptionNotes.Any(on => on.Note.Contains(subscriptionNotes)));
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return new PagedList<Subscription>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts an subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void InsertSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _subscriptionRepository.Insert(subscription);

            //event notification
            _eventPublisher.EntityInserted(subscription);
        }

        /// <summary>
        /// Updates the subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        public virtual void UpdateSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _subscriptionRepository.Update(subscription);

            //event notification
            _eventPublisher.EntityUpdated(subscription);
        }

        /// <summary>
        /// Get an subscription by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Subscription</returns>
        public virtual Subscription GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, 
            string paymentMethodSystemName)
        { 
            var query = _subscriptionRepository.Table;
            if (!String.IsNullOrWhiteSpace(authorizationTransactionId))
                query = query.Where(o => o.AuthorizationTransactionId == authorizationTransactionId);
            
            if (!String.IsNullOrWhiteSpace(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            var subscription = query.FirstOrDefault();
            return subscription;
        }
        
        #endregion

        #region Subscriptions items

        /// <summary>
        /// Gets an subscription item
        /// </summary>
        /// <param name="subscriptionItemId">Subscription item identifier</param>
        /// <returns>Subscription item</returns>
        public virtual SubscriptionItem GetSubscriptionItemById(int subscriptionItemId)
        {
            if (subscriptionItemId == 0)
                return null;

            return _subscriptionItemRepository.GetById(subscriptionItemId);
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="subscriptionItemGuid">Subscription identifier</param>
        /// <returns>Subscription item</returns>
        public virtual SubscriptionItem GetSubscriptionItemByGuid(Guid subscriptionItemGuid)
        {
            if (subscriptionItemGuid == Guid.Empty)
                return null;

            var query = from subscriptionItem in _subscriptionItemRepository.Table
                        where subscriptionItem.SubscriptionItemGuid == subscriptionItemGuid
                        select subscriptionItem;
            var item = query.FirstOrDefault();
            return item;
        }
        
        /// <summary>
        /// Gets all downloadable subscription items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>Subscription items</returns>
        public virtual IList<SubscriptionItem> GetDownloadableSubscriptionItems(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException("customerId");

            var query = from subscriptionItem in _subscriptionItemRepository.Table
                        join o in _subscriptionRepository.Table on subscriptionItem.SubscriptionId equals o.Id
                        join p in _articleRepository.Table on subscriptionItem.ArticleId equals p.Id
                        where customerId == o.CustomerId &&
                       
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, subscriptionItem.Id
                        select subscriptionItem;

            var subscriptionItems = query.ToList();
            return subscriptionItems;
        }

        /// <summary>
        /// Delete an subscription item
        /// </summary>
        /// <param name="subscriptionItem">The subscription item</param>
        public virtual void DeleteSubscriptionItem(SubscriptionItem subscriptionItem)
        {
            if (subscriptionItem == null)
                throw new ArgumentNullException("subscriptionItem");

            _subscriptionItemRepository.Delete(subscriptionItem);

            //event notification
            _eventPublisher.EntityDeleted(subscriptionItem);
        }

        #endregion

        #region Subscriptions notes

        /// <summary>
        /// Gets an subscription note
        /// </summary>
        /// <param name="subscriptionNoteId">The subscription note identifier</param>
        /// <returns>Subscription note</returns>
        public virtual SubscriptionNote GetSubscriptionNoteById(int subscriptionNoteId)
        {
            if (subscriptionNoteId == 0)
                return null;

            return _subscriptionNoteRepository.GetById(subscriptionNoteId);
        }

        /// <summary>
        /// Deletes an subscription note
        /// </summary>
        /// <param name="subscriptionNote">The subscription note</param>
        public virtual void DeleteSubscriptionNote(SubscriptionNote subscriptionNote)
        {
            if (subscriptionNote == null)
                throw new ArgumentNullException("subscriptionNote");

            _subscriptionNoteRepository.Delete(subscriptionNote);

            //event notification
            _eventPublisher.EntityDeleted(subscriptionNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);

            //event notification
            _eventPublisher.EntityDeleted(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public virtual RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

           return _recurringPaymentRepository.GetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Insert(recurringPayment);

            //event notification
            _eventPublisher.EntityInserted(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Update(recurringPayment);

            //event notification
            _eventPublisher.EntityUpdated(recurringPayment);
        }

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
        public virtual IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialSubscriptionId = 0, SubscriptionStatus? initialSubscriptionStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialSubscriptionStatusId = null;
            if (initialSubscriptionStatus.HasValue)
                initialSubscriptionStatusId = (int)initialSubscriptionStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join c in _customerRepository.Table on rp.InitialSubscription.CustomerId equals c.Id
                         where
                         (!rp.Deleted) &&
                         (showHidden || !rp.InitialSubscription.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || rp.InitialSubscription.CustomerId == customerId) &&
                         (storeId == 0 || rp.InitialSubscription.StoreId == storeId) &&
                         (initialSubscriptionId == 0 || rp.InitialSubscription.Id == initialSubscriptionId) &&
                         (!initialSubscriptionStatusId.HasValue || initialSubscriptionStatusId.Value == 0 || rp.InitialSubscription.SubscriptionStatusId == initialSubscriptionStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = new PagedList<RecurringPayment>(query2, pageIndex, pageSize);
            return recurringPayments;
        }

        #endregion

        #endregion
    }
}
