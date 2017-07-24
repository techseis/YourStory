using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Common;

namespace YStory.Web.Models.Common
{
    [Validator(typeof(ContactContributorValidator))]
    public partial class ContactContributorModel : BaseYStoryModel
    {
        public int ContributorId { get; set; }
        public string ContributorName { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactContributor.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactContributor.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactContributor.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ContactContributor.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}