using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Templates;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Templates
{
    [Validator(typeof(PublisherTemplateValidator))]
    public partial class PublisherTemplateModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.System.Templates.Publisher.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Publisher.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Publisher.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
    }
}