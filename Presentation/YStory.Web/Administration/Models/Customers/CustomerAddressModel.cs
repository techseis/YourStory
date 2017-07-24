using YStory.Admin.Models.Common;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Customers
{
    public partial class CustomerAddressModel : BaseYStoryModel
    {
        public int CustomerId { get; set; }

        public AddressModel Address { get; set; }
    }
}