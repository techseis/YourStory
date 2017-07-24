using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using YStory.Core;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
using YStory.Core.Plugins;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Directory;
using YStory.Services.Localization;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Web.Models.Checkout;
using YStory.Web.Models.Common;

namespace YStory.Web.Factories
{
    public partial class CheckoutModelFactory : ICheckoutModelFactory
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPaymentService _paymentService;
        private readonly ISubscriptionTotalCalculationService _subscriptionTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IWebHelper _webHelper;

        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

		#region Ctor

        public CheckoutModelFactory(IAddressModelFactory addressModelFactory, 
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ILocalizationService localizationService, 
            ITaxService taxService, 
            ICurrencyService currencyService, 
            IPriceFormatter priceFormatter, 
            ISubscriptionProcessingService subscriptionProcessingService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IPaymentService paymentService,
            ISubscriptionTotalCalculationService subscriptionTotalCalculationService,
            IRewardPointService rewardPointService,
            IWebHelper webHelper,
            SubscriptionSettings subscriptionSettings, 
            RewardPointsSettings rewardPointsSettings,
            PaymentSettings paymentSettings,
            AddressSettings addressSettings)
        {
            this._addressModelFactory = addressModelFactory;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._paymentService = paymentService;
            this._subscriptionTotalCalculationService = subscriptionTotalCalculationService;
            this._rewardPointService = rewardPointService;
            this._webHelper = webHelper;

            this._subscriptionSettings = subscriptionSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>Billing address model</returns>
        public virtual CheckoutBillingAddressModel PrepareBillingAddressModel(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressModel();
           
            model.ShipToSameAddress = true;

            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.Country == null || 
                    (//published
                    a.Country.Published && 
                    //allow billing
                    a.Country.AllowsBilling && 
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country)))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressModelFactory.PrepareAddressModel(addressModel,
                    address: address, 
                    excludeProperties: false, 
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            _addressModelFactory.PrepareAddressModel(model.NewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountriesForBilling(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer,
                overrideAttributesXml: overrideAttributesXml);
            return model;
        }

        

     

        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>Payment method model</returns>
        public virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _subscriptionTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero && 
                    _subscriptionTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;

                    //are points enough to pay for entire subscription? like if this option (to use them) was selected
                    model.RewardPointsEnoughToPayForSubscription = !_subscriptionProcessingService.IsPaymentWorkflowRequired(cart, true);
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    Description = _paymentSettings.ShowPaymentMethodDescriptions ? pm.PaymentMethodDescription : string.Empty,
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }
            
            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        /// <summary>
        /// Prepare payment info model
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>Payment info model</returns>
        public virtual CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod)
        {
            var model = new CheckoutPaymentInfoModel();
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;
            model.DisplaySubscriptionTotals = _subscriptionSettings.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab;
            return model;
        }

        /// <summary>
        /// Prepare confirm subscription model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Confirm subscription model</returns>
        public virtual CheckoutConfirmModel PrepareConfirmSubscriptionModel(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnSubscriptionConfirmPage = _subscriptionSettings.TermsOfServiceOnSubscriptionConfirmPage;
            //min subscription amount validation
            bool minSubscriptionTotalAmountOk = _subscriptionProcessingService.ValidateMinSubscriptionTotalAmount(cart);
            if (!minSubscriptionTotalAmountOk)
            {
                decimal minSubscriptionTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_subscriptionSettings.MinSubscriptionTotalAmount, _workContext.WorkingCurrency);
                model.MinSubscriptionTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinSubscriptionTotalAmount"), _priceFormatter.FormatPrice(minSubscriptionTotalAmount, true, false));
            }
            return model;
        }

        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Checkout completed model</returns>
        public virtual CheckoutCompletedModel PrepareCheckoutCompletedModel(Subscription subscription)
        {
            if (subscription ==null)
                throw new ArgumentNullException("subscription");

            var model = new CheckoutCompletedModel
            {
                SubscriptionId = subscription.Id,
                OnePageCheckoutEnabled = _subscriptionSettings.OnePageCheckoutEnabled,
                CustomSubscriptionNumber = subscription.CustomSubscriptionNumber
            };

            return model;
        }

        /// <summary>
        /// Prepare checkout progress model
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns>Checkout progress model</returns>
        public virtual CheckoutProgressModel PrepareCheckoutProgressModel(CheckoutProgressStep step)
        {
            var model = new CheckoutProgressModel {CheckoutProgressStep = step};
            return model;
        }

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>One page checkout model</returns>
        public virtual OnePageCheckoutModel PrepareOnePageCheckoutModel(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw  new ArgumentNullException("cart");

            var model = new OnePageCheckoutModel
            {
               
                DisableBillingAddressCheckoutStep = _subscriptionSettings.DisableBillingAddressCheckoutStep
            };
            return model;
        }

        #endregion
    }
}
