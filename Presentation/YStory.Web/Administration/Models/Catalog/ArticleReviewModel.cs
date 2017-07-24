using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    [Validator(typeof(ArticleReviewValidator))]
    public partial class ArticleReviewModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Store")]
        public string StoreName { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Article")]
        public int ArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Article")]
        public string ArticleName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Customer")]
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Customer")]
        public string CustomerInfo { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Title")]
        public string Title { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.ReplyText")]
        public string ReplyText { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.Rating")]
        public int Rating { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //contributor
        public bool IsLoggedInAsContributor { get; set; }
    }
}