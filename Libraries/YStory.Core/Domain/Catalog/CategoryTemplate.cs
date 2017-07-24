
namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a category template
    /// </summary>
    public partial class CategoryTemplate : BaseEntity
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
    }
}
