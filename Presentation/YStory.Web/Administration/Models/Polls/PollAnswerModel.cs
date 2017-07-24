using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Polls;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Polls
{
    [Validator(typeof(PollAnswerValidator))]
    public partial class PollAnswerModel : BaseYStoryEntityModel
    {
        public int PollId { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes")]
        public int NumberOfVotes { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

    }
}