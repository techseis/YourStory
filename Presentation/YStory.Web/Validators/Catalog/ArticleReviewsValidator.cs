using FluentValidation;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Validators.Catalog
{
    public partial class ArticleReviewsValidator : BaseYStoryValidator<ArticleReviewsModel>
    {
        public ArticleReviewsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddArticleReview.Title).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Title.Required")).When(x => x.AddArticleReview != null);
            RuleFor(x => x.AddArticleReview.Title).Length(1, 200).WithMessage(string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200)).When(x => x.AddArticleReview != null && !string.IsNullOrEmpty(x.AddArticleReview.Title));
            RuleFor(x => x.AddArticleReview.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.ReviewText.Required")).When(x => x.AddArticleReview != null);
        }
    }
}