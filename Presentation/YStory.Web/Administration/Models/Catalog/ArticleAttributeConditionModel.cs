using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class ArticleAttributeConditionModel : BaseYStoryModel
    {
        public ArticleAttributeConditionModel()
        {
            ArticleAttributes = new List<ArticleAttributeModel>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Condition.EnableCondition")]
        public bool EnableCondition { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Condition.Attributes")]
        public int SelectedArticleAttributeId { get; set; }
        public IList<ArticleAttributeModel> ArticleAttributes { get; set; }

        public int ArticleAttributeMappingId { get; set; }

        #region Nested classes

        public partial class ArticleAttributeModel : BaseYStoryEntityModel
        {
            public ArticleAttributeModel()
            {
                Values = new List<ArticleAttributeValueModel>();
            }

            public int ArticleAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ArticleAttributeValueModel> Values { get; set; }
        }

        public partial class ArticleAttributeValueModel : BaseYStoryEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }
        #endregion
    }
}