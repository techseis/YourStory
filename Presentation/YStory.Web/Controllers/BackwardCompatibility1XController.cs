using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using YStory.Services.Blogs;
using YStory.Services.Catalog;
using YStory.Services.Customers;
using YStory.Services.Forums;
using YStory.Services.News;
using YStory.Services.Seo;
using YStory.Services.Topics;

namespace YStory.Web.Controllers
{
    public partial class BackwardCompatibility1XController : BasePublicController
    {
		#region Fields

        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IArticleTagService _articleTagService;
        private readonly INewsService _newsService;
        private readonly IBlogService _blogService;
        private readonly ITopicService _topicService;
        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        #endregion

		#region Constructors

        public BackwardCompatibility1XController(IArticleService articleService,
            ICategoryService categoryService, IPublisherService publisherService,
            IArticleTagService articleTagService, INewsService newsService,
            IBlogService blogService, ITopicService topicService,
            IForumService forumService, ICustomerService customerService)
        {
            this._articleService = articleService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleTagService = articleTagService;
            this._newsService = newsService;
            this._blogService = blogService;
            this._topicService = topicService;
            this._forumService = forumService;
            this._customerService = customerService;
        }

		#endregion
        
        #region Methods

        public virtual ActionResult GeneralRedirect()
        {
            
            // use Request.RawUrl, for instance to parse out what was invoked
            // this regex will extract anything between a "/" and a ".aspx"
            var regex = new Regex(@"(?<=/).+(?=\.aspx)", RegexOptions.Compiled);
            var aspxfileName = regex.Match(Request.RawUrl).Value.ToLowerInvariant();


            switch (aspxfileName)
            {
                //URL without rewriting
                case "article":
                    {
                        return RedirectArticle(Request.QueryString["articleid"], false);
                    }
                case "category":
                    {
                        return RedirectCategory(Request.QueryString["categoryid"], false);
                    }
                case "publisher":
                    {
                        return RedirectPublisher(Request.QueryString["publisherid"], false);
                    }
                case "articletag":
                    {
                        return RedirectArticleTag(Request.QueryString["tagid"], false);
                    }
                case "news":
                    {
                        return RedirectNewsItem(Request.QueryString["newsid"], false);
                    }
                case "blog":
                    {
                        return RedirectBlogPost(Request.QueryString["blogpostid"], false);
                    }
                case "topic":
                    {
                        return RedirectTopic(Request.QueryString["topicid"], false);
                    }
                case "profile":
                    {
                        return RedirectUserProfile(Request.QueryString["UserId"]);
                    }
                case "comparearticles":
                    {
                        return RedirectToRoutePermanent("CompareArticles");
                    }
                case "contactus":
                    {
                        return RedirectToRoutePermanent("ContactUs");
                    }
                case "passwordrecovery":
                    {
                        return RedirectToRoutePermanent("PasswordRecovery");
                    }
                case "login":
                    {
                        return RedirectToRoutePermanent("Login");
                    }
                case "register":
                    {
                        return RedirectToRoutePermanent("Register");
                    }
                case "newsarchive":
                    {
                        return RedirectToRoutePermanent("NewsArchive");
                    }
                case "search":
                    {
                        return RedirectToRoutePermanent("ArticleSearch");
                    }
                case "sitemap":
                    {
                        return RedirectToRoutePermanent("Sitemap");
                    }
                case "recentlyaddedarticles":
                    {
                        return RedirectToRoutePermanent("NewArticles");
                    }
                case "shoppingcart":
                    {
                        return RedirectToRoutePermanent("ShoppingCart");
                    }
                case "wishlist":
                    {
                        return RedirectToRoutePermanent("Wishlist");
                    }
                default:
                    break;
            }

            //no permanent redirect in this case
            return RedirectToRoute("HomePage");
        }

        public virtual ActionResult RedirectArticle(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var articleId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Article", new { SeName = article.GetSeName() });
        }

        public virtual ActionResult RedirectCategory(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var categoryid = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var category = _categoryService.GetCategoryById(categoryid);
            if (category == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Category", new { SeName = category.GetSeName() });
        }

        public virtual ActionResult RedirectPublisher(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var publisherId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var publisher = _publisherService.GetPublisherById(publisherId);
            if (publisher == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Publisher", new { SeName = publisher.GetSeName() });
        }

        public virtual ActionResult RedirectArticleTag(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var tagId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var tag = _articleTagService.GetArticleTagById(tagId);
            if (tag == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ArticlesByTag", new { articleTagId = tag.Id });
        }

        public virtual ActionResult RedirectNewsItem(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var newsId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var newsItem = _newsService.GetNewsById(newsId);
            if (newsItem == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("NewsItem", new { newsItemId = newsItem.Id, SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual ActionResult RedirectBlogPost(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var blogPostId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("BlogPost", new { blogPostId = blogPost.Id, SeName = blogPost.GetSeName(blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual ActionResult RedirectTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var topicid = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _topicService.GetTopicById(topicid);
            if (topic == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Topic", new { SeName = topic.GetSeName() });
        }

        public virtual ActionResult RedirectForumGroup(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumGroupId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var forumGroup = _forumService.GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ForumGroupSlug", new { id = forumGroup.Id, slug = forumGroup.GetSeName() });
        }

        public virtual ActionResult RedirectForum(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var forum = _forumService.GetForumById(forumId);
            if (forum == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ForumSlug", new { id = forum.Id, slug = forum.GetSeName() });
        }

        public virtual ActionResult RedirectForumTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumTopicId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _forumService.GetTopicById(forumTopicId);
            if (topic == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() });
        }

        public virtual ActionResult RedirectUserProfile(string id)
        {
            //we can't use dash in MVC
            var userId = Convert.ToInt32(id);
            var user = _customerService.GetCustomerById(userId);
            if (user == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("CustomerProfile", new { id = user.Id});
        }
        
        #endregion
    }
}
