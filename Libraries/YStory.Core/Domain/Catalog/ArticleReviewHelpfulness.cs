

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article review helpfulness
    /// </summary>
    public partial class ArticleReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article review identifier
        /// </summary>
        public int ArticleReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual ArticleReview ArticleReview { get; set; }
    }
}
