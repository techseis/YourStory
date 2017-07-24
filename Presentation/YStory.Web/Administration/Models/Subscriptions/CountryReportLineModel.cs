using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class CountryReportLineModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.SalesReport.Country.Fields.CountryName")]
        public string CountryName { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Country.Fields.TotalSubscriptions")]
        public int TotalSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Country.Fields.SumSubscriptions")]
        public string SumSubscriptions { get; set; }
    }
}