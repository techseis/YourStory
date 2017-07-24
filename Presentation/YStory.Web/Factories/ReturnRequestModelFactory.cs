using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Tax;
using YStory.Services.Catalog;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Seo;
using YStory.Web.Infrastructure.Cache;
using YStory.Web.Models.Subscription;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
		#region Fields

        private readonly IReturnRequestService _returnRequestService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public ReturnRequestModelFactory(IReturnRequestService returnRequestService,
            ISubscriptionService subscriptionService, 
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService, 
            IPriceFormatter priceFormatter,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService, 
            SubscriptionSettings subscriptionSettings,
            ICacheManager cacheManager)
        {
            this._returnRequestService = returnRequestService;
            this._subscriptionService = subscriptionService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._downloadService = downloadService;
            this._subscriptionSettings = subscriptionSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the subscription item model
        /// </summary>
        /// <param name="subscriptionItem">Subscription item</param>
        /// <returns>Subscription item model</returns>
        public virtual SubmitReturnRequestModel.SubscriptionItemModel PrepareSubmitReturnRequestSubscriptionItemModel(SubscriptionItem subscriptionItem)
        {
            if (subscriptionItem == null)
                throw new ArgumentNullException("subscriptionItem");

            var subscription = subscriptionItem.Subscription;

            var model = new SubmitReturnRequestModel.SubscriptionItemModel
            {
                Id = subscriptionItem.Id,
                ArticleId = subscriptionItem.Article.Id,
                ArticleName = subscriptionItem.Article.GetLocalized(x => x.Name),
                ArticleSeName = subscriptionItem.Article.GetSeName(),
                AttributeInfo = subscriptionItem.AttributeDescription,
                Quantity = subscriptionItem.Quantity
            };

            //unit price
            if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceInclTax, subscription.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceExclTax, subscription.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            return model;
        }

        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="subscription">Subscription</param>
        /// <returns>Submit return request model</returns>
        public virtual SubmitReturnRequestModel PrepareSubmitReturnRequestModel(SubmitReturnRequestModel model, Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (model == null)
                throw new ArgumentNullException("model");

            model.SubscriptionId = subscription.Id;
            model.AllowFiles = _subscriptionSettings.ReturnRequestsAllowFiles;
            model.CustomSubscriptionNumber = subscription.CustomSubscriptionNumber;

            //return reasons
            model.AvailableReturnReasons = _cacheManager.Get(string.Format(ModelCacheEventConsumer.RETURNREQUESTREASONS_MODEL_KEY, _workContext.WorkingLanguage.Id),
                () =>
                {
                    var reasons = new List<SubmitReturnRequestModel.ReturnRequestReasonModel>();
                    foreach (var rrr in _returnRequestService.GetAllReturnRequestReasons())
                        reasons.Add(new SubmitReturnRequestModel.ReturnRequestReasonModel
                        {
                            Id = rrr.Id,
                            Name = rrr.GetLocalized(x => x.Name)
                        });
                    return reasons;
                });

            //return actions
            model.AvailableReturnActions = _cacheManager.Get(string.Format(ModelCacheEventConsumer.RETURNREQUESTACTIONS_MODEL_KEY, _workContext.WorkingLanguage.Id),
                () =>
                {
                    var actions = new List<SubmitReturnRequestModel.ReturnRequestActionModel>();
                    foreach (var rra in _returnRequestService.GetAllReturnRequestActions())
                        actions.Add(new SubmitReturnRequestModel.ReturnRequestActionModel
                        {
                            Id = rra.Id,
                            Name = rra.GetLocalized(x => x.Name)
                        });
                    return actions;
                });

            //returnable articles
            var subscriptionItems = subscription.SubscriptionItems.Where(oi => !oi.Article.NotReturnable);
            foreach (var subscriptionItem in subscriptionItems)
            {
                var subscriptionItemModel = PrepareSubmitReturnRequestSubscriptionItemModel(subscriptionItem);
                model.Items.Add(subscriptionItemModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the customer return requests model
        /// </summary>
        /// <returns>Customer return requests model</returns>
        public virtual CustomerReturnRequestsModel PrepareCustomerReturnRequestsModel()
        {
            var model = new CustomerReturnRequestsModel();

            var returnRequests = _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id, _workContext.CurrentCustomer.Id);
            foreach (var returnRequest in returnRequests)
            {
                var subscriptionItem = _subscriptionService.GetSubscriptionItemById(returnRequest.SubscriptionItemId);
                if (subscriptionItem != null)
                {
                    var article = subscriptionItem.Article;
                    var download = _downloadService.GetDownloadById(returnRequest.UploadedFileId);

                    var itemModel = new CustomerReturnRequestsModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        CustomNumber = returnRequest.CustomNumber,
                        ReturnRequestStatus = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ArticleId = article.Id,
                        ArticleName = article.GetLocalized(x => x.Name),
                        ArticleSeName = article.GetSeName(),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.CustomerComments,
                        UploadedFileGuid = download != null ? download.DownloadGuid : Guid.Empty,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    model.Items.Add(itemModel);
                }
            }

            return model;
        }
        
        #endregion
    }
}
