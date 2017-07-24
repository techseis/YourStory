using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Validators.Catalog;

namespace YStory.Web.Models.Catalog
{
    [Validator(typeof(ArticleEmailAFriendValidator))]
    public partial class ArticleEmailAFriendModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string ArticleSeName { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Articles.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Articles.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Articles.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}