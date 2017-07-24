using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class ModeModel : BaseYStoryModel
    {
        public string ModeName { get; set; }
        public bool Enabled { get; set; }
    }
}