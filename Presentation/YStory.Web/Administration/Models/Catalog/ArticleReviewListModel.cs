using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class ArticleReviewListModel : BaseYStoryModel
    {
        public ArticleReviewListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableApprovedOptions = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.SearchText")]
        [AllowHtml]
        public string SearchText { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.SearchArticle")]
        public int SearchArticleId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleReviews.List.SearchApproved")]
        public int SearchApprovedId { get; set; }

        //contributor
        public bool IsLoggedInAsContributor { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableApprovedOptions { get; set; }
    }
}