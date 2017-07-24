using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Customers
{
    public partial class BestCustomerReportLineModel : BaseYStoryModel
    {
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.Customer")]
        public string CustomerName { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.SubscriptionTotal")]
        public string SubscriptionTotal { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.SubscriptionCount")]
        public decimal SubscriptionCount { get; set; }
    }
}