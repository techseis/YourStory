using System;
using System.Collections.Generic;
using System.Web;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Compare articles service
    /// </summary>
    public partial class CompareArticlesService : ICompareArticlesService
    {
        #region Constants

        /// <summary>
        /// Compare articles cookie name
        /// </summary>
        private const string COMPARE_ARTICLES_COOKIE_NAME = "nop.CompareArticles";

        #endregion
        
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
        public CompareArticlesService(HttpContextBase httpContext, IArticleService articleService,
            CatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._articleService = articleService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a "compare articles" identifier list
        /// </summary>
        /// <returns>"compare articles" identifier list</returns>
        protected virtual List<int> GetComparedArticleIds()
        {
            var articleIds = new List<int>();
            HttpCookie compareCookie = _httpContext.Request.Cookies.Get(COMPARE_ARTICLES_COOKIE_NAME);
            if (compareCookie == null)
                return articleIds;
            string[] values = compareCookie.Values.GetValues("CompareArticleIds");
            if (values == null)
                return articleIds;
            foreach (string articleId in values)
            {
                int prodId = int.Parse(articleId);
                if (!articleIds.Contains(prodId))
                    articleIds.Add(prodId);
            }

            return articleIds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare articles" list
        /// </summary>
        public virtual void ClearCompareArticles()
        {
            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_ARTICLES_COOKIE_NAME);
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                compareCookie.Expires = DateTime.Now.AddYears(-1);
                _httpContext.Response.Cookies.Set(compareCookie);
            }
        }

        /// <summary>
        /// Gets a "compare articles" list
        /// </summary>
        /// <returns>"Compare articles" list</returns>
        public virtual IList<Article> GetComparedArticles()
        {
            var articles = new List<Article>();
            var articleIds = GetComparedArticleIds();
            foreach (int articleId in articleIds)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.Published && !article.Deleted)
                    articles.Add(article);
            }
            return articles;
        }

        /// <summary>
        /// Removes a article from a "compare articles" list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        public virtual void RemoveArticleFromCompareList(int articleId)
        {
            var oldArticleIds = GetComparedArticleIds();
            var newArticleIds = new List<int>();
            newArticleIds.AddRange(oldArticleIds);
            newArticleIds.Remove(articleId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_ARTICLES_COOKIE_NAME);
            if (compareCookie == null)
                return;
            compareCookie.Values.Clear();
            foreach (int newArticleId in newArticleIds)
                compareCookie.Values.Add("CompareArticleIds", newArticleId.ToString());
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Adds a article to a "compare articles" list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        public virtual void AddArticleToCompareList(int articleId)
        {
            var oldArticleIds = GetComparedArticleIds();
            var newArticleIds = new List<int>();
            newArticleIds.Add(articleId);
            foreach (int oldArticleId in oldArticleIds)
                if (oldArticleId != articleId)
                    newArticleIds.Add(oldArticleId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_ARTICLES_COOKIE_NAME);
            if (compareCookie == null)
            {
                compareCookie = new HttpCookie(COMPARE_ARTICLES_COOKIE_NAME);
                compareCookie.HttpOnly = true;
            }
            compareCookie.Values.Clear();
            int i = 1;
            foreach (int newArticleId in newArticleIds)
            {
                compareCookie.Values.Add("CompareArticleIds", newArticleId.ToString());
                if (i == _catalogSettings.CompareArticlesNumber)
                    break;
                i++;
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        #endregion
    }
}
