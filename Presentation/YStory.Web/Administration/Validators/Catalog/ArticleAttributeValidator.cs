using FluentValidation;
using YStory.Admin.Models.Catalog;
using YStory.Core.Domain.Catalog;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Catalog
{
    public partial class ArticleAttributeValidator : BaseYStoryValidator<ArticleAttributeModel>
    {
        public ArticleAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributes.Fields.Name.Required"));
            SetDatabaseValidationRules<ArticleAttribute>(dbContext);
        }
    }
}