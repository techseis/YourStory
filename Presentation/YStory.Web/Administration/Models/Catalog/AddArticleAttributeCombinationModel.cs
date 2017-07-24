using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YStory.Core.Domain.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class AddArticleAttributeCombinationModel : BaseYStoryModel
    {
        public AddArticleAttributeCombinationModel()
        {
            ArticleAttributes = new List<ArticleAttributeModel>();
            Warnings = new List<string>();
        }
        
        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.AllowOutOfStockSubscriptions")]
        public bool AllowOutOfStockSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.Sku")]
        public string Sku { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.PublisherPartNumber")]
        public string PublisherPartNumber { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.Gtin")]
        public string Gtin { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.OverriddenPrice")]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        public IList<ArticleAttributeModel> ArticleAttributes { get; set; }

        public IList<string> Warnings { get; set; }

        public int ArticleId { get; set; }

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