using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Configuration;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Events;
using YStory.Core.Infrastructure;
using YStory.Services.Events;

namespace YStory.Services.Catalog.Cache
{
    /// <summary>
    /// Price cache event consumer (used for caching of prices)
    /// </summary>
    public partial class PriceCacheEventConsumer: 
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //publishers
        IConsumer<EntityInserted<Publisher>>,
        IConsumer<EntityUpdated<Publisher>>,
        IConsumer<EntityDeleted<Publisher>>,
        //article categories
        IConsumer<EntityInserted<ArticleCategory>>,
        IConsumer<EntityUpdated<ArticleCategory>>,
        IConsumer<EntityDeleted<ArticleCategory>>,
        //article publishers
        IConsumer<EntityInserted<ArticlePublisher>>,
        IConsumer<EntityUpdated<ArticlePublisher>>,
        IConsumer<EntityDeleted<ArticlePublisher>>,
        //articles
        IConsumer<EntityInserted<Article>>,
        IConsumer<EntityUpdated<Article>>,
        IConsumer<EntityDeleted<Article>>,
        
        //subscriptions
        IConsumer<EntityInserted<Subscription>>,
        IConsumer<EntityUpdated<Subscription>>,
        IConsumer<EntityDeleted<Subscription>>
    {
        /// <summary>
        /// Key for article prices
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : overridden article price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public const string ARTICLE_PRICE_MODEL_KEY = "YStory.totals.articleprice-{0}-{1}-{2}-{3}-{4}-{5}-{6}";
        public const string ARTICLE_PRICE_PATTERN_KEY = "YStory.totals.articleprice";

        /// <summary>
        /// Key for category IDs of a article
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLE_CATEGORY_IDS_MODEL_KEY = "YStory.totals.article.categoryids-{0}-{1}-{2}";
        public const string ARTICLE_CATEGORY_IDS_PATTERN_KEY = "YStory.totals.article.categoryids";

        /// <summary>
        /// Key for publisher IDs of a article
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLE_PUBLISHER_IDS_MODEL_KEY = "YStory.totals.article.publisherids-{0}-{1}-{2}";
        public const string ARTICLE_PUBLISHER_IDS_PATTERN_KEY = "YStory.totals.article.publisherids";

        private readonly ICacheManager _cacheManager;

        public PriceCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        //settings
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }

        //publishers
        public void HandleEvent(EntityInserted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }

        //article categories
        public void HandleEvent(EntityInserted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_CATEGORY_IDS_PATTERN_KEY);
        }

        //article publishers
        public void HandleEvent(EntityInserted<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHER_IDS_PATTERN_KEY);
        }

        //articles
        public void HandleEvent(EntityInserted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }

        

        //subscriptions
        public void HandleEvent(EntityInserted<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_PRICE_PATTERN_KEY);
        }
    }
}
