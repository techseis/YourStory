using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Catalog
{
    public class CustomerArticleReviewModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleSeName { get; set; }
        public string Title { get; set; }
        public string ReviewText { get; set; }
        public string ReplyText { get; set; }
        public int Rating { get; set; }
        public string WrittenOnStr { get; set; }
        public string ApprovalStatus { get; set; }
    }

    public class CustomerArticleReviewsModel : BaseYStoryModel
    {
        public CustomerArticleReviewsModel()
        {
            ArticleReviews = new List<CustomerArticleReviewModel>();
        }

        public IList<CustomerArticleReviewModel> ArticleReviews { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested class

        /// <summary>
        /// Class that has only page for route value. Used for (My Account) My Article Reviews pagination
        /// </summary>
        public partial class CustomerArticleReviewsRouteValues : IRouteValues
        {
            public int page { get; set; }
        }

        #endregion
    }
}