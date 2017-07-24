namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a cross-sell article
    /// </summary>
    public partial class CrossSellArticle : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first article identifier
        /// </summary>
        public int ArticleId1 { get; set; }

        /// <summary>
        /// Gets or sets the second article identifier
        /// </summary>
        public int ArticleId2 { get; set; }
    }

}
