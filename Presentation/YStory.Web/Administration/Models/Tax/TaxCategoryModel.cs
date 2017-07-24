using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Tax;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Tax
{
    [Validator(typeof(TaxCategoryValidator))]
    public partial class TaxCategoryModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Tax.Categories.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
    }
}