using FluentValidation;
using YStory.Admin.Models.Templates;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Templates
{
    public partial class PublisherTemplateValidator : BaseYStoryValidator<PublisherTemplateModel>
    {
        public PublisherTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.Templates.Publisher.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.Templates.Publisher.ViewPath.Required"));
        }
    }
}