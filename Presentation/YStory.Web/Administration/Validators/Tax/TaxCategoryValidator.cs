using FluentValidation;
using YStory.Admin.Models.Tax;
using YStory.Core.Domain.Tax;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Tax
{
    public partial class TaxCategoryValidator : BaseYStoryValidator<TaxCategoryModel>
    {
        public TaxCategoryValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

            SetDatabaseValidationRules<TaxCategory>(dbContext);
        }
    }
}