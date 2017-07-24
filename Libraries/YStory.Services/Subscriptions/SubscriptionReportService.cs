using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
 
using YStory.Core.Domain.Stores;
using YStory.Services.Helpers;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription report service
    /// </summary>
    public partial class SubscriptionReportService : ISubscriptionReportService
    {
        #region Fields

        private readonly IRepository<Subscription> _subscriptionRepository;
        private readonly IRepository<SubscriptionItem> _subscriptionItemRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionRepository">Subscription repository</param>
        /// <param name="subscriptionItemRepository">Subscription item repository</param>
        /// <param name="articleRepository">Article repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public SubscriptionReportService(IRepository<Subscription> subscriptionRepository,
            IRepository<SubscriptionItem> subscriptionItemRepository,
            IRepository<Article> articleRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDateTimeHelper dateTimeHelper,
            CatalogSettings catalogSettings)
        {
            this._subscriptionRepository = subscriptionRepository;
            this._subscriptionItemRepository = subscriptionItemRepository;
            this._articleRepository = articleRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dateTimeHelper = dateTimeHelper;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get "subscription by country" report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Subscription status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        public virtual IList<OrderByCountryReportLine> GetCountryReport(int storeId, SubscriptionStatus? os,
            PaymentStatus? ps,   DateTime? startTimeUtc, DateTime? endTimeUtc)
        {
            int? subscriptionStatusId = null;
            if (os.HasValue)
                subscriptionStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;
 

            var query = _subscriptionRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (subscriptionStatusId.HasValue)
                query = query.Where(o => o.SubscriptionStatusId == subscriptionStatusId.Value);
            if (paymentStatusId.HasValue)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId.Value);
 
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            
            var report = (from oq in query
                        group oq by oq.BillingAddress.CountryId into result
                        select new
                        {
                            CountryId = result.Key,
                            TotalSubscriptions = result.Count(),
                            SumSubscriptions = result.Sum(o => o.SubscriptionTotal)
                        }
                       )
                       .OrderByDescending(x => x.SumSubscriptions)
                       .Select(r => new OrderByCountryReportLine
                       {
                           CountryId = r.CountryId,
                           TotalSubscriptions = r.TotalSubscriptions,
                           SumSubscriptions = r.SumSubscriptions
                       })

                       .ToList();

            return report;
        }

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
        public virtual SubscriptionAverageReportLine GetSubscriptionAverageReportLine(int storeId = 0,
            int contributorId = 0, int billingCountryId = 0, 
            int subscriptionId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string subscriptionNotes = null)
        {
            var query = _subscriptionRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (subscriptionId > 0)
                query = query.Where(o => o.Id == subscriptionId);
            if (contributorId > 0)
            {
                query = query
                    .Where(o => o.SubscriptionItems
                    .Any(subscriptionItem => subscriptionItem.Article.ContributorId == contributorId));
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.SubscriptionStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(subscriptionNotes))
                query = query.Where(o => o.SubscriptionNotes.Any(on => on.Note.Contains(subscriptionNotes)));
            
			var item = (from oq in query
						group oq by 1 into result
						select new
						           {
                                       SubscriptionCount = result.Count(),
                                       SubscriptionShippingExclTaxSum = result.Sum(o => o.SubscriptionShippingExclTax),
                                       SubscriptionTaxSum = result.Sum(o => o.SubscriptionTax), 
                                       SubscriptionTotalSum = result.Sum(o => o.SubscriptionTotal)
						           }
					   ).Select(r => new SubscriptionAverageReportLine
                       {
                           CountSubscriptions = r.SubscriptionCount,
                           SumShippingExclTax = r.SubscriptionShippingExclTaxSum, 
                           SumTax = r.SubscriptionTaxSum, 
                           SumSubscriptions = r.SubscriptionTotalSum
                       })
                       .FirstOrDefault();

			item = item ?? new SubscriptionAverageReportLine
			                   {
                                   CountSubscriptions = 0,
                                   SumShippingExclTax = decimal.Zero,
                                   SumTax = decimal.Zero,
                                   SumSubscriptions = decimal.Zero, 
			                   };
            return item;
        }

        /// <summary>
        /// Get subscription average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Subscription status</param>
        /// <returns>Result</returns>
        public virtual SubscriptionAverageReportLineSummary SubscriptionAverageReport(int storeId, SubscriptionStatus os)
        {
            var item = new SubscriptionAverageReportLineSummary();
            item.SubscriptionStatus = os;
            var subscriptionStatuses = new List<int>() { (int)os };

            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            var t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                var todayResult = GetSubscriptionAverageReportLine(storeId: storeId,
                    osIds: subscriptionStatuses, 
                    startTimeUtc: startTime1);
                item.SumTodaySubscriptions = todayResult.SumSubscriptions;
                item.CountTodaySubscriptions = todayResult.CountSubscriptions;
            }
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                var weekResult = GetSubscriptionAverageReportLine(storeId: storeId,
                    osIds: subscriptionStatuses,
                    startTimeUtc: startTime2);
                item.SumThisWeekSubscriptions = weekResult.SumSubscriptions;
                item.CountThisWeekSubscriptions = weekResult.CountSubscriptions;
            }
            //month
            var t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                var monthResult = GetSubscriptionAverageReportLine(storeId: storeId,
                    osIds: subscriptionStatuses,
                    startTimeUtc: startTime3);
                item.SumThisMonthSubscriptions = monthResult.SumSubscriptions;
                item.CountThisMonthSubscriptions = monthResult.CountSubscriptions;
            }
            //year
            var t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                var yearResult = GetSubscriptionAverageReportLine(storeId: storeId,
                    osIds: subscriptionStatuses,
                    startTimeUtc: startTime4);
                item.SumThisYearSubscriptions = yearResult.SumSubscriptions;
                item.CountThisYearSubscriptions = yearResult.CountSubscriptions;
            }
            //all time
            var allTimeResult = GetSubscriptionAverageReportLine(storeId: storeId, osIds: subscriptionStatuses);
            item.SumAllTimeSubscriptions = allTimeResult.SumSubscriptions;
            item.CountAllTimeSubscriptions = allTimeResult.CountSubscriptions;

            return item;
        }

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
        public virtual IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int publisherId = 0,
            int storeId = 0, int contributorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            SubscriptionStatus? os = null, PaymentStatus? ps = null,  
            int billingCountryId = 0,
            int subscriptionBy = 1,
            int pageIndex = 0, int pageSize = int.MaxValue, 
            bool showHidden = false)
        {
            int? subscriptionStatusId = null;
            if (os.HasValue)
                subscriptionStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            

            var query1 = from subscriptionItem in _subscriptionItemRepository.Table
                         join o in _subscriptionRepository.Table on subscriptionItem.SubscriptionId equals o.Id
                         join p in _articleRepository.Table on subscriptionItem.ArticleId equals p.Id
                         //join pc in _articleCategoryRepository.Table on p.Id equals pc.ArticleId into p_pc from pc in p_pc.DefaultIfEmpty()
                         //join pm in _articlePublisherRepository.Table on p.Id equals pm.ArticleId into p_pm from pm in p_pm.DefaultIfEmpty()
                         where (storeId == 0 || storeId == o.StoreId) &&
                         (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!subscriptionStatusId.HasValue || subscriptionStatusId == o.SubscriptionStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                        
                         (!o.Deleted) &&
                         (!p.Deleted) &&
                         (contributorId == 0 || p.ContributorId == contributorId) &&
                         //(categoryId == 0 || pc.CategoryId == categoryId) &&
                         //(publisherId == 0 || pm.PublisherId == publisherId) &&
                         (categoryId == 0 || p.ArticleCategories.Count(pc => pc.CategoryId == categoryId) > 0) &&
                         (publisherId == 0 || p.ArticlePublishers.Count(pm => pm.PublisherId == publisherId) > 0) &&
                         (billingCountryId == 0 || o.BillingAddress.CountryId == billingCountryId) &&
                         (showHidden || p.Published)
                         select subscriptionItem;

            IQueryable<BestsellersReportLine> query2 = 
                //group by articles
                from subscriptionItem in query1
                group subscriptionItem by subscriptionItem.ArticleId into g
                select new BestsellersReportLine
                {
                    ArticleId = g.Key,
                    TotalAmount = g.Sum(x => x.PriceExclTax),
                    TotalQuantity = g.Sum(x => x.Quantity),
                }
                ;

            switch (subscriptionBy)
            {
                case 1:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalQuantity);
                    }
                    break;
                case 2:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalAmount);
                    }
                    break;
                default:
                    throw new ArgumentException("Wrong subscriptionBy parameter", "subscriptionBy");
            }

            var result = new PagedList<BestsellersReportLine>(query2, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets a list of articles (identifiers) purchased by other customers who purchased a specified article
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="articleId">Article identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only articles marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Articles</returns>
        public virtual int[] GetAlsoPurchasedArticlesIds(int storeId, int articleId,
            int recordsToReturn = 5, bool visibleIndividuallyOnly = true, bool showHidden = false)
        {
            if (articleId == 0)
                throw new ArgumentException("Article ID is not specified");

            //this inner query should retrieve all subscriptions that contains a specified article ID
            var query1 = from subscriptionItem in _subscriptionItemRepository.Table
                          where subscriptionItem.ArticleId == articleId
                          select subscriptionItem.SubscriptionId;

            var query2 = from subscriptionItem in _subscriptionItemRepository.Table
                         join p in _articleRepository.Table on subscriptionItem.ArticleId equals p.Id
                         where (query1.Contains(subscriptionItem.SubscriptionId)) &&
                         (p.Id != articleId) &&
                         (showHidden || p.Published) &&
                         (!subscriptionItem.Subscription.Deleted) &&
                         (storeId == 0 || subscriptionItem.Subscription.StoreId == storeId) &&
                         (!p.Deleted) &&
                         (!visibleIndividuallyOnly || p.VisibleIndividually)
                         select new { subscriptionItem, p };

            var query3 = from subscriptionItem_p in query2
                         group subscriptionItem_p by subscriptionItem_p.p.Id into g
                         select new
                         {
                             ArticleId = g.Key,
                             ArticlesPurchased = g.Sum(x => x.subscriptionItem.Quantity),
                         };
            query3 = query3.OrderByDescending(x => x.ArticlesPurchased);

            if (recordsToReturn > 0)
                query3 = query3.Take(recordsToReturn);

            var report = query3.ToList();
            
            var ids = new List<int>();
            foreach (var reportLine in report)
                ids.Add(reportLine.ArticleId);

            return ids.ToArray();
        }

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
        public virtual IPagedList<Article> ArticlesNeverSold(int contributorId = 0, int storeId = 0,
            int categoryId = 0, int publisherId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            //this inner query should retrieve all purchased article identifiers
            var query_tmp = (from subscriptionItem in _subscriptionItemRepository.Table
                join o in _subscriptionRepository.Table on subscriptionItem.SubscriptionId equals o.Id
                where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                      (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                      (!o.Deleted)
                select subscriptionItem.ArticleId).Distinct();

            var simpleArticleTypeId = (int) ArticleType.SimpleArticle;

            var query = from p in _articleRepository.Table
                where (!query_tmp.Contains(p.Id)) &&
                      //include only simple articles
                      (p.ArticleTypeId == simpleArticleTypeId) &&
                      (!p.Deleted) &&
                      (contributorId == 0 || p.ContributorId == contributorId) &&
                      (categoryId == 0 || p.ArticleCategories.Count(pc => pc.CategoryId == categoryId) > 0) &&
                      (publisherId == 0 || p.ArticlePublishers.Count(pm => pm.PublisherId == publisherId) > 0) &&
                      (showHidden || p.Published)
                select p;


            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Article" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            query = query.OrderBy(p => p.Name);

            var articles = new PagedList<Article>(query, pageIndex, pageSize);
            return articles;
        }

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
        public virtual decimal ProfitReport(int storeId = 0, int contributorId = 0,
            int billingCountryId = 0, int subscriptionId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string subscriptionNotes = null)
        {
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchEmail = String.IsNullOrEmpty(billingEmail);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchLastName = String.IsNullOrEmpty(billingLastName);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchSubscriptionNotes = String.IsNullOrEmpty(subscriptionNotes);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchPaymentMethods = String.IsNullOrEmpty(paymentMethodSystemName);

            var subscriptions = _subscriptionRepository.Table;
            if (osIds != null && osIds.Any())
                subscriptions = subscriptions.Where(o => osIds.Contains(o.SubscriptionStatusId));
            if (psIds != null && psIds.Any())
                subscriptions = subscriptions.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                subscriptions = subscriptions.Where(o => ssIds.Contains(o.ShippingStatusId));

            var query = from subscriptionItem in _subscriptionItemRepository.Table
                        join o in subscriptions on subscriptionItem.SubscriptionId equals o.Id
                        where (storeId == 0 || storeId == o.StoreId) &&
                              (subscriptionId == 0 || subscriptionId == o.Id) &&
                              (billingCountryId ==0 || (o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId)) &&
                              (dontSearchPaymentMethods || paymentMethodSystemName == o.PaymentMethodSystemName) &&
                              (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                              (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                              (!o.Deleted) &&
                              (contributorId == 0 || subscriptionItem.Article.ContributorId == contributorId) &&
                              //we do not ignore deleted articles when calculating subscription reports
                              //(!p.Deleted)
                              (dontSearchEmail || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail))) &&
                              (dontSearchLastName || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName))) &&
                              (dontSearchSubscriptionNotes || o.SubscriptionNotes.Any(oNote => oNote.Note.Contains(subscriptionNotes)))
                        select subscriptionItem;

            var articleCost = Convert.ToDecimal(query.Sum(subscriptionItem => (decimal?)subscriptionItem.OriginalArticleCost * subscriptionItem.Quantity));

            var reportSummary = GetSubscriptionAverageReportLine(
                storeId: storeId,
                contributorId: contributorId,
                billingCountryId: billingCountryId,
                subscriptionId: subscriptionId,
                paymentMethodSystemName: paymentMethodSystemName,
                osIds: osIds, 
                psIds: psIds, 
                ssIds: ssIds,
                startTimeUtc: startTimeUtc,
                endTimeUtc: endTimeUtc,
                billingEmail: billingEmail,
                billingLastName: billingLastName,
                subscriptionNotes: subscriptionNotes);
            var profit = reportSummary.SumSubscriptions - reportSummary.SumShippingExclTax - reportSummary.SumTax - articleCost;
            return profit;
        }

        #endregion
    }
}
