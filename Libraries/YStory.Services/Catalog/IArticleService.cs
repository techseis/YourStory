using System;
using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
 

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article service
    /// </summary>
    public partial interface IArticleService
    {
        #region Articles

        /// <summary>
        /// Delete a article
        /// </summary>
        /// <param name="article">Article</param>
        void DeleteArticle(Article article);

        /// <summary>
        /// Delete articles
        /// </summary>
        /// <param name="articles">Articles</param>
        void DeleteArticles(IList<Article> articles);

        /// <summary>
        /// Gets all articles displayed on the home page
        /// </summary>
        /// <returns>Articles</returns>
        IList<Article> GetAllArticlesDisplayedOnHomePage();
        
        /// <summary>
        /// Gets article
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article</returns>
        Article GetArticleById(int articleId);
        
        /// <summary>
        /// Gets articles by identifier
        /// </summary>
        /// <param name="articleIds">Article identifiers</param>
        /// <returns>Articles</returns>
        IList<Article> GetArticlesByIds(int[] articleIds);

        /// <summary>
        /// Inserts a article
        /// </summary>
        /// <param name="article">Article</param>
        void InsertArticle(Article article);

        /// <summary>
        /// Updates the article
        /// </summary>
        /// <param name="article">Article</param>
        void UpdateArticle(Article article);

        /// <summary>
        /// Updates the articles
        /// </summary>
        /// <param name="articles">Article</param>
        void UpdateArticles(IList<Article> articles);

        /// <summary>
        /// Get number of article (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Number of articles</returns>
        int GetNumberOfArticlesInCategory(IList<int> categoryIds = null, int storeId = 0);

        /// <summary>
        /// Search articles
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="publisherId">Publisher identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="contributorId">Contributor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="articleType">Article type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only articles marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only articles marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredArticles">A value indicating whether loaded articles are marked as featured (relates only to categories and publishers). 0 to load featured articles only, 1 to load not featured articles only, null to load all articles</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="articleTagId">Article tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in article descriptions</param>
        /// <param name="searchPublisherPartNumber">A value indicating whether to search by a specified "keyword" in publisher part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in article SKU</param>
        /// <param name="searchArticleTags">A value indicating whether to search by a specified "keyword" in article tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered article specification identifiers</param>
        /// <param name="subscriptionBy">Subscription by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" articles
        /// false - load only "Unpublished" articles
        /// </param>
        /// <returns>Articles</returns>
        IPagedList<Article> SearchArticles(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int publisherId = 0,
            int storeId = 0,
            int contributorId = 0,
            int warehouseId = 0,
            ArticleType? articleType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredArticles = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int articleTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchPublisherPartNumber = true,
            bool searchSku = true,
            bool searchArticleTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ArticleSortingEnum subscriptionBy = ArticleSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Search articles
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded articles (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded articles (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="publisherId">Publisher identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="contributorId">Contributor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="articleType">Article type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only articles marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only articles marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredArticles">A value indicating whether loaded articles are marked as featured (relates only to categories and publishers). 0 to load featured articles only, 1 to load not featured articles only, null to load all articles</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="articleTagId">Article tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in article descriptions</param>
        /// <param name="searchPublisherPartNumber">A value indicating whether to search by a specified "keyword" in publisher part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in article SKU</param>
        /// <param name="searchArticleTags">A value indicating whether to search by a specified "keyword" in article tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered article specification identifiers</param>
        /// <param name="subscriptionBy">Subscription by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" articles
        /// false - load only "Unpublished" articles
        /// </param>
        /// <returns>Articles</returns>
        IPagedList<Article> SearchArticles(
            out IList<int> filterableSpecificationAttributeOptionIds,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int publisherId = 0,
            int storeId = 0,
            int contributorId = 0,
            int warehouseId = 0,
            ArticleType? articleType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredArticles = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int articleTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchPublisherPartNumber = true,
            bool searchSku = true,
            bool searchArticleTags = false, 
            int languageId = 0,
            IList<int> filteredSpecs = null, 
            ArticleSortingEnum subscriptionBy = ArticleSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Gets articles by article attribute
        /// </summary>
        /// <param name="articleAttributeId">Article attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Articles</returns>
        IPagedList<Article> GetArticlesByArticleAtributeId(int articleAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets associated articles
        /// </summary>
        /// <param name="parentGroupedArticleId">Parent article identifier (used with grouped articles)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="contributorId">Contributor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Articles</returns>
        IList<Article> GetAssociatedArticles(int parentGroupedArticleId,
            int storeId = 0, int contributorId = 0, bool showHidden = false);

        /// <summary>
        /// Update article review totals
        /// </summary>
        /// <param name="article">Article</param>
        void UpdateArticleReviewTotals(Article article);

      

        /// <summary>
        /// Gets a article by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Article</returns>
        Article GetArticleBySku(string sku);

        /// <summary>
        /// Gets a articles by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="contributorId">Contributor ID; 0 to load all records</param>
        /// <returns>Articles</returns>
        IList<Article> GetArticlesBySku(string[] skuArray, int contributorId = 0);

    

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="article">Article</param>
        void UpdateHasDiscountsApplied(Article article);

        /// <summary>
        /// Gets number of articles by contributor identifier
        /// </summary>
        /// <param name="contributorId">Contributor identifier</param>
        /// <returns>Number of articles</returns>
        int GetNumberOfArticlesByContributorId(int contributorId);

        #endregion

        

        #region Related articles

        /// <summary>
        /// Deletes a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void DeleteRelatedArticle(RelatedArticle relatedArticle);

        /// <summary>
        /// Gets related articles by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related articles</returns>
        IList<RelatedArticle> GetRelatedArticlesByArticleId1(int articleId1, bool showHidden = false);

        /// <summary>
        /// Gets a related article
        /// </summary>
        /// <param name="relatedArticleId">Related article identifier</param>
        /// <returns>Related article</returns>
        RelatedArticle GetRelatedArticleById(int relatedArticleId);

        /// <summary>
        /// Inserts a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void InsertRelatedArticle(RelatedArticle relatedArticle);

        /// <summary>
        /// Updates a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void UpdateRelatedArticle(RelatedArticle relatedArticle);

        #endregion

        #region Cross-sell articles

        /// <summary>
        /// Deletes a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell</param>
        void DeleteCrossSellArticle(CrossSellArticle crossSellArticle);

        /// <summary>
        /// Gets cross-sell articles by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell articles</returns>
        IList<CrossSellArticle> GetCrossSellArticlesByArticleId1(int articleId1, bool showHidden = false);

        /// <summary>
        /// Gets a cross-sell article
        /// </summary>
        /// <param name="crossSellArticleId">Cross-sell article identifier</param>
        /// <returns>Cross-sell article</returns>
        CrossSellArticle GetCrossSellArticleById(int crossSellArticleId);

        /// <summary>
        /// Inserts a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell article</param>
        void InsertCrossSellArticle(CrossSellArticle crossSellArticle);

        /// <summary>
        /// Updates a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell article</param>
        void UpdateCrossSellArticle(CrossSellArticle crossSellArticle);
        
        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfArticles">Number of articles to return</param>
        /// <returns>Cross-sells</returns>
        IList<Article> GetCrosssellArticlesByShoppingCart(IList<ShoppingCartItem> cart, int numberOfArticles);

        #endregion
        

        #region Article pictures

        /// <summary>
        /// Deletes a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        void DeleteArticlePicture(ArticlePicture articlePicture);

        /// <summary>
        /// Gets a article pictures by article identifier
        /// </summary>
        /// <param name="articleId">The article identifier</param>
        /// <returns>Article pictures</returns>
        IList<ArticlePicture> GetArticlePicturesByArticleId(int articleId);

        /// <summary>
        /// Gets a article picture
        /// </summary>
        /// <param name="articlePictureId">Article picture identifier</param>
        /// <returns>Article picture</returns>
        ArticlePicture GetArticlePictureById(int articlePictureId);

        /// <summary>
        /// Inserts a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        void InsertArticlePicture(ArticlePicture articlePicture);

        /// <summary>
        /// Updates a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        void UpdateArticlePicture(ArticlePicture articlePicture);

        /// <summary>
        /// Get the IDs of all article images 
        /// </summary>
        /// <param name="articlesIds">Articles IDs</param>
        /// <returns>All picture identifiers grouped by article ID</returns>
        IDictionary<int, int[]> GetArticlesImagesIds(int [] articlesIds);

        #endregion

        #region Article reviews

        /// <summary>
        /// Gets all article reviews
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="articleId">The article identifier; pass 0 to load all records</param>
        /// <param name="contributorId">The contributor identifier (limit to articles of this contributor); pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reviews</returns>
        IPagedList<ArticleReview> GetAllArticleReviews(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int articleId = 0, int contributorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets article review
        /// </summary>
        /// <param name="articleReviewId">Article review identifier</param>
        /// <returns>Article review</returns>
        ArticleReview GetArticleReviewById(int articleReviewId);

        /// <summary>
        /// Get article reviews by identifiers
        /// </summary>
        /// <param name="articleReviewIds">Article review identifiers</param>
        /// <returns>Article reviews</returns>
        IList<ArticleReview> GetProducReviewsByIds(int[] articleReviewIds);

        /// <summary>
        /// Deletes a article review
        /// </summary>
        /// <param name="articleReview">Article review</param>
        void DeleteArticleReview(ArticleReview articleReview);

        /// <summary>
        /// Deletes article reviews
        /// </summary>
        /// <param name="articleReviews">Article reviews</param>
        void DeleteArticleReviews(IList<ArticleReview> articleReviews);

        #endregion

        

        
    }
}
