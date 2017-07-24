using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    [Validator(typeof(ArticleAttributeValidator))]
    public partial class ArticleAttributeModel : BaseYStoryEntityModel, ILocalizedModel<ArticleAttributeLocalizedModel>
    {
        public ArticleAttributeModel()
        {
            Locales = new List<ArticleAttributeLocalizedModel>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.Fields.Description")]
        [AllowHtml]
        public string Description {get;set;}
        


        public IList<ArticleAttributeLocalizedModel> Locales { get; set; }

        #region Nested classes

        public partial class UsedByArticleModel : BaseYStoryEntityModel
        {
            [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.UsedByArticles.Article")]
            public string ArticleName { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.UsedByArticles.Published")]
            public bool Published { get; set; }
        }

        #endregion
    }

    public partial class ArticleAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.Fields.Description")]
        [AllowHtml]
        public string Description {get;set;}
    }


    [Validator(typeof(PredefinedArticleAttributeValueModelValidator))]
    public partial class PredefinedArticleAttributeValueModel : BaseYStoryEntityModel, ILocalizedModel<PredefinedArticleAttributeValueLocalizedModel>
    {
        public PredefinedArticleAttributeValueModel()
        {
            Locales = new List<PredefinedArticleAttributeValueLocalizedModel>();
        }

        public int ArticleAttributeId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.PriceAdjustment")]
        public decimal PriceAdjustment { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.PriceAdjustment")]
        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.WeightAdjustment")]
        public decimal WeightAdjustment { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.WeightAdjustment")]
        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.Cost")]
        public decimal Cost { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        public IList<PredefinedArticleAttributeValueLocalizedModel> Locales { get; set; }
    }
    public partial class PredefinedArticleAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Attributes.ArticleAttributes.PredefinedValues.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}