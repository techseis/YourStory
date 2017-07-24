using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Affiliates
{
    public partial class AffiliateListModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
        [AllowHtml]
        public string SearchFriendlyUrlName { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.List.LoadOnlyWithSubscriptions")]
        public bool LoadOnlyWithSubscriptions { get; set; }
        [YStoryResourceDisplayName("Admin.Affiliates.List.SubscriptionsCreatedFromUtc")]
        [UIHint("DateNullable")]
        public DateTime? SubscriptionsCreatedFromUtc { get; set; }
        [YStoryResourceDisplayName("Admin.Affiliates.List.SubscriptionsCreatedToUtc")]
        [UIHint("DateNullable")]
        public DateTime? SubscriptionsCreatedToUtc { get; set; }
    }
}