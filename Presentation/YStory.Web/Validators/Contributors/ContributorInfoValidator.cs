using FluentValidation;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Contributors;

namespace YStory.Web.Validators.Contributors
{
    public partial class ContributorInfoValidator : BaseYStoryValidator<ContributorInfoModel>
    {
        public ContributorInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Account.ContributorInfo.Name.Required"));

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.ContributorInfo.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}