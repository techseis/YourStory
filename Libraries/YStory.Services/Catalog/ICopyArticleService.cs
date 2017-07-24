
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Copy article service
    /// </summary>
    public partial interface ICopyArticleService
    {
        /// <summary>
        /// Create a copy of article with all depended data
        /// </summary>
        /// <param name="article">The article to copy</param>
        /// <param name="newName">The name of article duplicate</param>
        /// <param name="isPublished">A value indicating whether the article duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the article images should be copied</param>
        /// <param name="copyAssociatedArticles">A value indicating whether the copy associated articles</param>
        /// <returns>Article copy</returns>
        Article CopyArticle(Article article, string newName,
            bool isPublished = true, bool copyImages = true, bool copyAssociatedArticles = true);
    }
}
