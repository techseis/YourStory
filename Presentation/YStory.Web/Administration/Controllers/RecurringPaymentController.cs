using System;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Models.Subscriptions;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Security;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;

namespace YStory.Admin.Controllers
{
    public partial class RecurringPaymentController : BaseAdminController
    {
        #region Fields

        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public RecurringPaymentController(ISubscriptionService subscriptionService,
            ISubscriptionProcessingService subscriptionProcessingService, ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper, IPaymentService paymentService,
            IPermissionService permissionService)
        {
            this._subscriptionService = subscriptionService;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareRecurringPaymentModel(RecurringPaymentModel model, 
            RecurringPayment recurringPayment)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");
            
            model.Id = recurringPayment.Id;
            model.CycleLength = recurringPayment.CycleLength;
            model.CyclePeriodId = recurringPayment.CyclePeriodId;
            model.CyclePeriodStr = recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext);
            model.TotalCycles = recurringPayment.TotalCycles;
            model.StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString();
            model.IsActive = recurringPayment.IsActive;
            model.NextPaymentDate = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "";
            model.CyclesRemaining = recurringPayment.CyclesRemaining;
            model.InitialSubscriptionId = recurringPayment.InitialSubscription.Id;
            var customer = recurringPayment.InitialSubscription.Customer;
            model.CustomerId = customer.Id;
            model.CustomerEmail = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.PaymentType = _paymentService.GetRecurringPaymentType(recurringPayment.InitialSubscription.PaymentMethodSystemName).GetLocalizedEnum(_localizationService, _workContext);
            model.CanCancelRecurringPayment = _subscriptionProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment);
            model.LastPaymentFailed = recurringPayment.LastPaymentFailed;
        }

        [NonAction]
        protected virtual void PrepareRecurringPaymentHistoryModel(RecurringPaymentModel.RecurringPaymentHistoryModel model,
            RecurringPaymentHistory history)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (history == null)
                throw new ArgumentNullException("history");

            var subscription = _subscriptionService.GetOrderById(history.SubscriptionId);

            model.Id = history.Id;
            model.SubscriptionId = history.SubscriptionId;
            model.RecurringPaymentId = history.RecurringPaymentId;
            model.SubscriptionStatus = subscription.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.PaymentStatus = subscription.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(history.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomSubscriptionNumber = subscription.CustomSubscriptionNumber;
        }

        #endregion

        #region Recurring payment

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedKendoGridJson();

            var payments = _subscriptionService.SearchRecurringPayments(0, 0, 0, null, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = payments.Select(x =>
                {
                    var m = new RecurringPaymentModel();
                    PrepareRecurringPaymentModel(m, x);
                    return m;
                }),
                Total = payments.TotalCount,
            };

            return Json(gridModel);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _subscriptionService.GetRecurringPaymentById(id);
            if (payment == null || payment.Deleted)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            var model = new RecurringPaymentModel();
            PrepareRecurringPaymentModel(model, payment);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(RecurringPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _subscriptionService.GetRecurringPaymentById(model.Id);
            if (payment == null || payment.Deleted)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            payment.CycleLength = model.CycleLength;
            payment.CyclePeriodId = model.CyclePeriodId;
            payment.TotalCycles = model.TotalCycles;
            payment.IsActive = model.IsActive;
            _subscriptionService.UpdateRecurringPayment(payment);

            SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Updated"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit",  new {id = payment.Id});
            }
            return RedirectToAction("List");
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _subscriptionService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            _subscriptionService.DeleteRecurringPayment(payment);

            SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region History

        [HttpPost]
        public virtual ActionResult HistoryList(int recurringPaymentId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedKendoGridJson();

            var payment = _subscriptionService.GetRecurringPaymentById(recurringPaymentId);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            var historyModel = payment.RecurringPaymentHistory.OrderBy(x => x.CreatedOnUtc)
                .Select(x =>
                {
                    var m = new RecurringPaymentModel.RecurringPaymentHistoryModel();
                    PrepareRecurringPaymentHistoryModel(m, x);
                    return m;
                })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = historyModel,
                Total = historyModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public virtual ActionResult ProcessNextPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _subscriptionService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");
            
            try
            {
                var errors = _subscriptionProcessingService.ProcessNextRecurringPayment(payment);

                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);

                if (errors.Any())
                    errors.ToList().ForEach(error => ErrorNotification(error, false));
                else
                    SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.NextPaymentProcessed"), false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                ErrorNotification(exc, false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public virtual ActionResult CancelRecurringPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _subscriptionService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            try
            {
                var errors = _subscriptionProcessingService.CancelRecurringPayment(payment);
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                if (errors.Any())
                {
                    foreach (var error in errors)
                        ErrorNotification(error, false);
                }
                else
                    SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Cancelled"), false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                ErrorNotification(exc, false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
        }

        #endregion
    }
}
