using System;
using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Subscription
{
    public partial class CustomerReturnRequestsModel : BaseYStoryModel
    {
        public CustomerReturnRequestsModel()
        {
            Items = new List<ReturnRequestModel>();
        }

        public IList<ReturnRequestModel> Items { get; set; }

        #region Nested classes
        public partial class ReturnRequestModel : BaseYStoryEntityModel
        {
            public string CustomNumber { get; set; }
            public string ReturnRequestStatus { get; set; }
            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public int Quantity { get; set; }

            public string ReturnReason { get; set; }
            public string ReturnAction { get; set; }
            public string Comments { get; set; }
            public Guid UploadedFileGuid { get; set; }

            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }
}