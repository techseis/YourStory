using System.Collections.Generic;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Payments;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Subscription processing service interface
    /// </summary>
    public partial interface ISubscriptionProcessingService
    {
        /// <summary>
        /// Checks subscription status
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Validated subscription</returns>
        void CheckSubscriptionStatus(Subscription subscription);

        /// <summary>
        /// Places an subscription
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place subscription result</returns>
        PlaceSubscriptionResult PlaceSubscription(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Update subscription totals
        /// </summary>
        /// <param name="updateSubscriptionParameters">Parameters for the updating subscription</param>
        void UpdateSubscriptionTotals(UpdateSubscriptionParameters updateSubscriptionParameters);

        /// <summary>
        /// Deletes an subscription
        /// </summary>
        /// <param name="subscription">The subscription</param>
        void DeleteSubscription(Subscription subscription);


        /// <summary>
        /// Process next recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
        /// <returns>Collection of errors</returns>
        IEnumerable<string> ProcessNextRecurringPayment(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null);

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        IList<string> CancelRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a value indicating whether a customer can retry last failed recurring payment
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>True if a customer can retry payment; otherwise false</returns>
        bool CanRetryLastRecurringPayment(Customer customer, RecurringPayment recurringPayment);


 


        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        bool CanCancelSubscription(Subscription subscription);

        /// <summary>
        /// Cancels subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void CancelSubscription(Subscription subscription, bool notifyCustomer);



        /// <summary>
        /// Gets a value indicating whether subscription can be marked as authorized
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as authorized</returns>
        bool CanMarkSubscriptionAsAuthorized(Subscription subscription);

        /// <summary>
        /// Marks subscription as authorized
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void MarkAsAuthorized(Subscription subscription);



        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        bool CanCapture(Subscription subscription);

        /// <summary>
        /// Capture an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Capture(Subscription subscription);

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as paid
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as paid</returns>
        bool CanMarkSubscriptionAsPaid(Subscription subscription);

        /// <summary>
        /// Marks subscription as paid
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void MarkSubscriptionAsPaid(Subscription subscription);



        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanRefund(Subscription subscription);

        /// <summary>
        /// Refunds an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Refund(Subscription subscription);

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as refunded
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as refunded</returns>
        bool CanRefundOffline(Subscription subscription);

        /// <summary>
        /// Refunds an subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void RefundOffline(Subscription subscription);

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanPartiallyRefund(Subscription subscription, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> PartiallyRefund(Subscription subscription, decimal amountToRefund);

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as partially refunded
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether subscription can be marked as partially refunded</returns>
        bool CanPartiallyRefundOffline(Subscription subscription, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="amountToRefund">Amount to refund</param>
        void PartiallyRefundOffline(Subscription subscription, decimal amountToRefund);



        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        bool CanVoid(Subscription subscription);

        /// <summary>
        /// Voids subscription (from admin panel)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Voided subscription</returns>
        IList<string> Void(Subscription subscription);

        /// <summary>
        /// Gets a value indicating whether subscription can be marked as voided
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A value indicating whether subscription can be marked as voided</returns>
        bool CanVoidOffline(Subscription subscription);

        /// <summary>
        /// Voids subscription (offline)
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void VoidOffline(Subscription subscription);




        /// <summary>
        /// Place subscription items in current user shopping cart.
        /// </summary>
        /// <param name="subscription">The subscription</param>
        void ReSubscription(Subscription subscription);
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Result</returns>
        bool IsReturnRequestAllowed(Subscription subscription);



        /// <summary>
        /// Valdiate minimum subscription sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum subscription sub-total amount is not reached</returns>
        bool ValidateMinSubscriptionSubtotalAmount(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Valdiate minimum subscription total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum subscription total amount is not reached</returns>
        bool ValidateMinSubscriptionTotalAmount(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets a value indicating whether payment workflow is required
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <returns>true - OK; false - minimum subscription total amount is not reached</returns>
        bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool? useRewardPoints = null);
    }
}
