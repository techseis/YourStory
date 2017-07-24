using System;
using System.Web.Mvc;
using YStory.Admin.Models.Common;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Affiliates
{
    public partial class AffiliateModel : BaseYStoryEntityModel
    {
        public AffiliateModel()
        {
            Address = new AddressModel();
        }

        [YStoryResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }
        
        [YStoryResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [YStoryResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        [AllowHtml]
        public string FriendlyUrlName { get; set; }
        
        [YStoryResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        public AddressModel Address { get; set; }

        #region Nested classes
        
        public partial class AffiliatedSubscriptionModel : BaseYStoryEntityModel
        {
            public override int Id { get; set; }
            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.CustomSubscriptionNumber")]
            public string CustomSubscriptionNumber { get; set; }

            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.SubscriptionStatus")]
            public string SubscriptionStatus { get; set; }
            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.SubscriptionStatus")]
            public int SubscriptionStatusId { get; set; }

            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.SubscriptionTotal")]
            public string SubscriptionTotal { get; set; }

            [YStoryResourceDisplayName("Admin.Affiliates.Subscriptions.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class AffiliatedCustomerModel : BaseYStoryEntityModel
        {
            [YStoryResourceDisplayName("Admin.Affiliates.Customers.Name")]
            public string Name { get; set; }
        }

        #endregion
    }
}