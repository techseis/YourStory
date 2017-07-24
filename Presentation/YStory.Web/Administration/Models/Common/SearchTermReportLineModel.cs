using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Common
{
    public partial class SearchTermReportLineModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [YStoryResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }
    }
}
