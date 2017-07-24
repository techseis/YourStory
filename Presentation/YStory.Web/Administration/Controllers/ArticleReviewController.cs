using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Models.Catalog;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Services.Catalog;
using YStory.Services.Events;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Security;
using YStory.Services.Stores;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;

namespace YStory.Admin.Controllers
{
    public partial class ArticleReviewController : BaseAdminController
    {
        #region Fields

        private readonly IArticleService _articleService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreService _storeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Constructors

        public ArticleReviewController(IArticleService articleService, 
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, 
            IPermissionService permissionService,
            IEventPublisher eventPublisher,
            IStoreService storeService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            this._articleService = articleService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;
            this._storeService = storeService;
            this._customerActivityService = customerActivityService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareArticleReviewModel(ArticleReviewModel model,
            ArticleReview articleReview, bool excludeProperties, bool formatReviewAndReplyText)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (articleReview == null)
                throw new ArgumentNullException("articleReview");

            model.Id = articleReview.Id;
            model.StoreName = articleReview.Store.Name;
            model.ArticleId = articleReview.ArticleId;
            model.ArticleName = articleReview.Article.Name;
            model.CustomerId = articleReview.CustomerId;
            var customer = articleReview.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.Rating = articleReview.Rating;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(articleReview.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                model.Title = articleReview.Title;
                if (formatReviewAndReplyText)
                {
                    model.ReviewText = Core.Html.HtmlHelper.FormatText(articleReview.ReviewText, false, true, false, false, false, false);
                    model.ReplyText = Core.Html.HtmlHelper.FormatText(articleReview.ReplyText, false, true, false, false, false, false);
                }
                else
                {
                    model.ReviewText = articleReview.ReviewText;
                    model.ReplyText = articleReview.ReplyText;
                }
                model.IsApproved = articleReview.IsApproved;
            }

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            var model = new ArticleReviewListModel();
            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;

            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var stores = _storeService.GetAllStores().Select(st => new SelectListItem() { Text = st.Name, Value = st.Id.ToString() });
            foreach (var selectListItem in stores)
                model.AvailableStores.Add(selectListItem);

            //"approved" property
            //0 - all
            //1 - approved only
            //2 - disapproved only
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ArticleReviews.List.SearchApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ArticleReviews.List.SearchApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ArticleReviews.List.SearchApproved.DisapprovedOnly"), Value = "2" });
            
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, ArticleReviewListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                contributorId = _workContext.CurrentContributor.Id;
            }

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            bool? approved = null;
            if (model.SearchApprovedId > 0)
                approved = model.SearchApprovedId == 1;

            var articleReviews = _articleService.GetAllArticleReviews(0, approved, 
                createdOnFromValue, createdToFromValue, model.SearchText, 
                model.SearchStoreId, model.SearchArticleId, contributorId, 
                command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = articleReviews.Select(x =>
                {
                    var m = new ArticleReviewModel();
                    PrepareArticleReviewModel(m, x, false, true);
                    return m;
                }),
                Total = articleReviews.TotalCount
            };

            return Json(gridModel);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            var articleReview = _articleService.GetArticleReviewById(id);
            if (articleReview == null)
                //No article review found with the specified id
                return RedirectToAction("List");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && articleReview.Article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            var model = new ArticleReviewModel();
            PrepareArticleReviewModel(model, articleReview, false, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ArticleReviewModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            var articleReview = _articleService.GetArticleReviewById(model.Id);
            if (articleReview == null)
                //No article review found with the specified id
                return RedirectToAction("List");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && articleReview.Article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var isLoggedInAsContributor = _workContext.CurrentContributor != null;

                var previousIsApproved = articleReview.IsApproved;
                //contributor can edit "Reply text" only
                if (!isLoggedInAsContributor)
                {
                    articleReview.Title = model.Title;
                    articleReview.ReviewText = model.ReviewText;
                    articleReview.IsApproved = model.IsApproved;
                }

                articleReview.ReplyText = model.ReplyText;
                _articleService.UpdateArticle(articleReview.Article);

                //activity log
                _customerActivityService.InsertActivity("EditArticleReview", _localizationService.GetResource("ActivityLog.EditArticleReview"), articleReview.Id);

                //contributor can edit "Reply text" only
                if (!isLoggedInAsContributor)
                {
                    //update article totals
                    _articleService.UpdateArticleReviewTotals(articleReview.Article);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && articleReview.IsApproved)
                        _eventPublisher.Publish(new ArticleReviewApprovedEvent(articleReview));

                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ArticleReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = articleReview.Id }) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareArticleReviewModel(model, articleReview, true, false);
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            var articleReview = _articleService.GetArticleReviewById(id);
            if (articleReview == null)
                //No article review found with the specified id
                return RedirectToAction("List");

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("List");

            var article = articleReview.Article;
            _articleService.DeleteArticleReview(articleReview);

            //activity log
            _customerActivityService.InsertActivity("DeleteArticleReview", _localizationService.GetResource("ActivityLog.DeleteArticleReview"), articleReview.Id);

            //update article totals
            _articleService.UpdateArticleReviewTotals(article);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.ArticleReviews.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter not approved reviews
                var articleReviews = _articleService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => !review.IsApproved);
                foreach (var articleReview in articleReviews)
                {
                    articleReview.IsApproved = true;
                    _articleService.UpdateArticle(articleReview.Article);
                    
                    //update article totals
                    _articleService.UpdateArticleReviewTotals(articleReview.Article);

                    //raise event 
                    _eventPublisher.Publish(new ArticleReviewApprovedEvent(articleReview));
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter approved reviews
                var articleReviews = _articleService.GetProducReviewsByIds(selectedIds.ToArray()).Where(review => review.IsApproved);
                foreach (var articleReview in articleReviews)
                {
                    articleReview.IsApproved = false;
                    _articleService.UpdateArticle(articleReview.Article);

                    //update article totals
                    _articleService.UpdateArticleReviewTotals(articleReview.Article);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return AccessDeniedView();

            //a contributor does not have access to this functionality
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                var articleReviews = _articleService.GetProducReviewsByIds(selectedIds.ToArray());
                var articles = _articleService.GetArticlesByIds(articleReviews.Select(p => p.ArticleId).Distinct().ToArray());

                _articleService.DeleteArticleReviews(articleReviews);

                //update article totals
                foreach (var article in articles)
                {
                    _articleService.UpdateArticleReviewTotals(article);
                }
            }

            return Json(new { Result = true });
        }

        public virtual ActionResult ArticleSearchAutoComplete(string term)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleReviews))
                return Content("");

            const int searchTermMinimumLength = 3;
            if (String.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                contributorId = _workContext.CurrentContributor.Id;
            }

            //articles
            const int articleNumber = 15;
            var articles = _articleService.SearchArticles(
                keywords: term,
                contributorId: contributorId,
                pageSize: articleNumber,
                showHidden: true);

            var result = (from p in articles
                select new
                {
                    label = p.Name,
                    articleid = p.Id
                })
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
