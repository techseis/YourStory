using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Customers
{
    public partial class CustomerReportsModel : BaseYStoryModel
    {
        public BestCustomersReportModel BestCustomersBySubscriptionTotal { get; set; }
        public BestCustomersReportModel BestCustomersByNumberOfSubscriptions { get; set; }
    }
}