using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Security;
using YStory.Core.Domain.Stores;
using YStory.Services.Customers;
using YStory.Services.Events;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Publisher service
    /// </summary>
    public partial class PublisherService : IPublisherService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : publisher ID
        /// </remarks>
        private const string PUBLISHERS_BY_ID_KEY = "YStory.publisher.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : publisher ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string ARTICLEPUBLISHERS_ALLBYPUBLISHERID_KEY = "YStory.articlepublisher.allbypublisherid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : article ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string ARTICLEPUBLISHERS_ALLBYARTICLEID_KEY = "YStory.articlepublisher.allbyarticleid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PUBLISHERS_PATTERN_KEY = "YStory.publisher.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLEPUBLISHERS_PATTERN_KEY = "YStory.articlepublisher.";

        #endregion

        #region Fields

        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IRepository<ArticlePublisher> _articlePublisherRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="publisherRepository">Category repository</param>
        /// <param name="articlePublisherRepository">ArticleCategory repository</param>
        /// <param name="articleRepository">Article repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public PublisherService(ICacheManager cacheManager,
            IRepository<Publisher> publisherRepository,
            IRepository<ArticlePublisher> articlePublisherRepository,
            IRepository<Article> articleRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._publisherRepository = publisherRepository;
            this._articlePublisherRepository = articlePublisherRepository;
            this._articleRepository = articleRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Deletes a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void DeletePublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");
            
            publisher.Deleted = true;
            UpdatePublisher(publisher);

            //event notification
            _eventPublisher.EntityDeleted(publisher);
        }

        /// <summary>
        /// Gets all publishers
        /// </summary>
        /// <param name="publisherName">Publisher name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Publishers</returns>
        public virtual IPagedList<Publisher> GetAllPublishers(string publisherName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue, 
            bool showHidden = false)
        {
            var query = _publisherRepository.Table;
            if (!showHidden)
                query = query.Where(m => m.Published);
            if (!String.IsNullOrWhiteSpace(publisherName))
                query = query.Where(m => m.Name.Contains(publisherName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplaySubscription).ThenBy(m => m.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from m in query
                            join acl in _aclRepository.Table
                            on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                            from acl in m_acl.DefaultIfEmpty()
                            where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select m;
                }
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from m in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                            from sm in m_sm.DefaultIfEmpty()
                            where !m.LimitedToStores || storeId == sm.StoreId
                            select m;
                }
                //only distinct publishers (group by ID)
                query = from m in query
                        group m by m.Id
                            into mGroup
                            orderby mGroup.Key
                            select mGroup.FirstOrDefault();
                query = query.OrderBy(m => m.DisplaySubscription).ThenBy(m => m.Id);
            }

            return new PagedList<Publisher>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a publisher
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <returns>Publisher</returns>
        public virtual Publisher GetPublisherById(int publisherId)
        {
            if (publisherId == 0)
                return null;
            
            string key = string.Format(PUBLISHERS_BY_ID_KEY, publisherId);
            return _cacheManager.Get(key, () => _publisherRepository.GetById(publisherId));
        }

        /// <summary>
        /// Inserts a publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void InsertPublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _publisherRepository.Insert(publisher);

            //cache
            _cacheManager.RemoveByPattern(PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEPUBLISHERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(publisher);
        }

        /// <summary>
        /// Updates the publisher
        /// </summary>
        /// <param name="publisher">Publisher</param>
        public virtual void UpdatePublisher(Publisher publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _publisherRepository.Update(publisher);

            //cache
            _cacheManager.RemoveByPattern(PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEPUBLISHERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(publisher);
        }
        

        /// <summary>
        /// Deletes a article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        public virtual void DeleteArticlePublisher(ArticlePublisher articlePublisher)
        {
            if (articlePublisher == null)
                throw new ArgumentNullException("articlePublisher");

            _articlePublisherRepository.Delete(articlePublisher);

            //cache
            _cacheManager.RemoveByPattern(PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEPUBLISHERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articlePublisher);
        }

        /// <summary>
        /// Gets article publisher collection
        /// </summary>
        /// <param name="publisherId">Publisher identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article publisher collection</returns>
        public virtual IPagedList<ArticlePublisher> GetArticlePublishersByPublisherId(int publisherId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (publisherId == 0)
                return new PagedList<ArticlePublisher>(new List<ArticlePublisher>(), pageIndex, pageSize);

            string key = string.Format(ARTICLEPUBLISHERS_ALLBYPUBLISHERID_KEY, showHidden, publisherId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pm in _articlePublisherRepository.Table
                            join p in _articleRepository.Table on pm.ArticleId equals p.Id
                            where pm.PublisherId == publisherId &&
                                  !p.Deleted &&
                                  (showHidden || p.Published)
                            orderby pm.DisplaySubscription, pm.Id
                            select pm;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pm in query
                                join m in _publisherRepository.Table on pm.PublisherId equals m.Id
                                join acl in _aclRepository.Table
                                on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                                from acl in m_acl.DefaultIfEmpty()
                                where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pm;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pm in query
                                join m in _publisherRepository.Table on pm.PublisherId equals m.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                                from sm in m_sm.DefaultIfEmpty()
                                where !m.LimitedToStores || currentStoreId == sm.StoreId
                                select pm;
                    }

                    //only distinct publishers (group by ID)
                    query = from pm in query
                            group pm by pm.Id
                            into pmGroup
                            orderby pmGroup.Key
                            select pmGroup.FirstOrDefault();
                    query = query.OrderBy(pm => pm.DisplaySubscription).ThenBy(pm => pm.Id);
                }

                var articlePublishers = new PagedList<ArticlePublisher>(query, pageIndex, pageSize);
                return articlePublishers;
            });
        }

        /// <summary>
        /// Gets a article publisher mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article publisher mapping collection</returns>
        public virtual IList<ArticlePublisher> GetArticlePublishersByArticleId(int articleId, bool showHidden = false)
        {
            if (articleId == 0)
                return new List<ArticlePublisher>();

            string key = string.Format(ARTICLEPUBLISHERS_ALLBYARTICLEID_KEY, showHidden, articleId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pm in _articlePublisherRepository.Table
                            join m in _publisherRepository.Table on pm.PublisherId equals m.Id
                            where pm.ArticleId == articleId &&
                                !m.Deleted &&
                                (showHidden || m.Published)
                            orderby pm.DisplaySubscription, pm.Id
                            select pm;


                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pm in query
                                join m in _publisherRepository.Table on pm.PublisherId equals m.Id
                                join acl in _aclRepository.Table
                                on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                                from acl in m_acl.DefaultIfEmpty()
                                where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pm;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pm in query
                                join m in _publisherRepository.Table on pm.PublisherId equals m.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = m.Id, c2 = "Publisher" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                                from sm in m_sm.DefaultIfEmpty()
                                where !m.LimitedToStores || currentStoreId == sm.StoreId
                                select pm;
                    }

                    //only distinct publishers (group by ID)
                    query = from pm in query
                            group pm by pm.Id
                            into mGroup
                            orderby mGroup.Key
                            select mGroup.FirstOrDefault();
                    query = query.OrderBy(pm => pm.DisplaySubscription).ThenBy(pm => pm.Id);
                }

                var articlePublishers = query.ToList();
                return articlePublishers;
            });
        }
        
        /// <summary>
        /// Gets a article publisher mapping 
        /// </summary>
        /// <param name="articlePublisherId">Article publisher mapping identifier</param>
        /// <returns>Article publisher mapping</returns>
        public virtual ArticlePublisher GetArticlePublisherById(int articlePublisherId)
        {
            if (articlePublisherId == 0)
                return null;

            return _articlePublisherRepository.GetById(articlePublisherId);
        }

        /// <summary>
        /// Inserts a article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        public virtual void InsertArticlePublisher(ArticlePublisher articlePublisher)
        {
            if (articlePublisher == null)
                throw new ArgumentNullException("articlePublisher");

            _articlePublisherRepository.Insert(articlePublisher);

            //cache
            _cacheManager.RemoveByPattern(PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEPUBLISHERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articlePublisher);
        }

        /// <summary>
        /// Updates the article publisher mapping
        /// </summary>
        /// <param name="articlePublisher">Article publisher mapping</param>
        public virtual void UpdateArticlePublisher(ArticlePublisher articlePublisher)
        {
            if (articlePublisher == null)
                throw new ArgumentNullException("articlePublisher");

            _articlePublisherRepository.Update(articlePublisher);

            //cache
            _cacheManager.RemoveByPattern(PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEPUBLISHERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articlePublisher);
        }


        /// <summary>
        /// Get publisher IDs for articles
        /// </summary>
        /// <param name="articleIds">Articles IDs</param>
        /// <returns>Publisher IDs for articles</returns>
        public virtual IDictionary<int, int[]> GetArticlePublisherIds(int[] articleIds)
        {
            var query = _articlePublisherRepository.Table;

            return query.Where(p => articleIds.Contains(p.ArticleId))
                .Select(p => new {p.ArticleId, p.PublisherId}).ToList()
                .GroupBy(a => a.ArticleId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.PublisherId).ToArray());
        }


        /// <summary>
        /// Returns a list of names of not existing publishers
        /// </summary>
        /// <param name="publisherNames">The names of the publishers to check</param>
        /// <returns>List of names not existing publishers</returns>
        public virtual string[] GetNotExistingPublishers(string[] publisherNames)
        {
            if (publisherNames == null)
                throw new ArgumentNullException("publisherNames");

            var query = _publisherRepository.Table;
            var queryFilter = publisherNames.Distinct().ToArray();
            var filter = query.Select(m => m.Name).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion
    }
}
