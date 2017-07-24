using FluentValidation;
using YStory.Admin.Models.Settings;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Settings
{
    public partial class ReturnRequestActionValidator : BaseYStoryValidator<ReturnRequestActionModel>
    {
        public ReturnRequestActionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestActions.Name.Required"));
        }
    }
}