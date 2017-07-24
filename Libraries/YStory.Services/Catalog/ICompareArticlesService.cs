using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Compare articles service interface
    /// </summary>
    public partial interface ICompareArticlesService
    {
        /// <summary>
        /// Clears a "compare articles" list
        /// </summary>
        void ClearCompareArticles();

        /// <summary>
        /// Gets a "compare articles" list
        /// </summary>
        /// <returns>"Compare articles" list</returns>
        IList<Article> GetComparedArticles();

        /// <summary>
        /// Removes a article from a "compare articles" list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        void RemoveArticleFromCompareList(int articleId);

        /// <summary>
        /// Adds a article to a "compare articles" list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        void AddArticleToCompareList(int articleId);
    }
}
