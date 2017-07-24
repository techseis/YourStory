using System.Web.Mvc;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class ArticleSpecificationAttributeModel : BaseYStoryEntityModel
    {
        public int AttributeTypeId { get; set; }

        [AllowHtml]
        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }

        [AllowHtml]
        public string AttributeName { get; set; }

        [AllowHtml]
        public string ValueRaw { get; set; }

        public bool AllowFiltering { get; set; }

        public bool ShowOnArticlePage { get; set; }

        public int DisplaySubscription { get; set; }

        public int SpecificationAttributeOptionId { get; set; } 
    }
}