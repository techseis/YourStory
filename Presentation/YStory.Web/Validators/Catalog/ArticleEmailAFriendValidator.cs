using FluentValidation;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Validators.Catalog
{
    public partial class ArticleEmailAFriendValidator : BaseYStoryValidator<ArticleEmailAFriendModel>
    {
        public ArticleEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessage(localizationService.GetResource("Articles.EmailAFriend.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage(localizationService.GetResource("Articles.EmailAFriend.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }}
}