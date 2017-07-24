using System.Collections.Generic;
using YStory.Admin.Models.Directory;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Payments
{
    public partial class PaymentMethodRestrictionModel : BaseYStoryModel
    {
        public PaymentMethodRestrictionModel()
        {
            AvailablePaymentMethods = new List<PaymentMethodModel>();
            AvailableCountries = new List<CountryModel>();
            Resticted = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }
        public IList<CountryModel> AvailableCountries { get; set; }

        //[payment method system name] / [customer role id] / [resticted]
        public IDictionary<string, IDictionary<int, bool>> Resticted { get; set; }
    }
}