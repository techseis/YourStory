namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a related article
    /// </summary>
    public partial class RelatedArticle : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first article identifier
        /// </summary>
        public int ArticleId1 { get; set; }

        /// <summary>
        /// Gets or sets the second article identifier
        /// </summary>
        public int ArticleId2 { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }
    }

}
