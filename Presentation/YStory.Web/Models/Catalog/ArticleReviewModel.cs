using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Catalog;

namespace YStory.Web.Models.Catalog
{
    public partial class ArticleReviewOverviewModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }

        public int RatingSum { get; set; }

        public int TotalReviews { get; set; }

        public bool AllowCustomerReviews { get; set; }
    }

    [Validator(typeof(ArticleReviewsValidator))]
    public partial class ArticleReviewsModel : BaseYStoryModel
    {
        public ArticleReviewsModel()
        {
            Items = new List<ArticleReviewModel>();
            AddArticleReview = new AddArticleReviewModel();
        }
        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string ArticleSeName { get; set; }

        public IList<ArticleReviewModel> Items { get; set; }
        public AddArticleReviewModel AddArticleReview { get; set; }
    }

    public partial class ArticleReviewModel : BaseYStoryEntityModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public bool AllowViewingProfiles { get; set; }
        
        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public int Rating { get; set; }

        public ArticleReviewHelpfulnessModel Helpfulness { get; set; }

        public string WrittenOnStr { get; set; }
    }


    public partial class ArticleReviewHelpfulnessModel : BaseYStoryModel
    {
        public int ArticleReviewId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
    }

    public partial class AddArticleReviewModel : BaseYStoryModel
    {
        [AllowHtml]
        [YStoryResourceDisplayName("Reviews.Fields.Title")]
        public string Title { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Reviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [YStoryResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentCustomerLeaveReview { get; set; }
        public bool SuccessfullyAdded { get; set; }
        public string Result { get; set; }
    }
}