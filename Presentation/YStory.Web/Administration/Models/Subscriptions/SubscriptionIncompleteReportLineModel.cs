using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class SubscriptionIncompleteReportLineModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.SalesReport.Incomplete.Item")]
        public string Item { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Incomplete.Total")]
        public string Total { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Incomplete.Count")]
        public int Count { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Incomplete.View")]
        public string ViewLink { get; set; }
    }
}
