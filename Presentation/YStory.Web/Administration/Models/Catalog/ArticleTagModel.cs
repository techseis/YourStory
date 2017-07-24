using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    [Validator(typeof(ArticleTagValidator))]
    public partial class ArticleTagModel : BaseYStoryEntityModel, ILocalizedModel<ArticleTagLocalizedModel>
    {
        public ArticleTagModel()
        {
            Locales = new List<ArticleTagLocalizedModel>();
        }
        [YStoryResourceDisplayName("Admin.Catalog.ArticleTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleTags.Fields.ArticleCount")]
        public int ArticleCount { get; set; }

        public IList<ArticleTagLocalizedModel> Locales { get; set; }
    }

    public partial class ArticleTagLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.ArticleTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}