using System;
using System.Collections.Generic;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Stores;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article review
    /// </summary>
    public partial class ArticleReview : BaseEntity
    {
        private ICollection<ArticleReviewHelpfulness> _articleReviewHelpfulnessEntries;

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the review text
        /// </summary>
        public string ReviewText { get; set; }

        /// <summary>
        /// Gets or sets the reply text
        /// </summary>
        public string ReplyText { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }

        /// <summary>
        /// Gets or sets the store
        /// </summary>
        public virtual Store Store { get; set; }

        /// <summary>
        /// Gets the entries of article review helpfulness
        /// </summary>
        public virtual ICollection<ArticleReviewHelpfulness> ArticleReviewHelpfulnessEntries
        {
            get { return _articleReviewHelpfulnessEntries ?? (_articleReviewHelpfulnessEntries = new List<ArticleReviewHelpfulness>()); }
            protected set { _articleReviewHelpfulnessEntries = value; }
        }
    }
}
