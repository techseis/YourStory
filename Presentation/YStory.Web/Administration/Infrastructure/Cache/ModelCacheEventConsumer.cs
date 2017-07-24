using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Configuration;
using YStory.Core.Domain.Contributors;
using YStory.Core.Events;
using YStory.Core.Infrastructure;
using YStory.Services.Events;

namespace YStory.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //specification attributes
        IConsumer<EntityInserted<SpecificationAttribute>>,
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //publishers
        IConsumer<EntityInserted<Publisher>>,
        IConsumer<EntityUpdated<Publisher>>,
        IConsumer<EntityDeleted<Publisher>>,
        //contributors
        IConsumer<EntityInserted<Contributor>>,
        IConsumer<EntityUpdated<Contributor>>,
        IConsumer<EntityDeleted<Contributor>>
    {
        /// <summary>
        /// Key for yourStory.com news cache
        /// </summary>
        public const string OFFICIAL_NEWS_MODEL_KEY = "YStory.pres.admin.official.news";
        public const string OFFICIAL_NEWS_PATTERN_KEY = "YStory.pres.admin.official.news";
        
        /// <summary>
        /// Key for specification attributes caching (article details page)
        /// </summary>
        public const string SPEC_ATTRIBUTES_MODEL_KEY = "YStory.pres.admin.article.specs";
        public const string SPEC_ATTRIBUTES_PATTERN_KEY = "YStory.pres.admin.article.specs";

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string CATEGORIES_LIST_KEY = "YStory.pres.admin.categories.list-{0}";
        public const string CATEGORIES_LIST_PATTERN_KEY = "YStory.pres.admin.categories.list";

        /// <summary>
        /// Key for publishers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string PUBLISHERS_LIST_KEY = "YStory.pres.admin.publishers.list-{0}";
        public const string PUBLISHERS_LIST_PATTERN_KEY = "YStory.pres.admin.publishers.list";

        /// <summary>
        /// Key for contributors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string CONTRIBUTORS_LIST_KEY = "YStory.pres.admin.contributors.list-{0}";
        public const string CONTRIBUTORS_LIST_PATTERN_KEY = "YStory.pres.admin.contributors.list";


        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(OFFICIAL_NEWS_PATTERN_KEY); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }
        
        //specification attributes
        public void HandleEvent(EntityInserted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(SPEC_ATTRIBUTES_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORIES_LIST_PATTERN_KEY);
        }

        //publishers
        public void HandleEvent(EntityInserted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHERS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHERS_LIST_PATTERN_KEY);
        }

        //contributors
        public void HandleEvent(EntityInserted<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTORS_LIST_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTORS_LIST_PATTERN_KEY);
        }
    }
}
