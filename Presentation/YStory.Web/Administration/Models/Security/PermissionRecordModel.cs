using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseYStoryModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}