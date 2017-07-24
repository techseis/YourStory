using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Tax;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Payments;
using YStory.Services.Tax;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription service
    /// </summary>
    public partial class SubscriptionTotalCalculationService : ISubscriptionTotalCalculationService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IPaymentService _paymentService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRewardPointService _rewardPointService;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public SubscriptionTotalCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IPaymentService paymentService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            IRewardPointService rewardPointService,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._paymentService = paymentService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._rewardPointService = rewardPointService;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets an subscription discount (applied to subscription subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="subscriptionSubTotal">Subscription subtotal</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Subscription discount</returns>
        protected virtual decimal GetSubscriptionSubtotalDiscount(Customer customer,
            decimal subscriptionSubTotal)
        {
            
            decimal discountAmount = decimal.Zero;
            

           

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return discountAmount;
        }

       
        /// <summary>
        /// Gets an subscription discount (applied to subscription total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="subscriptionTotal">Subscription total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Subscription discount</returns>
        protected virtual decimal GetSubscriptionTotalDiscount(Customer customer, decimal subscriptionTotal)
        {
           
            decimal discountAmount = decimal.Zero;
          

           

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = RoundingHelper.RoundPrice(discountAmount);

            return discountAmount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, 
            bool includingTax,
            out decimal discountAmount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            SortedDictionary<decimal, decimal> taxRates;
            GetShoppingCartSubTotal(cart, includingTax, 
                out discountAmount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of subscription sub total)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates)
        {
            discountAmount = decimal.Zero;
           
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            if (!cart.Any())
                return;

            //get the customer 
            Customer customer = cart.GetCustomer();
            
            //sub totals
            decimal subTotalExclTaxWithoutDiscount = decimal.Zero;
            decimal subTotalInclTaxWithoutDiscount = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                decimal sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem);

                decimal taxRate;
                decimal sciExclTax = _taxService.GetArticlePrice(shoppingCartItem.Article, sciSubTotal, false, customer, out taxRate);
                decimal sciInclTax = _taxService.GetArticlePrice(shoppingCartItem.Article, sciSubTotal, true, customer, out taxRate);
                subTotalExclTaxWithoutDiscount += sciExclTax;
                subTotalInclTaxWithoutDiscount += sciInclTax;
                
                //tax rates
                decimal sciTax = sciInclTax - sciExclTax;
                if (taxRate > decimal.Zero && sciTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                    {
                        taxRates.Add(taxRate, sciTax);
                    }
                    else
                    {
                        taxRates[taxRate] = taxRates[taxRate] + sciTax;
                    }
                }
            }

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        decimal taxRate;

                        decimal caExclTax = _taxService.GetCheckoutAttributePrice(attributeValue, false, customer, out taxRate);
                        decimal caInclTax = _taxService.GetCheckoutAttributePrice(attributeValue, true, customer, out taxRate);
                        subTotalExclTaxWithoutDiscount += caExclTax;
                        subTotalInclTaxWithoutDiscount += caInclTax;

                        //tax rates
                        decimal caTax = caInclTax - caExclTax;
                        if (taxRate > decimal.Zero && caTax > decimal.Zero)
                        {
                            if (!taxRates.ContainsKey(taxRate))
                            {
                                taxRates.Add(taxRate, caTax);
                            }
                            else
                            {
                                taxRates[taxRate] = taxRates[taxRate] + caTax;
                            }
                        }
                    }
                }
            }

            //subtotal without discount
            subTotalWithoutDiscount = includingTax ? subTotalInclTaxWithoutDiscount : subTotalExclTaxWithoutDiscount;
            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithoutDiscount = RoundingHelper.RoundPrice(subTotalWithoutDiscount);

            //We calculate discount amount on subscription subtotal excl tax (discount first)
            //calculate discount amount ('Applied to subscription subtotal' discount)
            decimal discountAmountExclTax = GetSubscriptionSubtotalDiscount(customer, subTotalExclTaxWithoutDiscount);
            if (subTotalExclTaxWithoutDiscount < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTaxWithoutDiscount;
            decimal discountAmountInclTax = discountAmountExclTax;
            //subtotal with discount (excl tax)
            decimal subTotalExclTaxWithDiscount = subTotalExclTaxWithoutDiscount - discountAmountExclTax;
            decimal subTotalInclTaxWithDiscount = subTotalExclTaxWithDiscount;

            //add tax for shopping items & checkout attributes
            var tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
            foreach (KeyValuePair<decimal, decimal> kvp in tempTaxRates)
            {
                decimal taxRate = kvp.Key;
                decimal taxValue = kvp.Value;

                if (taxValue != decimal.Zero)
                {
                    //discount the tax amount that applies to subtotal items
                    if (subTotalExclTaxWithoutDiscount > decimal.Zero)
                    {
                        decimal discountTax = taxRates[taxRate] * (discountAmountExclTax / subTotalExclTaxWithoutDiscount);
                        discountAmountInclTax += discountTax;
                        taxValue = taxRates[taxRate] - discountTax;
                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            taxValue = RoundingHelper.RoundPrice(taxValue);
                        taxRates[taxRate] = taxValue;
                    }

                    //subtotal with discount (incl tax)
                    subTotalInclTaxWithDiscount += taxValue;
                }
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                discountAmountInclTax = RoundingHelper.RoundPrice(discountAmountInclTax);
                discountAmountExclTax = RoundingHelper.RoundPrice(discountAmountExclTax);
            }

            if (includingTax)
            {
                subTotalWithDiscount = subTotalInclTaxWithDiscount;
                discountAmount = discountAmountInclTax;
            }
            else
            {
                subTotalWithDiscount = subTotalExclTaxWithDiscount;
                discountAmount = discountAmountExclTax;
            }

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithDiscount = RoundingHelper.RoundPrice(subTotalWithDiscount);
        }

        /// <summary>
        /// Update subscription totals
        /// </summary>
        /// <param name="updateSubscriptionParameters">Parameters for the updating subscription</param>
        /// <param name="restoredCart">Shopping cart</param>
        public virtual void UpdateSubscriptionTotals(UpdateSubscriptionParameters updateSubscriptionParameters, IList<ShoppingCartItem> restoredCart)
        {
            var updatedSubscription = updateSubscriptionParameters.UpdatedSubscription;
            var updatedSubscriptionItem = updateSubscriptionParameters.UpdatedSubscriptionItem;

            //get the customer 
            var customer = restoredCart.GetCustomer();

            #region Sub total

            var subTotalExclTax = decimal.Zero;
            var subTotalInclTax = decimal.Zero;
            var subTotalTaxRates = new SortedDictionary<decimal, decimal>();

            foreach (var shoppingCartItem in restoredCart)
            {
                var itemSubTotalExclTax = decimal.Zero;
                var itemSubTotalInclTax = decimal.Zero;
                var taxRate = decimal.Zero;
                 

                //calculate subtotal for the updated subscription item
                if (shoppingCartItem.Id == updatedSubscriptionItem.Id)
                {
                    //update subscription item 
                    updatedSubscriptionItem.UnitPriceExclTax = updateSubscriptionParameters.PriceExclTax;
                    updatedSubscriptionItem.UnitPriceInclTax = updateSubscriptionParameters.PriceInclTax;
                    updatedSubscriptionItem.DiscountAmountExclTax = updateSubscriptionParameters.DiscountAmountExclTax;
                    updatedSubscriptionItem.DiscountAmountInclTax = updateSubscriptionParameters.DiscountAmountInclTax;
                    updatedSubscriptionItem.PriceExclTax = itemSubTotalExclTax = updateSubscriptionParameters.SubTotalExclTax;
                    updatedSubscriptionItem.PriceInclTax = itemSubTotalInclTax = updateSubscriptionParameters.SubTotalInclTax;
                    updatedSubscriptionItem.Quantity = shoppingCartItem.Quantity;

                    taxRate = Math.Round((100 * (itemSubTotalInclTax - itemSubTotalExclTax)) / itemSubTotalExclTax, 3);
                }
                else
                {
                    //get the already calculated subtotal from the subscription item
                    itemSubTotalExclTax = updatedSubscription.SubscriptionItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceExclTax;
                    itemSubTotalInclTax = updatedSubscription.SubscriptionItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceInclTax;
                    taxRate = Math.Round((100 * (itemSubTotalInclTax - itemSubTotalExclTax)) / itemSubTotalExclTax, 3);
                }

              

                subTotalExclTax += itemSubTotalExclTax;
                subTotalInclTax += itemSubTotalInclTax;

                //tax rates
                var itemTaxValue = itemSubTotalInclTax - itemSubTotalExclTax;
                if (taxRate > decimal.Zero && itemTaxValue > decimal.Zero)
                {
                    if (!subTotalTaxRates.ContainsKey(taxRate))
                        subTotalTaxRates.Add(taxRate, itemTaxValue);
                    else
                        subTotalTaxRates[taxRate] = subTotalTaxRates[taxRate] + itemTaxValue;
                }
            }

            if (subTotalExclTax < decimal.Zero)
                subTotalExclTax = decimal.Zero;

            if (subTotalInclTax < decimal.Zero)
                subTotalInclTax = decimal.Zero;

            //We calculate discount amount on subscription subtotal excl tax (discount first)
            //calculate discount amount ('Applied to subscription subtotal' discount)
            
            
            //add tax for shopping items
            var tempTaxRates = new Dictionary<decimal, decimal>(subTotalTaxRates);
            foreach (var kvp in tempTaxRates)
            {
                if (kvp.Value != decimal.Zero && subTotalExclTax > decimal.Zero)
                {

                    subTotalTaxRates[kvp.Key] = kvp.Value;
                }
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalExclTax = RoundingHelper.RoundPrice(subTotalExclTax);
                subTotalInclTax = RoundingHelper.RoundPrice(subTotalInclTax);
                 
            }

            updatedSubscription.SubscriptionSubtotalExclTax = subTotalExclTax;
            updatedSubscription.SubscriptionSubtotalInclTax = subTotalInclTax;
            

            #endregion

             

            #region Tax rates

            var taxRates = new SortedDictionary<decimal, decimal>();

            //subscription subtotal taxes
            var subTotalTax = decimal.Zero;
            foreach (var kvp in subTotalTaxRates)
            {
                subTotalTax += kvp.Value;
                if (kvp.Key > decimal.Zero && kvp.Value > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(kvp.Key))
                        taxRates.Add(kvp.Key, kvp.Value);
                    else
                        taxRates[kvp.Key] = taxRates[kvp.Key] + kvp.Value;
                }
            }

            

            //payment method additional fee tax
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                paymentMethodAdditionalFeeTax = updatedSubscription.PaymentMethodAdditionalFeeInclTax - updatedSubscription.PaymentMethodAdditionalFeeExclTax;
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                if (updatedSubscription.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    var paymentTaxRate = Math.Round(100 * paymentMethodAdditionalFeeTax / updatedSubscription.PaymentMethodAdditionalFeeExclTax, 3);
                    if (paymentTaxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(paymentTaxRate))
                            taxRates.Add(paymentTaxRate, paymentMethodAdditionalFeeTax);
                        else
                            taxRates[paymentTaxRate] = taxRates[paymentTaxRate] + paymentMethodAdditionalFeeTax;
                    }
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTax +  paymentMethodAdditionalFeeTax;
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = RoundingHelper.RoundPrice(taxTotal);

            updatedSubscription.SubscriptionTax = taxTotal;
            updatedSubscription.TaxRates = taxRates.Aggregate(string.Empty, (current, next) =>
                string.Format("{0}{1}:{2};   ", current, next.Key.ToString(CultureInfo.InvariantCulture), next.Value.ToString(CultureInfo.InvariantCulture)));

            #endregion

            #region Total

            var total = (subTotalExclTax ) +  updatedSubscription.PaymentMethodAdditionalFeeExclTax + taxTotal;

          
           

            

            //reward points
            var rewardPointsOfSubscription = _rewardPointService.GetRewardPointsHistory(customer.Id, true).FirstOrDefault(history => history.UsedWithSubscription == updatedSubscription);
            if (rewardPointsOfSubscription != null)
            {
                var rewardPoints = -rewardPointsOfSubscription.Points;
                var rewardPointsAmount = ConvertRewardPointsToAmount(rewardPoints);
                if (total < rewardPointsAmount)
                {
                    rewardPoints = ConvertAmountToRewardPoints(total);
                    rewardPointsAmount = total;
                }
                if (total > decimal.Zero)
                    total -= rewardPointsAmount;

                //uncomment here for the return unused reward points if new subscription total less redeemed reward points amount
                //if (rewardPoints < -rewardPointsOfSubscription.Points)
                //    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfSubscription.Points - rewardPoints, _storeContext.CurrentStore.Id, "Return unused reward points");

                if (rewardPointsAmount != rewardPointsOfSubscription.UsedAmount)
                {
                    rewardPointsOfSubscription.UsedAmount = rewardPointsAmount;
                    rewardPointsOfSubscription.Points = -rewardPoints;
                    _rewardPointService.UpdateRewardPointsHistoryEntry(rewardPointsOfSubscription);
                }
            }

            //rounding
            if (total < decimal.Zero)
                total = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                total = RoundingHelper.RoundPrice(total);

           

            #endregion
        }

        



        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            SortedDictionary<decimal, decimal> taxRates;
            return GetTaxTotal(cart, out taxRates, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart,
            out SortedDictionary<decimal, decimal> taxRates, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            taxRates = new SortedDictionary<decimal, decimal>();

            var customer = cart.GetCustomer();
            string paymentMethodSystemName = "";
            if (customer != null)
            {
                paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService,
                    _storeContext.CurrentStore.Id);
            }

            //subscription sub total (items + checkout attributes)
            decimal subTotalTaxTotal = decimal.Zero;
            decimal subscriptionSubTotalDiscountAmount;
           
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            SortedDictionary<decimal, decimal> subscriptionSubTotalTaxRates;
            GetShoppingCartSubTotal(cart, false, 
                out subscriptionSubTotalDiscountAmount, 
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase,
                out subscriptionSubTotalTaxRates);
            foreach (KeyValuePair<decimal, decimal> kvp in subscriptionSubTotalTaxRates)
            {
                decimal taxRate = kvp.Key;
                decimal taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate > decimal.Zero && taxValue > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, taxValue);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + taxValue;
                }
            }

            

            //payment method additional fee
            decimal paymentMethodAdditionalFeeTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                decimal taxRate;
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out taxRate);
                decimal paymentMethodAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                //ensure that tax is equal or greater than zero
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            decimal taxTotal = subTotalTaxTotal +  paymentMethodAdditionalFeeTax;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;
            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = RoundingHelper.RoundPrice(taxTotal);
            return taxTotal;
        }





        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating subscription total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null,  bool usePaymentMethodAdditionalFee = true)
        {
            decimal discountAmount;
          
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
       
            return GetShoppingCartTotal(cart, 
                out discountAmount,
              
                out redeemedRewardPoints, 
                out redeemedRewardPointsAmount,
                useRewardPoints,
                usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating subscription total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount,  
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            var customer = cart.GetCustomer();
            string paymentMethodSystemName = "";
            if (customer != null)
            {
                paymentMethodSystemName = customer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService,
                    _storeContext.CurrentStore.Id);
            }


            //subtotal without tax
            decimal subscriptionSubTotalDiscountAmount;
         
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            GetShoppingCartSubTotal(cart, false,
                out subscriptionSubTotalDiscountAmount,  
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            //subtotal with discount
            decimal subtotalBase = subTotalWithDiscountBase;



           



            //payment method additional fee without tax
            decimal paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !String.IsNullOrEmpty(paymentMethodSystemName))
            {
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart,
                    paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax =
                    _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
                        false, customer);
            }




            //tax
            decimal shoppingCartTax = GetTaxTotal(cart, usePaymentMethodAdditionalFee);




            //subscription total
            decimal resultTemp = decimal.Zero;
            resultTemp += subtotalBase;
           
            resultTemp += paymentMethodAdditionalFeeWithoutTax;
            resultTemp += shoppingCartTax;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

            #region Subscription total discount

            discountAmount = GetSubscriptionTotalDiscount(customer, resultTemp);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

            #endregion

             
             
            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = RoundingHelper.RoundPrice(resultTemp);

          
            decimal subscriptionTotal = resultTemp;

            #region Reward points

            if (_rewardPointsSettings.Enabled)
            {
                if (!useRewardPoints.HasValue)
                    useRewardPoints = customer.GetAttribute<bool>(SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, _genericAttributeService, _storeContext.CurrentStore.Id);
                if (useRewardPoints.Value)
                {

                    int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
                    if (CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                    {
                        decimal rewardPointsBalanceAmount = ConvertRewardPointsToAmount(rewardPointsBalance);
                        if (subscriptionTotal > decimal.Zero)
                        {
                            if (subscriptionTotal > rewardPointsBalanceAmount)
                            {
                                redeemedRewardPoints = rewardPointsBalance;
                                redeemedRewardPointsAmount = rewardPointsBalanceAmount;
                            }
                            else
                            {
                                redeemedRewardPointsAmount = subscriptionTotal;
                                redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
                            }
                        }
                    }
                }
            }

            #endregion

            subscriptionTotal = subscriptionTotal - redeemedRewardPointsAmount;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subscriptionTotal = RoundingHelper.RoundPrice(subscriptionTotal);
            return subscriptionTotal;
        }





        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = RoundingHelper.RoundPrice(result);
            return result;
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            int result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }
 
        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        /// <summary>
        /// Calculate how subscription total (maximum amount) for which reward points could be earned/reduced
        /// </summary>
        /// <param name="subscriptionShippingInclTax">Subscription shipping (including tax)</param>
        /// <param name="subscriptionTotal">Subscription total</param>
        /// <returns>Applicable subscription total</returns>
        public virtual decimal CalculateApplicableSubscriptionTotalForRewardPoints(decimal subscriptionShippingInclTax, decimal subscriptionTotal)
        {
            //do you give reward points for subscription total? or do you exclude shipping?
            //since shipping costs vary some of store owners don't give reward points based on shipping total
            //you can put your custom logic here
            return subscriptionTotal - subscriptionShippingInclTax;
        }
        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>Number of reward points</returns>
        public virtual int CalculateRewardPoints(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || customer.IsGuest())
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
            return points;
        }

        #endregion
    }
}
