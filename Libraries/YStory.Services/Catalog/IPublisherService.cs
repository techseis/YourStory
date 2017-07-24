using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Publisher service
    /// </summary>
    public partial interface IPublisherService
    {
        /// <summary>
        /// Deletes a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void DeletePublisher(Publisher publisher);
        
        /// <summary>
        /// Gets all publishers
        /// </summary>
        /// <param name="publisherName">Publisher name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Publishers</returns>
        IPagedList<Publisher> GetAllPublishers(string publisherName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false);

        /// <summary>
        /// Gets a publisher
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns>Publisher</returns>
        Publisher GetPublisherById(int publisherId);

        /// <summary>
        /// Inserts a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void InsertPublisher(Publisher publisher);

        /// <summary>
        /// Updates the publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        void UpdatePublisher(Publisher publisher);
        

        /// <summary>
        /// Deletes a article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        void DeleteArticlePublisher(ArticlePublisher articlePublisher);
        
        /// <summary>
        /// Gets article publisher collection
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article publisher collection</returns>
        IPagedList<ArticlePublisher> GetArticlePublishersByPublisherId(int publisherId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a article publisher mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article publisher mapping collection</returns>
        IList<ArticlePublisher> GetArticlePublishersByArticleId(int articleId, bool showHidden = false);
        
        /// <summary>
        /// Gets a article publisher mapping 
        /// </summary>
        /// <param name="articlePublisherId">Article publisher mapping identifier</param>
        /// <returns>Article publisher mapping</returns>
        ArticlePublisher GetArticlePublisherById(int articlePublisherId);

        /// <summary>
        /// Inserts a article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        void InsertArticlePublisher(ArticlePublisher articlePublisher);

        /// <summary>
        /// Updates the article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        void UpdateArticlePublisher(ArticlePublisher articlePublisher);

        /// <summary>
        /// Get publisher IDs for articles
        /// </summary>
        /// <param name="articleIds">Articles IDs</param>
        /// <returns>Publisher IDs for articles</returns>
        IDictionary<int, int[]> GetArticlePublisherIds(int[] articleIds);

        /// <summary>
        /// Returns a list of names of not existing publishers
        /// </summary>
        /// <param name="publisherNames">The names of the publishers to check</param>
        /// <returns>List of names not existing publishers</returns>
        string[] GetNotExistingPublishers(string[] publisherNames);
    }
}
