using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.ShoppingCart
{
    public partial class ShoppingCartModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.CurrentCarts.Customer")]
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.CurrentCarts.Customer")]
        public string CustomerEmail { get; set; }

        [YStoryResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }
    }
}