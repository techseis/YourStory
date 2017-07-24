using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class PublisherExtensions
    {
        /// <summary>
        /// Returns a ArticlePublisher that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="articleId">Article identifier</param>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns>A ArticlePublisher that has the specified values; otherwise null</returns>
        public static ArticlePublisher FindArticlePublisher(this IList<ArticlePublisher> source,
            int articleId, int publisherId)
        {
            foreach (var articlePublisher in source)
                if (articlePublisher.ArticleId == articleId && articlePublisher.PublisherId == publisherId)
                    return articlePublisher;

            return null;
        }

    }
}
