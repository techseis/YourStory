using FluentValidation;
using YStory.Admin.Models.News;
using YStory.Core.Domain.News;
using YStory.Data;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;

namespace YStory.Admin.Validators.News
{
    public partial class NewsItemValidator : BaseYStoryValidator<NewsItemModel>
    {
        public NewsItemValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

            RuleFor(x => x.Short).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

            RuleFor(x => x.Full).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));

            SetDatabaseValidationRules<NewsItem>(dbContext);
        }
    }
}