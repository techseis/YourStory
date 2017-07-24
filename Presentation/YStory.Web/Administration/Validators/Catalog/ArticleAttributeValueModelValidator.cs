using FluentValidation;
using YStory.Admin.Models.Catalog;
using YStory.Core.Domain.Catalog;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Catalog
{
    public partial class ArticleAttributeValueModelValidator : BaseYStoryValidator<ArticleModel.ArticleAttributeValueModel>
    {
        public ArticleAttributeValueModelValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Name.Required"));

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1"))
                .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToArticle && !x.CustomerEntersQty);

            RuleFor(x => x.AssociatedArticleId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AssociatedArticle.Choose"))
                .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToArticle);

            SetDatabaseValidationRules<ArticleAttributeValue>(dbContext);
        }
    }
}