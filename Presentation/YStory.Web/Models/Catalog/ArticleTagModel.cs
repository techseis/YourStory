using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class ArticleTagModel : BaseYStoryEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int ArticleCount { get; set; }
    }
}