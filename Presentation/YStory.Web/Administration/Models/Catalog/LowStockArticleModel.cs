using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class LowStockArticleModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Name")]
        public string Name { get; set; }

        public string Attributes { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ManageInventoryMethod")]
        public string ManageInventoryMethod { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Published")]
        public bool Published { get; set; }
    }
}