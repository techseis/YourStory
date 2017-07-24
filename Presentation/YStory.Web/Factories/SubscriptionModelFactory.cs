using System;
using System.Linq;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Tax;
using YStory.Services.Catalog;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Seo;
using YStory.Web.Models.Common;
using YStory.Web.Models.Subscription;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the subscription model factory
    /// </summary>
    public partial class SubscriptionModelFactory : ISubscriptionModelFactory
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly ICountryService _countryService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IStoreContext _storeContext;
        private readonly ISubscriptionTotalCalculationService _subscriptionTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;

        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;

        #endregion

		#region Constructors

        public SubscriptionModelFactory(IAddressModelFactory addressModelFactory, 
            ISubscriptionService subscriptionService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ISubscriptionProcessingService subscriptionProcessingService, 
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService, 
            ILocalizationService localizationService,
            ICountryService countryService, 
            IArticleAttributeParser articleAttributeParser,
            IDownloadService downloadService,
            IStoreContext storeContext,
            ISubscriptionTotalCalculationService subscriptionTotalCalculationService,
            IRewardPointService rewardPointService,
            CatalogSettings catalogSettings,
            SubscriptionSettings subscriptionSettings,
            TaxSettings taxSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings)
        {
            this._addressModelFactory = addressModelFactory;
            this._subscriptionService = subscriptionService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._countryService = countryService;
            this._articleAttributeParser = articleAttributeParser;
            this._downloadService = downloadService;
            this._storeContext = storeContext;
            this._subscriptionTotalCalculationService = subscriptionTotalCalculationService;
            this._rewardPointService = rewardPointService;

            this._catalogSettings = catalogSettings;
            this._subscriptionSettings = subscriptionSettings;
            this._taxSettings = taxSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer subscription list model
        /// </summary>
        /// <returns>Customer subscription list model</returns>
        public virtual CustomerSubscriptionListModel PrepareCustomerSubscriptionListModel()
        {
            var model = new CustomerSubscriptionListModel();
            var subscriptions = _subscriptionService.SearchSubscriptions(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id);
            foreach (var subscription in subscriptions)
            {
                var subscriptionModel = new CustomerSubscriptionListModel.SubscriptionDetailsModel
                {
                    Id = subscription.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc),
                    SubscriptionStatusEnum = subscription.SubscriptionStatus,
                    SubscriptionStatus = subscription.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = subscription.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    
                    IsReturnRequestAllowed = _subscriptionProcessingService.IsReturnRequestAllowed(subscription),
                    CustomSubscriptionNumber = subscription.CustomSubscriptionNumber
                };
                var subscriptionTotalInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTotal, subscription.CurrencyRate);
                subscriptionModel.SubscriptionTotal = _priceFormatter.FormatPrice(subscriptionTotalInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.Subscriptions.Add(subscriptionModel);
            }

            var recurringPayments = _subscriptionService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerSubscriptionListModel.RecurringSubscriptionModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialSubscriptionId = recurringPayment.InitialSubscription.Id,
                    InitialSubscriptionNumber = recurringPayment.InitialSubscription.CustomSubscriptionNumber,
                    CanCancel = _subscriptionProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    CanRetryLastPayment = _subscriptionProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
                };

                model.RecurringSubscriptions.Add(recurringPaymentModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the subscription details model
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Subscription details model</returns>
        public virtual SubscriptionDetailsModel PrepareSubscriptionDetailsModel(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");
            var model = new SubscriptionDetailsModel();

            model.Id = subscription.Id;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc);
            model.SubscriptionStatus = subscription.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.IsReSubscriptionAllowed = _subscriptionSettings.IsReSubscriptionAllowed;
            model.IsReturnRequestAllowed = _subscriptionProcessingService.IsReturnRequestAllowed(subscription);
            model.PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingSubscriptions && subscription.SubscriptionStatus == SubscriptionStatus.Pending;
            model.CustomSubscriptionNumber = subscription.CustomSubscriptionNumber;

             


            //billing info
            _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                address: subscription.BillingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = subscription.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(subscription.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : subscription.PaymentMethodSystemName;
            model.PaymentMethodStatus = subscription.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(subscription);
            //custom values
            model.CustomValues = subscription.DeserializeCustomValues();

            //subscription subtotal
            if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionSubtotal)
            {
                //including tax

                //subscription subtotal
                var subscriptionSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalInclTax, subscription.CurrencyRate);
                model.SubscriptionSubtotal = _priceFormatter.FormatPrice(subscriptionSubtotalInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to subscription subtotal)
                var subscriptionSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountInclTax, subscription.CurrencyRate);
                if (subscriptionSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.SubscriptionSubTotalDiscount = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //subscription subtotal
                var subscriptionSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalExclTax, subscription.CurrencyRate);
                model.SubscriptionSubtotal = _priceFormatter.FormatPrice(subscriptionSubtotalExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to subscription subtotal)
                var subscriptionSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountExclTax, subscription.CurrencyRate);
                if (subscriptionSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.SubscriptionSubTotalDiscount = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //subscription shipping
                var subscriptionShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionShippingInclTax, subscription.CurrencyRate);
                model.SubscriptionShipping = _priceFormatter.FormatShippingPrice(subscriptionShippingInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeInclTax, subscription.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //subscription shipping
                var subscriptionShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionShippingExclTax, subscription.CurrencyRate);
                model.SubscriptionShipping = _priceFormatter.FormatShippingPrice(subscriptionShippingExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeExclTax, subscription.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInSubscriptionSummary && subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (subscription.SubscriptionTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    displayTaxRates = _taxSettings.DisplayTaxRates && subscription.TaxRatesDictionary.Any();
                    displayTax = !displayTaxRates;

                    var subscriptionTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTax, subscription.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(subscriptionTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in subscription.TaxRatesDictionary)
                    {
                        model.TaxRates.Add(new SubscriptionDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, subscription.CurrencyRate), true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoSubscriptionDetailsPage;
            model.PricesIncludeTax = subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to subscription total)
            var subscriptionDiscountInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionDiscount, subscription.CurrencyRate);
            if (subscriptionDiscountInCustomerCurrency > decimal.Zero)
                model.SubscriptionTotalDiscount = _priceFormatter.FormatPrice(-subscriptionDiscountInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage);


           

            //reward points           
            if (subscription.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -subscription.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(subscription.RedeemedRewardPointsEntry.UsedAmount, subscription.CurrencyRate)), true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var subscriptionTotalInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTotal, subscription.CurrencyRate);
            model.SubscriptionTotal = _priceFormatter.FormatPrice(subscriptionTotalInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = subscription.CheckoutAttributeDescription;

            //subscription notes
            foreach (var subscriptionNote in subscription.SubscriptionNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.SubscriptionNotes.Add(new SubscriptionDetailsModel.SubscriptionNote
                {
                    Id = subscriptionNote.Id,
                    HasDownload = subscriptionNote.DownloadId > 0,
                    Note = subscriptionNote.FormatSubscriptionNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(subscriptionNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }


            //purchased articles
            model.ShowSku = _catalogSettings.ShowSkuOnArticleDetailsPage;
            var subscriptionItems = subscription.SubscriptionItems;
            foreach (var subscriptionItem in subscriptionItems)
            {
                var subscriptionItemModel = new SubscriptionDetailsModel.SubscriptionItemModel
                {
                    Id = subscriptionItem.Id,
                    SubscriptionItemGuid = subscriptionItem.SubscriptionItemGuid,
                    Sku = subscriptionItem.Article.FormatSku(subscriptionItem.AttributesXml, _articleAttributeParser),
                    ArticleId = subscriptionItem.Article.Id,
                    ArticleName = subscriptionItem.Article.GetLocalized(x => x.Name),
                    ArticleSeName = subscriptionItem.Article.GetSeName(),
                    Quantity = subscriptionItem.Quantity,
                    AttributeInfo = subscriptionItem.AttributeDescription,
                };
                //rental info
                if (subscriptionItem.Article.IsRental)
                {
                    var rentalStartDate = subscriptionItem.RentalStartDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = subscriptionItem.RentalEndDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalEndDateUtc.Value) : "";
                    subscriptionItemModel.RentalInfo = string.Format(_localizationService.GetResource("Subscription.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(subscriptionItemModel);

                //unit price, subtotal
                if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceInclTax, subscription.CurrencyRate);
                    subscriptionItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceInclTax, subscription.CurrencyRate);
                    subscriptionItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceExclTax, subscription.CurrencyRate);
                    subscriptionItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceExclTax, subscription.CurrencyRate);
                    subscriptionItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

              
                if (_downloadService.IsLicenseDownloadAllowed(subscriptionItem))
                    subscriptionItemModel.LicenseId = subscriptionItem.LicenseDownloadId.HasValue ? subscriptionItem.LicenseDownloadId.Value : 0;
            }

            return model;
        }

        

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        public virtual CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page)
        {
            var customer = _workContext.CurrentCustomer;
            var pageSize = _rewardPointsSettings.PageSize;
            var model = new CustomerRewardPointsModel();
            var list = _rewardPointService.GetRewardPointsHistory(customer.Id, showNotActivated: true, pageIndex: --page ?? 0, pageSize: pageSize);

            model.RewardPoints = list.Select(rph =>
            {
                var activatingDate = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc);
                return new CustomerRewardPointsModel.RewardPointsHistoryModel
                {
                    Points = rph.Points,
                    PointsBalance = rph.PointsBalance.HasValue ? rph.PointsBalance.ToString()
                        : string.Format(_localizationService.GetResource("RewardPoints.ActivatedLater"), activatingDate),
                    Message = rph.Message,
                    CreatedOn = activatingDate
                };
            }).ToList();

            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = true,
                RouteActionName = "CustomerRewardPointsPaged",
                UseRouteLinks = true,
                RouteValues = new RewardPointsRouteValues { page = page ?? 0}
            };

            //current amount/balance
            int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            decimal rewardPointsAmountBase = _subscriptionTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
            //minimum amount/balance
            int minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            decimal minimumRewardPointsAmountBase = _subscriptionTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            decimal minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);
            return model;
        }

        #endregion
    }
}
