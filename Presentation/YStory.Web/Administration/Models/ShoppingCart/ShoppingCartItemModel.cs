using System;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.ShoppingCart
{
    public partial class ShoppingCartItemModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.CurrentCarts.Store")]
        public string Store { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.Article")]
        public int ArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.Article")]
        public string ArticleName { get; set; }
        public string AttributeInfo { get; set; }

        [YStoryResourceDisplayName("Admin.CurrentCarts.UnitPrice")]
        public string UnitPrice { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.Quantity")]
        public int Quantity { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.Total")]
        public string Total { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}