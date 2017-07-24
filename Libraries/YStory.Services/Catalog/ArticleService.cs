using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Security;
using YStory.Core.Domain.Stores;
using YStory.Data;
using YStory.Services.Customers;
using YStory.Services.Events;
using YStory.Services.Localization;
using YStory.Services.Messages;
using YStory.Services.Security;
using YStory.Services.Stores;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article service
    /// </summary>
    public partial class ArticleService : IArticleService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article ID
        /// </remarks>
        private const string ARTICLES_BY_ID_KEY = "YStory.article.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLES_PATTERN_KEY = "YStory.article.";
        #endregion

        #region Fields

        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<RelatedArticle> _relatedArticleRepository;
        private readonly IRepository<CrossSellArticle> _crossSellArticleRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<ArticlePicture> _articlePictureRepository;
        private readonly IRepository<ArticleSpecificationAttribute> _articleSpecificationAttributeRepository;
        private readonly IRepository<ArticleReview> _articleReviewRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="articleRepository">Article repository</param>
        /// <param name="relatedArticleRepository">Related article repository</param>
        /// <param name="crossSellArticleRepository">Cross-sell article repository</param>
        /// <param name="tierPriceRepository">Tier price repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="articlePictureRepository">Article picture repository</param>
        /// <param name="articleSpecificationAttributeRepository">Article specification attribute repository</param>
        /// <param name="articleReviewRepository">Article review repository</param>
        /// <param name="articleWarehouseInventoryRepository">Article warehouse inventory repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="stockQuantityHistoryRepository">Stock quantity history repository</param>
        /// <param name="articleAttributeService">Article attribute service</param>
        /// <param name="articleAttributeParser">Article attribute parser service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        public ArticleService(ICacheManager cacheManager,
            IRepository<Article> articleRepository,
            IRepository<RelatedArticle> relatedArticleRepository,
            IRepository<CrossSellArticle> crossSellArticleRepository,
            IRepository<ArticlePicture> articlePictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<ArticleSpecificationAttribute> articleSpecificationAttributeRepository,
            IRepository<ArticleReview>  articleReviewRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IArticleAttributeService articleAttributeService,
            IArticleAttributeParser articleAttributeParser,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, 
            IDbContext dbContext,
            IWorkContext workContext,
            LocalizationSettings localizationSettings, 
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IAclService aclService,
            IStoreMappingService storeMappingService)
        {
            this._cacheManager = cacheManager;
            this._articleRepository = articleRepository;
            this._relatedArticleRepository = relatedArticleRepository;
            this._crossSellArticleRepository = crossSellArticleRepository;
            this._articlePictureRepository = articlePictureRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._articleSpecificationAttributeRepository = articleSpecificationAttributeRepository;
            this._articleReviewRepository = articleReviewRepository;
            this._specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            this._articleAttributeService = articleAttributeService;
            this._articleAttributeParser = articleAttributeParser;
            this._languageService = languageService;
            this._workflowMessageService = workflowMessageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
        }

        #endregion
        
        #region Methods

        #region Articles

        /// <summary>
        /// Delete a article
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void DeleteArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            article.Deleted = true;
            //delete article
            UpdateArticle(article);

            //event notification
            _eventPublisher.EntityDeleted(article);
        }

        /// <summary>
        /// Delete articles
        /// </summary>
        /// <param name="articles">Articles</param>
        public virtual void DeleteArticles(IList<Article> articles)
        {
            if (articles == null)
                throw new ArgumentNullException("articles");

            foreach (var article in articles)
            {
                article.Deleted = true;
            }

            //delete article
            UpdateArticles(articles);

            foreach (var article in articles)
            {
                //event notification
                _eventPublisher.EntityDeleted(article);
            }
        }

        /// <summary>
        /// Gets all articles displayed on the home page
        /// </summary>
        /// <returns>Articles</returns>
        public virtual IList<Article> GetAllArticlesDisplayedOnHomePage()
        {
            var query = from p in _articleRepository.Table
                        orderby p.DisplaySubscription, p.Id
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var articles = query.ToList();
            return articles;
        }
        
        /// <summary>
        /// Gets article
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article</returns>
        public virtual Article GetArticleById(int articleId)
        {
            if (articleId == 0)
                return null;
            
            string key = string.Format(ARTICLES_BY_ID_KEY, articleId);
            return _cacheManager.Get(key, () => _articleRepository.GetById(articleId));
        }

        /// <summary>
        /// Get articles by identifiers
        /// </summary>
        /// <param name="articleIds">Article identifiers</param>
        /// <returns>Articles</returns>
        public virtual IList<Article> GetArticlesByIds(int[] articleIds)
        {
            if (articleIds == null || articleIds.Length == 0)
                return new List<Article>();

            var query = from p in _articleRepository.Table
                        where articleIds.Contains(p.Id) && !p.Deleted
                        select p;
            var articles = query.ToList();
            //sort by passed identifiers
            var sortedArticles = new List<Article>();
            foreach (int id in articleIds)
            {
                var article = articles.Find(x => x.Id == id);
                if (article != null)
                    sortedArticles.Add(article);
            }
            return sortedArticles;
        }

        /// <summary>
        /// Inserts a article
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void InsertArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //insert
            _articleRepository.Insert(article);

            //clear cache
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            
            //event notification
            _eventPublisher.EntityInserted(article);
        }

        /// <summary>
        /// Updates the article
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void UpdateArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //update
            _articleRepository.Update(article);

            //cache
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(article);
        }

        public virtual void UpdateArticles(IList<Article> articles)
        {
            if (articles == null)
                throw new ArgumentNullException("articles");

            //update
            _articleRepository.Update(articles);

            //cache
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);

            //event notification
            foreach (var article in articles)
            {
                _eventPublisher.EntityUpdated(article);
            }
        }

        /// <summary>
        /// Get number of article (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Number of articles</returns>
        public virtual int GetNumberOfArticlesInCategory(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _articleRepository.Table;
            query = query.Where(p => !p.Deleted && p.Published && p.VisibleIndividually);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        from pc in p.ArticleCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        select p;
            }

            if (!_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Article" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Article" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //only distinct articles
            var result = query.Select(p => p.Id).Distinct().Count();
            return result;
        }

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
        public virtual IPagedList<Article> SearchArticles(
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
            bool? overridePublished = null)
        {
            IList<int> filterableSpecificationAttributeOptionIds;
            return SearchArticles(out filterableSpecificationAttributeOptionIds, false,
                pageIndex, pageSize, categoryIds, publisherId,
                storeId, contributorId, warehouseId,
                articleType, visibleIndividuallyOnly, markedAsNewOnly, featuredArticles,
                priceMin, priceMax, articleTagId, keywords, searchDescriptions, searchPublisherPartNumber, searchSku,
                searchArticleTags, languageId, filteredSpecs, 
                subscriptionBy, showHidden, overridePublished);
        }

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
        public virtual IPagedList<Article> SearchArticles(
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
            bool? overridePublished = null)
        {
            filterableSpecificationAttributeOptionIds = new List<int>();

            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds !=null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Use stored procedure
                
                //pass category identifiers as comma-delimited string
                string commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);


                //pass customer role identifiers as comma-delimited string
                string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);


                //pass specification identifiers as comma-delimited string
                string commaSeparatedSpecIds = "";
                if (filteredSpecs != null)
                {
                    ((List<int>)filteredSpecs).Sort();
                    commaSeparatedSpecIds = string.Join(",", filteredSpecs);
                }

                //some databases don't support int.MaxValue
                if (pageSize == int.MaxValue)
                    pageSize = int.MaxValue - 1;
                
                //prepare parameters
                var pCategoryIds = _dataProvider.GetParameter();
                pCategoryIds.ParameterName = "CategoryIds";
                pCategoryIds.Value = commaSeparatedCategoryIds;
                pCategoryIds.DbType = DbType.String;
                
                var pPublisherId = _dataProvider.GetParameter();
                pPublisherId.ParameterName = "PublisherId";
                pPublisherId.Value = publisherId;
                pPublisherId.DbType = DbType.Int32;

                var pStoreId = _dataProvider.GetParameter();
                pStoreId.ParameterName = "StoreId";
                pStoreId.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
                pStoreId.DbType = DbType.Int32;

                var pContributorId = _dataProvider.GetParameter();
                pContributorId.ParameterName = "ContributorId";
                pContributorId.Value = contributorId;
                pContributorId.DbType = DbType.Int32;

                var pWarehouseId = _dataProvider.GetParameter();
                pWarehouseId.ParameterName = "WarehouseId";
                pWarehouseId.Value = warehouseId;
                pWarehouseId.DbType = DbType.Int32;

                var pArticleTypeId = _dataProvider.GetParameter();
                pArticleTypeId.ParameterName = "ArticleTypeId";
                pArticleTypeId.Value = articleType.HasValue ? (object)articleType.Value : DBNull.Value;
                pArticleTypeId.DbType = DbType.Int32;

                var pVisibleIndividuallyOnly = _dataProvider.GetParameter();
                pVisibleIndividuallyOnly.ParameterName = "VisibleIndividuallyOnly";
                pVisibleIndividuallyOnly.Value = visibleIndividuallyOnly;
                pVisibleIndividuallyOnly.DbType = DbType.Int32;

                var pMarkedAsNewOnly = _dataProvider.GetParameter();
                pMarkedAsNewOnly.ParameterName = "MarkedAsNewOnly";
                pMarkedAsNewOnly.Value = markedAsNewOnly;
                pMarkedAsNewOnly.DbType = DbType.Int32;

                var pArticleTagId = _dataProvider.GetParameter();
                pArticleTagId.ParameterName = "ArticleTagId";
                pArticleTagId.Value = articleTagId;
                pArticleTagId.DbType = DbType.Int32;

                var pFeaturedArticles = _dataProvider.GetParameter();
                pFeaturedArticles.ParameterName = "FeaturedArticles";
                pFeaturedArticles.Value = featuredArticles.HasValue ? (object)featuredArticles.Value : DBNull.Value;
                pFeaturedArticles.DbType = DbType.Boolean;

                var pPriceMin = _dataProvider.GetParameter();
                pPriceMin.ParameterName = "PriceMin";
                pPriceMin.Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value;
                pPriceMin.DbType = DbType.Decimal;
                
                var pPriceMax = _dataProvider.GetParameter();
                pPriceMax.ParameterName = "PriceMax";
                pPriceMax.Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value;
                pPriceMax.DbType = DbType.Decimal;

                var pKeywords = _dataProvider.GetParameter();
                pKeywords.ParameterName = "Keywords";
                pKeywords.Value = keywords != null ? (object)keywords : DBNull.Value;
                pKeywords.DbType = DbType.String;

                var pSearchDescriptions = _dataProvider.GetParameter();
                pSearchDescriptions.ParameterName = "SearchDescriptions";
                pSearchDescriptions.Value = searchDescriptions;
                pSearchDescriptions.DbType = DbType.Boolean;

                var pSearchPublisherPartNumber = _dataProvider.GetParameter();
                pSearchPublisherPartNumber.ParameterName = "SearchPublisherPartNumber";
                pSearchPublisherPartNumber.Value = searchPublisherPartNumber;
                pSearchPublisherPartNumber.DbType = DbType.Boolean;

                var pSearchSku = _dataProvider.GetParameter();
                pSearchSku.ParameterName = "SearchSku";
                pSearchSku.Value = searchSku;
                pSearchSku.DbType = DbType.Boolean;

                var pSearchArticleTags = _dataProvider.GetParameter();
                pSearchArticleTags.ParameterName = "SearchArticleTags";
                pSearchArticleTags.Value = searchArticleTags;
                pSearchArticleTags.DbType = DbType.Boolean;

                var pUseFullTextSearch = _dataProvider.GetParameter();
                pUseFullTextSearch.ParameterName = "UseFullTextSearch";
                pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
                pUseFullTextSearch.DbType = DbType.Boolean;

                var pFullTextMode = _dataProvider.GetParameter();
                pFullTextMode.ParameterName = "FullTextMode";
                pFullTextMode.Value = (int)_commonSettings.FullTextMode;
                pFullTextMode.DbType = DbType.Int32;

                var pFilteredSpecs = _dataProvider.GetParameter();
                pFilteredSpecs.ParameterName = "FilteredSpecs";
                pFilteredSpecs.Value = commaSeparatedSpecIds;
                pFilteredSpecs.DbType = DbType.String;

                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = searchLocalizedValue ? languageId : 0;
                pLanguageId.DbType = DbType.Int32;

                var pOrderBy = _dataProvider.GetParameter();
                pOrderBy.ParameterName = "OrderBy";
                pOrderBy.Value = (int)subscriptionBy;
                pOrderBy.DbType = DbType.Int32;

                var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
                pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
                pAllowedCustomerRoleIds.Value = !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : "";
                pAllowedCustomerRoleIds.DbType = DbType.String;

                var pPageIndex = _dataProvider.GetParameter();
                pPageIndex.ParameterName = "PageIndex";
                pPageIndex.Value = pageIndex;
                pPageIndex.DbType = DbType.Int32;

                var pPageSize = _dataProvider.GetParameter();
                pPageSize.ParameterName = "PageSize";
                pPageSize.Value = pageSize;
                pPageSize.DbType = DbType.Int32;

                var pShowHidden = _dataProvider.GetParameter();
                pShowHidden.ParameterName = "ShowHidden";
                pShowHidden.Value = showHidden;
                pShowHidden.DbType = DbType.Boolean;

                var pOverridePublished = _dataProvider.GetParameter();
                pOverridePublished.ParameterName = "OverridePublished";
                pOverridePublished.Value = overridePublished != null ? (object)overridePublished.Value : DBNull.Value;
                pOverridePublished.DbType = DbType.Boolean;

                var pLoadFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
                pLoadFilterableSpecificationAttributeOptionIds.ParameterName = "LoadFilterableSpecificationAttributeOptionIds";
                pLoadFilterableSpecificationAttributeOptionIds.Value = loadFilterableSpecificationAttributeOptionIds;
                pLoadFilterableSpecificationAttributeOptionIds.DbType = DbType.Boolean;
                
                var pFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
                pFilterableSpecificationAttributeOptionIds.ParameterName = "FilterableSpecificationAttributeOptionIds";
                pFilterableSpecificationAttributeOptionIds.Direction = ParameterDirection.Output;
                pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
                pFilterableSpecificationAttributeOptionIds.DbType = DbType.String;

                var pTotalRecords = _dataProvider.GetParameter();
                pTotalRecords.ParameterName = "TotalRecords";
                pTotalRecords.Direction = ParameterDirection.Output;
                pTotalRecords.DbType = DbType.Int32;

                //invoke stored procedure
                var articles = _dbContext.ExecuteStoredProcedureList<Article>(
                    "ArticleLoadAllPaged",
                    pCategoryIds,
                    pPublisherId,
                    pStoreId,
                    pContributorId,
                    pWarehouseId,
                    pArticleTypeId,
                    pVisibleIndividuallyOnly,
                    pMarkedAsNewOnly,
                    pArticleTagId,
                    pFeaturedArticles,
                    pPriceMin,
                    pPriceMax,
                    pKeywords,
                    pSearchDescriptions,
                    pSearchPublisherPartNumber,
                    pSearchSku,
                    pSearchArticleTags,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pFilteredSpecs,
                    pLanguageId,
                    pOrderBy,
                    pAllowedCustomerRoleIds,
                    pPageIndex,
                    pPageSize,
                    pShowHidden,
                    pOverridePublished,
                    pLoadFilterableSpecificationAttributeOptionIds,
                    pFilterableSpecificationAttributeOptionIds,
                    pTotalRecords);
                //get filterable specification attribute option identifier
                string filterableSpecificationAttributeOptionIdsStr = (pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value) ? (string)pFilterableSpecificationAttributeOptionIds.Value : "";
                if (loadFilterableSpecificationAttributeOptionIds &&
                    !string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
                {
                     filterableSpecificationAttributeOptionIds = filterableSpecificationAttributeOptionIdsStr
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Convert.ToInt32(x.Trim()))
                        .ToList();
                }
                //return articles
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Article>(articles, pageIndex, pageSize, totalRecords);

                #endregion
            }
            else
            {
                //stored procedures aren't supported. Use LINQ

                #region Search articles

                //articles
                var query = _articleRepository.Table;
                query = query.Where(p => !p.Deleted);
                if (!overridePublished.HasValue)
                {
                    //process according to "showHidden"
                    if (!showHidden)
                    {
                        query = query.Where(p => p.Published);
                    }
                }
                else if (overridePublished.Value)
                {
                    //published only
                    query = query.Where(p => p.Published);
                }
                else if (!overridePublished.Value)
                {
                    //unpublished only
                    query = query.Where(p => !p.Published);
                }
                if (visibleIndividuallyOnly)
                {
                    query = query.Where(p => p.VisibleIndividually);
                }
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                if (markedAsNewOnly)
                {
                    query = query.Where(p => p.MarkAsNew);
                    query = query.Where(p =>
                        (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
                }
                if (articleType.HasValue)
                {
                    var articleTypeId = (int) articleType.Value;
                    query = query.Where(p => p.ArticleTypeId == articleTypeId);
                }

                if (priceMin.HasValue)
                {
                    //min price
                    query = query.Where(p => p.Price >= priceMin.Value);
                }
                if (priceMax.HasValue)
                {
                    //max price
                    query = query.Where(p => p.Price <= priceMax.Value);
                }
                if (!showHidden)
                {
                    //available dates
                    query = query.Where(p =>
                        (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < nowUtc) &&
                        (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > nowUtc));
                }

                //searching by keyword
                if (!String.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                            from lp in p_lp.DefaultIfEmpty()
                            from pt in p.ArticleTags.DefaultIfEmpty()
                            where (p.Name.Contains(keywords)) ||
                                  (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                                  (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                                  //publisher part number
                                  (searchPublisherPartNumber && p.PublisherPartNumber == keywords) ||
                                  //sku (exact match)
                                  (searchSku && p.Sku == keywords) ||
                                  //article tags (exact match)
                                  (searchArticleTags && pt.Name == keywords) ||
                                  //localized values
                                  (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                            select p;
                }

                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    query = from p in query
                            join acl in _aclRepository.Table
                            on new { c1 = p.Id, c2 = "Article" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                            from acl in p_acl.DefaultIfEmpty()
                            where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select p;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from p in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = p.Id, c2 = "Article" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                            from sm in p_sm.DefaultIfEmpty()
                            where !p.LimitedToStores || storeId == sm.StoreId
                            select p;
                }

                //category filtering
                if (categoryIds != null && categoryIds.Any())
                {
                    query = from p in query
                            from pc in p.ArticleCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                            where (!featuredArticles.HasValue || featuredArticles.Value == pc.IsFeaturedArticle)
                            select p;
                }

                //publisher filtering
                if (publisherId > 0)
                {
                    query = from p in query
                            from pm in p.ArticlePublishers.Where(pm => pm.PublisherId == publisherId)
                            where (!featuredArticles.HasValue || featuredArticles.Value == pm.IsFeaturedArticle)
                            select p;
                }

                //contributor filtering
                if (contributorId > 0)
                {
                    query = query.Where(p => p.ContributorId == contributorId);
                }

                

                //related articles filtering
                //if (relatedToArticleId > 0)
                //{
                //    query = from p in query
                //            join rp in _relatedArticleRepository.Table on p.Id equals rp.ArticleId2
                //            where (relatedToArticleId == rp.ArticleId1)
                //            select p;
                //}

                //tag filtering
                if (articleTagId > 0)
                {
                    query = from p in query
                            from pt in p.ArticleTags.Where(pt => pt.Id == articleTagId)
                            select p;
                }

                //get filterable specification attribute option identifier
                if (loadFilterableSpecificationAttributeOptionIds)
                {
                    var querySpecs = from p in query
                                     join psa in _articleSpecificationAttributeRepository.Table on p.Id equals psa.ArticleId
                                     where psa.AllowFiltering
                                     select psa.SpecificationAttributeOptionId;
                    //only distinct attributes
                    filterableSpecificationAttributeOptionIds = querySpecs.Distinct().ToList();
                }

                //search by specs
                if (filteredSpecs != null && filteredSpecs.Any())
                {
                    var filteredAttributes = _specificationAttributeOptionRepository.Table
                        .Where(sao => filteredSpecs.Contains(sao.Id)).Select(sao => sao.SpecificationAttributeId).Distinct();

                    query = query.Where(p => !filteredAttributes.Except
                        (
                            _specificationAttributeOptionRepository.Table.Where(
                                sao => p.ArticleSpecificationAttributes.Where(
                                    psa => psa.AllowFiltering && filteredSpecs.Contains(psa.SpecificationAttributeOptionId))
                                .Select(psa => psa.SpecificationAttributeOptionId).Contains(sao.Id))
                            .Select(sao => sao.SpecificationAttributeId).Distinct()
                        ).Any());
                }

                //only distinct articles (group by ID)
                //if we use standard Distinct() method, then all fields will be compared (low performance)
                //it'll not work in SQL Server Compact when searching articles by a keyword)
                query = from p in query
                        group p by p.Id
                        into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

                //sort articles
                if (subscriptionBy == ArticleSortingEnum.Position && categoryIds != null && categoryIds.Any())
                {
                    //category position
                    var firstCategoryId = categoryIds[0];
                    query = query.OrderBy(p => p.ArticleCategories.FirstOrDefault(pc => pc.CategoryId == firstCategoryId).DisplaySubscription);
                }
                else if (subscriptionBy == ArticleSortingEnum.Position && publisherId > 0)
                {
                    //publisher position
                    query = 
                        query.OrderBy(p => p.ArticlePublishers.FirstOrDefault(pm => pm.PublisherId == publisherId).DisplaySubscription);
                }
                else if (subscriptionBy == ArticleSortingEnum.Position)
                {
                    //otherwise sort by name
                    query = query.OrderBy(p => p.Name);
                }
                else if (subscriptionBy == ArticleSortingEnum.NameAsc)
                {
                    //Name: A to Z
                    query = query.OrderBy(p => p.Name);
                }
                else if (subscriptionBy == ArticleSortingEnum.NameDesc)
                {
                    //Name: Z to A
                    query = query.OrderByDescending(p => p.Name);
                }
                else if (subscriptionBy == ArticleSortingEnum.PriceAsc)
                {
                    //Price: Low to High
                    query = query.OrderBy(p => p.Price);
                }
                else if (subscriptionBy == ArticleSortingEnum.PriceDesc)
                {
                    //Price: High to Low
                    query = query.OrderByDescending(p => p.Price);
                }
                else if (subscriptionBy == ArticleSortingEnum.CreatedOn)
                {
                    //creation date
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                }
                else
                {
                    //actually this code is not reachable
                    query = query.OrderBy(p => p.Name);
                }

                var articles = new PagedList<Article>(query, pageIndex, pageSize);


                //return articles
                return articles;

                #endregion
            }
        }

        /// <summary>
        /// Gets articles by article attribute
        /// </summary>
        /// <param name="articleAttributeId">Article attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Articles</returns>
        public virtual IPagedList<Article> GetArticlesByArticleAtributeId(int articleAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _articleRepository.Table;
            query = query.Where(x => x.ArticleAttributeMappings.Any(y => y.ArticleAttributeId == articleAttributeId));
            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.Name);

            var articles = new PagedList<Article>(query, pageIndex, pageSize);
            return articles;
        }

        /// <summary>
        /// Gets associated articles
        /// </summary>
        /// <param name="parentGroupedArticleId">Parent article identifier (used with grouped articles)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="contributorId">Contributor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Articles</returns>
        public virtual IList<Article> GetAssociatedArticles(int parentGroupedArticleId,
            int storeId = 0, int contributorId = 0, bool showHidden = false)
        {
            var query = _articleRepository.Table;
            query = query.Where(x => x.ParentGroupedArticleId == parentGroupedArticleId);
            if (!showHidden)
            {
                query = query.Where(x => x.Published);
            
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                //available dates
                query = query.Where(p =>
                    (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < nowUtc) &&
                    (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > nowUtc));
            }
            //contributor filtering
            if (contributorId > 0)
            {
                query = query.Where(p => p.ContributorId == contributorId);
            }
            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.DisplaySubscription).ThenBy(x => x.Id);

            var articles = query.ToList();

            //ACL mapping
            if (!showHidden)
            {
                articles = articles.Where(x => _aclService.Authorize(x)).ToList();
            }
            //Store mapping
            if (!showHidden && storeId > 0)
            {
                articles = articles.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
            }

            return articles;
        }
        
        /// <summary>
        /// Update article review totals
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void UpdateArticleReviewTotals(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            int approvedRatingSum = 0;
            int notApprovedRatingSum = 0; 
            int approvedTotalReviews = 0;
            int notApprovedTotalReviews = 0;
            var reviews = article.ArticleReviews;
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews ++;
                }
                else
                {
                    notApprovedRatingSum += pr.Rating;
                    notApprovedTotalReviews++;
                }
            }

            article.ApprovedRatingSum = approvedRatingSum;
            article.NotApprovedRatingSum = notApprovedRatingSum;
            article.ApprovedTotalReviews = approvedTotalReviews;
            article.NotApprovedTotalReviews = notApprovedTotalReviews;
            UpdateArticle(article);
        }
 

        

        /// <summary>
        /// Gets a article by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Article</returns>
        public virtual Article GetArticleBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _articleRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var article = query.FirstOrDefault();
            return article;
        }

        /// <summary>
        /// Gets a articles by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="contributorId">Contributor ID; 0 to load all records</param>
        /// <returns>Articles</returns>
        public IList<Article> GetArticlesBySku(string[] skuArray, int contributorId = 0)
        {
            if (skuArray == null)
                throw new ArgumentNullException("skuArray");

            var query = _articleRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            if (contributorId != 0)
                query = query.Where(p => p.ContributorId == contributorId);

            return query.ToList();
        }

        

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void UpdateHasDiscountsApplied(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

           
            UpdateArticle(article);
        }


        /// <summary>
        /// Gets number of articles by contributor identifier
        /// </summary>
        /// <param name="contributorId">Contributor identifier</param>
        /// <returns>Number of articles</returns>
        public int GetNumberOfArticlesByContributorId(int contributorId)
        {
            if (contributorId == 0)
                return 0;

            return _articleRepository.Table.Count(p => p.ContributorId == contributorId && !p.Deleted);
        }

        #endregion

        

        #region Related articles

        /// <summary>
        /// Deletes a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void DeleteRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Delete(relatedArticle);

            //event notification
            _eventPublisher.EntityDeleted(relatedArticle);
        }

        /// <summary>
        /// Gets related articles by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related articles</returns>
        public virtual IList<RelatedArticle> GetRelatedArticlesByArticleId1(int articleId1, bool showHidden = false)
        {
            var query = from rp in _relatedArticleRepository.Table
                        join p in _articleRepository.Table on rp.ArticleId2 equals p.Id
                        where rp.ArticleId1 == articleId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplaySubscription, rp.Id
                        select rp;
            var relatedArticles = query.ToList();

            return relatedArticles;
        }

        /// <summary>
        /// Gets a related article
        /// </summary>
        /// <param name="relatedArticleId">Related article identifier</param>
        /// <returns>Related article</returns>
        public virtual RelatedArticle GetRelatedArticleById(int relatedArticleId)
        {
            if (relatedArticleId == 0)
                return null;
            
            return _relatedArticleRepository.GetById(relatedArticleId);
        }

        /// <summary>
        /// Inserts a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void InsertRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Insert(relatedArticle);

            //event notification
            _eventPublisher.EntityInserted(relatedArticle);
        }

        /// <summary>
        /// Updates a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void UpdateRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Update(relatedArticle);

            //event notification
            _eventPublisher.EntityUpdated(relatedArticle);
        }

        #endregion

        #region Cross-sell articles

        /// <summary>
        /// Deletes a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell identifier</param>
        public virtual void DeleteCrossSellArticle(CrossSellArticle crossSellArticle)
        {
            if (crossSellArticle == null)
                throw new ArgumentNullException("crossSellArticle");

            _crossSellArticleRepository.Delete(crossSellArticle);

            //event notification
            _eventPublisher.EntityDeleted(crossSellArticle);
        }

        /// <summary>
        /// Gets cross-sell articles by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell articles</returns>
        public virtual IList<CrossSellArticle> GetCrossSellArticlesByArticleId1(int articleId1, bool showHidden = false)
        {
            var query = from csp in _crossSellArticleRepository.Table
                        join p in _articleRepository.Table on csp.ArticleId2 equals p.Id
                        where csp.ArticleId1 == articleId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby csp.Id
                        select csp;
            var crossSellArticles = query.ToList();
            return crossSellArticles;
        }

        /// <summary>
        /// Gets a cross-sell article
        /// </summary>
        /// <param name="crossSellArticleId">Cross-sell article identifier</param>
        /// <returns>Cross-sell article</returns>
        public virtual CrossSellArticle GetCrossSellArticleById(int crossSellArticleId)
        {
            if (crossSellArticleId == 0)
                return null;

            return _crossSellArticleRepository.GetById(crossSellArticleId);
        }

        /// <summary>
        /// Inserts a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell article</param>
        public virtual void InsertCrossSellArticle(CrossSellArticle crossSellArticle)
        {
            if (crossSellArticle == null)
                throw new ArgumentNullException("crossSellArticle");

            _crossSellArticleRepository.Insert(crossSellArticle);

            //event notification
            _eventPublisher.EntityInserted(crossSellArticle);
        }

        /// <summary>
        /// Updates a cross-sell article
        /// </summary>
        /// <param name="crossSellArticle">Cross-sell article</param>
        public virtual void UpdateCrossSellArticle(CrossSellArticle crossSellArticle)
        {
            if (crossSellArticle == null)
                throw new ArgumentNullException("crossSellArticle");

            _crossSellArticleRepository.Update(crossSellArticle);

            //event notification
            _eventPublisher.EntityUpdated(crossSellArticle);
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfArticles">Number of articles to return</param>
        /// <returns>Cross-sells</returns>
        public virtual IList<Article> GetCrosssellArticlesByShoppingCart(IList<ShoppingCartItem> cart, int numberOfArticles)
        {
            var result = new List<Article>();

            if (numberOfArticles == 0)
                return result;

            if (cart == null || !cart.Any())
                return result;

            var cartArticleIds = new List<int>();
            foreach (var sci in cart)
            {
                int prodId = sci.ArticleId;
                if (!cartArticleIds.Contains(prodId))
                    cartArticleIds.Add(prodId);
            }

            foreach (var sci in cart)
            {
                var crossSells = GetCrossSellArticlesByArticleId1(sci.ArticleId);
                foreach (var crossSell in crossSells)
                {
                    //validate that this article is not added to result yet
                    //validate that this article is not in the cart
                    if (result.Find(p => p.Id == crossSell.ArticleId2) == null &&
                        !cartArticleIds.Contains(crossSell.ArticleId2))
                    {
                        var articleToAdd = GetArticleById(crossSell.ArticleId2);
                        //validate article
                        if (articleToAdd == null || articleToAdd.Deleted || !articleToAdd.Published)
                            continue;

                        //add a article to result
                        result.Add(articleToAdd);
                        if (result.Count >= numberOfArticles)
                            return result;
                    }
                }
            }
            return result;
        }
        #endregion
        
        

        #region Article pictures

        /// <summary>
        /// Deletes a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        public virtual void DeleteArticlePicture(ArticlePicture articlePicture)
        {
            if (articlePicture == null)
                throw new ArgumentNullException("articlePicture");

            _articlePictureRepository.Delete(articlePicture);

            //event notification
            _eventPublisher.EntityDeleted(articlePicture);
        }

        /// <summary>
        /// Gets a article pictures by article identifier
        /// </summary>
        /// <param name="articleId">The article identifier</param>
        /// <returns>Article pictures</returns>
        public virtual IList<ArticlePicture> GetArticlePicturesByArticleId(int articleId)
        {
            var query = from pp in _articlePictureRepository.Table
                        where pp.ArticleId == articleId
                        orderby pp.DisplaySubscription, pp.Id
                        select pp;
            var articlePictures = query.ToList();
            return articlePictures;
        }

        /// <summary>
        /// Gets a article picture
        /// </summary>
        /// <param name="articlePictureId">Article picture identifier</param>
        /// <returns>Article picture</returns>
        public virtual ArticlePicture GetArticlePictureById(int articlePictureId)
        {
            if (articlePictureId == 0)
                return null;

            return _articlePictureRepository.GetById(articlePictureId);
        }

        /// <summary>
        /// Inserts a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        public virtual void InsertArticlePicture(ArticlePicture articlePicture)
        {
            if (articlePicture == null)
                throw new ArgumentNullException("articlePicture");

            _articlePictureRepository.Insert(articlePicture);

            //event notification
            _eventPublisher.EntityInserted(articlePicture);
        }

        /// <summary>
        /// Updates a article picture
        /// </summary>
        /// <param name="articlePicture">Article picture</param>
        public virtual void UpdateArticlePicture(ArticlePicture articlePicture)
        {
            if (articlePicture == null)
                throw new ArgumentNullException("articlePicture");

            _articlePictureRepository.Update(articlePicture);

            //event notification
            _eventPublisher.EntityUpdated(articlePicture);
        }

        /// <summary>
        /// Get the IDs of all article images 
        /// </summary>
        /// <param name="articlesIds">Articles IDs</param>
        /// <returns>All picture identifiers grouped by article ID</returns>
        public IDictionary<int, int[]> GetArticlesImagesIds(int [] articlesIds)
        {
            return _articlePictureRepository.Table.Where(p => articlesIds.Contains(p.ArticleId))
                .GroupBy(p => p.ArticleId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

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
        public virtual IPagedList<ArticleReview> GetAllArticleReviews(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int articleId = 0, int contributorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _articleReviewRepository.Table;
            if (approved.HasValue)
                query = query.Where(pr => pr.IsApproved == approved);
            if (customerId > 0)
                query = query.Where(pr => pr.CustomerId == customerId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (!String.IsNullOrEmpty(message))
                query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));
            if (storeId > 0)
                query = query.Where(pr => pr.StoreId == storeId);
            if (articleId > 0)
                query = query.Where(pr => pr.ArticleId == articleId);
            if (contributorId > 0)
                query = query.Where(pr => pr.Article.ContributorId == contributorId);

            query = query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var articleReviews = new PagedList<ArticleReview>(query, pageIndex, pageSize);

            return articleReviews;
        }

        /// <summary>
        /// Gets article review
        /// </summary>
        /// <param name="articleReviewId">Article review identifier</param>
        /// <returns>Article review</returns>
        public virtual ArticleReview GetArticleReviewById(int articleReviewId)
        {
            if (articleReviewId == 0)
                return null;

            return _articleReviewRepository.GetById(articleReviewId);
        }

        /// <summary>
        /// Get article reviews by identifiers
        /// </summary>
        /// <param name="articleReviewIds">Article review identifiers</param>
        /// <returns>Article reviews</returns>
        public virtual IList<ArticleReview> GetProducReviewsByIds(int[] articleReviewIds)
        {
            if (articleReviewIds == null || articleReviewIds.Length == 0)
                return new List<ArticleReview>();

            var query = from pr in _articleReviewRepository.Table
                        where articleReviewIds.Contains(pr.Id)
                        select pr;
            var articleReviews = query.ToList();
            //sort by passed identifiers
            var sortedArticleReviews = new List<ArticleReview>();
            foreach (int id in articleReviewIds)
            {
                var articleReview = articleReviews.Find(x => x.Id == id);
                if (articleReview != null)
                    sortedArticleReviews.Add(articleReview);
            }
            return sortedArticleReviews;
        }

        /// <summary>
        /// Deletes a article review
        /// </summary>
        /// <param name="articleReview">Article review</param>
        public virtual void DeleteArticleReview(ArticleReview articleReview)
        {
            if (articleReview == null)
                throw new ArgumentNullException("articleReview");

            _articleReviewRepository.Delete(articleReview);

            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityDeleted(articleReview);
        }

        /// <summary>
        /// Deletes article reviews
        /// </summary>
        /// <param name="articleReviews">Article reviews</param>
        public virtual void DeleteArticleReviews(IList<ArticleReview> articleReviews)
        {
            if (articleReviews == null)
                throw new ArgumentNullException("articleReviews");

            _articleReviewRepository.Delete(articleReviews);

            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);
            //event notification
            foreach (var articleReview in articleReviews)
            {
                _eventPublisher.EntityDeleted(articleReview);
            }
        }

        #endregion

        

        

        #endregion
    }
}
