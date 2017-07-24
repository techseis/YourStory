namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article category mapping
    /// </summary>
    public partial class ArticleCategory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article is featured
        /// </summary>
        public bool IsFeaturedArticle { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }
        
        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }

    }

}
