using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Recently viewed articles service
    /// </summary>
    public partial class RecentlyViewedArticlesService : IRecentlyViewedArticlesService
    {
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IArticleService _articleService;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="articleService">Article service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public RecentlyViewedArticlesService(HttpContextBase httpContext, IArticleService articleService,
            CatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._articleService = articleService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a "recently viewed articles" identifier list
        /// </summary>
        /// <returns>"recently viewed articles" list</returns>
        protected IList<int> GetRecentlyViewedArticlesIds()
        {
            return GetRecentlyViewedArticlesIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a "recently viewed articles" identifier list
        /// </summary>
        /// <param name="number">Number of articles to load</param>
        /// <returns>"recently viewed articles" list</returns>
        protected IList<int> GetRecentlyViewedArticlesIds(int number)
        {
            var articleIds = new List<int>();
            var recentlyViewedCookie = _httpContext.Request.Cookies.Get("YourStory.RecentlyViewedArticles");
            if (recentlyViewedCookie == null)
                return articleIds;
            string[] values = recentlyViewedCookie.Values.GetValues("RecentlyViewedArticleIds");
            if (values == null)
                return articleIds;
            foreach (string articleId in values)
            {
                int prodId = int.Parse(articleId);
                if (!articleIds.Contains(prodId))
                {
                    articleIds.Add(prodId);
                    if (articleIds.Count >= number)
                        break;
                }

            }

            return articleIds;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Gets a "recently viewed articles" list
        /// </summary>
        /// <param name="number">Number of articles to load</param>
        /// <returns>"recently viewed articles" list</returns>
        public virtual IList<Article> GetRecentlyViewedArticles(int number)
        {
            var articles = new List<Article>();
            var articleIds = GetRecentlyViewedArticlesIds(number);
            foreach (var article in _articleService.GetArticlesByIds(articleIds.ToArray()))
                if (article.Published && !article.Deleted)
                    articles.Add(article);
            return articles;
        }

        /// <summary>
        /// Adds a article to a recently viewed articles list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        public virtual void AddArticleToRecentlyViewedList(int articleId)
        {
            if (!_catalogSettings.RecentlyViewedArticlesEnabled)
                return;

            var oldArticleIds = GetRecentlyViewedArticlesIds();
            var newArticleIds = new List<int>();
            newArticleIds.Add(articleId);
            foreach (int oldArticleId in oldArticleIds)
                if (oldArticleId != articleId)
                    newArticleIds.Add(oldArticleId);

            var recentlyViewedCookie = _httpContext.Request.Cookies.Get("YourStory.RecentlyViewedArticles");
            if (recentlyViewedCookie == null)
            {
                recentlyViewedCookie = new HttpCookie("YourStory.RecentlyViewedArticles");
                recentlyViewedCookie.HttpOnly = true;
            }
            recentlyViewedCookie.Values.Clear();
            int maxArticles = _catalogSettings.RecentlyViewedArticlesNumber;
            if (maxArticles <= 0)
                maxArticles = 10;
            int i = 1;
            foreach (int newArticleId in newArticleIds)
            {
                recentlyViewedCookie.Values.Add("RecentlyViewedArticleIds", newArticleId.ToString());
                if (i == maxArticles)
                    break;
                i++;
            }
            recentlyViewedCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(recentlyViewedCookie);
        }
        
        #endregion
    }
}
