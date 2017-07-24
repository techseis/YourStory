using YStory.Core.Caching;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Configuration;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.News;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Polls;
using YStory.Core.Domain.Topics;
using YStory.Core.Domain.Contributors;
using YStory.Core.Events;
using YStory.Core.Infrastructure;
using YStory.Services.Events;

namespace YStory.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //languages
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //currencies
        IConsumer<EntityInserted<Currency>>,
        IConsumer<EntityUpdated<Currency>>,
        IConsumer<EntityDeleted<Currency>>,
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //publishers
        IConsumer<EntityInserted<Publisher>>,
        IConsumer<EntityUpdated<Publisher>>,
        IConsumer<EntityDeleted<Publisher>>,
        //contributors
        IConsumer<EntityInserted<Contributor>>,
        IConsumer<EntityUpdated<Contributor>>,
        IConsumer<EntityDeleted<Contributor>>,
        //article publishers
        IConsumer<EntityInserted<ArticlePublisher>>,
        IConsumer<EntityUpdated<ArticlePublisher>>,
        IConsumer<EntityDeleted<ArticlePublisher>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //article categories
        IConsumer<EntityInserted<ArticleCategory>>,
        IConsumer<EntityUpdated<ArticleCategory>>,
        IConsumer<EntityDeleted<ArticleCategory>>,
        //articles
        IConsumer<EntityInserted<Article>>,
        IConsumer<EntityUpdated<Article>>,
        IConsumer<EntityDeleted<Article>>,
        //related article
        IConsumer<EntityInserted<RelatedArticle>>,
        IConsumer<EntityUpdated<RelatedArticle>>,
        IConsumer<EntityDeleted<RelatedArticle>>,
        //article tags
        IConsumer<EntityInserted<ArticleTag>>,
        IConsumer<EntityUpdated<ArticleTag>>,
        IConsumer<EntityDeleted<ArticleTag>>,
        //specification attributes
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdated<SpecificationAttributeOption>>,
        IConsumer<EntityDeleted<SpecificationAttributeOption>>,
        //Article specification attribute
        IConsumer<EntityInserted<ArticleSpecificationAttribute>>,
        IConsumer<EntityUpdated<ArticleSpecificationAttribute>>,
        IConsumer<EntityDeleted<ArticleSpecificationAttribute>>,
        //Article attributes
        IConsumer<EntityDeleted<ArticleAttribute>>,
        //Article attributes
        IConsumer<EntityInserted<ArticleAttributeMapping>>,
        IConsumer<EntityDeleted<ArticleAttributeMapping>>,
        //Article attribute values
        IConsumer<EntityUpdated<ArticleAttributeValue>>,
        //Topics
        IConsumer<EntityInserted<Topic>>,
        IConsumer<EntityUpdated<Topic>>,
        IConsumer<EntityDeleted<Topic>>,
        //Subscriptions
        IConsumer<EntityInserted<Subscription>>,
        IConsumer<EntityUpdated<Subscription>>,
        IConsumer<EntityDeleted<Subscription>>,
        //Picture
        IConsumer<EntityInserted<Picture>>,
        IConsumer<EntityUpdated<Picture>>,
        IConsumer<EntityDeleted<Picture>>,
        //Article picture mapping
        IConsumer<EntityInserted<ArticlePicture>>,
        IConsumer<EntityUpdated<ArticlePicture>>,
        IConsumer<EntityDeleted<ArticlePicture>>,
        //Article review
        IConsumer<EntityDeleted<ArticleReview>>,
        //polls
        IConsumer<EntityInserted<Poll>>,
        IConsumer<EntityUpdated<Poll>>,
        IConsumer<EntityDeleted<Poll>>,
        //blog posts
        IConsumer<EntityInserted<BlogPost>>,
        IConsumer<EntityUpdated<BlogPost>>,
        IConsumer<EntityDeleted<BlogPost>>,
        //blog comments
        IConsumer<EntityDeleted<BlogComment>>,
        //news items
        IConsumer<EntityInserted<NewsItem>>,
        IConsumer<EntityUpdated<NewsItem>>,
        IConsumer<EntityDeleted<NewsItem>>,
        //news comments
        IConsumer<EntityDeleted<NewsComment>>,
        //states/province
        IConsumer<EntityInserted<StateProvince>>,
        IConsumer<EntityUpdated<StateProvince>>,
        IConsumer<EntityDeleted<StateProvince>>,
        //return requests
        IConsumer<EntityInserted<ReturnRequestAction>>,
        IConsumer<EntityUpdated<ReturnRequestAction>>,
        IConsumer<EntityDeleted<ReturnRequestAction>>,
        IConsumer<EntityInserted<ReturnRequestReason>>,
        IConsumer<EntityUpdated<ReturnRequestReason>>,
        IConsumer<EntityDeleted<ReturnRequestReason>>,
        //templates
        IConsumer<EntityInserted<CategoryTemplate>>,
        IConsumer<EntityUpdated<CategoryTemplate>>,
        IConsumer<EntityDeleted<CategoryTemplate>>,
        IConsumer<EntityInserted<PublisherTemplate>>,
        IConsumer<EntityUpdated<PublisherTemplate>>,
        IConsumer<EntityDeleted<PublisherTemplate>>,
        IConsumer<EntityInserted<ArticleTemplate>>,
        IConsumer<EntityUpdated<ArticleTemplate>>,
        IConsumer<EntityDeleted<ArticleTemplate>>,
        IConsumer<EntityInserted<TopicTemplate>>,
        IConsumer<EntityUpdated<TopicTemplate>>,
        IConsumer<EntityDeleted<TopicTemplate>>,
        //checkout attributes
        IConsumer<EntityInserted<CheckoutAttribute>>,
        IConsumer<EntityUpdated<CheckoutAttribute>>,
        IConsumer<EntityDeleted<CheckoutAttribute>>,
        //shopping cart items
        IConsumer<EntityUpdated<ShoppingCartItem>>
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(CatalogSettings catalogSettings)
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            this._catalogSettings = catalogSettings;
        }
        
        #endregion 

        #region Cache keys 

        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SEARCH_CATEGORIES_MODEL_KEY = "YStory.pres.search.categories-{0}-{1}-{2}";
        public const string SEARCH_CATEGORIES_PATTERN_KEY = "YStory.pres.search.categories";

        /// <summary>
        /// Key for PublisherNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current publisher id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string PUBLISHER_NAVIGATION_MODEL_KEY = "YStory.pres.publisher.navigation-{0}-{1}-{2}-{3}";
        public const string PUBLISHER_NAVIGATION_PATTERN_KEY = "YStory.pres.publisher.navigation";

        /// <summary>
        /// Key for ContributorNavigationModel caching
        /// </summary>
        public const string CONTRIBUTOR_NAVIGATION_MODEL_KEY = "YStory.pres.contributor.navigation";
        public const string CONTRIBUTOR_NAVIGATION_PATTERN_KEY = "YStory.pres.contributor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a publisher has featured articles
        /// </summary>
        /// <remarks>
        /// {0} : publisher id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string PUBLISHER_HAS_FEATURED_ARTICLES_KEY = "YStory.pres.publisher.hasfeaturedarticles-{0}-{1}-{2}";
        public const string PUBLISHER_HAS_FEATURED_ARTICLES_PATTERN_KEY = "YStory.pres.publisher.hasfeaturedarticles";
        public const string PUBLISHER_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID = "YStory.pres.publisher.hasfeaturedarticles-{0}-";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_ALL_MODEL_KEY = "YStory.pres.category.all-{0}-{1}-{2}";
        public const string CATEGORY_ALL_PATTERN_KEY = "YStory.pres.category.all";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of customer roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public const string CATEGORY_NUMBER_OF_ARTICLES_MODEL_KEY = "YStory.pres.category.numberofarticles-{0}-{1}-{2}";
        public const string CATEGORY_NUMBER_OF_ARTICLES_PATTERN_KEY = "YStory.pres.category.numberofarticles";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured articles
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_HAS_FEATURED_ARTICLES_KEY = "YStory.pres.category.hasfeaturedarticles-{0}-{1}-{2}";
        public const string CATEGORY_HAS_FEATURED_ARTICLES_PATTERN_KEY = "YStory.pres.category.hasfeaturedarticles";
        public const string CATEGORY_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID = "YStory.pres.category.hasfeaturedarticles-{0}-";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public const string CATEGORY_BREADCRUMB_KEY = "YStory.pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public const string CATEGORY_BREADCRUMB_PATTERN_KEY = "YStory.pres.category.breadcrumb";

        /// <summary>
        /// Key for caching of subcategories of certain category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_SUBCATEGORIES_KEY = "YStory.pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_SUBCATEGORIES_PATTERN_KEY = "YStory.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_HOMEPAGE_KEY = "YStory.pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public const string CATEGORY_HOMEPAGE_PATTERN_KEY = "YStory.pres.category.homepage";
        
        /// <summary>
        /// Key for GetChildCategoryIds method results caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "YStory.pres.category.childidentifiers-{0}-{1}-{2}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "YStory.pres.category.childidentifiers";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public const string SPECS_FILTER_MODEL_KEY = "YStory.pres.filter.specs-{0}-{1}";
        public const string SPECS_FILTER_PATTERN_KEY = "YStory.pres.filter.specs";

        /// <summary>
        /// Key for ArticleBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public const string ARTICLE_BREADCRUMB_MODEL_KEY = "YStory.pres.article.breadcrumb-{0}-{1}-{2}-{3}";
        public const string ARTICLE_BREADCRUMB_PATTERN_KEY = "YStory.pres.article.breadcrumb";
        public const string ARTICLE_BREADCRUMB_PATTERN_KEY_BY_ID = "YStory.pres.article.breadcrumb-{0}-";

        /// <summary>
        /// Key for ArticleTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLETAG_BY_ARTICLE_MODEL_KEY = "YStory.pres.articletag.byarticle-{0}-{1}-{2}";
        public const string ARTICLETAG_BY_ARTICLE_PATTERN_KEY = "YStory.pres.articletag.byarticle";

        /// <summary>
        /// Key for PopularArticleTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string ARTICLETAG_POPULAR_MODEL_KEY = "YStory.pres.articletag.popular-{0}-{1}";
        public const string ARTICLETAG_POPULAR_PATTERN_KEY = "YStory.pres.articletag.popular";

        /// <summary>
        /// Key for ArticlePublishers model caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string ARTICLE_PUBLISHERS_MODEL_KEY = "YStory.pres.article.publishers-{0}-{1}-{2}-{3}";
        public const string ARTICLE_PUBLISHERS_PATTERN_KEY = "YStory.pres.article.publishers";
        public const string ARTICLE_PUBLISHERS_PATTERN_KEY_BY_ID = "YStory.pres.article.publishers-{0}-";

        /// <summary>
        /// Key for ArticleSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : language id
        /// </remarks>
        public const string ARTICLE_SPECS_MODEL_KEY = "YStory.pres.article.specs-{0}-{1}";
        public const string ARTICLE_SPECS_PATTERN_KEY = "YStory.pres.article.specs";
        public const string ARTICLE_SPECS_PATTERN_KEY_BY_ID = "YStory.pres.article.specs-{0}-";

        /// <summary>
        /// Key for caching of a value indicating whether a article has article attributes
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// </remarks>
        public const string ARTICLE_HAS_ARTICLE_ATTRIBUTES_KEY = "YStory.pres.article.hasarticleattributes-{0}-";
        public const string ARTICLE_HAS_ARTICLE_ATTRIBUTES_PATTERN_KEY = "YStory.pres.article.hasarticleattributes";
        public const string ARTICLE_HAS_ARTICLE_ATTRIBUTES_PATTERN_KEY_BY_ID = "YStory.pres.article.hasarticleattributes-{0}-";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_SYSTEMNAME_KEY = "YStory.pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_ID_KEY = "YStory.pres.topic.details.byid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_SENAME_BY_SYSTEMNAME = "YStory.pres.topic.sename.bysystemname-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_TOP_MENU_MODEL_KEY = "YStory.pres.topic.topmenu-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_FOOTER_MODEL_KEY = "YStory.pres.topic.footer-{0}-{1}-{2}";
        public const string TOPIC_PATTERN_KEY = "YStory.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public const string CATEGORY_TEMPLATE_MODEL_KEY = "YStory.pres.categorytemplate-{0}";
        public const string CATEGORY_TEMPLATE_PATTERN_KEY = "YStory.pres.categorytemplate";

        /// <summary>
        /// Key for PublisherTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : publisher template id
        /// </remarks>
        public const string PUBLISHER_TEMPLATE_MODEL_KEY = "YStory.pres.publishertemplate-{0}";
        public const string PUBLISHER_TEMPLATE_PATTERN_KEY = "YStory.pres.publishertemplate";

        /// <summary>
        /// Key for ArticleTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : article template id
        /// </remarks>
        public const string ARTICLE_TEMPLATE_MODEL_KEY = "YStory.pres.articletemplate-{0}";
        public const string ARTICLE_TEMPLATE_PATTERN_KEY = "YStory.pres.articletemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public const string TOPIC_TEMPLATE_MODEL_KEY = "YStory.pres.topictemplate-{0}";
        public const string TOPIC_TEMPLATE_PATTERN_KEY = "YStory.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string HOMEPAGE_BESTSELLERS_IDS_KEY = "YStory.pres.bestsellers.homepage-{0}";
        public const string HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY = "YStory.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" article identifiers displayed on the article details page
        /// </summary>
        /// <remarks>
        /// {0} : current article id
        /// {1} : current store ID
        /// </remarks>
        public const string ARTICLES_ALSO_PURCHASED_IDS_KEY = "YStory.pres.alsopuchased-{0}-{1}";
        public const string ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY = "YStory.pres.alsopuchased";

        /// <summary>
        /// Key for "related" article identifiers displayed on the article details page
        /// </summary>
        /// <remarks>
        /// {0} : current article id
        /// {1} : current store ID
        /// </remarks>
        public const string ARTICLES_RELATED_IDS_KEY = "YStory.pres.related-{0}-{1}";
        public const string ARTICLES_RELATED_IDS_PATTERN_KEY = "YStory.pres.related";

        /// <summary>
        /// Key for default article picture caching (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : picture size
        /// {2} : isAssociatedArticle?
        /// {3} : language ID ("alt" and "title" can depend on localized article name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string ARTICLE_DEFAULTPICTURE_MODEL_KEY = "YStory.pres.article.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string ARTICLE_DEFAULTPICTURE_PATTERN_KEY = "YStory.pres.article.detailspictures";
        public const string ARTICLE_DEFAULTPICTURE_PATTERN_KEY_BY_ID = "YStory.pres.article.detailspictures-{0}-";

        /// <summary>
        /// Key for article picture caching on the article details page
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized article name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string ARTICLE_DETAILS_PICTURES_MODEL_KEY = "YStory.pres.article.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string ARTICLE_DETAILS_PICTURES_PATTERN_KEY = "YStory.pres.article.picture";
        public const string ARTICLE_DETAILS_PICTURES_PATTERN_KEY_BY_ID = "YStory.pres.article.picture-{0}-";

        /// <summary>
        /// Key for article reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : article id
        /// {1} : current store ID
        /// </remarks>
        public const string ARTICLE_REVIEWS_MODEL_KEY = "YStory.pres.article.reviews-{0}-{1}";
        public const string ARTICLE_REVIEWS_PATTERN_KEY = "YStory.pres.article.reviews";
        public const string ARTICLE_REVIEWS_PATTERN_KEY_BY_ID = "YStory.pres.article.reviews-{0}-";

        /// <summary>
        /// Key for article attribute picture caching on the article details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLEATTRIBUTE_PICTURE_MODEL_KEY = "YStory.pres.articleattribute.picture-{0}-{1}-{2}";
        public const string ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY = "YStory.pres.articleattribute.picture";

        /// <summary>
        /// Key for article attribute picture caching on the article details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string ARTICLEATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "YStory.pres.articleattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string ARTICLEATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "YStory.pres.articleattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CATEGORY_PICTURE_MODEL_KEY = "YStory.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "YStory.pres.category.picture";
        public const string CATEGORY_PICTURE_PATTERN_KEY_BY_ID = "YStory.pres.category.picture-{0}-";

        /// <summary>
        /// Key for publisher picture caching
        /// </summary>
        /// <remarks>
        /// {0} : publisher id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized publisher name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PUBLISHER_PICTURE_MODEL_KEY = "YStory.pres.publisher.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PUBLISHER_PICTURE_PATTERN_KEY = "YStory.pres.publisher.picture";
        public const string PUBLISHER_PICTURE_PATTERN_KEY_BY_ID = "YStory.pres.publisher.picture-{0}-";

        /// <summary>
        /// Key for contributor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : contributor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CONTRIBUTOR_PICTURE_MODEL_KEY = "YStory.pres.contributor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CONTRIBUTOR_PICTURE_PATTERN_KEY = "YStory.pres.contributor.picture";
        public const string CONTRIBUTOR_PICTURE_PATTERN_KEY_BY_ID = "YStory.pres.contributor.picture-{0}-";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by article ID. it could increase performance.
        /// but it won't work for article attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized article name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CART_PICTURE_MODEL_KEY = "YStory.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CART_PICTURE_PATTERN_KEY = "YStory.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string HOMEPAGE_POLLS_MODEL_KEY = "YStory.pres.poll.homepage-{0}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// </remarks>
        public const string POLL_BY_SYSTEMNAME_MODEL_KEY = "YStory.pres.poll.systemname-{0}-{1}";
        public const string POLLS_PATTERN_KEY = "YStory.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_TAGS_MODEL_KEY = "YStory.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_MONTHS_MODEL_KEY = "YStory.pres.blog.months-{0}-{1}";
        public const string BLOG_PATTERN_KEY = "YStory.pres.blog";
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public const string BLOG_COMMENTS_NUMBER_KEY = "YStory.pres.blog.comments.number-{0}-{1}-{2}";
        public const string BLOG_COMMENTS_PATTERN_KEY = "YStory.pres.blog.comments";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string HOMEPAGE_NEWSMODEL_KEY = "YStory.pres.news.homepage-{0}-{1}";
        public const string NEWS_PATTERN_KEY = "YStory.pres.news";
        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public const string NEWS_COMMENTS_NUMBER_KEY = "YStory.pres.news.comments.number-{0}-{1}-{2}";
        public const string NEWS_COMMENTS_PATTERN_KEY = "YStory.pres.news.comments";
        
        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "YStory.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "YStory.pres.stateprovinces";

        /// <summary>
        /// Key for return request reasons
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTREASONS_MODEL_KEY = "YStory.pres.returnrequesreasons-{0}";
        public const string RETURNREQUESTREASONS_PATTERN_KEY = "YStory.pres.returnrequesreasons";

        /// <summary>
        /// Key for return request actions
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTACTIONS_MODEL_KEY = "YStory.pres.returnrequestactions-{0}";
        public const string RETURNREQUESTACTIONS_PATTERN_KEY = "YStory.pres.returnrequestactions";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public const string STORE_LOGO_PATH = "YStory.pres.logo-{0}-{1}-{2}";
        public const string STORE_LOGO_PATH_PATTERN_KEY = "YStory.pres.logo";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "YStory.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "YStory.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "YStory.pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "YStory.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public const string CHECKOUTATTRIBUTES_EXIST_KEY = "YStory.pres.checkoutattributes.exist-{0}";
        public const string CHECKOUTATTRIBUTES_PATTERN_KEY = "YStory.pres.checkoutattributes";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "YStory.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string SITEMAP_SEO_MODEL_KEY = "YStory.pres.sitemap.seo-{0}-{1}-{2}-{3}";
        public const string SITEMAP_PATTERN_KEY = "YStory.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current customer ID
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public const string WIDGET_MODEL_KEY = "YStory.pres.widget-{0}-{1}-{2}-{3}";
        public const string WIDGET_PATTERN_KEY = "YStory.pres.widget";

        #endregion

        #region Methods

        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        
        //currencies
        public void HandleEvent(EntityInserted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY); //depends on CatalogSettings.NumberOfArticleTags
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.PublishersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CONTRIBUTOR_NAVIGATION_PATTERN_KEY); //depends on ContributorSettings.ContributorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryArticleNumber and CatalogSettings.ShowCategoryArticleNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_ARTICLES_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryArticleNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY); //depends on CatalogSettings.ArticlesAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(WIDGET_PATTERN_KEY); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPattern(STORE_LOGO_PATH_PATTERN_KEY); //depends on StoreInformationSettings.LogoPictureId
        }

        //contributors
        public void HandleEvent(EntityInserted<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTOR_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTOR_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CONTRIBUTOR_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeleted<Contributor> eventMessage)
        {
            _cacheManager.RemoveByPattern(CONTRIBUTOR_NAVIGATION_PATTERN_KEY);
        }

        //publishers
        public void HandleEvent(EntityInserted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            
        }
        public void HandleEvent(EntityUpdated<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(PUBLISHER_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeleted<Publisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_PUBLISHERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //article publishers
        public void HandleEvent(EntityInserted<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_PUBLISHERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(PUBLISHER_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.PublisherId));
        }
        public void HandleEvent(EntityUpdated<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_PUBLISHERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(PUBLISHER_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.PublisherId));
        }
        public void HandleEvent(EntityDeleted<ArticlePublisher> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_PUBLISHERS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(PUBLISHER_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.PublisherId));
        }
        
        //categories
        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_PICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLE_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //article categories
        public void HandleEvent(EntityInserted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            if (_catalogSettings.ShowCategoryArticleNumber)
            {
                //depends on CatalogSettings.ShowCategoryArticleNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            }
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityUpdated<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityDeleted<ArticleCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_BREADCRUMB_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            if (_catalogSettings.ShowCategoryArticleNumber)
            {
                //depends on CatalogSettings.ShowCategoryArticleNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            }
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_ARTICLES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(CATEGORY_HAS_FEATURED_ARTICLES_PATTERN_KEY_BY_ID, eventMessage.Entity.CategoryId));
        }

        //articles
        public void HandleEvent(EntityInserted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_REVIEWS_PATTERN_KEY_BY_ID, eventMessage.Entity.Id));
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Article> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        
        //article tags
        public void HandleEvent(EntityInserted<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLETAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLETAG_BY_ARTICLE_PATTERN_KEY);
        }
        
        //related articles
        public void HandleEvent(EntityInserted<RelatedArticle> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<RelatedArticle> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<RelatedArticle> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLES_RELATED_IDS_PATTERN_KEY);
        }
        
        //specification attributes
        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //specification attribute options
        public void HandleEvent(EntityUpdated<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //Article specification attribute
        public void HandleEvent(EntityInserted<ArticleSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_SPECS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //Article attributes
        public void HandleEvent(EntityDeleted<ArticleAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_HAS_ARTICLE_ATTRIBUTES_PATTERN_KEY);
        }
        //Article attributes
        public void HandleEvent(EntityInserted<ArticleAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_HAS_ARTICLE_ATTRIBUTES_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
        }
        public void HandleEvent(EntityDeleted<ArticleAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_HAS_ARTICLE_ATTRIBUTES_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
        }
        //Article attributes
        public void HandleEvent(EntityUpdated<ArticleAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY);
        }

        //Topics
        public void HandleEvent(EntityInserted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        
        //Subscriptions
        public void HandleEvent(EntityInserted<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Subscription> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLES_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }

        //Pictures
        public void HandleEvent(EntityInserted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Article picture mappings
        public void HandleEvent(EntityInserted<ArticlePicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticlePicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticlePicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DEFAULTPICTURE_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_DETAILS_PICTURES_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
            _cacheManager.RemoveByPattern(ARTICLEATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Polls
        public void HandleEvent(EntityInserted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }

        //Blog posts
        public void HandleEvent(EntityInserted<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }

        //Blog comments
        public void HandleEvent(EntityDeleted<BlogComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_COMMENTS_PATTERN_KEY);
        }

        //News items
        public void HandleEvent(EntityInserted<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        //News comments
        public void HandleEvent(EntityDeleted<NewsComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_COMMENTS_PATTERN_KEY);
        }

        //State/province
        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //return requests
        public void HandleEvent(EntityInserted<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }

        //templates
        public void HandleEvent(EntityInserted<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<PublisherTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<PublisherTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<PublisherTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PUBLISHER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<ArticleTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ArticleTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ArticleTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ARTICLE_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }

        //checkout attributes
        public void HandleEvent(EntityInserted<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }

        //shopping cart items
        public void HandleEvent(EntityUpdated<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //article reviews
        public void HandleEvent(EntityDeleted<ArticleReview> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(ARTICLE_REVIEWS_PATTERN_KEY_BY_ID, eventMessage.Entity.ArticleId));
        }

        #endregion
    }
}
