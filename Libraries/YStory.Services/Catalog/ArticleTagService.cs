using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using YStory.Core.Caching;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Stores;
using YStory.Data;
using YStory.Services.Events;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article tag service
    /// </summary>
    public partial class ArticleTagService : IArticleTagService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string ARTICLETAG_COUNT_KEY = "YStory.articletag.count-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLETAG_PATTERN_KEY = "YStory.articletag.";

        #endregion

        #region Fields

        private readonly IRepository<ArticleTag> _articleTagRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IArticleService _articleService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="articleTagRepository">Article tag repository</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="articleService">Article service</param>
        public ArticleTagService(IRepository<ArticleTag> articleTagRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDataProvider dataProvider,
            IDbContext dbContext,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IArticleService articleService)
        {
            this._articleTagRepository = articleTagRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._articleService = articleService;
        }

        #endregion

        #region Nested classes

        private class ArticleTagWithCount
        {
            public int ArticleTagId { get; set; }
            public int ArticleCount { get; set; }
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Get article count for each of existing article tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "article tag ID : article count"</returns>
        private Dictionary<int, int> GetArticleCount(int storeId)
        {
            string key = string.Format(ARTICLETAG_COUNT_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {

                if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
                {
                    //stored procedures are enabled and supported by the database. 
                    //It's much faster than the LINQ implementation below 

                    #region Use stored procedure

                    //prepare parameters
                    var pStoreId = _dataProvider.GetParameter();
                    pStoreId.ParameterName = "StoreId";
                    pStoreId.Value = storeId;
                    pStoreId.DbType = DbType.Int32;


                    //invoke stored procedure
                    var result = _dbContext.SqlQuery<ArticleTagWithCount>(
                        "Exec ArticleTagCountLoadAll @StoreId",
                        pStoreId);

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in result)
                        dictionary.Add(item.ArticleTagId, item.ArticleCount);
                    return dictionary;

                    #endregion
                }
                else
                {
                    //stored procedures aren't supported. Use LINQ
                    #region Search articles
                    var query = _articleTagRepository.Table.Select(pt => new
                    {
                        Id = pt.Id,
                        ArticleCount = (storeId == 0 || _catalogSettings.IgnoreStoreLimitations) ?
                            pt.Articles.Count(p => !p.Deleted && p.Published)
                            : (from p in pt.Articles
                               join sm in _storeMappingRepository.Table
                               on new { p1 = p.Id, p2 = "Article" } equals new { p1 = sm.EntityId, p2 = sm.EntityName } into p_sm
                               from sm in p_sm.DefaultIfEmpty()
                               where (!p.LimitedToStores || storeId == sm.StoreId) && !p.Deleted && p.Published
                               select p).Count()
                    });
                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in query)
                        dictionary.Add(item.Id, item.ArticleCount);
                    return dictionary;

                    #endregion

                }
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void DeleteArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Delete(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleTag);
        }

        /// <summary>
        /// Gets all article tags
        /// </summary>
        /// <returns>Article tags</returns>
        public virtual IList<ArticleTag> GetAllArticleTags()
        {
            var query = _articleTagRepository.Table;
            var articleTags = query.ToList();
            return articleTags;
        }

        /// <summary>
        /// Gets article tag
        /// </summary>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <returns>Article tag</returns>
        public virtual ArticleTag GetArticleTagById(int articleTagId)
        {
            if (articleTagId == 0)
                return null;

            return _articleTagRepository.GetById(articleTagId);
        }

        /// <summary>
        /// Gets article tag by name
        /// </summary>
        /// <param name="name">Article tag name</param>
        /// <returns>Article tag</returns>
        public virtual ArticleTag GetArticleTagByName(string name)
        {
            var query = from pt in _articleTagRepository.Table
                        where pt.Name == name
                        select pt;

            var articleTag = query.FirstOrDefault();
            return articleTag;
        }

        /// <summary>
        /// Inserts a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void InsertArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Insert(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleTag);
        }

        /// <summary>
        /// Updates the article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void UpdateArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Update(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleTag);
        }

        /// <summary>
        /// Get number of articles
        /// </summary>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of articles</returns>
        public virtual int GetArticleCount(int articleTagId, int storeId)
        {
            var dictionary = GetArticleCount(storeId);
            if (dictionary.ContainsKey(articleTagId))
                return dictionary[articleTagId];
            
            return 0;
        }

        /// <summary>
        /// Update article tags
        /// </summary>
        /// <param name="article">Article for update</param>
        /// <param name="articleTags">Article tags</param>
        public virtual void UpdateArticleTags(Article article, string[] articleTags)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //article tags
            var existingArticleTags = article.ArticleTags.ToList();
            var articleTagsToRemove = new List<ArticleTag>();
            foreach (var existingArticleTag in existingArticleTags)
            {
                var found = false;
                foreach (var newArticleTag in articleTags)
                {
                    if (existingArticleTag.Name.Equals(newArticleTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    articleTagsToRemove.Add(existingArticleTag);
                }
            }
            foreach (var articleTag in articleTagsToRemove)
            {
                article.ArticleTags.Remove(articleTag);
                _articleService.UpdateArticle(article);
            }
            foreach (var articleTagName in articleTags)
            {
                ArticleTag articleTag;
                var articleTag2 = GetArticleTagByName(articleTagName);
                if (articleTag2 == null)
                {
                    //add new article tag
                    articleTag = new ArticleTag
                    {
                        Name = articleTagName
                    };
                    InsertArticleTag(articleTag);
                }
                else
                {
                    articleTag = articleTag2;
                }
                if (!article.ArticleTagExists(articleTag.Id))
                {
                    article.ArticleTags.Add(articleTag);
                    _articleService.UpdateArticle(article);
                }
            }
        }
        #endregion
    }
}
