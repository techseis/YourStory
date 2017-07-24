using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Common
{
    public partial class SystemWarningModel : BaseYStoryModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}