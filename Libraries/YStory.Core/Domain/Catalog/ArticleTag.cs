using System.Collections.Generic;
using YStory.Core.Domain.Localization;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article tag
    /// </summary>
    public partial class ArticleTag : BaseEntity, ILocalizedEntity
    {
        private ICollection<Article> _articles;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the articles
        /// </summary>
        public virtual ICollection<Article> Articles
        {
            get { return _articles ?? (_articles = new List<Article>()); }
            protected set { _articles = value; }
        }
    }
}
