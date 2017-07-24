
namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article template
    /// </summary>
    public partial class ArticleTemplate : BaseEntity
    {
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of article type identifiers NOT supported by this template
        /// </summary>
        public string IgnoredArticleTypes { get; set; }
    }
}
