using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Templates;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Templates
{
    [Validator(typeof(ArticleTemplateValidator))]
    public partial class ArticleTemplateModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.System.Templates.Article.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Article.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Article.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.System.Templates.Article.IgnoredArticleTypes")]
        [AllowHtml]
        public string IgnoredArticleTypes { get; set; }
    }
}