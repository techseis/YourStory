using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Customers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Subscriptions;
using YStory.Web.Factories;
using YStory.Web.Framework.Security;
using YStory.Web.Models.Subscription;

namespace YStory.Web.Controllers
{
    public partial class ReturnRequestController : BasePublicController
    {
        #region Fields

        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IDownloadService _downloadService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly SubscriptionSettings _subscriptionSettings;

        #endregion

        #region Constructors

        public ReturnRequestController(IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            ISubscriptionService subscriptionService, 
            IWorkContext workContext, 
            IStoreContext storeContext,
            ISubscriptionProcessingService subscriptionProcessingService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            ICustomNumberFormatter customNumberFormatter,
            IDownloadService downloadService,
            LocalizationSettings localizationSettings,
            SubscriptionSettings subscriptionSettings)
        {
            this._returnRequestModelFactory = returnRequestModelFactory;
            this._returnRequestService = returnRequestService;
            this._subscriptionService = subscriptionService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._customNumberFormatter = customNumberFormatter;
            this._downloadService = downloadService;
            this._localizationSettings = localizationSettings;
            this._subscriptionSettings = subscriptionSettings;
        }

        #endregion

        #region Methods

        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult CustomerReturnRequests()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _returnRequestModelFactory.PrepareCustomerReturnRequestsModel();
            return View(model);
        }

        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ReturnRequest(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_subscriptionProcessingService.IsReturnRequestAllowed(subscription))
                return RedirectToRoute("HomePage");

            var model = new SubmitReturnRequestModel();
            model = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(model, subscription);
            return View(model);
        }

        [HttpPost, ActionName("ReturnRequest")]
        [ValidateInput(false)]
        [PublicAntiForgery]
        public virtual ActionResult ReturnRequestSubmit(int subscriptionId, SubmitReturnRequestModel model, FormCollection form)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_subscriptionProcessingService.IsReturnRequestAllowed(subscription))
                return RedirectToRoute("HomePage");

            int count = 0;

            var downloadId = 0;
            if (_subscriptionSettings.ReturnRequestsAllowFiles)
            {
                var download = _downloadService.GetDownloadByGuid(model.UploadedFileGuid);
                if (download != null)
                    downloadId = download.Id;
            }

            //returnable articles
            var subscriptionItems = subscription.SubscriptionItems.Where(oi => !oi.Article.NotReturnable);
            foreach (var subscriptionItem in subscriptionItems)
            {
                int quantity = 0; //parse quantity
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("quantity{0}", subscriptionItem.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out quantity);
                        break;
                    }
                if (quantity > 0)
                {
                    var rrr = _returnRequestService.GetReturnRequestReasonById(model.ReturnRequestReasonId);
                    var rra = _returnRequestService.GetReturnRequestActionById(model.ReturnRequestActionId);
                    
                    var rr = new ReturnRequest
                    {
                        CustomNumber = "",
                        StoreId = _storeContext.CurrentStore.Id,
                        SubscriptionItemId = subscriptionItem.Id,
                        Quantity = quantity,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = rrr != null ? rrr.GetLocalized(x => x.Name) : "not available",
                        RequestedAction = rra != null ? rra.GetLocalized(x => x.Name) : "not available",
                        CustomerComments = model.Comments,
                        UploadedFileId = downloadId,
                        StaffNotes = string.Empty,
                        ReturnRequestStatus = ReturnRequestStatus.Pending,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    _workContext.CurrentCustomer.ReturnRequests.Add(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //set return request custom number
                    rr.CustomNumber = _customNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //notify store owner
                    _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, subscriptionItem, _localizationSettings.DefaultAdminLanguageId);
                    //notify customer
                    _workflowMessageService.SendNewReturnRequestCustomerNotification(rr, subscriptionItem, subscription.CustomerLanguageId);

                    count++;
                }
            }

            model = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(model, subscription);
            if (count > 0)
                model.Result = _localizationService.GetResource("ReturnRequests.Submitted");
            else
                model.Result = _localizationService.GetResource("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult UploadFileReturnRequest()
        {
            if (!_subscriptionSettings.ReturnRequestsEnabled && !_subscriptionSettings.ReturnRequestsAllowFiles)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, MimeTypes.TextPlain);
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            int validationFileMaximumSize = _subscriptionSettings.ReturnRequestsFileMaximumSize;
            if (validationFileMaximumSize > 0)
            {
                //compare in bytes
                var maxFileSizeBytes = validationFileMaximumSize * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), validationFileMaximumSize),
                        downloadGuid = Guid.Empty,
                    }, MimeTypes.TextPlain);
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new {downloadId = download.DownloadGuid}),
                downloadGuid = download.DownloadGuid,
            }, MimeTypes.TextPlain);
        }

        #endregion
    }
}
