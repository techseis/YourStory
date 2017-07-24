using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Home
{
    public partial class DashboardModel : BaseYStoryModel
    {
        public bool IsLoggedInAsContributor { get; set; }
    }
}