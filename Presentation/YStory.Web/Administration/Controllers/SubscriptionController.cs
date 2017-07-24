using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Helpers;
using YStory.Admin.Models.Subscriptions;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
using YStory.Core.Domain.Tax;
using YStory.Services;
using YStory.Services.Affiliates;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Directory;
using YStory.Services.ExportImport;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Security;
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Services.Contributors;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class SubscriptionController : BaseAdminController
    {
        #region Fields

        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionReportService _subscriptionReportService;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly IReturnRequestService _returnRequestService;
	    private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IEncryptionService _encryptionService;
        private readonly IPaymentService _paymentService;
        private readonly IMeasureService _measureService;
        private readonly IPdfService _pdfService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IArticleService _articleService;
        private readonly IExportManager _exportManager;
        private readonly IPermissionService _permissionService;
	    private readonly IWorkflowMessageService _workflowMessageService;
	    private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
	    private readonly IArticleAttributeService _articleAttributeService;
	    private readonly IArticleAttributeParser _articleAttributeParser;
	    private readonly IArticleAttributeFormatter _articleAttributeFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IDownloadService _downloadService;
        private readonly IStoreService _storeService;
        private readonly IContributorService _contributorService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
	    private readonly IAddressAttributeFormatter _addressAttributeFormatter;
	    private readonly IAffiliateService _affiliateService;
	    private readonly IPictureService _pictureService;
        private readonly ICustomerActivityService _customerActivityService;
	    private readonly ICacheManager _cacheManager;

        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public SubscriptionController(ISubscriptionService subscriptionService, 
            ISubscriptionReportService subscriptionReportService, 
            ISubscriptionProcessingService subscriptionProcessingService,
            IReturnRequestService returnRequestService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IEncryptionService encryptionService,
            IPaymentService paymentService,
            IMeasureService measureService,
            IPdfService pdfService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IArticleService articleService,
            IExportManager exportManager,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICategoryService categoryService, 
            IPublisherService publisherService,
            IArticleAttributeService articleAttributeService, 
            IArticleAttributeParser articleAttributeParser,
            IArticleAttributeFormatter articleAttributeFormatter, 
            IShoppingCartService shoppingCartService,
            IDownloadService downloadService,
            IStoreService storeService,
            IContributorService contributorService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAffiliateService affiliateService,
            IPictureService pictureService,
            ICustomerActivityService customerActivityService,
            ICacheManager cacheManager,
            SubscriptionSettings subscriptionSettings,
            CurrencySettings currencySettings, 
            TaxSettings taxSettings,
            MeasureSettings measureSettings,
            AddressSettings addressSettings)
		{
            this._subscriptionService = subscriptionService;
            this._subscriptionReportService = subscriptionReportService;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._returnRequestService = returnRequestService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._encryptionService = encryptionService;
            this._paymentService = paymentService;
            this._measureService = measureService;
            this._pdfService = pdfService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._articleService = articleService;
            this._exportManager = exportManager;
            this._permissionService = permissionService;
            this._workflowMessageService = workflowMessageService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleAttributeService = articleAttributeService;
            this._articleAttributeParser = articleAttributeParser;
            this._articleAttributeFormatter = articleAttributeFormatter;
            this._shoppingCartService = shoppingCartService;
            this._downloadService = downloadService;
            this._storeService = storeService;
            this._contributorService = contributorService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._affiliateService = affiliateService;
            this._pictureService = pictureService;
            this._customerActivityService = customerActivityService;
            this._cacheManager = cacheManager;
            this._subscriptionSettings = subscriptionSettings;
            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._addressSettings = addressSettings;
		}
        
        #endregion

        #region Utilities

        [NonAction]
        protected virtual bool HasAccessToSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (_workContext.CurrentContributor == null)
                //not a contributor; has access
                return true;

            var contributorId = _workContext.CurrentContributor.Id;
            var hasContributorArticles = subscription.SubscriptionItems.Any(subscriptionItem => subscriptionItem.Article.ContributorId == contributorId);
            return hasContributorArticles;
        }

        [NonAction]
        protected virtual bool HasAccessToSubscriptionItem(SubscriptionItem subscriptionItem)
        {
            if (subscriptionItem == null)
                throw new ArgumentNullException("subscriptionItem");

            if (_workContext.CurrentContributor == null)
                //not a contributor; has access
                return true;

            var contributorId = _workContext.CurrentContributor.Id;
            return subscriptionItem.Article.ContributorId == contributorId;
        }

        [NonAction]
        protected virtual bool HasAccessToArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (_workContext.CurrentContributor == null)
                //not a contributor; has access
                return true;

            var contributorId = _workContext.CurrentContributor.Id;
            return article.ContributorId == contributorId;
        }

      

	    /// <summary>
	    /// Parse article attributes on the add article to subscription details page
	    /// </summary>
	    /// <param name="article">Article</param>
	    /// <param name="form">Form</param>
        /// <param name="errors">Errors</param>
        /// <returns>Parsed attributes</returns>
	    [NonAction]
        protected virtual string ParseArticleAttributes(Article article, FormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;

            #region Article attributes

            var articleAttributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
            foreach (var attribute in articleAttributes)
            {
                var controlId = string.Format("article_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[string.Format("article_attribute_{0}_{1}_qty", attribute.Id, selectedAttributeId)];
                                    if (quantityStr != null && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[string.Format("article_attribute_{0}_{1}_qty", attribute.Id, item)];
                                        if (quantityStr != null && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[string.Format("article_attribute_{0}_{1}_qty", attribute.Id, selectedAttributeId)];
                                if (quantityStr != null && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in articleAttributes)
            {
                var conditionMet = _articleAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _articleAttributeParser.RemoveArticleAttribute(attributesXml, attribute);
                }
            }

            #endregion

            return attributesXml;
        }

        /// <summary>
        /// Parse rental dates on the add article to subscription details page
        /// </summary>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [NonAction]
        protected virtual void ParseRentalDates(FormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var ctrlStartDate = form["rental_start_date"];
            var ctrlEndDate = form["rental_end_date"];
            try
            {
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        [NonAction]
        protected virtual void PrepareSubscriptionDetailsModel(SubscriptionModel model, Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = subscription.Id;
            model.SubscriptionStatus = subscription.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.SubscriptionStatusId = subscription.SubscriptionStatusId;
            model.SubscriptionGuid = subscription.SubscriptionGuid;
            model.CustomSubscriptionNumber = subscription.CustomSubscriptionNumber;
            var store = _storeService.GetStoreById(subscription.StoreId);
            model.StoreName = store != null ? store.Name : "Unknown";
            model.CustomerId = subscription.CustomerId;
            var customer = subscription.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CustomerIp = subscription.CustomerIp;
            model.VatNumber = subscription.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc);
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;

            var affiliate = _affiliateService.GetAffiliateById(subscription.AffiliateId);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
                model.AffiliateName = affiliate.GetFullName();
            }

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            //custom values
            model.CustomValues = subscription.DeserializeCustomValues();
            
            #region Subscription totals

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            //subtotal
            model.SubscriptionSubtotalInclTax = _priceFormatter.FormatPrice(subscription.SubscriptionSubtotalInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.SubscriptionSubtotalExclTax = _priceFormatter.FormatPrice(subscription.SubscriptionSubtotalExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.SubscriptionSubtotalInclTaxValue = subscription.SubscriptionSubtotalInclTax;
            model.SubscriptionSubtotalExclTaxValue = subscription.SubscriptionSubtotalExclTax;
            //discount (applied to subscription subtotal)
            string subscriptionSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(subscription.SubscriptionSubTotalDiscountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            string subscriptionSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(subscription.SubscriptionSubTotalDiscountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (subscription.SubscriptionSubTotalDiscountInclTax > decimal.Zero)
                model.SubscriptionSubTotalDiscountInclTax = subscriptionSubtotalDiscountInclTaxStr;
            if (subscription.SubscriptionSubTotalDiscountExclTax > decimal.Zero)
                model.SubscriptionSubTotalDiscountExclTax = subscriptionSubtotalDiscountExclTaxStr;
            model.SubscriptionSubTotalDiscountInclTaxValue = subscription.SubscriptionSubTotalDiscountInclTax;
            model.SubscriptionSubTotalDiscountExclTaxValue = subscription.SubscriptionSubTotalDiscountExclTax;

            //shipping
            model.SubscriptionShippingInclTax = _priceFormatter.FormatShippingPrice(subscription.SubscriptionShippingInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.SubscriptionShippingExclTax = _priceFormatter.FormatShippingPrice(subscription.SubscriptionShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.SubscriptionShippingInclTaxValue = subscription.SubscriptionShippingInclTax;
            model.SubscriptionShippingExclTaxValue = subscription.SubscriptionShippingExclTax;

            //payment method additional fee
            if (subscription.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(subscription.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(subscription.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }
            model.PaymentMethodAdditionalFeeInclTaxValue = subscription.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = subscription.PaymentMethodAdditionalFeeExclTax;


            //tax
            model.Tax = _priceFormatter.FormatPrice(subscription.SubscriptionTax, true, false);
            SortedDictionary<decimal, decimal> taxRates = subscription.TaxRatesDictionary;
            bool displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
            bool displayTax = !displayTaxRates;
            foreach (var tr in subscription.TaxRatesDictionary)
            {
                model.TaxRates.Add(new SubscriptionModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = _priceFormatter.FormatPrice(tr.Value, true, false),
                });
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.TaxValue = subscription.SubscriptionTax;
            model.TaxRatesValue = subscription.TaxRates;

            //discount
            if (subscription.SubscriptionDiscount > 0)
                model.SubscriptionTotalDiscount = _priceFormatter.FormatPrice(-subscription.SubscriptionDiscount, true, false);
            model.SubscriptionTotalDiscountValue = subscription.SubscriptionDiscount;

           

            //reward points
            if (subscription.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -subscription.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-subscription.RedeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.SubscriptionTotal = _priceFormatter.FormatPrice(subscription.SubscriptionTotal, true, false);
            model.SubscriptionTotalValue = subscription.SubscriptionTotal;

            //refunded amount
            if (subscription.RefundedAmount > decimal.Zero)
                model.RefundedAmount = _priceFormatter.FormatPrice(subscription.RefundedAmount, true, false);

            

            //profit (hide for contributors)
            if (_workContext.CurrentContributor == null)
            {
                var profit = _subscriptionReportService.ProfitReport(subscriptionId: subscription.Id);
                model.Profit = _priceFormatter.FormatPrice(profit, true, false);
            }

            #endregion

            #region Payment info

            if (subscription.AllowStoringCreditCardNumber)
            {
                //card type
                model.CardType = _encryptionService.DecryptText(subscription.CardType);
                //cardholder name
                model.CardName = _encryptionService.DecryptText(subscription.CardName);
                //card number
                model.CardNumber = _encryptionService.DecryptText(subscription.CardNumber);
                //cvv
                model.CardCvv2 = _encryptionService.DecryptText(subscription.CardCvv2);
                //expiry date
                string cardExpirationMonthDecrypted = _encryptionService.DecryptText(subscription.CardExpirationMonth);
                if (!String.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                    model.CardExpirationMonth = cardExpirationMonthDecrypted;
                string cardExpirationYearDecrypted = _encryptionService.DecryptText(subscription.CardExpirationYear);
                if (!String.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                    model.CardExpirationYear = cardExpirationYearDecrypted;

                model.AllowStoringCreditCardNumber = true;
            }
            else
            {
                string maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(subscription.MaskedCreditCardNumber);
                if (!String.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                    model.CardNumber = maskedCreditCardNumberDecrypted;
            }


            //payment transaction info
            model.AuthorizationTransactionId = subscription.AuthorizationTransactionId;
            model.CaptureTransactionId = subscription.CaptureTransactionId;
            model.SubscriptionTransactionId = subscription.SubscriptionTransactionId;

            //payment method info
            var pm = _paymentService.LoadPaymentMethodBySystemName(subscription.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : subscription.PaymentMethodSystemName;
            model.PaymentStatus = subscription.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);

            //payment method buttons
            model.CanCancelSubscription = _subscriptionProcessingService.CanCancelSubscription(subscription);
            model.CanCapture = _subscriptionProcessingService.CanCapture(subscription);
            model.CanMarkSubscriptionAsPaid = _subscriptionProcessingService.CanMarkSubscriptionAsPaid(subscription);
            model.CanRefund = _subscriptionProcessingService.CanRefund(subscription);
            model.CanRefundOffline = _subscriptionProcessingService.CanRefundOffline(subscription);
            model.CanPartiallyRefund = _subscriptionProcessingService.CanPartiallyRefund(subscription, decimal.Zero);
            model.CanPartiallyRefundOffline = _subscriptionProcessingService.CanPartiallyRefundOffline(subscription, decimal.Zero);
            model.CanVoid = _subscriptionProcessingService.CanVoid(subscription);
            model.CanVoidOffline = _subscriptionProcessingService.CanVoidOffline(subscription);
            
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.MaxAmountToRefund = subscription.SubscriptionTotal - subscription.RefundedAmount;

            //recurring payment record
            var recurringPayment = _subscriptionService.SearchRecurringPayments(initialSubscriptionId: subscription.Id, showHidden: true).FirstOrDefault();
            if (recurringPayment != null)
            {
                model.RecurringPaymentId = recurringPayment.Id;
            }
            #endregion

            #region Billing & shipping info

            model.BillingAddress = subscription.BillingAddress.ToModel();
            model.BillingAddress.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(subscription.BillingAddress.CustomAttributes);
            model.BillingAddress.FirstNameEnabled = true;
            model.BillingAddress.FirstNameRequired = true;
            model.BillingAddress.LastNameEnabled = true;
            model.BillingAddress.LastNameRequired = true;
            model.BillingAddress.EmailEnabled = true;
            model.BillingAddress.EmailRequired = true;
            model.BillingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.BillingAddress.CompanyRequired = _addressSettings.CompanyRequired;
            model.BillingAddress.CountryEnabled = _addressSettings.CountryEnabled;
            model.BillingAddress.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.BillingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.BillingAddress.CityEnabled = _addressSettings.CityEnabled;
            model.BillingAddress.CityRequired = _addressSettings.CityRequired;
            model.BillingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.BillingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.BillingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.BillingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.BillingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.BillingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.BillingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.BillingAddress.PhoneRequired = _addressSettings.PhoneRequired;
            model.BillingAddress.FaxEnabled = _addressSettings.FaxEnabled;
            model.BillingAddress.FaxRequired = _addressSettings.FaxRequired;

             
            #endregion

            #region Articles

            model.CheckoutAttributeInfo = subscription.CheckoutAttributeDescription;
            bool hasDownloadableItems = false;
            var articles = subscription.SubscriptionItems;
            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                articles = articles
                    .Where(subscriptionItem => subscriptionItem.Article.ContributorId == _workContext.CurrentContributor.Id)
                    .ToList();
            }
            foreach (var subscriptionItem in articles)
            {
                

                var subscriptionItemModel = new SubscriptionModel.SubscriptionItemModel
                {
                    Id = subscriptionItem.Id,
                    ArticleId = subscriptionItem.ArticleId,
                    ArticleName = subscriptionItem.Article.Name,
                    Sku = subscriptionItem.Article.FormatSku(subscriptionItem.AttributesXml, _articleAttributeParser),
                    Quantity = subscriptionItem.Quantity,
                   
                };
                //picture
                var subscriptionItemPicture = subscriptionItem.Article.GetArticlePicture(subscriptionItem.AttributesXml, _pictureService, _articleAttributeParser);
                subscriptionItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(subscriptionItemPicture, 75, true);

                 
                //contributor
                var contributor = _contributorService.GetContributorById(subscriptionItem.Article.ContributorId);
                subscriptionItemModel.ContributorName = contributor != null ? contributor.Name : "";

                //unit price
                subscriptionItemModel.UnitPriceInclTaxValue = subscriptionItem.UnitPriceInclTax;
                subscriptionItemModel.UnitPriceExclTaxValue = subscriptionItem.UnitPriceExclTax;
                subscriptionItemModel.UnitPriceInclTax = _priceFormatter.FormatPrice(subscriptionItem.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionItemModel.UnitPriceExclTax = _priceFormatter.FormatPrice(subscriptionItem.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //discounts
                subscriptionItemModel.DiscountInclTaxValue = subscriptionItem.DiscountAmountInclTax;
                subscriptionItemModel.DiscountExclTaxValue = subscriptionItem.DiscountAmountExclTax;
                subscriptionItemModel.DiscountInclTax = _priceFormatter.FormatPrice(subscriptionItem.DiscountAmountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionItemModel.DiscountExclTax = _priceFormatter.FormatPrice(subscriptionItem.DiscountAmountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //subtotal
                subscriptionItemModel.SubTotalInclTaxValue = subscriptionItem.PriceInclTax;
                subscriptionItemModel.SubTotalExclTaxValue = subscriptionItem.PriceExclTax;
                subscriptionItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(subscriptionItem.PriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                subscriptionItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(subscriptionItem.PriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                subscriptionItemModel.AttributeInfo = subscriptionItem.AttributeDescription;
                if (subscriptionItem.Article.IsRecurring)
                    subscriptionItemModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Subscriptions.Articles.RecurringPeriod"), subscriptionItem.Article.RecurringCycleLength, subscriptionItem.Article.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));
                //rental info
                if (subscriptionItem.Article.IsRental)
                {
                    var rentalStartDate = subscriptionItem.RentalStartDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = subscriptionItem.RentalEndDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalEndDateUtc.Value) : "";
                    subscriptionItemModel.RentalInfo = string.Format(_localizationService.GetResource("Subscription.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //return requests
                subscriptionItemModel.ReturnRequests = _returnRequestService
                    .SearchReturnRequests(subscriptionItemId: subscriptionItem.Id)
                    .Select(item => new SubscriptionModel.SubscriptionItemModel.ReturnRequestBriefModel
                    {
                        CustomNumber = item.CustomNumber,
                        Id = item.Id
                    }).ToList();

               

                model.Items.Add(subscriptionItemModel);
            }
            model.HasDownloadableArticles = hasDownloadableItems;
            #endregion
        }

        [NonAction]
        protected virtual SubscriptionModel.AddSubscriptionArticleModel.ArticleDetailsModel PrepareAddArticleToSubscriptionModel(int subscriptionId, int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");

            var presetQty = 1;
            var presetPrice = _priceCalculationService.GetFinalPrice(article, subscription.Customer, decimal.Zero, true, presetQty);
            decimal taxRate;
            decimal presetPriceInclTax = _taxService.GetArticlePrice(article, presetPrice, true, subscription.Customer, out taxRate);
            decimal presetPriceExclTax = _taxService.GetArticlePrice(article, presetPrice, false, subscription.Customer, out taxRate);

            var model = new SubscriptionModel.AddSubscriptionArticleModel.ArticleDetailsModel
            {
                ArticleId = articleId,
                SubscriptionId = subscriptionId,
                Name = article.Name,
                ArticleType = article.ArticleType,
                UnitPriceExclTax = presetPriceExclTax,
                UnitPriceInclTax = presetPriceInclTax,
                Quantity = presetQty,
                SubTotalExclTax = presetPriceExclTax,
                SubTotalInclTax = presetPriceInclTax,
                AutoUpdateSubscriptionTotals = _subscriptionSettings.AutoUpdateSubscriptionTotalsOnEditingSubscription
            };

            //attributes
            var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
            foreach (var attribute in attributes)
            {
                var attributeModel = new SubscriptionModel.AddSubscriptionArticleModel.ArticleAttributeModel
                {
                    Id = attribute.Id,
                    ArticleAttributeId = attribute.ArticleAttributeId,
                    Name = attribute.ArticleAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    HasCondition = !String.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        //price adjustment
                        var priceAdjustment = _taxService.GetArticlePrice(article,
                            _priceCalculationService.GetArticleAttributeValuePriceAdjustment(attributeValue), out taxRate);

                        attributeModel.Values.Add(new SubscriptionModel.AddSubscriptionArticleModel.ArticleAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity,
                            PriceAdjustment = priceAdjustment == decimal.Zero ? string.Empty : priceAdjustment > decimal.Zero
                                ? string.Concat("+", _priceFormatter.FormatPrice(priceAdjustment, false, false))
                                : string.Concat("-", _priceFormatter.FormatPrice(-priceAdjustment, false, false)),
                            PriceAdjustmentValue = priceAdjustment
                        });
                    }
                }

                model.ArticleAttributes.Add(attributeModel);
            }
            model.HasCondition = model.ArticleAttributes.Any(a => a.HasCondition);
            //gift card
            
            //rental
            model.IsRental = article.IsRental;
            return model;
        }

      
        #endregion

        #region Subscription list

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List(
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> subscriptionStatusIds = null,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> paymentStatusIds = null,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> shippingStatusIds = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //subscription statuses
            var model = new SubscriptionListModel();
            model.AvailableSubscriptionStatuses = SubscriptionStatus.Pending.ToSelectList(false).ToList();
            model.AvailableSubscriptionStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (subscriptionStatusIds != null && subscriptionStatusIds.Any())
            {
                foreach (var item in model.AvailableSubscriptionStatuses.Where(os => subscriptionStatusIds.Contains(os.Value)))
                    item.Selected = true;
                model.AvailableSubscriptionStatuses.First().Selected = false;
            }
            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (paymentStatusIds != null && paymentStatusIds.Any())
            {
                foreach (var item in model.AvailablePaymentStatuses.Where(ps => paymentStatusIds.Contains(ps.Value)))
                    item.Selected = true;
                model.AvailablePaymentStatuses.First().Selected = false;
            }

         

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //contributors
            model.AvailableContributors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var contributors = SelectListHelper.GetContributorList(_contributorService, _cacheManager, true);
            foreach (var v in contributors)
                model.AvailableContributors.Add(v);

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            

            //payment methods
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var pm in _paymentService.LoadAllPaymentMethods())
                model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
            {
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //a contributor should have access only to subscriptions with his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            
            return View(model);
        }

        [HttpPost]
		public virtual ActionResult SubscriptionList(DataSourceRequest command, SubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.ContributorId = _workContext.CurrentContributor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var subscriptionStatusIds = !model.SubscriptionStatusIds.Contains(0) ? model.SubscriptionStatusIds : null;
            var paymentStatusIds = !model.PaymentStatusIds.Contains(0) ? model.PaymentStatusIds : null;
            var shippingStatusIds = !model.ShippingStatusIds.Contains(0) ? model.ShippingStatusIds : null;

		    var filterByArticleId = 0;
		    var article = _articleService.GetArticleById(model.ArticleId);
		    if (article != null && HasAccessToArticle(article))
                filterByArticleId = model.ArticleId;

            //load subscriptions
            var subscriptions = _subscriptionService.SearchSubscriptions(storeId: model.StoreId,
                contributorId: model.ContributorId,
                articleId: filterByArticleId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue, 
                createdToUtc: endDateValue,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes,
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = subscriptions.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new SubscriptionModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        SubscriptionTotal = _priceFormatter.FormatPrice(x.SubscriptionTotal, true, false),
                        SubscriptionStatus = x.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext),
                        SubscriptionStatusId = x.SubscriptionStatusId,
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatusId = x.PaymentStatusId,
                       
                        ShippingStatusId = x.ShippingStatusId,
                        CustomerEmail = x.BillingAddress.Email,
                        CustomerFullName = string.Format("{0} {1}", x.BillingAddress.FirstName, x.BillingAddress.LastName),
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomSubscriptionNumber = x.CustomSubscriptionNumber
                    };
                }),
                Total = subscriptions.TotalCount
            };

            //summary report
            //currently we do not support articleId and warehouseId parameters for this report
            var reportSummary = _subscriptionReportService.GetSubscriptionAverageReportLine(
                storeId: model.StoreId,
                contributorId: model.ContributorId,
                subscriptionId: 0,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes);

            var profit = _subscriptionReportService.ProfitReport(
                storeId: model.StoreId,
                contributorId: model.ContributorId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue, 
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes);
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            gridModel.ExtraData = new SubscriptionAggreratorModel
            {
                aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                aggregatorshipping = _priceFormatter.FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false),
                aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
                aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumSubscriptions, true, false)
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-subscription-by-number")]
        public virtual ActionResult GoToSubscriptionId(SubscriptionListModel model)
        {
            var subscription = _subscriptionService.GetOrderByCustomSubscriptionNumber(model.GoDirectlyToCustomSubscriptionNumber);

            if (subscription == null)
                return List();

            return RedirectToAction("Edit", "Subscription", new { id = subscription.Id });
        }

        public virtual ActionResult ArticleSearchAutoComplete(string term)
        {
            const int searchTermMinimumLength = 3;
            if (String.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                contributorId = _workContext.CurrentContributor.Id;
            }

            //articles
            const int articleNumber = 15;
            var articles = _articleService.SearchArticles(
                contributorId: contributorId,
                keywords: term,
                pageSize: articleNumber,
                showHidden: true);

            var result = (from p in articles
                          select new
                          {
                              label = p.Name,
                              articleid = p.Id
                          })
                          .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual ActionResult ExportXmlAll(SubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.ContributorId = _workContext.CurrentContributor.Id;
            }

            var subscriptionStatusIds = !model.SubscriptionStatusIds.Contains(0) ? model.SubscriptionStatusIds : null;
            var paymentStatusIds = !model.PaymentStatusIds.Contains(0) ? model.PaymentStatusIds : null;
            var shippingStatusIds = !model.ShippingStatusIds.Contains(0) ? model.ShippingStatusIds : null;

            var filterByArticleId = 0;
            var article = _articleService.GetArticleById(model.ArticleId);
            if (article != null && HasAccessToArticle(article))
                filterByArticleId = model.ArticleId;

            //load subscriptions
            var subscriptions = _subscriptionService.SearchSubscriptions(storeId: model.StoreId,
                contributorId: model.ContributorId,
                articleId: filterByArticleId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes);

            try
            {
                var xml = _exportManager.ExportSubscriptionsToXml(subscriptions);
                return new XmlDownloadResult(xml, "subscriptions.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();
            
            var subscriptions = new List<Subscription>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                subscriptions.AddRange(_subscriptionService.GetSubscriptionsByIds(ids).Where(HasAccessToSubscription));
            }

            var xml = _exportManager.ExportSubscriptionsToXml(subscriptions);
            return new XmlDownloadResult(xml, "subscriptions.xml");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual ActionResult ExportExcelAll(SubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();
            
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.ContributorId = _workContext.CurrentContributor.Id;
            }

            var subscriptionStatusIds = !model.SubscriptionStatusIds.Contains(0) ? model.SubscriptionStatusIds : null;
            var paymentStatusIds = !model.PaymentStatusIds.Contains(0) ? model.PaymentStatusIds : null;
            var shippingStatusIds = !model.ShippingStatusIds.Contains(0) ? model.ShippingStatusIds : null;

            var filterByArticleId = 0;
            var article = _articleService.GetArticleById(model.ArticleId);
            if (article != null && HasAccessToArticle(article))
                filterByArticleId = model.ArticleId;

            //load subscriptions
            var subscriptions = _subscriptionService.SearchSubscriptions(storeId: model.StoreId,
                contributorId: model.ContributorId,
                articleId: filterByArticleId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes);

            try
            {
                byte[] bytes = _exportManager.ExportSubscriptionsToXlsx(subscriptions);
                return File(bytes, MimeTypes.TextXlsx, "subscriptions.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscriptions = new List<Subscription>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                subscriptions.AddRange(_subscriptionService.GetSubscriptionsByIds(ids).Where(HasAccessToSubscription));
            }

            try
            {
                byte[] bytes = _exportManager.ExportSubscriptionsToXlsx(subscriptions);
                return File(bytes, MimeTypes.TextXlsx, "subscriptions.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Subscription details

        #region Payments and other subscription workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelsubscription")]
        public virtual ActionResult CancelSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _subscriptionProcessingService.CancelSubscription(subscription, true);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("capturesubscription")]
        public virtual ActionResult CaptureSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _subscriptionProcessingService.Capture(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }

        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("marksubscriptionaspaid")]
        public virtual ActionResult MarkSubscriptionAsPaid(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _subscriptionProcessingService.MarkSubscriptionAsPaid(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundsubscription")]
        public virtual ActionResult RefundSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _subscriptionProcessingService.Refund(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundsubscriptionoffline")]
        public virtual ActionResult RefundSubscriptionOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _subscriptionProcessingService.RefundOffline(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidsubscription")]
        public virtual ActionResult VoidSubscription(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                var errors = _subscriptionProcessingService.Void(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidsubscriptionoffline")]
        public virtual ActionResult VoidSubscriptionOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                _subscriptionProcessingService.VoidOffline(subscription);
                LogEditSubscription(subscription.Id);
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }
        
        public virtual ActionResult PartiallyRefundSubscriptionPopup(int id, bool online)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            var model = new SubscriptionModel();
            PrepareSubscriptionDetailsModel(model, subscription);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundsubscription")]
        public virtual ActionResult PartiallyRefundSubscriptionPopup(string btnId, string formId, int id, bool online, SubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                decimal amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new YStoryException("Enter amount to refund");

                decimal maxAmountToRefund = subscription.SubscriptionTotal - subscription.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = _subscriptionProcessingService.PartiallyRefund(subscription, amountToRefund).ToList();
                else
                    _subscriptionProcessingService.PartiallyRefundOffline(subscription, amountToRefund);

                LogEditSubscription(subscription.Id);

                if (!errors.Any())
                {
                    //success
                    ViewBag.RefreshPage = true;
                    ViewBag.btnId = btnId;
                    ViewBag.formId = formId;

                    PrepareSubscriptionDetailsModel(model, subscription);
                    return View(model);
                }
                
                //error
                PrepareSubscriptionDetailsModel(model, subscription);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveSubscriptionStatus")]
        public virtual ActionResult ChangeSubscriptionStatus(int id, SubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            try
            {
                subscription.SubscriptionStatusId = model.SubscriptionStatusId;
                _subscriptionService.UpdateSubscription(subscription);

                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Subscription status has been edited. New status: {0}", subscription.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext)),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
                LogEditSubscription(subscription.Id);

                model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null || subscription.Deleted)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null && !HasAccessToSubscription(subscription))
                return RedirectToAction("List");

            var model = new SubscriptionModel();
            PrepareSubscriptionDetailsModel(model, subscription);

            var warnings = TempData["nop.admin.subscription.warnings"] as List<string>;
            if (warnings != null)
                model.Warnings = warnings;

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            _subscriptionProcessingService.DeleteSubscription(subscription);

            //activity log
            _customerActivityService.InsertActivity("DeleteSubscription", _localizationService.GetResource("ActivityLog.DeleteSubscription"), subscription.Id);

            return RedirectToAction("List");
        }

        public virtual ActionResult PdfInvoice(int subscriptionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                contributorId = _workContext.CurrentContributor.Id;
            }

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            var subscriptions = new List<Subscription>();
            subscriptions.Add(subscription);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionsToPdf(stream, subscriptions, _subscriptionSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, contributorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, string.Format("subscription_{0}.pdf", subscription.Id));
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("pdf-invoice-all")]
        public virtual ActionResult PdfInvoiceAll(SubscriptionListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.ContributorId = _workContext.CurrentContributor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var subscriptionStatusIds = !model.SubscriptionStatusIds.Contains(0) ? model.SubscriptionStatusIds : null;
            var paymentStatusIds = !model.PaymentStatusIds.Contains(0) ? model.PaymentStatusIds : null;
            var shippingStatusIds = !model.ShippingStatusIds.Contains(0) ? model.ShippingStatusIds : null;

            var filterByArticleId = 0;
            var article = _articleService.GetArticleById(model.ArticleId);
            if (article != null && HasAccessToArticle(article))
                filterByArticleId = model.ArticleId;

            //load subscriptions
            var subscriptions = _subscriptionService.SearchSubscriptions(storeId: model.StoreId,
                contributorId: model.ContributorId,
                articleId: filterByArticleId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: subscriptionStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                subscriptionNotes: model.SubscriptionNotes);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionsToPdf(stream, subscriptions, _subscriptionSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, model.ContributorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "subscriptions.pdf");
        }

        [HttpPost]
        public virtual ActionResult PdfInvoiceSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscriptions = new List<Subscription>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                subscriptions.AddRange(_subscriptionService.GetSubscriptionsByIds(ids));
            }

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                subscriptions = subscriptions.Where(HasAccessToSubscription).ToList();
                contributorId = _workContext.CurrentContributor.Id;
            }
            
            //ensure that we at least one subscription selected
            if (!subscriptions.Any())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Subscriptions.PdfInvoice.NoSubscriptions"));
                return RedirectToAction("List");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionsToPdf(stream, subscriptions, _subscriptionSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, contributorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "subscriptions.pdf");
        }

        //currently we use this method on the add article to subscription details pages
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult ArticleDetails_AttributeChange(int articleId, bool validateAttributeConditions,
	        FormCollection form)
	    {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = ParseArticleAttributes(article, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _articleAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            return Json(new
            {
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public virtual ActionResult EditCreditCardInfo(int id, SubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            if (subscription.AllowStoringCreditCardNumber)
            {
                string cardType = model.CardType;
                string cardName = model.CardName;
                string cardNumber = model.CardNumber;
                string cardCvv2 = model.CardCvv2;
                string cardExpirationMonth = model.CardExpirationMonth;
                string cardExpirationYear = model.CardExpirationYear;

                subscription.CardType = _encryptionService.EncryptText(cardType);
                subscription.CardName = _encryptionService.EncryptText(cardName);
                subscription.CardNumber = _encryptionService.EncryptText(cardNumber);
                subscription.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                subscription.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                subscription.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                subscription.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                _subscriptionService.UpdateSubscription(subscription);
            }

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            PrepareSubscriptionDetailsModel(model, subscription);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveSubscriptionTotals")]
        public virtual ActionResult EditSubscriptionTotals(int id, SubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            subscription.SubscriptionSubtotalInclTax = model.SubscriptionSubtotalInclTaxValue;
            subscription.SubscriptionSubtotalExclTax = model.SubscriptionSubtotalExclTaxValue;
            subscription.SubscriptionSubTotalDiscountInclTax = model.SubscriptionSubTotalDiscountInclTaxValue;
            subscription.SubscriptionSubTotalDiscountExclTax = model.SubscriptionSubTotalDiscountExclTaxValue;
            subscription.SubscriptionShippingInclTax = model.SubscriptionShippingInclTaxValue;
            subscription.SubscriptionShippingExclTax = model.SubscriptionShippingExclTaxValue;
            subscription.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
            subscription.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
            subscription.TaxRates = model.TaxRatesValue;
            subscription.SubscriptionTax = model.TaxValue;
            subscription.SubscriptionDiscount = model.SubscriptionTotalDiscountValue;
            subscription.SubscriptionTotal = model.SubscriptionTotalValue;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            PrepareSubscriptionDetailsModel(model, subscription);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public virtual ActionResult EditShippingMethod(int id, SubscriptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            subscription.ShippingMethod = model.ShippingMethod;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            PrepareSubscriptionDetailsModel(model, subscription);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveSubscriptionItem")]
        [ValidateInput(false)]
        public virtual ActionResult EditSubscriptionItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            //get subscription item identifier
            int subscriptionItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnSaveSubscriptionItem", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionItemId = Convert.ToInt32(formValue.Substring("btnSaveSubscriptionItem".Length));

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == subscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");


            decimal unitPriceInclTax, unitPriceExclTax, discountInclTax, discountExclTax,priceInclTax,priceExclTax;
            int quantity;
            if (!decimal.TryParse(form["pvUnitPriceInclTax" + subscriptionItemId], out unitPriceInclTax))
                unitPriceInclTax = subscriptionItem.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + subscriptionItemId], out unitPriceExclTax))
                unitPriceExclTax = subscriptionItem.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + subscriptionItemId], out quantity))
                quantity = subscriptionItem.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + subscriptionItemId], out discountInclTax))
                discountInclTax = subscriptionItem.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + subscriptionItemId], out discountExclTax))
                discountExclTax = subscriptionItem.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + subscriptionItemId], out priceInclTax))
                priceInclTax = subscriptionItem.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + subscriptionItemId], out priceExclTax))
                priceExclTax = subscriptionItem.PriceExclTax;

            if (quantity > 0)
            {
                int qtyDifference = subscriptionItem.Quantity - quantity;

                if (!_subscriptionSettings.AutoUpdateSubscriptionTotalsOnEditingSubscription)
                {
                    subscriptionItem.UnitPriceInclTax = unitPriceInclTax;
                    subscriptionItem.UnitPriceExclTax = unitPriceExclTax;
                    subscriptionItem.Quantity = quantity;
                    subscriptionItem.DiscountAmountInclTax = discountInclTax;
                    subscriptionItem.DiscountAmountExclTax = discountExclTax;
                    subscriptionItem.PriceInclTax = priceInclTax;
                    subscriptionItem.PriceExclTax = priceExclTax;
                    _subscriptionService.UpdateSubscription(subscription);
                }

               

            }
            else
            {
              
                //delete item
                _subscriptionService.DeleteSubscriptionItem(subscriptionItem);
            }

            //update subscription totals
            var updateSubscriptionParameters = new UpdateSubscriptionParameters
            {
                UpdatedSubscription = subscription,
                UpdatedSubscriptionItem = subscriptionItem,
                PriceInclTax = unitPriceInclTax,
                PriceExclTax = unitPriceExclTax,
                DiscountAmountInclTax = discountInclTax,
                DiscountAmountExclTax = discountExclTax,
                SubTotalInclTax = priceInclTax,
                SubTotalExclTax = priceExclTax,
                Quantity = quantity
            };
            _subscriptionProcessingService.UpdateSubscriptionTotals(updateSubscriptionParameters);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            var model = new SubscriptionModel();
            PrepareSubscriptionDetailsModel(model, subscription);
            model.Warnings = updateSubscriptionParameters.Warnings;

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteSubscriptionItem")]
        [ValidateInput(false)]
        public virtual ActionResult DeleteSubscriptionItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = id });

            //get subscription item identifier
            int subscriptionItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnDeleteSubscriptionItem", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionItemId = Convert.ToInt32(formValue.Substring("btnDeleteSubscriptionItem".Length));

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == subscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

           
                //delete item
                _subscriptionService.DeleteSubscriptionItem(subscriptionItem);

                //update subscription totals
                var updateSubscriptionParameters = new UpdateSubscriptionParameters
                {
                    UpdatedSubscription = subscription,
                    UpdatedSubscriptionItem = subscriptionItem
                };
                _subscriptionProcessingService.UpdateSubscriptionTotals(updateSubscriptionParameters);



                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = "Subscription item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
                LogEditSubscription(subscription.Id);

                var model = new SubscriptionModel();
                PrepareSubscriptionDetailsModel(model, subscription);
                model.Warnings = updateSubscriptionParameters.Warnings;

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
             
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        [ValidateInput(false)]
        public virtual ActionResult ResetDownloadCount(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //get subscription item identifier
            int subscriptionItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionItemId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == subscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

            //ensure a contributor has access only to his articles 
            if (_workContext.CurrentContributor != null && !HasAccessToSubscriptionItem(subscriptionItem))
                return RedirectToAction("List");

            subscriptionItem.DownloadCount = 0;
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            var model = new SubscriptionModel();
            PrepareSubscriptionDetailsModel(model, subscription);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        [ValidateInput(false)]
        public virtual ActionResult ActivateDownloadItem(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //get subscription item identifier
            int subscriptionItemId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    subscriptionItemId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == subscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

            //ensure a contributor has access only to his articles 
            if (_workContext.CurrentContributor != null && !HasAccessToSubscriptionItem(subscriptionItem))
                return RedirectToAction("List");

            subscriptionItem.IsDownloadActivated = !subscriptionItem.IsDownloadActivated;
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            var model = new SubscriptionModel();
            PrepareSubscriptionDetailsModel(model, subscription);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        public virtual ActionResult UploadLicenseFilePopup(int id, int subscriptionItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(id);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == subscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

           
            //ensure a contributor has access only to his articles 
            if (_workContext.CurrentContributor != null && !HasAccessToSubscriptionItem(subscriptionItem))
                return RedirectToAction("List");

            var model = new SubscriptionModel.UploadLicenseModel
            {
                LicenseDownloadId = subscriptionItem.LicenseDownloadId.HasValue ? subscriptionItem.LicenseDownloadId.Value : 0,
                SubscriptionId = subscription.Id,
                SubscriptionItemId = subscriptionItem.Id
            };

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public virtual ActionResult UploadLicenseFilePopup(string btnId, string formId, SubscriptionModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(model.SubscriptionId);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == model.SubscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

            //ensure a contributor has access only to his articles 
            if (_workContext.CurrentContributor != null && !HasAccessToSubscriptionItem(subscriptionItem))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                subscriptionItem.LicenseDownloadId = model.LicenseDownloadId;
            else
                subscriptionItem.LicenseDownloadId = null;
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public virtual ActionResult DeleteLicenseFilePopup(string btnId, string formId, SubscriptionModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(model.SubscriptionId);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            var subscriptionItem = subscription.SubscriptionItems.FirstOrDefault(x => x.Id == model.SubscriptionItemId);
            if (subscriptionItem == null)
                throw new ArgumentException("No subscription item found with the specified id");

            //ensure a contributor has access only to his articles 
            if (_workContext.CurrentContributor != null && !HasAccessToSubscriptionItem(subscriptionItem))
                return RedirectToAction("List");

            //attach license
            subscriptionItem.LicenseDownloadId = null;
            _subscriptionService.UpdateSubscription(subscription);
            LogEditSubscription(subscription.Id);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        public virtual ActionResult AddArticleToSubscription(int subscriptionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscriptionId });

            var model = new SubscriptionModel.AddSubscriptionArticleModel();
            model.SubscriptionId = subscriptionId;
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //publishers
            model.AvailablePublishers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var publishers = SelectListHelper.GetPublisherList(_publisherService, _cacheManager, true);
            foreach (var m in publishers)
                model.AvailablePublishers.Add(m);

            //article types
            model.AvailableArticleTypes = ArticleType.SimpleArticle.ToSelectList(false).ToList();
            model.AvailableArticleTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult AddArticleToSubscription(DataSourceRequest command, SubscriptionModel.AddSubscriptionArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return Content("");

            var gridModel = new DataSourceResult();
            var articles = _articleService.SearchArticles(categoryIds: new List<int> {model.SearchCategoryId},
                publisherId: model.SearchPublisherId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize,
                showHidden: true);
            gridModel.Data = articles.Select(x =>
            {
                var articleModel = new SubscriptionModel.AddSubscriptionArticleModel.ArticleModel
                {
                    Id = x.Id,
                    Name =  x.Name,
                    Sku = x.Sku,
                };

                return articleModel;
            });
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }

        public virtual ActionResult AddArticleToSubscriptionDetails(int subscriptionId, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscriptionId });

            var model = PrepareAddArticleToSubscriptionModel(subscriptionId, articleId);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult AddArticleToSubscriptionDetails(int subscriptionId, int articleId, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscriptionId });

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            var article = _articleService.GetArticleById(articleId);
            //save subscription item

            //basic properties
            decimal unitPriceInclTax;
            decimal.TryParse(form["UnitPriceInclTax"], out unitPriceInclTax);
            decimal unitPriceExclTax;
            decimal.TryParse(form["UnitPriceExclTax"], out unitPriceExclTax);
            int quantity;
            int.TryParse(form["Quantity"], out quantity);
            decimal priceInclTax;
            decimal.TryParse(form["SubTotalInclTax"], out priceInclTax);
            decimal priceExclTax;
            decimal.TryParse(form["SubTotalExclTax"], out priceExclTax);

            //warnings
            var warnings = new List<string>();

            //attributes
            var attributesXml = ParseArticleAttributes(article, form, warnings);

             

            #region Rental article

            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (article.IsRental)
            {
                ParseRentalDates(form, out rentalStartDate, out rentalEndDate);
            }

            #endregion

            //warnings
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(subscription.Customer, ShoppingCartType.ShoppingCart, article, quantity, attributesXml));
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemGiftCardWarnings(ShoppingCartType.ShoppingCart, article, attributesXml));
            warnings.AddRange(_shoppingCartService.GetRentalArticleWarnings(article, rentalStartDate, rentalEndDate));
            if (!warnings.Any())
            {
                //no errors

                //attributes
                var attributeDescription = _articleAttributeFormatter.FormatAttributes(article, attributesXml, subscription.Customer);

                //save item
                var subscriptionItem = new SubscriptionItem
                {
                    SubscriptionItemGuid = Guid.NewGuid(),
                    Subscription = subscription,
                    ArticleId = article.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    OriginalArticleCost = _priceCalculationService.GetArticleCost(article, attributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributesXml,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate
                };
                subscription.SubscriptionItems.Add(subscriptionItem);
                _subscriptionService.UpdateSubscription(subscription);

               
                //update subscription totals
                var updateSubscriptionParameters = new UpdateSubscriptionParameters
                {
                    UpdatedSubscription = subscription,
                    UpdatedSubscriptionItem = subscriptionItem,
                    PriceInclTax = unitPriceInclTax,
                    PriceExclTax = unitPriceExclTax,
                    SubTotalInclTax = priceInclTax,
                    SubTotalExclTax = priceExclTax,
                    Quantity = quantity
                };
                _subscriptionProcessingService.UpdateSubscriptionTotals(updateSubscriptionParameters);

                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = "A new subscription item has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
                LogEditSubscription(subscription.Id);

                

                //redirect to subscription details page
                TempData["nop.admin.subscription.warnings"] = updateSubscriptionParameters.Warnings;
                return RedirectToAction("Edit", "Subscription", new { id = subscription.Id });
            }
            
            //errors
            var model = PrepareAddArticleToSubscriptionModel(subscription.Id, article.Id);
            model.Warnings.AddRange(warnings);
            return View(model);
        }

        #endregion

        #endregion

        #region Addresses

        public virtual ActionResult AddressEdit(int addressId, int subscriptionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscriptionId });

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new SubscriptionAddressModel();
            model.SubscriptionId = subscriptionId;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;

            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult AddressEdit(SubscriptionAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(model.SubscriptionId);
            if (subscription == null)
                //No subscription found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscription.Id });

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                throw new ArgumentException("No address found with the specified id");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
                LogEditSubscription(subscription.Id);

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, subscriptionId = model.SubscriptionId });
            }

            //If we got this far, something failed, redisplay form
            model.SubscriptionId = subscription.Id;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }

        #endregion
        
         

        #region Subscription notes
        
        [HttpPost]
        public virtual ActionResult SubscriptionNotesSelect(int subscriptionId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return Content("");

            //subscription notes
            var subscriptionNoteModels = new List<SubscriptionModel.SubscriptionNote>();
            foreach (var subscriptionNote in subscription.SubscriptionNotes
                .OrderByDescending(on => on.CreatedOnUtc))
            {
                var download = _downloadService.GetDownloadById(subscriptionNote.DownloadId);
                subscriptionNoteModels.Add(new SubscriptionModel.SubscriptionNote
                {
                    Id = subscriptionNote.Id,
                    SubscriptionId = subscriptionNote.SubscriptionId,
                    DownloadId = subscriptionNote.DownloadId,
                    DownloadGuid = download != null ? download.DownloadGuid : Guid.Empty,
                    DisplayToCustomer = subscriptionNote.DisplayToCustomer,
                    Note = subscriptionNote.FormatSubscriptionNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(subscriptionNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = subscriptionNoteModels,
                Total = subscriptionNoteModels.Count
            };

            return Json(gridModel);
        }
        
        [ValidateInput(false)]
        public virtual ActionResult SubscriptionNoteAdd(int subscriptionId, int downloadId, bool displayToCustomer, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var subscriptionNote = new SubscriptionNote
            {
                DisplayToCustomer = displayToCustomer,
                Note = message,
                DownloadId = downloadId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            subscription.SubscriptionNotes.Add(subscriptionNote);
            _subscriptionService.UpdateSubscription(subscription);

            //new subscription notification
            if (displayToCustomer)
            {
                //email
                _workflowMessageService.SendNewSubscriptionNoteAddedCustomerNotification(
                    subscriptionNote, _workContext.WorkingLanguage.Id);

            }

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult SubscriptionNoteDelete(int id, int subscriptionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null)
                throw new ArgumentException("No subscription found with the specified id");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("Edit", "Subscription", new { id = subscriptionId });

            var subscriptionNote = subscription.SubscriptionNotes.FirstOrDefault(on => on.Id == id);
            if (subscriptionNote == null)
                throw new ArgumentException("No subscription note found with the specified id");
            _subscriptionService.DeleteSubscriptionNote(subscriptionNote);

            return new NullJsonResult();
        }

        #endregion

        #region Reports

        [NonAction]
        protected virtual DataSourceResult GetBestsellersBriefReportModel(int pageIndex,
            int pageSize, int subscriptionBy)
        {
            //a contributor should have access only to his articles
            int contributorId = 0;
            if (_workContext.CurrentContributor != null)
                contributorId = _workContext.CurrentContributor.Id;

            var items = _subscriptionReportService.BestSellersReport(
                contributorId : contributorId,
                subscriptionBy: subscriptionBy,
                pageIndex: pageIndex,
                pageSize: pageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        ArticleId = x.ArticleId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var article = _articleService.GetArticleById(x.ArticleId);
                    if (article != null)
                        m.ArticleName = article.Name;
                    return m;
                }),
                Total = items.TotalCount
            };
            return gridModel;
        }
        public virtual ActionResult BestsellersBriefReportByQuantity()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public virtual ActionResult BestsellersBriefReportByQuantityList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 1);

            return Json(gridModel);
        }
        public virtual ActionResult BestsellersBriefReportByAmount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");
            
            return PartialView();
        }
        [HttpPost]
        public virtual ActionResult BestsellersBriefReportByAmountList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 2);

            return Json(gridModel);
        }



        public virtual ActionResult BestsellersReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var model = new BestsellersReportModel();
            //contributor
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //subscription statuses
            model.AvailableSubscriptionStatuses = SubscriptionStatus.Pending.ToSelectList(false).ToList();
            model.AvailableSubscriptionStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //publishers
            model.AvailablePublishers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var publishers = SelectListHelper.GetPublisherList(_publisherService, _cacheManager, true);
            foreach (var m in publishers)
                model.AvailablePublishers.Add(m);

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //contributors
            model.AvailableContributors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var contributors = SelectListHelper.GetContributorList(_contributorService, _cacheManager, true);
            foreach (var v in contributors)
                model.AvailableContributors.Add(v);

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult BestsellersReportList(DataSourceRequest command, BestsellersReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.ContributorId = _workContext.CurrentContributor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionStatus? subscriptionStatus = model.SubscriptionStatusId > 0 ? (SubscriptionStatus?)(model.SubscriptionStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            var items = _subscriptionReportService.BestSellersReport(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: subscriptionStatus,
                ps: paymentStatus,
                billingCountryId: model.BillingCountryId,
                subscriptionBy: 2,
                contributorId: model.ContributorId,
                categoryId: model.CategoryId,
                publisherId: model.PublisherId,
                storeId: model.StoreId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        ArticleId = x.ArticleId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var article = _articleService.GetArticleById(x.ArticleId);
                    if (article!= null)
                        m.ArticleName = article.Name;
                    return m;
                }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }



        public virtual ActionResult NeverSoldReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedView();

            var model = new NeverSoldReportModel();

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //publishers
            model.AvailablePublishers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var publishers = SelectListHelper.GetPublisherList(_publisherService, _cacheManager, true);
            foreach (var m in publishers)
                model.AvailablePublishers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //contributors
            model.AvailableContributors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var contributors = SelectListHelper.GetContributorList(_contributorService, _cacheManager, true);
            foreach (var v in contributors)
                model.AvailableContributors.Add(v);


            return View(model);
        }
        [HttpPost]
        public virtual ActionResult NeverSoldReportList(DataSourceRequest command, NeverSoldReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            
            var items = _subscriptionReportService.ArticlesNeverSold(contributorId: model.SearchContributorId,
                storeId: model.SearchStoreId,
                categoryId: model.SearchCategoryId,
                publisherId: model.SearchPublisherId,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex : command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                    new NeverSoldReportLineModel
                    {
                        ArticleId = x.Id,
                        ArticleName = x.Name,
                    }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }


        [ChildActionOnly]
        public virtual ActionResult SubscriptionAverageReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public virtual ActionResult SubscriptionAverageReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor doesn't have access to this report
            if (_workContext.CurrentContributor != null)
                return Content("");

            var report = new List<SubscriptionAverageReportLineSummary>();
            report.Add(_subscriptionReportService.SubscriptionAverageReport(0, SubscriptionStatus.Pending));
            report.Add(_subscriptionReportService.SubscriptionAverageReport(0, SubscriptionStatus.Processing));
            report.Add(_subscriptionReportService.SubscriptionAverageReport(0, SubscriptionStatus.Complete));
            report.Add(_subscriptionReportService.SubscriptionAverageReport(0, SubscriptionStatus.Cancelled));
            var model = report.Select(x => new SubscriptionAverageReportLineSummaryModel
            {
                SubscriptionStatus = x.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext),
                SumTodaySubscriptions = _priceFormatter.FormatPrice(x.SumTodaySubscriptions, true, false),
                SumThisWeekSubscriptions = _priceFormatter.FormatPrice(x.SumThisWeekSubscriptions, true, false),
                SumThisMonthSubscriptions = _priceFormatter.FormatPrice(x.SumThisMonthSubscriptions, true, false),
                SumThisYearSubscriptions = _priceFormatter.FormatPrice(x.SumThisYearSubscriptions, true, false),
                SumAllTimeSubscriptions = _priceFormatter.FormatPrice(x.SumAllTimeSubscriptions, true, false),
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }

        [ChildActionOnly]
        public virtual ActionResult SubscriptionIncompleteReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            return PartialView();
        }

        [HttpPost]
        public virtual ActionResult SubscriptionIncompleteReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return AccessDeniedKendoGridJson();

            //a contributor doesn't have access to this report
            if (_workContext.CurrentContributor != null)
                return Content("");

            var model = new List<SubscriptionIncompleteReportLineModel>();
            //not paid
            var subscriptionStatuses = Enum.GetValues(typeof(SubscriptionStatus)).Cast<int>().Where(os => os != (int)SubscriptionStatus.Cancelled).ToList();
            var paymentStatuses = new List<int>() { (int)PaymentStatus.Pending };
            var psPending = _subscriptionReportService.GetSubscriptionAverageReportLine(psIds: paymentStatuses, osIds: subscriptionStatuses);
            model.Add(new SubscriptionIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidSubscriptions"),
                Count = psPending.CountSubscriptions,
                Total = _priceFormatter.FormatPrice(psPending.SumSubscriptions, true, false),
                ViewLink = Url.Action("List", "Subscription", new {
                    subscriptionStatusIds = string.Join(",", subscriptionStatuses),
                    paymentStatusIds = string.Join(",", paymentStatuses) })
            });
            //not shipped
          
            var ssPending = _subscriptionReportService.GetSubscriptionAverageReportLine(osIds: subscriptionStatuses);
            model.Add(new SubscriptionIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedSubscriptions"),
                Count = ssPending.CountSubscriptions,
                Total = _priceFormatter.FormatPrice(ssPending.SumSubscriptions, true, false),
                ViewLink = Url.Action("List", "Subscription", new {
                    subscriptionStatusIds = string.Join(",", subscriptionStatuses),
                     })
            });
            //pending
            subscriptionStatuses = new List<int>() { (int)SubscriptionStatus.Pending };
            var osPending = _subscriptionReportService.GetSubscriptionAverageReportLine(osIds: subscriptionStatuses);
            model.Add(new SubscriptionIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalIncompleteSubscriptions"),
                Count = osPending.CountSubscriptions,
                Total = _priceFormatter.FormatPrice(osPending.SumSubscriptions, true, false),
                ViewLink = Url.Action("List", "Subscription", new { subscriptionStatusIds = string.Join(",", subscriptionStatuses) })
            });

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }
        
        public virtual ActionResult CountryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubscriptionCountryReport))
                return AccessDeniedView();

            var model = new CountryReportModel();

            //subscription statuses
            model.AvailableSubscriptionStatuses = SubscriptionStatus.Pending.ToSelectList(false).ToList();
            model.AvailableSubscriptionStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult CountryReportList(DataSourceRequest command, CountryReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubscriptionCountryReport))
                return AccessDeniedKendoGridJson();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            SubscriptionStatus? subscriptionStatus = model.SubscriptionStatusId > 0 ? (SubscriptionStatus?)(model.SubscriptionStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            var items = _subscriptionReportService.GetCountryReport(
                os: subscriptionStatus,
                ps: paymentStatus,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var country = _countryService.GetCountryById(x.CountryId.HasValue ? x.CountryId.Value : 0);
                    var m = new CountryReportLineModel
                    {
                        CountryName = country != null ? country.Name : "Unknown",
                        SumSubscriptions = _priceFormatter.FormatPrice(x.SumSubscriptions, true, false),
                        TotalSubscriptions = x.TotalSubscriptions,
                    };
                    return m;
                }),
                Total = items.Count
            };

            return Json(gridModel);
        }

        [ChildActionOnly]
	    public virtual ActionResult SubscriptionStatistics()
	    {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            //a contributor doesn't have access to this report
            if (_workContext.CurrentContributor != null)
                return Content("");

            return PartialView();
	    }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult LoadSubscriptionStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            //a contributor doesn't have access to this report
            if (_workContext.CurrentContributor != null)
                return Content("");

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    if (!timeZone.IsInvalidTime(searchYearDateUser))
                    {
                        for (int i = 0; i <= 12; i++)
                        {
                            result.Add(new
                            {
                                date = searchYearDateUser.Date.ToString("Y", culture),
                                value = _subscriptionService.SearchSubscriptions(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchYearDateUser = searchYearDateUser.AddMonths(1);
                        }
                    }
                    break;

                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchMonthDateUser))
                    {
                        for (int i = 0; i <= 30; i++)
                        {
                            result.Add(new
                            {
                                date = searchMonthDateUser.Date.ToString("M", culture),
                                value = _subscriptionService.SearchSubscriptions(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchMonthDateUser = searchMonthDateUser.AddDays(1);
                        }
                    }
                    break;

                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchWeekDateUser))
                    {
                        for (int i = 0; i <= 7; i++)
                        {
                            result.Add(new
                            {
                                date = searchWeekDateUser.Date.ToString("d dddd", culture),
                                value = _subscriptionService.SearchSubscriptions(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchWeekDateUser = searchWeekDateUser.AddDays(1);
                        }
                    }
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public virtual ActionResult LatestSubscriptions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions))
                return Content("");

            return PartialView();
        }

        #endregion

        #region Activity log

        [NonAction]
        protected virtual void LogEditSubscription(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);

            _customerActivityService.InsertActivity("EditSubscription", _localizationService.GetResource("ActivityLog.EditSubscription"), subscription.CustomSubscriptionNumber);
        }

        #endregion
    }
}
