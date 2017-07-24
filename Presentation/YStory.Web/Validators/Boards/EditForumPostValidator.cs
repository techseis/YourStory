using FluentValidation;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Boards;

namespace YStory.Web.Validators.Boards
{
    public partial class EditForumPostValidator : BaseYStoryValidator<EditForumPostModel>
    {
        public EditForumPostValidator(ILocalizationService localizationService)
        {            
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}