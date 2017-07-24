using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Home
{
    public partial class CommonStatisticsModel : BaseYStoryModel
    {
        public int NumberOfSubscriptions { get; set; }

        public int NumberOfCustomers { get; set; }

        public int NumberOfPendingReturnRequests { get; set; }

        public int NumberOfLowStockArticles { get; set; }
    }
}