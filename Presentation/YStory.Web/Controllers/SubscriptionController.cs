using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Common;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Web.Factories;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Security;

namespace YStory.Web.Controllers
{
    public partial class SubscriptionController : BasePublicController
    {
        #region Fields

        private readonly ISubscriptionModelFactory _subscriptionModelFactory;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWorkContext _workContext;
        private readonly ISubscriptionProcessingService _subscriptionProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IWebHelper _webHelper;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

		#region Constructors

        public SubscriptionController(ISubscriptionModelFactory subscriptionModelFactory,
            ISubscriptionService subscriptionService, 
            IWorkContext workContext,
            ISubscriptionProcessingService subscriptionProcessingService, 
            IPaymentService paymentService, 
            IPdfService pdfService, 
            IWebHelper webHelper,
            RewardPointsSettings rewardPointsSettings)
        {
            this._subscriptionModelFactory = subscriptionModelFactory;
            this._subscriptionService = subscriptionService;
            this._workContext = workContext;
            this._subscriptionProcessingService = subscriptionProcessingService;
            this._paymentService = paymentService;
            this._pdfService = pdfService;
            this._webHelper = webHelper;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Methods

        //My account / Subscriptions
        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult CustomerSubscriptions()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _subscriptionModelFactory.PrepareCustomerSubscriptionListModel();
            return View(model);
        }

        //My account / Subscriptions / Cancel recurring subscription
        [HttpPost, ActionName("CustomerSubscriptions")]
        [PublicAntiForgery]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public virtual ActionResult CancelRecurringPayment(FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //get recurring payment identifier
            int recurringPaymentId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

            var recurringPayment = _subscriptionService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
            {
                return RedirectToRoute("CustomerSubscriptions");
            }

            if (_subscriptionProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
            {
                var errors = _subscriptionProcessingService.CancelRecurringPayment(recurringPayment);

                var model = _subscriptionModelFactory.PrepareCustomerSubscriptionListModel();
                model.RecurringPaymentErrors = errors;

                return View(model);
            }
            else
            {
                return RedirectToRoute("CustomerSubscriptions");
            }
        }

        //My account / Subscriptions / Retry last recurring subscription
        [HttpPost, ActionName("CustomerSubscriptions")]
        [PublicAntiForgery]
        [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
        public virtual ActionResult RetryLastRecurringPayment(FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            if (!form.AllKeys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                int.TryParse(formValue.Substring(formValue.IndexOf('_') + 1), out recurringPaymentId)))
            {
                return RedirectToRoute("CustomerSubscriptions");
            }

            var recurringPayment = _subscriptionService.GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
                return RedirectToRoute("CustomerSubscriptions");

            if (!_subscriptionProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
                return RedirectToRoute("CustomerSubscriptions");

            var errors = _subscriptionProcessingService.ProcessNextRecurringPayment(recurringPayment);
            var model = _subscriptionModelFactory.PrepareCustomerSubscriptionListModel();
            model.RecurringPaymentErrors = errors.ToList();

            return View(model);
        }

        //My account / Reward points
        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult CustomerRewardPoints(int? page)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToRoute("CustomerInfo");

            var model = _subscriptionModelFactory.PrepareCustomerRewardPoints(page);
            return View(model);
        }

        //My account / Subscription details page
        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Details(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            var model = _subscriptionModelFactory.PrepareSubscriptionDetailsModel(subscription);
            return View(model);
        }

        //My account / Subscription details page / Print
        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult PrintSubscriptionDetails(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            var model = _subscriptionModelFactory.PrepareSubscriptionDetailsModel(subscription);
            model.PrintMode = true;

            return View("Details", model);
        }

        //My account / Subscription details page / PDF invoice
        public virtual ActionResult GetPdfInvoice(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            var subscriptions = new List<Subscription>();
            subscriptions.Add(subscription);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintSubscriptionsToPdf(stream, subscriptions, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, string.Format("subscription_{0}.pdf", subscription.Id));
        }

        //My account / Subscription details page / re-subscription
        public virtual ActionResult ReSubscription(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            _subscriptionProcessingService.ReSubscription(subscription);
            return RedirectToRoute("ShoppingCart");
        }

        //My account / Subscription details page / Complete payment
        [HttpPost, ActionName("Details")]
        [PublicAntiForgery]
        [FormValueRequired("repost-payment")]
        public virtual ActionResult RePostPayment(int subscriptionId)
        {
            var subscription = _subscriptionService.GetOrderById(subscriptionId);
            if (subscription == null || subscription.Deleted || _workContext.CurrentCustomer.Id != subscription.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_paymentService.CanRePostProcessPayment(subscription))
                return RedirectToRoute("SubscriptionDetails", new { subscriptionId = subscriptionId });

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Subscription = subscription
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return RedirectToRoute("SubscriptionDetails", new { subscriptionId = subscriptionId });
        }

        

        #endregion
    }
}
