using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class NeverSoldReportLineModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.NeverSold.Fields.Name")]
        public string ArticleName { get; set; }
    }
}