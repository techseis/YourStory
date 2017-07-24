using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Contributors
{
    public partial class ContributorListModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.Contributors.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }
    }
}