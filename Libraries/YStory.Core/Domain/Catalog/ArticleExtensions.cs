using System;
using System.Collections.Generic;
using System.Linq;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Article extensions
    /// </summary>
    public static class ArticleExtensions
    {
        /// <summary>
        /// Parse "required article Ids" property
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>A list of required article IDs</returns>
        public static int[] ParseRequiredArticleIds(this Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (String.IsNullOrEmpty(article.RequiredArticleIds))
                return new int[0];

            var ids = new List<int>();

            foreach (var idStr in article.RequiredArticleIds
                .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
            {
                int id;
                if (int.TryParse(idStr, out id))
                    ids.Add(id);
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a article is available now (availability dates)
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Article article)
        {
            return IsAvailable(article, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a article is available now (availability dates)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Article article, DateTime dateTime)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (article.AvailableStartDateTimeUtc.HasValue && article.AvailableStartDateTimeUtc.Value > dateTime)
            {
                return false;
            }

            if (article.AvailableEndDateTimeUtc.HasValue && article.AvailableEndDateTimeUtc.Value < dateTime)
            {
                return false;
            }

            return true;
        }
    }
}
