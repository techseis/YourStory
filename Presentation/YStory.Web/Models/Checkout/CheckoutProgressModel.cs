using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Checkout
{
    public partial class CheckoutProgressModel : BaseYStoryModel
    {
        public CheckoutProgressStep CheckoutProgressStep { get; set; }
    }

    public enum CheckoutProgressStep
    {
        Cart,
        Address,
        Shipping,
        Payment,
        Confirm,
        Complete
    }
}