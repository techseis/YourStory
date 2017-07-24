using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Customer
{
    public partial class CustomerAddressEditModel : BaseYStoryModel
    {
        public CustomerAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
    }
}