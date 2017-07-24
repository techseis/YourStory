using YStory.Admin.Models.Common;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class SubscriptionAddressModel : BaseYStoryModel
    {
        public int SubscriptionId { get; set; }

        public AddressModel Address { get; set; }
    }
}