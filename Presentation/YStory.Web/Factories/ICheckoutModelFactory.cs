using System.Collections.Generic;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Payments;
using YStory.Web.Models.Checkout;

namespace YStory.Web.Factories
{
    public partial interface ICheckoutModelFactory
    {
        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>Billing address model</returns>
        CheckoutBillingAddressModel PrepareBillingAddressModel(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "");

   

       

        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>Payment method model</returns>
        CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId);

        /// <summary>
        /// Prepare payment info model
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>Payment info model</returns>
        CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod);

        /// <summary>
        /// Prepare confirm subscription model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Confirm subscription model</returns>
        CheckoutConfirmModel PrepareConfirmSubscriptionModel(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Checkout completed model</returns>
        CheckoutCompletedModel PrepareCheckoutCompletedModel(Subscription subscription);

        /// <summary>
        /// Prepare checkout progress model
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns>Checkout progress model</returns>
        CheckoutProgressModel PrepareCheckoutProgressModel(CheckoutProgressStep step);

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>One page checkout model</returns>
        OnePageCheckoutModel PrepareOnePageCheckoutModel(IList<ShoppingCartItem> cart);
    }
}
