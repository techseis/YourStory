using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class CopyArticleModel : BaseYStoryEntityModel
    {

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Copy.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Copy.Published")]
        public bool Published { get; set; }

    }
}