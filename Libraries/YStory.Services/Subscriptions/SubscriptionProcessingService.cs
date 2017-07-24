using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Logging;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Services.Affiliates;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Events;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Messages;
using YStory.Services.Payments;
using YStory.Services.Security;
using YStory.Services.Tax;
using YStory.Services.Contributors;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription processing service
    /// </summary>
    public partial class SubscriptionProcessingService : ISubscriptionProcessingService
    {
        #region Fields
        
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IArticleService _articleService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly ISubscriptionTotalCalculationService _subscriptionTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IArticleAttributeFormatter _articleAttributeFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IContributorService _contributorService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly IAffiliateService _affiliateService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPdfService _pdfService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;

        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomNumberFormatter _customNumberFormatter;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionService">Subscription service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="articleService">Article service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="subscriptionTotalCalculationService">Subscription total calculationservice</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="articleAttributeParser">Article attribute parser</param>
        /// <param name="articleAttributeFormatter">Article attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="shoppingCartService">Shopping cart service</param>
        /// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shipmentService">Shipment service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="contributorService">Contributor service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="affiliateService">Affiliate service</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="pdfService">PDF service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="countryService">Country service</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="subscriptionSettings">Subscription settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="customNumberFormatter">Custom number formatter</param>
        public SubscriptionProcessingService(ISubscriptionService subscriptionService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IArticleService articleService,
            IPaymentService paymentService,
            ILogger logger,
            ISubscriptionTotalCalculationService subscriptionTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IArticleAttributeParser articleAttributeParser,
            IArticleAttributeFormatter articleAttributeFormatter,
            IShoppingCartService shoppingCartService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ITaxService taxService,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IContributorService contributorService,
            ICustomerActivityService customerActivityService,
            ICurrencyService currencyService,
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher,
            IPdfService pdfService,
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            SubscriptionSettings subscriptionSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings,
            ICustomNumberFormatter customNumberFormatter)
        {
            this._subscriptionService = subscriptionService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._articleService = articleService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._subscriptionTotalCalculationService = subscriptionTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._articleAttributeParser = articleAttributeParser;
            this._articleAttributeFormatter = articleAttributeFormatter;
            this._shoppingCartService = shoppingCartService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._contributorService = contributorService;
            this._taxService = taxService;
            this._customerService = customerService;
            this._encryptionService = encryptionService;
            this._customerActivityService = customerActivityService;
            this._currencyService = currencyService;
            this._affiliateService = affiliateService;
            this._eventPublisher = eventPublisher;
            this._pdfService = pdfService;
            this._rewardPointService = rewardPointService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;

            this._paymentSettings = paymentSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._subscriptionSettings = subscriptionSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._currencySettings = currencySettings;
            this._customNumberFormatter = customNumberFormatter;
        }

        #endregion

        #region Nested classes

        protected class PlaceSubscriptionContainter
        {
            public PlaceSubscriptionContainter()
            {
                this.Cart = new List<ShoppingCartItem>();
           
            }

            public Customer Customer { get; set; }
            public Language CustomerLanguage { get; set; }
            public int AffiliateId { get; set; }
            public TaxDisplayType CustomerTaxDisplayType {get; set; }
            public string CustomerCurrencyCode { get; set; }
            public decimal CustomerCurrencyRate { get; set; }

            public Address BillingAddress { get; set; }
            public Address ShippingAddress {get; set; }
             
            public string ShippingMethodName { get; set; }
            public string ShippingRateComputationMethodSystemName { get; set; }
            public bool PickUpInStore { get; set; }
            public Address PickupAddress { get; set; }

            public bool IsRecurringShoppingCart { get; set; }
            //initial subscription (used with recurring payments)
            public Subscription InitialSubscription { get; set; }

            public string CheckoutAttributeDescription { get; set; }
            public string CheckoutAttributesXml { get; set; }

            public IList<ShoppingCartItem> Cart { get; set; }
          

            public decimal SubscriptionSubTotalInclTax { get; set; }
            public decimal SubscriptionSubTotalExclTax { get; set; }
            public decimal SubscriptionSubTotalDiscountInclTax { get; set; }
            public decimal SubscriptionSubTotalDiscountExclTax { get; set; }
            public decimal SubscriptionShippingTotalInclTax { get; set; }
            public decimal SubscriptionShippingTotalExclTax { get; set; }
            public decimal PaymentAdditionalFeeInclTax {get; set; }
            public decimal PaymentAdditionalFeeExclTax { get; set; }
            public decimal SubscriptionTaxTotal  {get; set; }
            public string VatNumber {get; set; }
            public string TaxRates {get; set; }
            public decimal SubscriptionDiscountAmount { get; set; }
            public int RedeemedRewardPoints { get; set; }
            public decimal RedeemedRewardPointsAmount { get; set; }
            public decimal SubscriptionTotal { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare details to place an subscription. It also sets some properties to "processPaymentRequest"
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceSubscriptionContainter PreparePlaceSubscriptionDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceSubscriptionContainter();

            //customer
            details.Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_subscriptionSettings.AnonymousCheckoutAllowed)
                throw new YStoryException("Anonymous checkout is not allowed");

            //customer currency
            var currencyTmp = _currencyService.GetCurrencyById(
                details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, processPaymentRequest.StoreId));
            var customerCurrency = (currencyTmp != null && currencyTmp.Published) ? currencyTmp : _workContext.WorkingCurrency;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
            details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(
                details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, processPaymentRequest.StoreId));
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //billing address
            if (details.Customer.BillingAddress == null)
                throw new YStoryException("Billing address is not provided");

            if (!CommonHelper.IsValidEmail(details.Customer.BillingAddress.Email))
                throw new YStoryException("Email is not valid");

            details.BillingAddress = (Address)details.Customer.BillingAddress.Clone();
            if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                throw new YStoryException(string.Format("Country '{0}' is not allowed for billing", details.BillingAddress.Country.Name));

            //checkout attributes
            details.CheckoutAttributesXml = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, processPaymentRequest.StoreId);
            details.CheckoutAttributeDescription = _checkoutAttributeFormatter.FormatAttributes(details.CheckoutAttributesXml, details.Customer);

            //load shopping cart
            details.Cart = details.Customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(processPaymentRequest.StoreId).ToList();

            if (!details.Cart.Any())
                throw new YStoryException("Cart is empty");

            //validate the entire shopping cart
            var warnings = _shoppingCartService.GetShoppingCartWarnings(details.Cart, details.CheckoutAttributesXml, true);
            if (warnings.Any())
                throw new YStoryException(warnings.Aggregate(string.Empty, (current, next) => string.Format("{0}{1};", current, next)));

            //validate individual cart items
            foreach (var sci in details.Cart)
            {
                var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(details.Customer,
                    sci.ShoppingCartType, sci.Article, processPaymentRequest.StoreId, sci.AttributesXml,
                    sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
                if (sciWarnings.Any())
                    throw new YStoryException(sciWarnings.Aggregate(string.Empty, (current, next) => string.Format("{0}{1};", current, next)));
            }

            //min totals validation
            if (!ValidateMinSubscriptionSubtotalAmount(details.Cart))
            {
                var minSubscriptionSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_subscriptionSettings.MinSubscriptionSubtotalAmount, _workContext.WorkingCurrency);
                throw new YStoryException(string.Format(_localizationService.GetResource("Checkout.MinSubscriptionSubtotalAmount"),
                    _priceFormatter.FormatPrice(minSubscriptionSubtotalAmount, true, false)));
            }

            if (!ValidateMinSubscriptionTotalAmount(details.Cart))
            {
                var minSubscriptionTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_subscriptionSettings.MinSubscriptionTotalAmount, _workContext.WorkingCurrency);
                throw new YStoryException(string.Format(_localizationService.GetResource("Checkout.MinSubscriptionTotalAmount"),
                    _priceFormatter.FormatPrice(minSubscriptionTotalAmount, true, false)));
            }

            //tax display type
            if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                details.CustomerTaxDisplayType = (TaxDisplayType)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, processPaymentRequest.StoreId);
            else
                details.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

            //sub total (incl tax)
            decimal subscriptionSubTotalDiscountAmount;
          
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            _subscriptionTotalCalculationService.GetShoppingCartSubTotal(details.Cart, true, out subscriptionSubTotalDiscountAmount,
                  out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            details.SubscriptionSubTotalInclTax = subTotalWithoutDiscountBase;
            details.SubscriptionSubTotalDiscountInclTax = subscriptionSubTotalDiscountAmount;

            

            //sub total (excl tax)
            _subscriptionTotalCalculationService.GetShoppingCartSubTotal(details.Cart, false, out subscriptionSubTotalDiscountAmount,
                 out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            details.SubscriptionSubTotalExclTax = subTotalWithoutDiscountBase;
            details.SubscriptionSubTotalDiscountExclTax = subscriptionSubTotalDiscountAmount;

            
             
            

            //shipping total
            decimal tax;
           
           
 

          

            //payment total
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(details.Cart, processPaymentRequest.PaymentMethodSystemName);
            details.PaymentAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, details.Customer);
            details.PaymentAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, details.Customer);

            //tax amount
            SortedDictionary<decimal, decimal> taxRatesDictionary;
            details.SubscriptionTaxTotal = _subscriptionTotalCalculationService.GetTaxTotal(details.Cart, out taxRatesDictionary);

            //VAT number
            var customerVatStatus = (VatNumberStatus)details.Customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId);
            if (_taxSettings.EuVatEnabled && customerVatStatus == VatNumberStatus.Valid)
                details.VatNumber = details.Customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

            //tax rates
            details.TaxRates = taxRatesDictionary.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2};   ", current, next.Key.ToString(CultureInfo.InvariantCulture), next.Value.ToString(CultureInfo.InvariantCulture)));

            //subscription total (and applied discounts, gift cards, reward points)
         
            decimal subscriptionDiscountAmount;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            var subscriptionTotal = _subscriptionTotalCalculationService.GetShoppingCartTotal(details.Cart, out subscriptionDiscountAmount,
               out redeemedRewardPoints, out redeemedRewardPointsAmount);
            if (!subscriptionTotal.HasValue)
                throw new YStoryException("Subscription total couldn't be calculated");

            details.SubscriptionDiscountAmount = subscriptionDiscountAmount;
            details.RedeemedRewardPoints = redeemedRewardPoints;
            details.RedeemedRewardPointsAmount = redeemedRewardPointsAmount;
            
            details.SubscriptionTotal = subscriptionTotal.Value;

            

            processPaymentRequest.SubscriptionTotal = details.SubscriptionTotal;

            //recurring or standard shopping cart?
            details.IsRecurringShoppingCart = details.Cart.IsRecurring();
            if (details.IsRecurringShoppingCart)
            {
                int recurringCycleLength;
                RecurringArticleCyclePeriod recurringCyclePeriod;
                int recurringTotalCycles;
                var recurringCyclesError = details.Cart.GetRecurringCycleInfo(_localizationService,
                    out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
                if (!string.IsNullOrEmpty(recurringCyclesError))
                    throw new YStoryException(recurringCyclesError);

                processPaymentRequest.RecurringCycleLength = recurringCycleLength;
                processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
                processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
            }

            return details;
        }

        /// <summary>
        /// Prepare details to place subscription based on the recurring payment.
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceSubscriptionContainter PrepareRecurringSubscriptionDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceSubscriptionContainter();
            details.IsRecurringShoppingCart = true;

            //Load initial subscription
            details.InitialSubscription = _subscriptionService.GetOrderById(processPaymentRequest.InitialSubscriptionId);
            if (details.InitialSubscription == null)
                throw new ArgumentException("Initial subscription is not set for recurring payment");

            processPaymentRequest.PaymentMethodSystemName = details.InitialSubscription.PaymentMethodSystemName;

            //customer
            details.Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_subscriptionSettings.AnonymousCheckoutAllowed)
                throw new YStoryException("Anonymous checkout is not allowed");

            //customer currency
            details.CustomerCurrencyCode = details.InitialSubscription.CustomerCurrencyCode;
            details.CustomerCurrencyRate = details.InitialSubscription.CurrencyRate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(details.InitialSubscription.CustomerLanguageId);
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //billing address
            if (details.InitialSubscription.BillingAddress == null)
                throw new YStoryException("Billing address is not available");
            
            details.BillingAddress = (Address)details.InitialSubscription.BillingAddress.Clone();
            if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                throw new YStoryException(string.Format("Country '{0}' is not allowed for billing", details.BillingAddress.Country.Name));

            //checkout attributes
            details.CheckoutAttributesXml = details.InitialSubscription.CheckoutAttributesXml;
            details.CheckoutAttributeDescription = details.InitialSubscription.CheckoutAttributeDescription;

            //tax display type
            details.CustomerTaxDisplayType = details.InitialSubscription.CustomerTaxDisplayType;

            //sub total
            details.SubscriptionSubTotalInclTax = details.InitialSubscription.SubscriptionSubtotalInclTax;
            details.SubscriptionSubTotalExclTax = details.InitialSubscription.SubscriptionSubtotalExclTax;

           
            

            //shipping total
            details.SubscriptionShippingTotalInclTax = details.InitialSubscription.SubscriptionShippingInclTax;
            details.SubscriptionShippingTotalExclTax = details.InitialSubscription.SubscriptionShippingExclTax;

            //payment total
            details.PaymentAdditionalFeeInclTax = details.InitialSubscription.PaymentMethodAdditionalFeeInclTax;
            details.PaymentAdditionalFeeExclTax = details.InitialSubscription.PaymentMethodAdditionalFeeExclTax;

            //tax total
            details.SubscriptionTaxTotal = details.InitialSubscription.SubscriptionTax;

            //VAT number
            details.VatNumber = details.InitialSubscription.VatNumber;

            //subscription total
            details.SubscriptionDiscountAmount = details.InitialSubscription.SubscriptionDiscount;
            details.SubscriptionTotal = details.InitialSubscription.SubscriptionTotal;
            processPaymentRequest.SubscriptionTotal = details.SubscriptionTotal;

            return details;
        }

        /// <summary>
        /// Save subscription and add subscription notes
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="processPaymentResult">Process payment result</param>
        /// <param name="details">Details</param>
        /// <returns>Subscription</returns>
        protected virtual Subscription SaveSubscriptionDetails(ProcessPaymentRequest processPaymentRequest, 
            ProcessPaymentResult processPaymentResult, PlaceSubscriptionContainter details)
        {
            var subscription = new Subscription
            {
                StoreId = processPaymentRequest.StoreId,
                SubscriptionGuid = processPaymentRequest.SubscriptionGuid,
                CustomerId = details.Customer.Id,
                CustomerLanguageId = details.CustomerLanguage.Id,
                CustomerTaxDisplayType = details.CustomerTaxDisplayType,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                SubscriptionSubtotalInclTax = details.SubscriptionSubTotalInclTax,
                SubscriptionSubtotalExclTax = details.SubscriptionSubTotalExclTax,
                SubscriptionSubTotalDiscountInclTax = details.SubscriptionSubTotalDiscountInclTax,
                SubscriptionSubTotalDiscountExclTax = details.SubscriptionSubTotalDiscountExclTax,
                SubscriptionShippingInclTax = details.SubscriptionShippingTotalInclTax,
                SubscriptionShippingExclTax = details.SubscriptionShippingTotalExclTax,
                PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
                TaxRates = details.TaxRates,
                SubscriptionTax = details.SubscriptionTaxTotal,
                SubscriptionTotal = details.SubscriptionTotal,
                RefundedAmount = decimal.Zero,
                SubscriptionDiscount = details.SubscriptionDiscountAmount,
                CheckoutAttributeDescription = details.CheckoutAttributeDescription,
                CheckoutAttributesXml = details.CheckoutAttributesXml,
                CustomerCurrencyCode = details.CustomerCurrencyCode,
                CurrencyRate = details.CustomerCurrencyRate,
                AffiliateId = details.AffiliateId,
                SubscriptionStatus = SubscriptionStatus.Pending,
                AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
                CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
                CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
                MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
                CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
                CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
                CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
                PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                PaymentStatus = processPaymentResult.NewPaymentStatus,
                PaidDateUtc = null,
                BillingAddress = details.BillingAddress,
                ShippingAddress = details.ShippingAddress,
                
                ShippingMethod = details.ShippingMethodName,
                PickUpInStore = details.PickUpInStore,
                PickupAddress = details.PickupAddress,
                ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
                VatNumber = details.VatNumber,
                CreatedOnUtc = DateTime.UtcNow,
                CustomSubscriptionNumber = string.Empty
            };

            _subscriptionService.InsertSubscription(subscription);

            //generate and set custom subscription number
            subscription.CustomSubscriptionNumber = _customNumberFormatter.GenerateSubscriptionCustomNumber(subscription);
            _subscriptionService.UpdateSubscription(subscription);

            //reward points history
            if (details.RedeemedRewardPointsAmount > decimal.Zero)
            {
                _rewardPointService.AddRewardPointsHistoryEntry(details.Customer, -details.RedeemedRewardPoints, subscription.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForSubscription", subscription.CustomerLanguageId), subscription.CustomSubscriptionNumber),
                    subscription, details.RedeemedRewardPointsAmount);
                _customerService.UpdateCustomer(details.Customer);
            }

            return subscription;
        }

        /// <summary>
        /// Send "subscription placed" notifications and save subscription notes
        /// </summary>
        /// <param name="subscription">Subscription</param>
        protected virtual void SendNotificationsAndSaveNotes(Subscription subscription)
        {
            //notes, messages
            if (_workContext.OriginalCustomerIfImpersonated != null)
                //this subscription is placed by a store administrator impersonating a customer
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Subscription placed by a store owner ('{0}'. ID = {1}) impersonating the customer.",
                        _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            else
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = "Subscription placed",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            _subscriptionService.UpdateSubscription(subscription);

            //send email notifications
            var subscriptionPlacedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionPlacedStoreOwnerNotification(subscription, _localizationSettings.DefaultAdminLanguageId);
            if (subscriptionPlacedStoreOwnerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription placed\" email (to store owner) has been queued. Queued email identifier: {0}.", subscriptionPlacedStoreOwnerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }

            var subscriptionPlacedAttachmentFilePath = _subscriptionSettings.AttachPdfInvoiceToSubscriptionPlacedEmail ?
                _pdfService.PrintSubscriptionToPdf(subscription) : null;
            var subscriptionPlacedAttachmentFileName = _subscriptionSettings.AttachPdfInvoiceToSubscriptionPlacedEmail ?
                "subscription.pdf" : null;
            var subscriptionPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
                .SendSubscriptionPlacedCustomerNotification(subscription, subscription.CustomerLanguageId, subscriptionPlacedAttachmentFilePath, subscriptionPlacedAttachmentFileName);
            if (subscriptionPlacedCustomerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription placed\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionPlacedCustomerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }

            var contributors = GetContributorsInSubscription(subscription);
            foreach (var contributor in contributors)
            {
                var subscriptionPlacedContributorNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionPlacedContributorNotification(subscription, contributor, _localizationSettings.DefaultAdminLanguageId);
                if (subscriptionPlacedContributorNotificationQueuedEmailId > 0)
                {
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = string.Format("\"Subscription placed\" email (to contributor) has been queued. Queued email identifier: {0}.", subscriptionPlacedContributorNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);
                }
            }
        }
        /// <summary>
        /// Award (earn) reward points (for placing a new subscription)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        protected virtual void AwardRewardPoints(Subscription subscription)
        {
            var totalForRewardPoints = _subscriptionTotalCalculationService.CalculateApplicableSubscriptionTotalForRewardPoints(subscription.SubscriptionShippingInclTax, subscription.SubscriptionTotal);
            int points = _subscriptionTotalCalculationService.CalculateRewardPoints(subscription.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //Ensure that reward points were not added (earned) before. We should not add reward points if they were already earned for this subscription
            if (subscription.RewardPointsHistoryEntryId.HasValue)
                return;

            //check whether delay is set
            DateTime? activatingDate = null;
            if (_rewardPointsSettings.ActivationDelay > 0)
            {
                var delayPeriod = (RewardPointsActivatingDelayPeriod)_rewardPointsSettings.ActivationDelayPeriodId;
                var delayInHours = delayPeriod.ToHours(_rewardPointsSettings.ActivationDelay);
                activatingDate = DateTime.UtcNow.AddHours(delayInHours);
            }

            //add reward points
            subscription.RewardPointsHistoryEntryId = _rewardPointService.AddRewardPointsHistoryEntry(subscription.Customer, points, subscription.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForSubscription"), subscription.CustomSubscriptionNumber), activatingDate: activatingDate);

            _subscriptionService.UpdateSubscription(subscription);
        }

        /// <summary>
        /// Reduce (cancel) reward points (previously awarded for placing an subscription)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        protected virtual void ReduceRewardPoints(Subscription subscription)
        {
            var totalForRewardPoints = _subscriptionTotalCalculationService.CalculateApplicableSubscriptionTotalForRewardPoints(subscription.SubscriptionShippingInclTax, subscription.SubscriptionTotal);
            int points = _subscriptionTotalCalculationService.CalculateRewardPoints(subscription.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //ensure that reward points were already earned for this subscription before
            if (!subscription.RewardPointsHistoryEntryId.HasValue)
                return;

            //get appropriate history entry
            var rewardPointsHistoryEntry = _rewardPointService.GetRewardPointsHistoryEntryById(subscription.RewardPointsHistoryEntryId.Value);
            if (rewardPointsHistoryEntry != null && rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                _rewardPointService.DeleteRewardPointsHistoryEntry(rewardPointsHistoryEntry);
            }
            else
            {
                //or reduce reward points if the entry already exists
                _rewardPointService.AddRewardPointsHistoryEntry(subscription.Customer, -points, subscription.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForSubscription"), subscription.CustomSubscriptionNumber));
            }

            _subscriptionService.UpdateSubscription(subscription);
        }

        /// <summary>
        /// Return back redeemded reward points to a customer (spent when placing an subscription)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        protected virtual void ReturnBackRedeemedRewardPoints(Subscription subscription)
        {
            //were some points redeemed when placing an subscription?
            if (subscription.RedeemedRewardPointsEntry == null)
                return;

            //return back
            _rewardPointService.AddRewardPointsHistoryEntry(subscription.Customer, -subscription.RedeemedRewardPointsEntry.Points, subscription.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForSubscription"), subscription.CustomSubscriptionNumber));
            _subscriptionService.UpdateSubscription(subscription);
        }


       
        /// <summary>
        /// Sets an subscription status
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="os">New subscription status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected virtual void SetSubscriptionStatus(Subscription subscription, SubscriptionStatus os, bool notifyCustomer)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            SubscriptionStatus prevSubscriptionStatus = subscription.SubscriptionStatus;
            if (prevSubscriptionStatus == os)
                return;

            //set and save new subscription status
            subscription.SubscriptionStatusId = (int)os;
            _subscriptionService.UpdateSubscription(subscription);

            //subscription notes, notifications
            subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Subscription status has been changed to {0}", os.ToString()),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            _subscriptionService.UpdateSubscription(subscription);


            if (prevSubscriptionStatus != SubscriptionStatus.Complete &&
                os == SubscriptionStatus.Complete
                && notifyCustomer)
            {
                //notification
                var subscriptionCompletedAttachmentFilePath = _subscriptionSettings.AttachPdfInvoiceToSubscriptionCompletedEmail ?
                    _pdfService.PrintSubscriptionToPdf(subscription) : null;
                var subscriptionCompletedAttachmentFileName = _subscriptionSettings.AttachPdfInvoiceToSubscriptionCompletedEmail ?
                    "subscription.pdf" : null;
                int subscriptionCompletedCustomerNotificationQueuedEmailId = _workflowMessageService
                    .SendSubscriptionCompletedCustomerNotification(subscription, subscription.CustomerLanguageId, subscriptionCompletedAttachmentFilePath,
                    subscriptionCompletedAttachmentFileName);
                if (subscriptionCompletedCustomerNotificationQueuedEmailId > 0)
                {
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = string.Format("\"Subscription completed\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionCompletedCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);
                }
            }

            if (prevSubscriptionStatus != SubscriptionStatus.Cancelled &&
                os == SubscriptionStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                int subscriptionCancelledCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionCancelledCustomerNotification(subscription, subscription.CustomerLanguageId);
                if (subscriptionCancelledCustomerNotificationQueuedEmailId > 0)
                {
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = string.Format("\"Subscription cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionCancelledCustomerNotificationQueuedEmailId),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);
                }
            }

            //reward points
            if (subscription.SubscriptionStatus == SubscriptionStatus.Complete)
            {
                AwardRewardPoints(subscription);
            }
            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            {
                ReduceRewardPoints(subscription);
            }

             
        }

        /// <summary>
        /// Process subscription paid status
        /// </summary>
        /// <param name="subscription">Subscription</param>
        protected virtual void ProcessSubscriptionPaid(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            //raise event
            _eventPublisher.Publish(new SubscriptionPaidEvent(subscription));

            //subscription paid email notification
            if (subscription.SubscriptionTotal != decimal.Zero)
            {
                //we should not send it for free ($0 total) subscriptions?
                //remove this "if" statement if you want to send it in this case

                var subscriptionPaidAttachmentFilePath = _subscriptionSettings.AttachPdfInvoiceToSubscriptionPaidEmail ?
                    _pdfService.PrintSubscriptionToPdf(subscription) : null;
                var subscriptionPaidAttachmentFileName = _subscriptionSettings.AttachPdfInvoiceToSubscriptionPaidEmail ?
                    "subscription.pdf" : null;
                _workflowMessageService.SendSubscriptionPaidCustomerNotification(subscription, subscription.CustomerLanguageId,
                    subscriptionPaidAttachmentFilePath, subscriptionPaidAttachmentFileName);

                _workflowMessageService.SendSubscriptionPaidStoreOwnerNotification(subscription, _localizationSettings.DefaultAdminLanguageId);
                var contributors = GetContributorsInSubscription(subscription);
                foreach (var contributor in contributors)
                {
                    _workflowMessageService.SendSubscriptionPaidContributorNotification(subscription, contributor, _localizationSettings.DefaultAdminLanguageId);
                }
                //TODO add "subscription paid email sent" subscription note
            }

            //customer roles with "purchased with article" specified
            ProcessCustomerRolesWithPurchasedArticleSpecified(subscription, true);
        }

        /// <summary>
        /// Process customer roles with "Purchased with Article" property configured
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
        protected virtual void ProcessCustomerRolesWithPurchasedArticleSpecified(Subscription subscription, bool add)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            //purchased article identifiers
            var purchasedArticleIds = new List<int>();
            foreach (var subscriptionItem in subscription.SubscriptionItems)
            {
                //standard items
                purchasedArticleIds.Add(subscriptionItem.ArticleId);

                //bundled (associated) articles
                var attributeValues = _articleAttributeParser.ParseArticleAttributeValues(subscriptionItem.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToArticle)
                    {
                       purchasedArticleIds.Add(attributeValue.AssociatedArticleId);
                    }
                }
            }

            //list of customer roles
            var customerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Where(cr => purchasedArticleIds.Contains(cr.PurchasedWithArticleId))
                .ToList();

            if (customerRoles.Any())
            {
                var customer = subscription.Customer;
                foreach (var customerRole in customerRoles)
                {
                    if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) == 0)
                    {
                        //not in the list yet
                        if (add)
                        {
                            //add
                            customer.CustomerRoles.Add(customerRole);
                        }
                    }
                    else
                    {
                        //already in the list
                        if (!add)
                        {
                            //remove
                            customer.CustomerRoles.Remove(customerRole);
                        }
                    }
                }
                _customerService.UpdateCustomer(customer);
            }
        }

        /// <summary>
        /// Get a list of contributors in subscription (subscription items)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Contributors</returns>
        protected virtual IList<Contributor> GetContributorsInSubscription(Subscription subscription)
        {
            var contributors = new List<Contributor>();
            foreach (var subscriptionItem in subscription.SubscriptionItems)
            {
                var contributorId = subscriptionItem.Article.ContributorId;
                //find existing
                var contributor = contributors.FirstOrDefault(v => v.Id == contributorId);
                if (contributor == null)
                {
                    //not found. load by Id
                    contributor = _contributorService.GetContributorById(contributorId);
                    if (contributor != null && !contributor.Deleted && contributor.Active)
                    {
                        contributors.Add(contributor);
                    }
                }
            }

            return contributors;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks subscription status
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Validated subscription</returns>
        public virtual void CheckSubscriptionStatus(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.PaymentStatus == PaymentStatus.Paid && !subscription.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                subscription.PaidDateUtc = DateTime.UtcNow;
                _subscriptionService.UpdateSubscription(subscription);
            }

            if (subscription.SubscriptionStatus == SubscriptionStatus.Pending)
            {
                if (subscription.PaymentStatus == PaymentStatus.Authorized ||
                    subscription.PaymentStatus == PaymentStatus.Paid)
                {
                    SetSubscriptionStatus(subscription, SubscriptionStatus.Processing, false);
                }
            }

          

            //is subscription complete?
            if (subscription.SubscriptionStatus != SubscriptionStatus.Cancelled &&
                subscription.SubscriptionStatus != SubscriptionStatus.Complete)
            {
                if (subscription.PaymentStatus == PaymentStatus.Paid)
                {
                    
                        SetSubscriptionStatus(subscription, SubscriptionStatus.Complete, true);
                }
            }
        }

        /// <summary>
        /// Places an subscription
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place subscription result</returns>
        public virtual PlaceSubscriptionResult PlaceSubscription(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException("processPaymentRequest");

            var result = new PlaceSubscriptionResult();
            try
            {
                if (processPaymentRequest.SubscriptionGuid == Guid.Empty)
                    processPaymentRequest.SubscriptionGuid = Guid.NewGuid();

                //prepare subscription details
                var details = PreparePlaceSubscriptionDetails(processPaymentRequest);

                #region Payment workflow


                //process payment
                ProcessPaymentResult processPaymentResult = null;
                //skip payment workflow if subscription total equals zero
                var skipPaymentWorkflow = details.SubscriptionTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        throw new YStoryException("Payment method couldn't be loaded");

                    //ensure that payment method is active
                    if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                        throw new YStoryException("Payment method is not active");

                    if (details.IsRecurringShoppingCart)
                    {
                        //recurring cart
                        switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                        {
                            case RecurringPaymentType.NotSupported:
                                throw new YStoryException("Recurring payments are not supported by selected payment method");
                            case RecurringPaymentType.Manual:
                            case RecurringPaymentType.Automatic:
                                processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                                break;
                            default:
                                throw new YStoryException("Not supported recurring payment type");
                        }
                    }
                    else
                        //standard cart
                        processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
                }
                else
                    //payment is not required
                    processPaymentResult = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

                if (processPaymentResult == null)
                    throw new YStoryException("processPaymentResult is not available");

                #endregion

                if (processPaymentResult.Success)
                {
                    #region Save subscription details

                    var subscription = SaveSubscriptionDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedSubscription = subscription;

                    //move shopping cart items to subscription items
                    foreach (var sc in details.Cart)
                    {
                        //prices
                        decimal taxRate;
                        decimal discountAmount;
                        int? maximumDiscountQty;
                        var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                        var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out discountAmount,   out maximumDiscountQty);
                        var scUnitPriceInclTax = _taxService.GetArticlePrice(sc.Article, scUnitPrice, true, details.Customer, out taxRate);
                        var scUnitPriceExclTax = _taxService.GetArticlePrice(sc.Article, scUnitPrice, false, details.Customer, out taxRate);
                        var scSubTotalInclTax = _taxService.GetArticlePrice(sc.Article, scSubTotal, true, details.Customer, out taxRate);
                        var scSubTotalExclTax = _taxService.GetArticlePrice(sc.Article, scSubTotal, false, details.Customer, out taxRate);
                        var discountAmountInclTax = _taxService.GetArticlePrice(sc.Article, discountAmount, true, details.Customer, out taxRate);
                        var discountAmountExclTax = _taxService.GetArticlePrice(sc.Article, discountAmount, false, details.Customer, out taxRate);
                         

                        //attributes
                        var attributeDescription = _articleAttributeFormatter.FormatAttributes(sc.Article, sc.AttributesXml, details.Customer);

                      

                        //save subscription item
                        var subscriptionItem = new SubscriptionItem
                        {
                            SubscriptionItemGuid = Guid.NewGuid(),
                            Subscription = subscription,
                            ArticleId = sc.ArticleId,
                            UnitPriceInclTax = scUnitPriceInclTax,
                            UnitPriceExclTax = scUnitPriceExclTax,
                            PriceInclTax = scSubTotalInclTax,
                            PriceExclTax = scSubTotalExclTax,
                            OriginalArticleCost = _priceCalculationService.GetArticleCost(sc.Article, sc.AttributesXml),
                            AttributeDescription = attributeDescription,
                            AttributesXml = sc.AttributesXml,
                            Quantity = sc.Quantity,
                            DiscountAmountInclTax = discountAmountInclTax,
                            DiscountAmountExclTax = discountAmountExclTax,
                            DownloadCount = 0,
                            IsDownloadActivated = false,
                            LicenseDownloadId = 0,
                            ItemWeight = 0,
                            RentalStartDateUtc = sc.RentalStartDateUtc,
                            RentalEndDateUtc = sc.RentalEndDateUtc
                        };
                        subscription.SubscriptionItems.Add(subscriptionItem);
                        _subscriptionService.UpdateSubscription(subscription);
                         
                      
                    }

                    //clear shopping cart
                    details.Cart.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
                     

                    //recurring subscriptions
                    if (details.IsRecurringShoppingCart)
                    {
                        //create recurring payment (the first payment)
                        var rp = new RecurringPayment
                        {
                            CycleLength = processPaymentRequest.RecurringCycleLength,
                            CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
                            TotalCycles = processPaymentRequest.RecurringTotalCycles,
                            StartDateUtc = DateTime.UtcNow,
                            IsActive = true,
                            CreatedOnUtc = DateTime.UtcNow,
                            InitialSubscription = subscription,
                        };
                        _subscriptionService.InsertRecurringPayment(rp);

                        switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                        {
                            case RecurringPaymentType.NotSupported:
                                //not supported
                                break;
                            case RecurringPaymentType.Manual:
                                rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                                {
                                    RecurringPayment = rp,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    SubscriptionId = subscription.Id,
                                });
                                _subscriptionService.UpdateRecurringPayment(rp);
                                break;
                            case RecurringPaymentType.Automatic:
                                //will be created later (process is automated)
                                break;
                            default:
                                break;
                        }
                    }

                    #endregion

                    //notifications
                    SendNotificationsAndSaveNotes(subscription);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceSubscription", _localizationService.GetResource("ActivityLog.PublicStore.PlaceSubscription"), subscription.Id);

                    //check subscription status
                    CheckSubscriptionStatus(subscription);

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionPlacedEvent(subscription));

                    if (subscription.PaymentStatus == PaymentStatus.Paid)
                        ProcessSubscriptionPaid(subscription);
                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            #region Process errors

            if (!result.Success)
            {
                //log errors
                var logError = result.Errors.Aggregate("Error while placing subscription. ",
                    (current, next) => string.Format("{0}Error {1}: {2}. ", current, result.Errors.IndexOf(next) + 1, next));
                var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                _logger.Error(logError, customer: customer);
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Update subscription totals
        /// </summary>
        /// <param name="updateSubscriptionParameters">Parameters for the updating subscription</param>
        public virtual void UpdateSubscriptionTotals(UpdateSubscriptionParameters updateSubscriptionParameters)
        {
            if (!_subscriptionSettings.AutoUpdateSubscriptionTotalsOnEditingSubscription)
                return;

            var updatedSubscription = updateSubscriptionParameters.UpdatedSubscription;
            var updatedSubscriptionItem = updateSubscriptionParameters.UpdatedSubscriptionItem;

            //restore shopping cart from subscription items
            var restoredCart = updatedSubscription.SubscriptionItems.Select(subscriptionItem => new ShoppingCartItem
            {
                Id = subscriptionItem.Id,
                AttributesXml = subscriptionItem.AttributesXml,
                Customer = updatedSubscription.Customer,
                Article = subscriptionItem.Article,
                Quantity = subscriptionItem.Id == updatedSubscriptionItem.Id ? updateSubscriptionParameters.Quantity : subscriptionItem.Quantity,
                RentalEndDateUtc = subscriptionItem.RentalEndDateUtc,
                RentalStartDateUtc = subscriptionItem.RentalStartDateUtc,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = updatedSubscription.StoreId
            }).ToList();

            //get shopping cart item which has been updated
            var updatedShoppingCartItem = restoredCart.FirstOrDefault(shoppingCartItem => shoppingCartItem.Id == updatedSubscriptionItem.Id);
            var itemDeleted = updatedShoppingCartItem == null;

            //validate shopping cart for warnings
            updateSubscriptionParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartWarnings(restoredCart, string.Empty, false));
            if (!itemDeleted)
                updateSubscriptionParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartItemWarnings(updatedSubscription.Customer, updatedShoppingCartItem.ShoppingCartType,
                    updatedShoppingCartItem.Article, updatedSubscription.StoreId, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.CustomerEnteredPrice,
                    updatedShoppingCartItem.RentalStartDateUtc, updatedShoppingCartItem.RentalEndDateUtc, updatedShoppingCartItem.Quantity, false));

            _subscriptionTotalCalculationService.UpdateSubscriptionTotals(updateSubscriptionParameters, restoredCart);

            

            if (!itemDeleted)
            {
                updatedSubscriptionItem.ItemWeight = 0;
                updatedSubscriptionItem.OriginalArticleCost = _priceCalculationService.GetArticleCost(updatedShoppingCartItem.Article, updatedShoppingCartItem.AttributesXml);
                updatedSubscriptionItem.AttributeDescription = _articleAttributeFormatter.FormatAttributes(updatedShoppingCartItem.Article,
                    updatedShoppingCartItem.AttributesXml, updatedSubscription.Customer);
 
            }

            _subscriptionService.UpdateSubscription(updatedSubscription);
 
            

            CheckSubscriptionStatus(updatedSubscription);
        }

        /// <summary>
        /// Deletes an subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        public virtual void DeleteSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            //check whether the subscription wasn't cancelled before
            //if it already was cancelled, then there's no need to make the following adjustments
            //(such as reward points, inventory, recurring payments)
            //they already was done when cancelling the subscription
            if (subscription.SubscriptionStatus != SubscriptionStatus.Cancelled)
            {
                //return (add) back redeemded reward points
                ReturnBackRedeemedRewardPoints(subscription);
                //reduce (cancel) back reward points (previously awarded for this subscription)
                ReduceRewardPoints(subscription);

                //cancel recurring payments
                var recurringPayments = _subscriptionService.SearchRecurringPayments(initialSubscriptionId: subscription.Id);
                foreach (var rp in recurringPayments)
                {
                    var errors = CancelRecurringPayment(rp);
                    //use "errors" variable?
                }

                 

            }

            
            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);
            
            //now delete an subscription
            _subscriptionService.DeleteSubscription(subscription);
        }

        /// <summary>
        /// Process next recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
        /// <returns>Collection of errors</returns>
        public virtual IEnumerable<string> ProcessNextRecurringPayment(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            try
            {
                if (!recurringPayment.IsActive)
                    throw new YStoryException("Recurring payment is not active");

                var initialSubscription = recurringPayment.InitialSubscription;
                if (initialSubscription == null)
                    throw new YStoryException("Initial subscription could not be loaded");

                var customer = initialSubscription.Customer;
                if (customer == null)
                    throw new YStoryException("Customer could not be loaded");

                var nextPaymentDate = recurringPayment.NextPaymentDate;
                if (!nextPaymentDate.HasValue)
                    throw new YStoryException("Next payment date could not be calculated");

                //payment info
                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = initialSubscription.StoreId,
                    CustomerId = customer.Id,
                    SubscriptionGuid = Guid.NewGuid(),
                    InitialSubscriptionId = initialSubscription.Id,
                    RecurringCycleLength = recurringPayment.CycleLength,
                    RecurringCyclePeriod = recurringPayment.CyclePeriod,
                    RecurringTotalCycles = recurringPayment.TotalCycles,
                    CustomValues = initialSubscription.DeserializeCustomValues()
                };

                //prepare subscription details
                var details = PrepareRecurringSubscriptionDetails(processPaymentRequest);

                ProcessPaymentResult processPaymentResult;
                //skip payment workflow if subscription total equals zero
                var skipPaymentWorkflow = details.SubscriptionTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        throw new YStoryException("Payment method couldn't be loaded");

                    if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
                        throw new YStoryException("Payment method is not active");

                    //Old credit card info
                    if (details.InitialSubscription.AllowStoringCreditCardNumber)
                    {
                        processPaymentRequest.CreditCardType = _encryptionService.DecryptText(details.InitialSubscription.CardType);
                        processPaymentRequest.CreditCardName = _encryptionService.DecryptText(details.InitialSubscription.CardName);
                        processPaymentRequest.CreditCardNumber = _encryptionService.DecryptText(details.InitialSubscription.CardNumber);
                        processPaymentRequest.CreditCardCvv2 = _encryptionService.DecryptText(details.InitialSubscription.CardCvv2);
                        try
                        {
                            processPaymentRequest.CreditCardExpireMonth = Convert.ToInt32(_encryptionService.DecryptText(details.InitialSubscription.CardExpirationMonth));
                            processPaymentRequest.CreditCardExpireYear = Convert.ToInt32(_encryptionService.DecryptText(details.InitialSubscription.CardExpirationYear));
                        }
                        catch { }
                    }

                    //payment type
                    switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                    {
                        case RecurringPaymentType.NotSupported:
                            throw new YStoryException("Recurring payments are not supported by selected payment method");
                        case RecurringPaymentType.Manual:
                            processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                            break;
                        case RecurringPaymentType.Automatic:
                            //payment is processed on payment gateway site, info about last transaction in paymentResult parameter
                            processPaymentResult = paymentResult ?? new ProcessPaymentResult();
                            break;
                        default:
                            throw new YStoryException("Not supported recurring payment type");
                    }
                }
                else
                    processPaymentResult = paymentResult ?? new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

                if (processPaymentResult == null)
                    throw new YStoryException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    //save subscription details
                    var subscription = SaveSubscriptionDetails(processPaymentRequest, processPaymentResult, details);

                    foreach (var subscriptionItem in details.InitialSubscription.SubscriptionItems)
                    {
                        //save item
                        var newSubscriptionItem = new SubscriptionItem
                        {
                            SubscriptionItemGuid = Guid.NewGuid(),
                            Subscription = subscription,
                            ArticleId = subscriptionItem.ArticleId,
                            UnitPriceInclTax = subscriptionItem.UnitPriceInclTax,
                            UnitPriceExclTax = subscriptionItem.UnitPriceExclTax,
                            PriceInclTax = subscriptionItem.PriceInclTax,
                            PriceExclTax = subscriptionItem.PriceExclTax,
                            OriginalArticleCost = subscriptionItem.OriginalArticleCost,
                            AttributeDescription = subscriptionItem.AttributeDescription,
                            AttributesXml = subscriptionItem.AttributesXml,
                            Quantity = subscriptionItem.Quantity,
                            DiscountAmountInclTax = subscriptionItem.DiscountAmountInclTax,
                            DiscountAmountExclTax = subscriptionItem.DiscountAmountExclTax,
                            DownloadCount = 0,
                            IsDownloadActivated = false,
                            LicenseDownloadId = 0,
                            ItemWeight = subscriptionItem.ItemWeight,
                            RentalStartDateUtc = subscriptionItem.RentalStartDateUtc,
                            RentalEndDateUtc = subscriptionItem.RentalEndDateUtc
                        };
                        subscription.SubscriptionItems.Add(newSubscriptionItem);
                        _subscriptionService.UpdateSubscription(subscription);
                    }

                    //notifications
                    SendNotificationsAndSaveNotes(subscription);

                    //check subscription status
                    CheckSubscriptionStatus(subscription);

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionPlacedEvent(subscription));

                    if (subscription.PaymentStatus == PaymentStatus.Paid)
                        ProcessSubscriptionPaid(subscription);

                    //last payment succeeded
                    recurringPayment.LastPaymentFailed = false;

                    //next recurring payment
                    recurringPayment.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                    {
                        RecurringPayment = recurringPayment,
                        CreatedOnUtc = DateTime.UtcNow,
                        SubscriptionId = subscription.Id,
                    });
                    _subscriptionService.UpdateRecurringPayment(recurringPayment);

                    return new List<string>();
                }
                else
                {
                    //log errors
                    var logError = processPaymentResult.Errors.Aggregate("Error while processing recurring subscription. ",
                        (current, next) => string.Format("{0}Error {1}: {2}. ", current, processPaymentResult.Errors.IndexOf(next) + 1, next));
                    _logger.Error(logError, customer: customer);

                    if (processPaymentResult.RecurringPaymentFailed)
                    {
                        //set flag that last payment failed
                        recurringPayment.LastPaymentFailed = true;
                        _subscriptionService.UpdateRecurringPayment(recurringPayment);

                        if (_paymentSettings.CancelRecurringPaymentsAfterFailedPayment)
                        {
                            //cancel recurring payment
                            CancelRecurringPayment(recurringPayment).ToList().ForEach(error => _logger.Error(error));

                            //notify a customer about cancelled payment
                            _workflowMessageService.SendRecurringPaymentCancelledCustomerNotification(recurringPayment, initialSubscription.CustomerLanguageId);
                        }
                        else
                            //notify a customer about failed payment
                            _workflowMessageService.SendRecurringPaymentFailedCustomerNotification(recurringPayment, initialSubscription.CustomerLanguageId);
                    }

                    return processPaymentResult.Errors;
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error while processing recurring subscription. {0}", exc.Message), exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual IList<string> CancelRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var initialSubscription = recurringPayment.InitialSubscription;
            if (initialSubscription == null)
                return new List<string> { "Initial subscription could not be loaded" };


            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.Subscription = initialSubscription;
                result = _paymentService.CancelRecurringPayment(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    _subscriptionService.UpdateRecurringPayment(recurringPayment);


                    //add a note
                    initialSubscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(initialSubscription);

                    //notify a store owner
                    _workflowMessageService
                        .SendRecurringPaymentCancelledStoreOwnerNotification(recurringPayment, 
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CancelRecurringPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                initialSubscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Unable to cancel recurring payment. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(initialSubscription);

                //log it
                string logError = string.Format("Error cancelling recurring payment. Subscription #{0}. Error: {1}", initialSubscription.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public virtual bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialSubscription = recurringPayment.InitialSubscription;
            if (initialSubscription == null)
                return false;

            var customer = recurringPayment.InitialSubscription.Customer;
            if (customer == null)
                return false;

            if (initialSubscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            if (!customerToValidate.IsAdmin())
            {
                if (customer.Id != customerToValidate.Id)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }


        /// <summary>
        /// Gets a value indicating whether a customer can retry last failed recurring payment
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>True if a customer can retry payment; otherwise false</returns>
        public virtual bool CanRetryLastRecurringPayment(Customer customer, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null || customer == null)
                return false;

            if (recurringPayment.InitialSubscription == null || recurringPayment.InitialSubscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            if (!recurringPayment.LastPaymentFailed || _paymentService.GetRecurringPaymentType(recurringPayment.InitialSubscription.PaymentMethodSystemName) != RecurringPaymentType.Manual)
                return false;

            if (recurringPayment.InitialSubscription.Customer == null || (!customer.IsAdmin() && recurringPayment.InitialSubscription.Customer.Id != customer.Id))
                return false;

            return true;
        }

         

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public virtual bool CanCancelSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void CancelSubscription(Subscription subscription, bool notifyCustomer)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanCancelSubscription(subscription))
                throw new YStoryException("Cannot do cancel for subscription.");

            //Cancel subscription
            SetSubscriptionStatus(subscription, SubscriptionStatus.Cancelled, notifyCustomer);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription has been cancelled",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            //return (add) back redeemded reward points
            ReturnBackRedeemedRewardPoints(subscription);

            //cancel recurring payments
            var recurringPayments = _subscriptionService.SearchRecurringPayments(initialSubscriptionId: subscription.Id);
            foreach (var rp in recurringPayments)
            {
                var errors = CancelRecurringPayment(rp);
                //use "errors" variable?
            }
 

            _eventPublisher.Publish(new SubscriptionCancelledEvent(subscription));

        }

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as authorized
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as authorized</returns>
        public virtual bool CanMarkSubscriptionAsAuthorized(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            if (subscription.PaymentStatus == PaymentStatus.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks subscription as authorized
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void MarkAsAuthorized(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            subscription.PaymentStatusId = (int)PaymentStatus.Authorized;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription has been marked as authorized",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            //check subscription status
            CheckSubscriptionStatus(subscription);
        }



        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public virtual bool CanCapture(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled ||
                subscription.SubscriptionStatus == SubscriptionStatus.Pending)
                return false;

            if (subscription.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportCapture(subscription.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Capture an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Capture(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanCapture(subscription))
                throw new YStoryException("Cannot do capture for subscription.");

            var request = new CapturePaymentRequest();
            CapturePaymentResult result = null;
            try
            {
                //old info from placing subscription
                request.Subscription = subscription;
                result = _paymentService.Capture(request);

                if (result.Success)
                {
                    var paidDate = subscription.PaidDateUtc;
                    if (result.NewPaymentStatus == PaymentStatus.Paid)
                        paidDate = DateTime.UtcNow;

                    subscription.CaptureTransactionId = result.CaptureTransactionId;
                    subscription.CaptureTransactionResult = result.CaptureTransactionResult;
                    subscription.PaymentStatus = result.NewPaymentStatus;
                    subscription.PaidDateUtc = paidDate;
                    _subscriptionService.UpdateSubscription(subscription);

                    //add a note
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = "Subscription has been captured",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);

                    CheckSubscriptionStatus(subscription);
     
                    if (subscription.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessSubscriptionPaid(subscription);
                    }
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CapturePaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Unable to capture subscription. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);

                //log it
                string logError = string.Format("Error capturing subscription #{0}. Error: {1}", subscription.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as paid
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as paid</returns>
        public virtual bool CanMarkSubscriptionAsPaid(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            if (subscription.PaymentStatus == PaymentStatus.Paid ||
                subscription.PaymentStatus == PaymentStatus.Refunded ||
                subscription.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks subscription as paid
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void MarkSubscriptionAsPaid(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanMarkSubscriptionAsPaid(subscription))
                throw new YStoryException("You can't mark this subscription as paid");

            subscription.PaymentStatusId = (int)PaymentStatus.Paid;
            subscription.PaidDateUtc = DateTime.UtcNow;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription has been marked as paid",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            CheckSubscriptionStatus(subscription);
   
            if (subscription.PaymentStatus == PaymentStatus.Paid)
            {
                ProcessSubscriptionPaid(subscription);
            }
        }



        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanRefund(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (subscription.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in subscription to disallow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //    return false;

            if (subscription.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.SupportRefund(subscription.PaymentMethodSystemName))
                return true;

            return false;
        }
        
        /// <summary>
        /// Refunds an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Refund(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanRefund(subscription))
                throw new YStoryException("Cannot do refund for subscription.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Subscription = subscription;
                request.AmountToRefund = subscription.SubscriptionTotal;
                request.IsPartialRefund = false;
                result = _paymentService.Refund(request);
                if (result.Success)
                {
                    //total amount refunded
                    decimal totalAmountRefunded = subscription.RefundedAmount + request.AmountToRefund;

                    //update subscription info
                    subscription.RefundedAmount = totalAmountRefunded;
                    subscription.PaymentStatus = result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscription(subscription);

                    //add a note
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = string.Format("Subscription has been refunded. Amount = {0}", request.AmountToRefund),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);

                    //check subscription status
                    CheckSubscriptionStatus(subscription);

                    //notifications
                    var subscriptionRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedStoreOwnerNotification(subscription, request.AmountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (subscriptionRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        subscription.SubscriptionNotes.Add(new SubscriptionNote
                        {
                            Note = string.Format("\"Subscription refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", subscriptionRefundedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscription(subscription);
                    }
                    var subscriptionRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedCustomerNotification(subscription, request.AmountToRefund, subscription.CustomerLanguageId);
                    if (subscriptionRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        subscription.SubscriptionNotes.Add(new SubscriptionNote
                        {
                            Note = string.Format("\"Subscription refunded\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionRefundedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscription(subscription);
                    }

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionRefundedEvent(subscription, request.AmountToRefund));
                }

            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Unable to refund subscription. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);

                //log it
                string logError = string.Format("Error refunding subscription #{0}. Error: {1}", subscription.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as refunded
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as refunded</returns>
        public virtual bool CanRefundOffline(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (subscription.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in subscription to disallow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //     return false;

            if (subscription.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void RefundOffline(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanRefundOffline(subscription))
                throw new YStoryException("You can't refund this subscription");

            //amout to refund
            decimal amountToRefund = subscription.SubscriptionTotal;

            //total amount refunded
            decimal totalAmountRefunded = subscription.RefundedAmount + amountToRefund;

            //update subscription info
            subscription.RefundedAmount = totalAmountRefunded;
            subscription.PaymentStatus = PaymentStatus.Refunded;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = string.Format("Subscription has been marked as refunded. Amount = {0}", amountToRefund),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            //check subscription status
            CheckSubscriptionStatus(subscription);

            //notifications
            var subscriptionRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedStoreOwnerNotification(subscription, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (subscriptionRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", subscriptionRefundedStoreOwnerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }
            var subscriptionRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedCustomerNotification(subscription, amountToRefund, subscription.CustomerLanguageId);
            if (subscriptionRefundedCustomerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription refunded\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionRefundedCustomerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }

            //raise event       
            _eventPublisher.Publish(new SubscriptionRefundedEvent(subscription, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanPartiallyRefund(Subscription subscription, decimal amountToRefund)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //uncomment the lines below in subscription to allow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //    return false;

            decimal canBeRefunded = subscription.SubscriptionTotal - subscription.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((subscription.PaymentStatus == PaymentStatus.Paid ||
                subscription.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.SupportPartiallyRefund(subscription.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> PartiallyRefund(Subscription subscription, decimal amountToRefund)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanPartiallyRefund(subscription, amountToRefund))
                throw new YStoryException("Cannot do partial refund for subscription.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Subscription = subscription;
                request.AmountToRefund = amountToRefund;
                request.IsPartialRefund = true;

                result = _paymentService.Refund(request);

                if (result.Success)
                {
                    //total amount refunded
                    decimal totalAmountRefunded = subscription.RefundedAmount + amountToRefund;

                    //update subscription info
                    subscription.RefundedAmount = totalAmountRefunded;
                    //mark payment status as 'Refunded' if the subscription total amount is fully refunded
                    subscription.PaymentStatus = subscription.SubscriptionTotal == totalAmountRefunded && result.NewPaymentStatus == PaymentStatus.PartiallyRefunded ? PaymentStatus.Refunded : result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscription(subscription);


                    //add a note
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = string.Format("Subscription has been partially refunded. Amount = {0}", amountToRefund),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);

                    //check subscription status
                    CheckSubscriptionStatus(subscription);

                    //notifications
                    var subscriptionRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedStoreOwnerNotification(subscription, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (subscriptionRefundedStoreOwnerNotificationQueuedEmailId > 0)
                    {
                        subscription.SubscriptionNotes.Add(new SubscriptionNote
                        {
                            Note = string.Format("\"Subscription refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", subscriptionRefundedStoreOwnerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscription(subscription);
                    }
                    var subscriptionRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedCustomerNotification(subscription, amountToRefund, subscription.CustomerLanguageId);
                    if (subscriptionRefundedCustomerNotificationQueuedEmailId > 0)
                    {
                        subscription.SubscriptionNotes.Add(new SubscriptionNote
                        {
                            Note = string.Format("\"Subscription refunded\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionRefundedCustomerNotificationQueuedEmailId),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _subscriptionService.UpdateSubscription(subscription);
                    }

                    //raise event       
                    _eventPublisher.Publish(new SubscriptionRefundedEvent(subscription, amountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Unable to partially refund subscription. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);

                //log it
                string logError = string.Format("Error refunding subscription #{0}. Error: {1}", subscription.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as partially refunded
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether subscription can be marked as partially refunded</returns>
        public virtual bool CanPartiallyRefundOffline(Subscription subscription, decimal amountToRefund)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //uncomment the lines below in subscription to allow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //    return false;

            decimal canBeRefunded = subscription.SubscriptionTotal - subscription.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (subscription.PaymentStatus == PaymentStatus.Paid ||
                subscription.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public virtual void PartiallyRefundOffline(Subscription subscription, decimal amountToRefund)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");
            
            if (!CanPartiallyRefundOffline(subscription, amountToRefund))
                throw new YStoryException("You can't partially refund (offline) this subscription");

            //total amount refunded
            decimal totalAmountRefunded = subscription.RefundedAmount + amountToRefund;

            //update subscription info
            subscription.RefundedAmount = totalAmountRefunded;
            //mark payment status as 'Refunded' if the subscription total amount is fully refunded
            subscription.PaymentStatus = subscription.SubscriptionTotal == totalAmountRefunded ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = string.Format("Subscription has been marked as partially refunded. Amount = {0}", amountToRefund),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            //check subscription status
            CheckSubscriptionStatus(subscription);

            //notifications
            var subscriptionRefundedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedStoreOwnerNotification(subscription, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (subscriptionRefundedStoreOwnerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription refunded\" email (to store owner) has been queued. Queued email identifier: {0}.", subscriptionRefundedStoreOwnerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }
            var subscriptionRefundedCustomerNotificationQueuedEmailId = _workflowMessageService.SendSubscriptionRefundedCustomerNotification(subscription, amountToRefund, subscription.CustomerLanguageId);
            if (subscriptionRefundedCustomerNotificationQueuedEmailId > 0)
            {
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("\"Subscription refunded\" email (to customer) has been queued. Queued email identifier: {0}.", subscriptionRefundedCustomerNotificationQueuedEmailId),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);
            }
            //raise event       
            _eventPublisher.Publish(new SubscriptionRefundedEvent(subscription, amountToRefund));
        }



        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public virtual bool CanVoid(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //uncomment the lines below in subscription to allow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //    return false;

            if (subscription.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportVoid(subscription.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Voids subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Voided subscription</returns>
        public virtual IList<string> Void(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanVoid(subscription))
                throw new YStoryException("Cannot do void for subscription.");

            var request = new VoidPaymentRequest();
            VoidPaymentResult result = null;
            try
            {
                request.Subscription = subscription;
                result = _paymentService.Void(request);

                if (result.Success)
                {
                    //update subscription info
                    subscription.PaymentStatus = result.NewPaymentStatus;
                    _subscriptionService.UpdateSubscription(subscription);

                    //add a note
                    subscription.SubscriptionNotes.Add(new SubscriptionNote
                    {
                        Note = "Subscription has been voided",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _subscriptionService.UpdateSubscription(subscription);

                    //check subscription status
                    CheckSubscriptionStatus(subscription);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new VoidPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                subscription.SubscriptionNotes.Add(new SubscriptionNote
                {
                    Note = string.Format("Unable to voiding subscription. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _subscriptionService.UpdateSubscription(subscription);

                //log it
                string logError = string.Format("Error voiding subscription #{0}. Error: {1}", subscription.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as voided
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as voided</returns>
        public virtual bool CanVoidOffline(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (subscription.SubscriptionTotal == decimal.Zero)
                return false;

            //uncomment the lines below in subscription to allow this operation for cancelled subscriptions
            //if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
            //    return false;

            if (subscription.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void VoidOffline(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            if (!CanVoidOffline(subscription))
                throw new YStoryException("You can't void this subscription");

            subscription.PaymentStatusId = (int)PaymentStatus.Voided;
            _subscriptionService.UpdateSubscription(subscription);

            //add a note
            subscription.SubscriptionNotes.Add(new SubscriptionNote
            {
                Note = "Subscription has been marked as voided",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _subscriptionService.UpdateSubscription(subscription);

            //check orer status
            CheckSubscriptionStatus(subscription);
        }



        /// <summary>
        /// Place subscription items in current user shopping cart.
        /// </summary>
        /// <param name="subscription">The subscription</param>
        public virtual void ReSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            //move shopping cart items (if possible)
            foreach (var subscriptionItem in subscription.SubscriptionItems)
            {
                _shoppingCartService.AddToCart(subscription.Customer, subscriptionItem.Article,
                    ShoppingCartType.ShoppingCart, subscription.StoreId, 
                    subscriptionItem.AttributesXml, subscriptionItem.UnitPriceExclTax,
                    subscriptionItem.RentalStartDateUtc, subscriptionItem.RentalEndDateUtc,
                    subscriptionItem.Quantity, false);
            }

            //set checkout attributes
            //comment the code below if you want to disable this functionality
            _genericAttributeService.SaveAttribute(subscription.Customer, SystemCustomerAttributeNames.CheckoutAttributes, subscription.CheckoutAttributesXml, subscription.StoreId);
        }
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Result</returns>
        public virtual bool IsReturnRequestAllowed(Subscription subscription)
        {
            if (!_subscriptionSettings.ReturnRequestsEnabled)
                return false;

            if (subscription == null || subscription.Deleted)
                return false;

            //status should be compelte
            if (subscription.SubscriptionStatus != SubscriptionStatus.Complete)
                return false;

            //validate allowed number of days
            if (_subscriptionSettings.NumberOfDaysReturnRequestAvailable > 0)
            {
                var daysPassed = (DateTime.UtcNow - subscription.CreatedOnUtc).TotalDays;
                if (daysPassed >= _subscriptionSettings.NumberOfDaysReturnRequestAvailable)
                    return false;
            }

            //ensure that we have at least one returnable article
            return subscription.SubscriptionItems.Any(oi => !oi.Article.NotReturnable);
        }
        


        /// <summary>
        /// Valdiate minimum subscription sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum subscription sub-total amount is not reached</returns>
        public virtual bool ValidateMinSubscriptionSubtotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            //min subscription amount sub-total validation
            if (cart.Any() && _subscriptionSettings.MinSubscriptionSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal subscriptionSubTotalDiscountAmountBase;
                
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                _subscriptionTotalCalculationService.GetShoppingCartSubTotal(cart, _subscriptionSettings.MinSubscriptionSubtotalAmountIncludingTax,
                    out subscriptionSubTotalDiscountAmountBase,  
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithoutDiscountBase < _subscriptionSettings.MinSubscriptionSubtotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Valdiate minimum subscription total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum subscription total amount is not reached</returns>
        public virtual bool ValidateMinSubscriptionTotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (cart.Any() && _subscriptionSettings.MinSubscriptionTotalAmount > decimal.Zero)
            {
                decimal? shoppingCartTotalBase = _subscriptionTotalCalculationService.GetShoppingCartTotal(cart);
                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _subscriptionSettings.MinSubscriptionTotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether payment workflow is required
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <returns>true - OK; false - minimum subscription total amount is not reached</returns>
        public virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool? useRewardPoints = null)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            bool result = true;

            //check whether subscription total equals zero
            decimal? shoppingCartTotalBase = _subscriptionTotalCalculationService.GetShoppingCartTotal(cart, useRewardPoints: useRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        #endregion
    }
}
