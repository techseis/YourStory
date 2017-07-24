using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class LogoModel : BaseYStoryModel
    {
        public string StoreName { get; set; }

        public string LogoPath { get; set; }
    }
}