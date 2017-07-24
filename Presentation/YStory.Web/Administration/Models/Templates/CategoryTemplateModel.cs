using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Templates;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Templates
{
    [Validator(typeof(CategoryTemplateValidator))]
    public partial class CategoryTemplateModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.System.Templates.Category.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Category.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Category.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
    }
}