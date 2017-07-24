using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Directory;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Directory
{
    [Validator(typeof(MeasureDimensionValidator))]
    public partial class MeasureDimensionModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword")]
        [AllowHtml]
        public string SystemKeyword { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio")]
        public decimal Ratio { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Shipping.Measures.Dimensions.Fields.IsPrimaryDimension")]
        public bool IsPrimaryDimension { get; set; }
    }
}