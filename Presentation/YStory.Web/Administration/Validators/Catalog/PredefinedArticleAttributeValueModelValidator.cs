using FluentValidation;
using YStory.Admin.Models.Catalog;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Catalog
{
    public partial class PredefinedArticleAttributeValueModelValidator : BaseYStoryValidator<PredefinedArticleAttributeValueModel>
    {
        public PredefinedArticleAttributeValueModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.Name.Required"));
        }
    }
}