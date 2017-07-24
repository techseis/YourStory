using FluentValidation;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Contributors;

namespace YStory.Web.Validators.Contributors
{
    public partial class ApplyContributorValidator : BaseYStoryValidator<ApplyContributorModel>
    {
        public ApplyContributorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Contributors.ApplyAccount.Name.Required"));

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Contributors.ApplyAccount.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}