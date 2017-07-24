using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Common;

namespace YStory.Web.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public partial class ContactUsModel : BaseYStoryModel
    {
        [AllowHtml]
        [YStoryResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactUs.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}