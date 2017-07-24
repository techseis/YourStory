using System.Collections.Generic;
using YStory.Core.Domain.Messages;

namespace YStory.Services.Messages
{
    /// <summary>
    /// Represents message template  extensions
    /// </summary>
    public static class MessageTemplateExtensions
    {
        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public static IEnumerable<string> GetTokenGroups(this MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            switch (messageTemplate.Name)
            {
                case MessageTemplateSystemNames.CustomerRegisteredNotification:
                case MessageTemplateSystemNames.CustomerWelcomeMessage:
                case MessageTemplateSystemNames.CustomerEmailValidationMessage:
                case MessageTemplateSystemNames.CustomerEmailRevalidationMessage:
                case MessageTemplateSystemNames.CustomerPasswordRecoveryMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.SubscriptionPlacedContributorNotification:
                case MessageTemplateSystemNames.SubscriptionPlacedStoreOwnerNotification:
                case MessageTemplateSystemNames.SubscriptionPaidStoreOwnerNotification:
                case MessageTemplateSystemNames.SubscriptionPaidCustomerNotification:
                case MessageTemplateSystemNames.SubscriptionPaidContributorNotification:
                case MessageTemplateSystemNames.SubscriptionPlacedCustomerNotification:
                case MessageTemplateSystemNames.SubscriptionCompletedCustomerNotification:
                case MessageTemplateSystemNames.SubscriptionCancelledCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.ShipmentSentCustomerNotification:
                case MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ShipmentTokens, TokenGroupNames.SubscriptionTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.SubscriptionRefundedStoreOwnerNotification:
                case MessageTemplateSystemNames.SubscriptionRefundedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens, TokenGroupNames.RefundedSubscriptionTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewSubscriptionNoteAddedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionNoteTokens, TokenGroupNames.SubscriptionTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification:
                case MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification:
                case MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.RecurringPaymentTokens };

                case MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage:
                case MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens };

                case MessageTemplateSystemNames.EmailAFriendMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ArticleTokens, TokenGroupNames.EmailAFriendTokens };

                case MessageTemplateSystemNames.WishlistToFriendMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.WishlistToFriendTokens };

                case MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification:
                case MessageTemplateSystemNames.NewReturnRequestCustomerNotification:
                case MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ReturnRequestTokens };

                case MessageTemplateSystemNames.NewForumTopicMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewForumPostMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumPostTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.PrivateMessageNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.PrivateMessageTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewContributorAccountApplyStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ContributorTokens };

                case MessageTemplateSystemNames.ContributorInformationChangeNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContributorTokens };

                case MessageTemplateSystemNames.GiftCardNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.GiftCardTokens};

                case MessageTemplateSystemNames.ArticleReviewNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ArticleReviewTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ArticleTokens };

                case MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ArticleTokens, TokenGroupNames.AttributeCombinationTokens };

                case MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.VatValidation };

                case MessageTemplateSystemNames.BlogCommentNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewsCommentNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.BackInStockNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ArticleBackInStockTokens };

                case MessageTemplateSystemNames.ContactUsMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactUs };

                case MessageTemplateSystemNames.ContactContributorMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactContributor };

                default:
                    return new string[] { };
            }
        }
    }
}