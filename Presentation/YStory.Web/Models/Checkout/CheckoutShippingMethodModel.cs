﻿using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Checkout
{
    public partial class CheckoutShippingMethodModel : BaseYStoryModel
    {
        public CheckoutShippingMethodModel()
        {
            ShippingMethods = new List<ShippingMethodModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingMethodModel> ShippingMethods { get; set; }

        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested classes

        public partial class ShippingMethodModel : BaseYStoryModel
        {
            public string ShippingRateComputationMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }

        }
        #endregion
    }
}