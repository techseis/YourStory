using YStory.Core.Configuration;

namespace YStory.Core.Domain.Subscriptions
{
    public class SubscriptionSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether customer can make re-subscription
        /// </summary>
        public bool IsReSubscriptionAllowed { get; set; }

        /// <summary>
        /// Gets or sets a minimum subscription subtotal amount
        /// </summary>
        public decimal MinSubscriptionSubtotalAmount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Minimum subscription subtotal amount' option
        /// should be evaluated over 'X' value including tax or not
        /// </summary>
        public bool MinSubscriptionSubtotalAmountIncludingTax { get; set; }
        /// <summary>
        /// Gets or sets a minimum subscription total amount
        /// </summary>
        public decimal MinSubscriptionTotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatically update subscription totals on editing an subscription in admin area
        /// </summary>
        public bool AutoUpdateSubscriptionTotalsOnEditingSubscription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        public bool AnonymousCheckoutAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Terms of service' enabled on the shopping cart page
        /// </summary>
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Terms of service' enabled on the subscription confirmation page
        /// </summary>
        public bool TermsOfServiceOnSubscriptionConfirmPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'One-page checkout' is enabled
        /// </summary>
        public bool OnePageCheckoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subscription totals should be displayed on 'Payment info' tab of 'One-page checkout' page
        /// </summary>
        public bool OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Billing address" step should be skipped
        /// </summary>
        public bool DisableBillingAddressCheckoutStep { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Subscription completed" page should be skipped
        /// </summary>
        public bool DisableSubscriptionCompletedPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "Subscription placed" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionPlacedEmail { get; set; }
        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "Subscription paid" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionPaidEmail { get; set; }
        /// <summary>
        /// Gets or sets a value indicating we should attach PDF invoice to "Subscription completed" email
        /// </summary>
        public bool AttachPdfInvoiceToSubscriptionCompletedEmail { get; set; }
        /// <summary>
        /// Gets or sets a value indicating we PDF invoices should be generated in customer language. Otherwise, use the current one
        /// </summary>
        public bool GeneratePdfInvoiceInCustomerLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Return requests" are allowed
        /// </summary>
        public bool ReturnRequestsEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload files
        /// </summary>
        public bool ReturnRequestsAllowFiles { get; set; }
        /// <summary>
        /// Gets or sets maximum file size for upload file (return request). Set 0 to allow any file size
        /// </summary>
        public int ReturnRequestsFileMaximumSize { get; set; }
        /// <summary>
        /// Gets or sets a value "Return requests" number mask
        /// </summary>
        public string ReturnRequestNumberMask { get; set; }
        /// <summary>
        /// Gets or sets a number of days that the Return Request Link will be available for customers after subscription placing.
        /// </summary>
        public int NumberOfDaysReturnRequestAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to activate related gift cards after completing the subscription
        /// </summary>
        public bool ActivateGiftCardsAfterCompletingSubscription { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to deactivate related gift cards after cancelling the subscription
        /// </summary>
        public bool DeactivateGiftCardsAfterCancellingSubscription { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to deactivate related gift cards after deleting the subscription
        /// </summary>
        public bool DeactivateGiftCardsAfterDeletingSubscription { get; set; }

        /// <summary>
        /// Gets or sets an subscription placement interval in seconds (prevent 2 subscriptions being placed within an X seconds time frame).
        /// </summary>
        public int MinimumSubscriptionPlacementInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an subscription status should be set to "Complete" only when its shipping status is "Delivered". Otherwise, "Shipped" status will be enough.
        /// </summary>
        public bool CompleteSubscriptionWhenDelivered { get; set; }

        /// <summary>
        /// Gets or sets a custom subscription number mask
        /// </summary>
        public string CustomSubscriptionNumberMask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriptions need to be exported with their articles
        /// </summary>
        public bool ExportWithArticles { get; set; }
    }
}