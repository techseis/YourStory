using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Customer
{
    public partial class CustomerAddressListModel : BaseYStoryModel
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}