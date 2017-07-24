using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Recently viewed articles service
    /// </summary>
    public partial interface IRecentlyViewedArticlesService
    {
        /// <summary>
        /// Gets a "recently viewed articles" list
        /// </summary>
        /// <param name="number">Number of articles to load</param>
        /// <returns>"recently viewed articles" list</returns>
        IList<Article> GetRecentlyViewedArticles(int number);

        /// <summary>
        /// Adds a article to a recently viewed articles list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        void AddArticleToRecentlyViewedList(int articleId);
    }
}
