using YStory.Core.Configuration;

namespace YStory.Core.Domain.Common
{
    public class DisplayDefaultMenuItemSettings: ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "home page" menu item
        /// </summary>
        public bool DisplayHomePageMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new articles" menu item
        /// </summary>
        public bool DisplayNewArticlesMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "article search" menu item
        /// </summary>
        public bool DisplayArticleSearchMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "customer info" menu item
        /// </summary>
        public bool DisplayCustomerInfoMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" menu item
        /// </summary>
        public bool DisplayBlogMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "forums" menu item
        /// </summary>
        public bool DisplayForumsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" menu item
        /// </summary>
        public bool DisplayContactUsMenuItem { get; set; }
    }
}