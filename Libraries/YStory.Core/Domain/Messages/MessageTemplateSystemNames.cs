namespace YStory.Core.Domain.Messages
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region Customer

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string CustomerRegisteredNotification = "NewCustomer.Notification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerWelcomeMessage = "Customer.WelcomeMessage";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string CustomerEmailValidationMessage = "Customer.EmailValidationMessage";

        /// <summary>
        /// Represents system name of email revalidation message
        /// </summary>
        public const string CustomerEmailRevalidationMessage = "Customer.EmailRevalidationMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string CustomerPasswordRecoveryMessage = "Customer.PasswordRecovery";

        #endregion

        #region Subscription

        /// <summary>
        /// Represents system name of notification contributor about placed subscription
        /// </summary>
        public const string SubscriptionPlacedContributorNotification = "SubscriptionPlaced.ContributorNotification";

        /// <summary>
        /// Represents system name of notification store owner about placed subscription
        /// </summary>
        public const string SubscriptionPlacedStoreOwnerNotification = "SubscriptionPlaced.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about paid subscription
        /// </summary>
        public const string SubscriptionPaidStoreOwnerNotification = "SubscriptionPaid.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification custoemer about paid subscription
        /// </summary>
        public const string SubscriptionPaidCustomerNotification = "SubscriptionPaid.CustomerNotification";

        /// <summary>
        /// Represents system name of notification contributor about paid subscription
        /// </summary>
        public const string SubscriptionPaidContributorNotification = "SubscriptionPaid.ContributorNotification";

        /// <summary>
        /// Represents system name of notification customer about placed subscription
        /// </summary>
        public const string SubscriptionPlacedCustomerNotification = "SubscriptionPlaced.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about sent shipment
        /// </summary>
        public const string ShipmentSentCustomerNotification = "ShipmentSent.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about delivered shipment
        /// </summary>
        public const string ShipmentDeliveredCustomerNotification = "ShipmentDelivered.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about completed subscription
        /// </summary>
        public const string SubscriptionCompletedCustomerNotification = "SubscriptionCompleted.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about cancelled subscription
        /// </summary>
        public const string SubscriptionCancelledCustomerNotification = "SubscriptionCancelled.CustomerNotification";

        /// <summary>
        /// Represents system name of notification store owner about refunded subscription
        /// </summary>
        public const string SubscriptionRefundedStoreOwnerNotification = "SubscriptionRefunded.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about refunded subscription
        /// </summary>
        public const string SubscriptionRefundedCustomerNotification = "SubscriptionRefunded.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about new subscription note
        /// </summary>
        public const string NewSubscriptionNoteAddedCustomerNotification = "Customer.NewSubscriptionNote";

        /// <summary>
        /// Represents system name of notification store owner about cancelled recurring subscription
        /// </summary>
        public const string RecurringPaymentCancelledStoreOwnerNotification = "RecurringPaymentCancelled.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about cancelled recurring subscription
        /// </summary>
        public const string RecurringPaymentCancelledCustomerNotification = "RecurringPaymentCancelled.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about failed payment for the recurring payments
        /// </summary>
        public const string RecurringPaymentFailedCustomerNotification = "RecurringPaymentFailed.CustomerNotification";

        #endregion

        #region Newsletter

        /// <summary>
        /// Represents system name of subscription activation message
        /// </summary>
        public const string NewsletterSubscriptionActivationMessage = "NewsLetterSubscription.ActivationMessage";

        /// <summary>
        /// Represents system name of subscription deactivation message
        /// </summary>
        public const string NewsletterSubscriptionDeactivationMessage = "NewsLetterSubscription.DeactivationMessage";

        #endregion

        #region To friend

        /// <summary>
        /// Represents system name of 'Email a friend' message
        /// </summary>
        public const string EmailAFriendMessage = "Service.EmailAFriend";

        /// <summary>
        /// Represents system name of 'Email a friend' message with wishlist
        /// </summary>
        public const string WishlistToFriendMessage = "Wishlist.EmailAFriend";

        #endregion

        #region Return requests

        /// <summary>
        /// Represents system name of notification store owner about new return request
        /// </summary>
        public const string NewReturnRequestStoreOwnerNotification = "NewReturnRequest.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification customer about new return request
        /// </summary>
        public const string NewReturnRequestCustomerNotification = "NewReturnRequest.CustomerNotification";

        /// <summary>
        /// Represents system name of notification customer about changing return request status
        /// </summary>
        public const string ReturnRequestStatusChangedCustomerNotification = "ReturnRequestStatusChanged.CustomerNotification";

        #endregion

        #region Forum

        /// <summary>
        /// Represents system name of notification about new forum topic
        /// </summary>
        public const string NewForumTopicMessage = "Forums.NewForumTopic";

        /// <summary>
        /// Represents system name of notification about new forum post
        /// </summary>
        public const string NewForumPostMessage = "Forums.NewForumPost";

        /// <summary>
        /// Represents system name of notification about new private message
        /// </summary>
        public const string PrivateMessageNotification = "Customer.NewPM";

        #endregion

        #region Misc

        /// <summary>
        /// Represents system name of notification store owner about applying new contributor account
        /// </summary>
        public const string NewContributorAccountApplyStoreOwnerNotification = "ContributorAccountApply.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification contributor about changing information
        /// </summary>
        public const string ContributorInformationChangeNotification = "ContributorInformationChange.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification about gift card
        /// </summary>
        public const string GiftCardNotification = "GiftCard.Notification";

        /// <summary>
        /// Represents system name of notification store owner about new article review
        /// </summary>
        public const string ArticleReviewNotification = "Article.ArticleReview";

        /// <summary>
        /// Represents system name of notification store owner about below quantity of article
        /// </summary>
        public const string QuantityBelowStoreOwnerNotification = "QuantityBelow.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about below quantity of article attribute combination
        /// </summary>
        public const string QuantityBelowAttributeCombinationStoreOwnerNotification = "QuantityBelow.AttributeCombination.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about submitting new VAT
        /// </summary>
        public const string NewVatSubmittedStoreOwnerNotification = "NewVATSubmitted.StoreOwnerNotification";

        /// <summary>
        /// Represents system name of notification store owner about new blog comment
        /// </summary>
        public const string BlogCommentNotification = "Blog.BlogComment";

        /// <summary>
        /// Represents system name of notification store owner about new news comment
        /// </summary>
        public const string NewsCommentNotification = "News.NewsComment";

        /// <summary>
        /// Represents system name of notification customer about article receipt
        /// </summary>
        public const string BackInStockNotification = "Customer.BackInStock";

        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";
        /// <summary>
        /// Represents system name of 'Contact contributor' message
        /// </summary>
        public const string ContactContributorMessage = "Service.ContactContributor";

        #endregion
    }
}