using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class NeverSoldReportModel : BaseYStoryModel
    {
        public NeverSoldReportModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailablePublishers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableContributors = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.SearchCategory")]
        public int SearchCategoryId { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.SearchPublisher")]
        public int SearchPublisherId { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        
        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.SearchContributor")]
        public int SearchContributorId { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }

        public bool IsLoggedInAsContributor { get; set; }
    }
}