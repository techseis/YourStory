
using YStory.Core.Domain.Media;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article picture mapping
    /// </summary>
    public partial class ArticlePicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }
        
        /// <summary>
        /// Gets the picture
        /// </summary>
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }
    }

}
