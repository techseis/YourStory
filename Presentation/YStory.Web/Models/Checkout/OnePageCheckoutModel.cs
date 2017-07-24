using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Checkout
{
    public partial class OnePageCheckoutModel : BaseYStoryModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
    }
}