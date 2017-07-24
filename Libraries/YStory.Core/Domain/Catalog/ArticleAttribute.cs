using YStory.Core.Domain.Localization;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article attribute
    /// </summary>
    public partial class ArticleAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }
    }
}
