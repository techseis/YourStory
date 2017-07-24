using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class AdminHeaderLinksModel : BaseYStoryModel
    {
        public string ImpersonatedCustomerName { get; set; }
        public bool IsCustomerImpersonated { get; set; }
        public bool DisplayAdminLink { get; set; }
        public string EditPageUrl { get; set; }
    }
}