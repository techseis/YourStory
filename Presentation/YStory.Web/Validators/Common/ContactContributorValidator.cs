using FluentValidation;
using YStory.Core.Domain.Common;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Common;

namespace YStory.Web.Validators.Common
{
    public partial class ContactContributorValidator : BaseYStoryValidator<ContactContributorModel>
    {
        public ContactContributorValidator(ILocalizationService localizationService, CommonSettings commonSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("ContactContributor.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizationService.GetResource("ContactContributor.FullName.Required"));
            if (commonSettings.SubjectFieldOnContactUsForm)
            {
                RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("ContactContributor.Subject.Required"));
            }
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResource("ContactContributor.Enquiry.Required"));
        }}
}