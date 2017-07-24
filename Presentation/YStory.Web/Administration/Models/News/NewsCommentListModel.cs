using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.News
{
    public partial class NewsCommentListModel : BaseYStoryModel
    {
        public NewsCommentListModel()
        {
            AvailableApprovedOptions = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchText")]
        [AllowHtml]
        public string SearchText { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchApproved")]
        public int SearchApprovedId { get; set; }

        public IList<SelectListItem> AvailableApprovedOptions { get; set; }
    }
}