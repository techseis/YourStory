using FluentValidation;
using YStory.Admin.Models.Catalog;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.Catalog
{
    public partial class ArticleReviewValidator : BaseYStoryValidator<ArticleReviewModel>
    {
        public ArticleReviewValidator(ILocalizationService localizationService, IDbContext dbContext, IWorkContext workContext)
        {
            var isLoggedInAsContributor = workContext.CurrentContributor != null;
            //contributor can edit "Reply text" only
            if (!isLoggedInAsContributor)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ArticleReviews.Fields.Title.Required"));
                RuleFor(x => x.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ArticleReviews.Fields.ReviewText.Required"));
            }

            SetDatabaseValidationRules<ArticleReview>(dbContext);
        }
    }
}