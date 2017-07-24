namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article publisher mapping
    /// </summary>
    public partial class ArticlePublisher : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the publisher identifier
        /// </summary>
        public int PublisherId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is featured
        /// </summary>
        public bool IsFeaturedArticle { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }

        /// <summary>
        /// Gets or sets the publisher
        /// </summary>
        public virtual Publisher Publisher { get; set; }

        /// <summary>
        /// Gets or sets the article
        /// </summary>
        public virtual Article Article { get; set; }
    }

}
