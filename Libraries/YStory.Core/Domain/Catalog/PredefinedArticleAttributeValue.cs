using YStory.Core.Domain.Localization;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a predefined (default) article attribute value
    /// </summary>
    public partial class PredefinedArticleAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the article attribute identifier
        /// </summary>
        public int ArticleAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the article attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the attibute value cost
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }

        /// <summary>
        /// Gets the article attribute
        /// </summary>
        public virtual ArticleAttribute ArticleAttribute { get; set; }
    }
}
