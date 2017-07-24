using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class SortOptionModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.IsActive")]
        public bool IsActive { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
    }
}