using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Contributors;

namespace YStory.Web.Models.Contributors
{
    [Validator(typeof(ApplyContributorValidator))]
    public partial class ApplyContributorModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Contributors.ApplyAccount.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Contributors.ApplyAccount.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [YStoryResourceDisplayName("Contributors.ApplyAccount.Description")]
        [AllowHtml]
        public string Description { get; set; }
        
        public bool DisplayCaptcha { get; set; }

        public bool DisableFormInput { get; set; }
        public string Result { get; set; }
    }
}