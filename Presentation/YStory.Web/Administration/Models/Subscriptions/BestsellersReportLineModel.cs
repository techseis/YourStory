using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class BestsellersReportLineModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.Name")]
        public string ArticleName { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalAmount")]
        public string TotalAmount { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }
    }
}