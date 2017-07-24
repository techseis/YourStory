using System.Web.Mvc;
using YStory.Services.Blogs;
using YStory.Services.Catalog;
using YStory.Services.News;
using YStory.Services.Seo;
using YStory.Services.Topics;
using YStory.Services.Contributors;

namespace YStory.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BasePublicController
    {
		#region Fields

        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly INewsService _newsService;
        private readonly IBlogService _blogService;
        private readonly ITopicService _topicService;
        private readonly IContributorService _contributorService;

        #endregion

		#region Constructors

        public BackwardCompatibility2XController(IArticleService articleService,
            ICategoryService categoryService, 
            IPublisherService publisherService,
            INewsService newsService, 
            IBlogService blogService,
            ITopicService topicService,
            IContributorService contributorService)
        {
            this._articleService = articleService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._newsService = newsService;
            this._blogService = blogService;
            this._topicService = topicService;
            this._contributorService = contributorService;
        }

		#endregion
        
        #region Methods
        
        //in versions 2.00-2.65 we had ID in article URLs
        public virtual ActionResult RedirectArticleById(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Article", new { SeName = article.GetSeName() });
        }
        //in versions 2.00-2.65 we had ID in category URLs
        public virtual ActionResult RedirectCategoryById(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Category", new { SeName = category.GetSeName() });
        }
        //in versions 2.00-2.65 we had ID in publisher URLs
        public virtual ActionResult RedirectPublisherById(int publisherId)
        {
            var publisher = _publisherService.GetPublisherById(publisherId);
            if (publisher == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Publisher", new { SeName = publisher.GetSeName() });
        }
        //in versions 2.00-2.70 we had ID in news URLs
        public virtual ActionResult RedirectNewsItemById(int newsItemId)
        {
            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("NewsItem", new { SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }
        //in versions 2.00-2.70 we had ID in blog URLs
        public virtual ActionResult RedirectBlogPostById(int blogPostId)
        {
            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("BlogPost", new { SeName = blogPost.GetSeName(blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }
        //in versions 2.00-3.20 we had SystemName in topic URLs
        public virtual ActionResult RedirectTopicBySystemName(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Topic", new { SeName = topic.GetSeName() });
        }
        //in versions 3.00-3.20 we had ID in contributor URLs
        public virtual ActionResult RedirectContributorById(int contributorId)
        {
            var contributor = _contributorService.GetContributorById(contributorId);
            if (contributor == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Contributor", new { SeName = contributor.GetSeName() });
        }
        #endregion
    }
}
