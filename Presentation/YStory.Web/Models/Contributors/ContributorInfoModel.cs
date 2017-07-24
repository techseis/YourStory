using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Contributors;

namespace YStory.Web.Models.Contributors
{
    [Validator(typeof(ContributorInfoValidator))]
    public class ContributorInfoModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Account.ContributorInfo.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Account.ContributorInfo.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [YStoryResourceDisplayName("Account.ContributorInfo.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [YStoryResourceDisplayName("Account.ContributorInfo.Picture")]
        public string PictureUrl { get; set; }
    }
}