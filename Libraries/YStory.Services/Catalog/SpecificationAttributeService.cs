using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Services.Events;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article ID
        /// {1} : allow filtering
        /// {2} : show on article page
        /// </remarks>
        private const string ARTICLESPECIFICATIONATTRIBUTE_ALLBYARTICLEID_KEY = "YStory.articlespecificationattribute.allbyarticleid-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY = "YStory.articlespecificationattribute.";
        #endregion

        #region Fields
        
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ArticleSpecificationAttribute> _articleSpecificationAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="specificationAttributeRepository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="articleSpecificationAttributeRepository">Article specification attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ArticleSpecificationAttribute> articleSpecificationAttributeRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _articleSpecificationAttributeRepository = articleSpecificationAttributeRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Specification attributes</returns>
        public virtual IPagedList<SpecificationAttribute> GetSpecificationAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplaySubscription, sa.Id
                        select sa;
            var specificationAttributes = new PagedList<SpecificationAttribute>(query, pageIndex, pageSize);
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
        }


        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>Specification attribute options</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return new List<SpecificationAttributeOption>();

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.Id)
                        select sao;
            var specificationAttributeOptions = query.ToList();
            //sort by passed identifiers
            var sortedSpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            foreach (int id in specificationAttributeOptionIds)
            {
                var sao = specificationAttributeOptions.Find(x => x.Id == id);
                if (sao != null)
                    sortedSpecificationAttributeOptions.Add(sao);
            }
            return sortedSpecificationAttributeOptions;
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplaySubscription, sao.Id
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        #endregion

        #region Article specification attribute

        /// <summary>
        /// Deletes a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute</param>
        public virtual void DeleteArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                throw new ArgumentNullException("articleSpecificationAttribute");

            _articleSpecificationAttributeRepository.Delete(articleSpecificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleSpecificationAttribute);
        }

        /// <summary>
        /// Gets a article specification attribute mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnArticlePage">0 to load attributes with ShowOnArticlePage set to false, 1 to load attributes with ShowOnArticlePage set to true, null to load all attributes</param>
        /// <returns>Article specification attribute mapping collection</returns>
        public virtual IList<ArticleSpecificationAttribute> GetArticleSpecificationAttributes(int articleId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnArticlePage = null)
        {
            string allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            string showOnArticlePageCacheStr = showOnArticlePage.HasValue ? showOnArticlePage.ToString() : "null";
            string key = string.Format(ARTICLESPECIFICATIONATTRIBUTE_ALLBYARTICLEID_KEY, articleId, allowFilteringCacheStr, showOnArticlePageCacheStr);
            
            return _cacheManager.Get(key, () =>
            {
                var query = _articleSpecificationAttributeRepository.Table;
                if (articleId > 0)
                    query = query.Where(psa => psa.ArticleId == articleId);
                if (specificationAttributeOptionId > 0)
                    query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnArticlePage.HasValue)
                    query = query.Where(psa => psa.ShowOnArticlePage == showOnArticlePage.Value);
                query = query.OrderBy(psa => psa.DisplaySubscription).ThenBy(psa => psa.Id);

                var articleSpecificationAttributes = query.ToList();
                return articleSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a article specification attribute mapping 
        /// </summary>
        /// <param name="articleSpecificationAttributeId">Article specification attribute mapping identifier</param>
        /// <returns>Article specification attribute mapping</returns>
        public virtual ArticleSpecificationAttribute GetArticleSpecificationAttributeById(int articleSpecificationAttributeId)
        {
            if (articleSpecificationAttributeId == 0)
                return null;
            
            return _articleSpecificationAttributeRepository.GetById(articleSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void InsertArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                throw new ArgumentNullException("articleSpecificationAttribute");

            _articleSpecificationAttributeRepository.Insert(articleSpecificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleSpecificationAttribute);
        }

        /// <summary>
        /// Updates the article specification attribute mapping
        /// </summary>
        /// <param name="articleSpecificationAttribute">Article specification attribute mapping</param>
        public virtual void UpdateArticleSpecificationAttribute(ArticleSpecificationAttribute articleSpecificationAttribute)
        {
            if (articleSpecificationAttribute == null)
                throw new ArgumentNullException("articleSpecificationAttribute");

            _articleSpecificationAttributeRepository.Update(articleSpecificationAttribute);

            _cacheManager.RemoveByPattern(ARTICLESPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleSpecificationAttribute);
        }

        /// <summary>
        /// Gets a count of article specification attribute mapping records
        /// </summary>
        /// <param name="articleId">Article identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>Count</returns>
        public virtual int GetArticleSpecificationAttributeCount(int articleId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _articleSpecificationAttributeRepository.Table;
            if (articleId > 0)
                query = query.Where(psa => psa.ArticleId == articleId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return query.Count();
        }

        #endregion

        #endregion
    }
}
