using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Templates;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Templates
{
    [Validator(typeof(TopicTemplateValidator))]
    public partial class TopicTemplateModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.System.Templates.Topic.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Topic.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Topic.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
    }
}