using System.Collections.Generic;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Forums;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.News;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Stores;
using YStory.Core.Domain.Contributors;

namespace YStory.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        void AddStoreTokens(IList<Token> tokens, Store store, EmailAccount emailAccount);

        /// <summary>
        /// Add subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="contributorId">Contributor identifier</param>
        void AddSubscriptionTokens(IList<Token> tokens, Subscription subscription, int languageId, int contributorId = 0);

        /// <summary>
        /// Add refunded subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Subscription</param>
        /// <param name="refundedAmount">Refunded amount of subscription</param>
        void AddSubscriptionRefundedTokens(IList<Token> tokens, Subscription subscription, decimal refundedAmount);
 

        /// <summary>
        /// Add subscription note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscriptionNote">Subscription note</param>
        void AddSubscriptionNoteTokens(IList<Token> tokens, SubscriptionNote subscriptionNote);

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment);

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="subscriptionItem">Subscription item</param>
        void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, SubscriptionItem subscriptionItem);
 

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        void AddCustomerTokens(IList<Token> tokens, Customer customer);

        /// <summary>
        /// Add contributor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="contributor">Contributor</param>
        void AddContributorTokens(IList<Token> tokens, Contributor contributor);

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        /// <summary>
        /// Add article review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="articleReview">Article review</param>
        void AddArticleReviewTokens(IList<Token> tokens, ArticleReview articleReview);

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment);

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment);

        /// <summary>
        /// Add article tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="article">Article</param>
        /// <param name="languageId">Language identifier</param>
        void AddArticleTokens(IList<Token> tokens, Article article, int languageId);

        /// <summary>
        /// Add article attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">Article attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        void AddAttributeCombinationTokens(IList<Token> tokens, ArticleAttributeCombination combination, int languageId);

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        void AddForumTokens(IList<Token> tokens, Forum forum);

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost);

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage);

       

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>Collection of allowed (supported) message tokens for campaigns</returns>
        IEnumerable<string> GetListOfCampaignAllowedTokens();

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null);
    }
}
