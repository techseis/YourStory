using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Checkout
{
    public partial class CheckoutCompletedModel : BaseYStoryModel
    {
        public int SubscriptionId { get; set; }
        public string CustomSubscriptionNumber { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
    }
}