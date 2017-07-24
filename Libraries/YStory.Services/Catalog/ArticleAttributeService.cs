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
    /// Article attribute service
    /// </summary>
    public partial class ArticleAttributeService : IArticleAttributeService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string ARTICLEATTRIBUTES_ALL_KEY = "YStory.articleattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article attribute ID
        /// </remarks>
        private const string ARTICLEATTRIBUTES_BY_ID_KEY = "YStory.articleattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article ID
        /// </remarks>
        private const string ARTICLEATTRIBUTEMAPPINGS_ALL_KEY = "YStory.articleattributemapping.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article attribute mapping ID
        /// </remarks>
        private const string ARTICLEATTRIBUTEMAPPINGS_BY_ID_KEY = "YStory.articleattributemapping.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article attribute mapping ID
        /// </remarks>
        private const string ARTICLEATTRIBUTEVALUES_ALL_KEY = "YStory.articleattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article attribute value ID
        /// </remarks>
        private const string ARTICLEATTRIBUTEVALUES_BY_ID_KEY = "YStory.articleattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : article ID
        /// </remarks>
        private const string ARTICLEATTRIBUTECOMBINATIONS_ALL_KEY = "YStory.articleattributecombination.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLEATTRIBUTES_PATTERN_KEY = "YStory.articleattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY = "YStory.articleattributemapping.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLEATTRIBUTEVALUES_PATTERN_KEY = "YStory.articleattributevalue.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY = "YStory.articleattributecombination.";

        #endregion

        #region Fields

        private readonly IRepository<ArticleAttribute> _articleAttributeRepository;
        private readonly IRepository<ArticleAttributeMapping> _articleAttributeMappingRepository;
        private readonly IRepository<ArticleAttributeCombination> _articleAttributeCombinationRepository;
        private readonly IRepository<ArticleAttributeValue> _articleAttributeValueRepository;
        private readonly IRepository<PredefinedArticleAttributeValue> _predefinedArticleAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="articleAttributeRepository">Article attribute repository</param>
        /// <param name="articleAttributeMappingRepository">Article attribute mapping repository</param>
        /// <param name="articleAttributeCombinationRepository">Article attribute combination repository</param>
        /// <param name="articleAttributeValueRepository">Article attribute value repository</param>
        /// <param name="predefinedArticleAttributeValueRepository">Predefined article attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ArticleAttributeService(ICacheManager cacheManager,
            IRepository<ArticleAttribute> articleAttributeRepository,
            IRepository<ArticleAttributeMapping> articleAttributeMappingRepository,
            IRepository<ArticleAttributeCombination> articleAttributeCombinationRepository,
            IRepository<ArticleAttributeValue> articleAttributeValueRepository,
            IRepository<PredefinedArticleAttributeValue> predefinedArticleAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._articleAttributeRepository = articleAttributeRepository;
            this._articleAttributeMappingRepository = articleAttributeMappingRepository;
            this._articleAttributeCombinationRepository = articleAttributeCombinationRepository;
            this._articleAttributeValueRepository = articleAttributeValueRepository;
            this._predefinedArticleAttributeValueRepository = predefinedArticleAttributeValueRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Article attributes

        /// <summary>
        /// Deletes a article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        public virtual void DeleteArticleAttribute(ArticleAttribute articleAttribute)
        {
            if (articleAttribute == null)
                throw new ArgumentNullException("articleAttribute");

            _articleAttributeRepository.Delete(articleAttribute);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleAttribute);
        }

        /// <summary>
        /// Gets all article attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Article attributes</returns>
        public virtual IPagedList<ArticleAttribute> GetAllArticleAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(ARTICLEATTRIBUTES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _articleAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var articleAttributes = new PagedList<ArticleAttribute>(query, pageIndex, pageSize);
                return articleAttributes;
            });
        }

        /// <summary>
        /// Gets a article attribute 
        /// </summary>
        /// <param name="articleAttributeId">Article attribute identifier</param>
        /// <returns>Article attribute </returns>
        public virtual ArticleAttribute GetArticleAttributeById(int articleAttributeId)
        {
            if (articleAttributeId == 0)
                return null;

            string key = string.Format(ARTICLEATTRIBUTES_BY_ID_KEY, articleAttributeId);
            return _cacheManager.Get(key, () => _articleAttributeRepository.GetById(articleAttributeId));
        }

        /// <summary>
        /// Inserts a article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        public virtual void InsertArticleAttribute(ArticleAttribute articleAttribute)
        {
            if (articleAttribute == null)
                throw new ArgumentNullException("articleAttribute");

            _articleAttributeRepository.Insert(articleAttribute);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleAttribute);
        }

        /// <summary>
        /// Updates the article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        public virtual void UpdateArticleAttribute(ArticleAttribute articleAttribute)
        {
            if (articleAttribute == null)
                throw new ArgumentNullException("articleAttribute");

            _articleAttributeRepository.Update(articleAttribute);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        public virtual int[] GetNotExistingAttributes(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException("attributeId");

            var query = _articleAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Article attributes mappings

        /// <summary>
        /// Deletes a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        public virtual void DeleteArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                throw new ArgumentNullException("articleAttributeMapping");

            _articleAttributeMappingRepository.Delete(articleAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleAttributeMapping);
        }

        /// <summary>
        /// Gets article attribute mappings by article identifier
        /// </summary>
        /// <param name="articleId">The article identifier</param>
        /// <returns>Article attribute mapping collection</returns>
        public virtual IList<ArticleAttributeMapping> GetArticleAttributeMappingsByArticleId(int articleId)
        {
            string key = string.Format(ARTICLEATTRIBUTEMAPPINGS_ALL_KEY, articleId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pam in _articleAttributeMappingRepository.Table
                            orderby pam.DisplaySubscription, pam.Id
                            where pam.ArticleId == articleId
                            select pam;
                var articleAttributeMappings = query.ToList();
                return articleAttributeMappings;
            });
        }

        /// <summary>
        /// Gets a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMappingId">Article attribute mapping identifier</param>
        /// <returns>Article attribute mapping</returns>
        public virtual ArticleAttributeMapping GetArticleAttributeMappingById(int articleAttributeMappingId)
        {
            if (articleAttributeMappingId == 0)
                return null;

            string key = string.Format(ARTICLEATTRIBUTEMAPPINGS_BY_ID_KEY, articleAttributeMappingId);
            return _cacheManager.Get(key, () => _articleAttributeMappingRepository.GetById(articleAttributeMappingId));
        }

        /// <summary>
        /// Inserts a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">The article attribute mapping</param>
        public virtual void InsertArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                throw new ArgumentNullException("articleAttributeMapping");

            _articleAttributeMappingRepository.Insert(articleAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleAttributeMapping);
        }

        /// <summary>
        /// Updates the article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">The article attribute mapping</param>
        public virtual void UpdateArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                throw new ArgumentNullException("articleAttributeMapping");

            _articleAttributeMappingRepository.Update(articleAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleAttributeMapping);
        }

        #endregion

        #region Article attribute values

        /// <summary>
        /// Deletes a article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">Article attribute value</param>
        public virtual void DeleteArticleAttributeValue(ArticleAttributeValue articleAttributeValue)
        {
            if (articleAttributeValue == null)
                throw new ArgumentNullException("articleAttributeValue");

            _articleAttributeValueRepository.Delete(articleAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleAttributeValue);
        }

        /// <summary>
        /// Gets article attribute values by article attribute mapping identifier
        /// </summary>
        /// <param name="articleAttributeMappingId">The article attribute mapping identifier</param>
        /// <returns>Article attribute mapping collection</returns>
        public virtual IList<ArticleAttributeValue> GetArticleAttributeValues(int articleAttributeMappingId)
        {
            string key = string.Format(ARTICLEATTRIBUTEVALUES_ALL_KEY, articleAttributeMappingId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pav in _articleAttributeValueRepository.Table
                            orderby pav.DisplaySubscription, pav.Id
                            where pav.ArticleAttributeMappingId == articleAttributeMappingId
                            select pav;
                var articleAttributeValues = query.ToList();
                return articleAttributeValues;
            });
        }

        /// <summary>
        /// Gets a article attribute value
        /// </summary>
        /// <param name="articleAttributeValueId">Article attribute value identifier</param>
        /// <returns>Article attribute value</returns>
        public virtual ArticleAttributeValue GetArticleAttributeValueById(int articleAttributeValueId)
        {
            if (articleAttributeValueId == 0)
                return null;
            
           string key = string.Format(ARTICLEATTRIBUTEVALUES_BY_ID_KEY, articleAttributeValueId);
           return _cacheManager.Get(key, () => _articleAttributeValueRepository.GetById(articleAttributeValueId));
        }

        /// <summary>
        /// Inserts a article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">The article attribute value</param>
        public virtual void InsertArticleAttributeValue(ArticleAttributeValue articleAttributeValue)
        {
            if (articleAttributeValue == null)
                throw new ArgumentNullException("articleAttributeValue");

            _articleAttributeValueRepository.Insert(articleAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleAttributeValue);
        }

        /// <summary>
        /// Updates the article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">The article attribute value</param>
        public virtual void UpdateArticleAttributeValue(ArticleAttributeValue articleAttributeValue)
        {
            if (articleAttributeValue == null)
                throw new ArgumentNullException("articleAttributeValue");

            _articleAttributeValueRepository.Update(articleAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleAttributeValue);
        }

        #endregion

        #region Predefined article attribute values

        /// <summary>
        /// Deletes a predefined article attribute value
        /// </summary>
        /// <param name="ppav">Predefined article attribute value</param>
        public virtual void DeletePredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedArticleAttributeValueRepository.Delete(ppav);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(ppav);
        }

        /// <summary>
        /// Gets predefined article attribute values by article attribute identifier
        /// </summary>
        /// <param name="articleAttributeId">The article attribute identifier</param>
        /// <returns>Article attribute mapping collection</returns>
        public virtual IList<PredefinedArticleAttributeValue> GetPredefinedArticleAttributeValues(int articleAttributeId)
        {
            var query = from ppav in _predefinedArticleAttributeValueRepository.Table
                        orderby ppav.DisplaySubscription, ppav.Id
                        where ppav.ArticleAttributeId == articleAttributeId
                        select ppav;
            var values = query.ToList();
            return values;
        }

        /// <summary>
        /// Gets a predefined article attribute value
        /// </summary>
        /// <param name="id">Predefined article attribute value identifier</param>
        /// <returns>Predefined article attribute value</returns>
        public virtual PredefinedArticleAttributeValue GetPredefinedArticleAttributeValueById(int id)
        {
            if (id == 0)
                return null;

            return _predefinedArticleAttributeValueRepository.GetById(id);
        }

        /// <summary>
        /// Inserts a predefined article attribute value
        /// </summary>
        /// <param name="ppav">The predefined article attribute value</param>
        public virtual void InsertPredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedArticleAttributeValueRepository.Insert(ppav);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(ppav);
        }

        /// <summary>
        /// Updates the predefined article attribute value
        /// </summary>
        /// <param name="ppav">The predefined article attribute value</param>
        public virtual void UpdatePredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException("ppav");

            _predefinedArticleAttributeValueRepository.Update(ppav);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(ppav);
        }

        #endregion

        #region Article attribute combinations

        /// <summary>
        /// Deletes a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        public virtual void DeleteArticleAttributeCombination(ArticleAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _articleAttributeCombinationRepository.Delete(combination);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(combination);
        }

        /// <summary>
        /// Gets all article attribute combinations
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article attribute combinations</returns>
        public virtual IList<ArticleAttributeCombination> GetAllArticleAttributeCombinations(int articleId)
        {
            if (articleId == 0)
                return new List<ArticleAttributeCombination>();

            string key = string.Format(ARTICLEATTRIBUTECOMBINATIONS_ALL_KEY, articleId);

            return _cacheManager.Get(key, () =>
            {
                var query = from c in _articleAttributeCombinationRepository.Table
                            orderby c.Id
                            where c.ArticleId == articleId
                            select c;
                var combinations = query.ToList();
                return combinations;
            });
        }

        /// <summary>
        /// Gets a article attribute combination
        /// </summary>
        /// <param name="articleAttributeCombinationId">Article attribute combination identifier</param>
        /// <returns>Article attribute combination</returns>
        public virtual ArticleAttributeCombination GetArticleAttributeCombinationById(int articleAttributeCombinationId)
        {
            if (articleAttributeCombinationId == 0)
                return null;
            
            return _articleAttributeCombinationRepository.GetById(articleAttributeCombinationId);
        }

        /// <summary>
        /// Gets a article attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Article attribute combination</returns>
        public virtual ArticleAttributeCombination GetArticleAttributeCombinationBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _articleAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.Sku == sku
                        select pac;
            var combination = query.FirstOrDefault();
            return combination;
        }
        
        /// <summary>
        /// Inserts a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        public virtual void InsertArticleAttributeCombination(ArticleAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _articleAttributeCombinationRepository.Insert(combination);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(combination);
        }

        /// <summary>
        /// Updates a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        public virtual void UpdateArticleAttributeCombination(ArticleAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _articleAttributeCombinationRepository.Update(combination);

            //cache
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(combination);
        }

        #endregion

        #endregion
    }
}
