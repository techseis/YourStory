using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Subscription
{
    public partial class SubmitReturnRequestModel : BaseYStoryModel
    {
        public SubmitReturnRequestModel()
        {
            Items = new List<SubscriptionItemModel>();
            AvailableReturnReasons = new List<ReturnRequestReasonModel>();
            AvailableReturnActions= new List<ReturnRequestActionModel>();
        }

        public int SubscriptionId { get; set; }
        public string CustomSubscriptionNumber { get; set; }

        public IList<SubscriptionItemModel> Items { get; set; }
        
        [AllowHtml]
        [YStoryResourceDisplayName("ReturnRequests.ReturnReason")]
        public int ReturnRequestReasonId { get; set; }
        public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ReturnRequests.ReturnAction")]
        public int ReturnRequestActionId { get; set; }
        public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("ReturnRequests.Comments")]
        public string Comments { get; set; }

        public bool AllowFiles { get; set; }
        [YStoryResourceDisplayName("ReturnRequests.UploadedFile")]
        public Guid UploadedFileGuid { get; set; }

        public string Result { get; set; }
        
        #region Nested classes

        public partial class SubscriptionItemModel : BaseYStoryEntityModel
        {
            public int ArticleId { get; set; }

            public string ArticleName { get; set; }

            public string ArticleSeName { get; set; }

            public string AttributeInfo { get; set; }

            public string UnitPrice { get; set; }

            public int Quantity { get; set; }
        }

        public partial class ReturnRequestReasonModel : BaseYStoryEntityModel
        {
            public string Name { get; set; }
        }
        public partial class ReturnRequestActionModel : BaseYStoryEntityModel
        {
            public string Name { get; set; }
        }

        #endregion
    }

}