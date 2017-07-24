using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Subscriptions;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    [Validator(typeof(ReturnRequestValidator))]
    public partial class ReturnRequestModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.CustomNumber")]
        public string CustomNumber { get; set; }
        
        public int SubscriptionId { get; set; }
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.CustomSubscriptionNumber")]
        public string CustomSubscriptionNumber { get; set; }

        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public int CustomerId { get; set; }
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public string CustomerInfo { get; set; }

        public int ArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Article")]
        public string ArticleName { get; set; }
        public string AttributeInfo { get; set; }

        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Quantity")]
        public int Quantity { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.ReasonForReturn")]
        public string ReasonForReturn { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.RequestedAction")]
        public string RequestedAction { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.CustomerComments")]
        public string CustomerComments { get; set; }

        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.UploadedFile")]
        public Guid UploadedFileGuid { get; set; }

        [AllowHtml]
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.StaffNotes")]
        public string StaffNotes { get; set; }

        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Status")]
        public int ReturnRequestStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.Status")]
        public string ReturnRequestStatusStr { get; set; }

        [YStoryResourceDisplayName("Admin.ReturnRequests.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}