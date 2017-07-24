using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Customer
{
    public partial class CustomerAttributeModel : BaseYStoryEntityModel
    {
        public CustomerAttributeModel()
        {
            Values = new List<CustomerAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<CustomerAttributeValueModel> Values { get; set; }

    }

    public partial class CustomerAttributeValueModel : BaseYStoryEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}