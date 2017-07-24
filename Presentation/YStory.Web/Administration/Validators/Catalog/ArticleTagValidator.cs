using FluentValidation;
using YStory.Admin.Models.Catalog;
using YStory.Core.Domain.Catalog;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Catalog
{
    public partial class ArticleTagValidator : BaseYStoryValidator<ArticleTagModel>
    {
        public ArticleTagValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ArticleTags.Fields.Name.Required"));

            SetDatabaseValidationRules<ArticleTag>(dbContext);
        }
    }
}