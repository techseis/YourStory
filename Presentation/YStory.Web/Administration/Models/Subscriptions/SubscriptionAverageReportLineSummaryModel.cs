using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class SubscriptionAverageReportLineSummaryModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.SalesReport.Average.SubscriptionStatus")]
        public string SubscriptionStatus { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Average.SumTodaySubscriptions")]
        public string SumTodaySubscriptions { get; set; }
        
        [YStoryResourceDisplayName("Admin.SalesReport.Average.SumThisWeekSubscriptions")]
        public string SumThisWeekSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Average.SumThisMonthSubscriptions")]
        public string SumThisMonthSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Average.SumThisYearSubscriptions")]
        public string SumThisYearSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Average.SumAllTimeSubscriptions")]
        public string SumAllTimeSubscriptions { get; set; }
    }
}
