﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Models.Subscriptions;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services;
using YStory.Services.Customers;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Subscriptions;
using YStory.Services.Security;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;

namespace YStory.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
    {
        #region Fields

        private readonly IReturnRequestService _returnRequestService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;

        #endregion Fields

        #region Constructors

        public ReturnRequestController(IReturnRequestService returnRequestService,
            ISubscriptionService subscriptionService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            ICustomerActivityService customerActivityService, 
            IPermissionService permissionService,
            IDownloadService downloadService)
        {
            this._returnRequestService = returnRequestService;
            this._subscriptionService = subscriptionService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var subscriptionItem = _subscriptionService.GetSubscriptionItemById(returnRequest.SubscriptionItemId);
            if (subscriptionItem != null)
            {
                model.ArticleId = subscriptionItem.ArticleId;
                model.ArticleName = subscriptionItem.Article.Name;
                model.SubscriptionId = subscriptionItem.SubscriptionId;
                model.AttributeInfo = subscriptionItem.AttributeDescription;
                model.CustomSubscriptionNumber = subscriptionItem.Subscription.CustomSubscriptionNumber;
            }
            model.Id = returnRequest.Id;
            model.CustomNumber = returnRequest.CustomNumber;
            model.CustomerId = returnRequest.CustomerId;
            var customer = returnRequest.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.Quantity = returnRequest.Quantity;
            model.ReturnRequestStatusStr = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext);

            var download = _downloadService.GetDownloadById(returnRequest.UploadedFileId);
            model.UploadedFileGuid = download != null ? download.DownloadGuid : Guid.Empty;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                model.ReasonForReturn = returnRequest.ReasonForReturn;
                model.RequestedAction = returnRequest.RequestedAction;
                model.CustomerComments = returnRequest.CustomerComments;
                model.StaffNotes = returnRequest.StaffNotes;
                model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;
            }
        }

        #endregion

        #region Methods

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var model = new ReturnRequestListModel
            {
                ReturnRequestStatusList = ReturnRequestStatus.Cancelled.ToSelectList(false).ToList(),
                ReturnRequestStatusId = -1
            };

            model.ReturnRequestStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = _localizationService.GetResource("Admin.ReturnRequests.SearchReturnRequestStatus.All"),
                Selected = true
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, ReturnRequestListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedKendoGridJson();

            var rrs = model.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?) model.ReturnRequestStatusId;
            
            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var returnRequests = _returnRequestService.SearchReturnRequests(0, 0, 0, model.CustomNumber, rrs, startDateValue, endDateValue, command.Page - 1, command.PageSize);
            var returnRequestModels = new List<ReturnRequestModel>();
            foreach (var rr in returnRequests)
            {
                var m = new ReturnRequestModel();
                PrepareReturnRequestModel(m, rr, false);
                returnRequestModels.Add(m);
            }
            var gridModel = new DataSourceResult
            {
                Data = returnRequestModels,
                Total = returnRequests.TotalCount,
            };

            return Json(gridModel);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");
            
            var model = new ReturnRequestModel();
            PrepareReturnRequestModel(model, returnRequest, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                returnRequest.Quantity = model.Quantity;
                returnRequest.ReasonForReturn = model.ReasonForReturn;
                returnRequest.RequestedAction = model.RequestedAction;
                returnRequest.CustomerComments = model.CustomerComments;
                returnRequest.StaffNotes = model.StaffNotes;
                returnRequest.ReturnRequestStatusId = model.ReturnRequestStatusId;
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(returnRequest.Customer);

                //activity log
                _customerActivityService.InsertActivity("EditReturnRequest", _localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id);

                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id}) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareReturnRequestModel(model, returnRequest, true);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual ActionResult NotifyCustomer(ReturnRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            var subscriptionItem = _subscriptionService.GetSubscriptionItemById(returnRequest.SubscriptionItemId);
            if (subscriptionItem == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.ReturnRequests.SubscriptionItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }
            
            int queuedEmailId = _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, subscriptionItem, subscriptionItem.Subscription.CustomerLanguageId);
            if (queuedEmailId > 0)
                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Notified"));
            return RedirectToAction("Edit",  new {id = returnRequest.Id});
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            _customerActivityService.InsertActivity("DeleteReturnRequest", _localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}