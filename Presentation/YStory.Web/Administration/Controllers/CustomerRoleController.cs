using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Helpers;
using YStory.Admin.Models.Customers;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Services;
using YStory.Services.Catalog;
using YStory.Services.Customers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Security;
using YStory.Services.Stores;
using YStory.Services.Contributors;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;

namespace YStory.Admin.Controllers
{
    public partial class CustomerRoleController : BaseAdminController
	{
		#region Fields

		private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IStoreService _storeService;
        private readonly IContributorService _contributorService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors

        public CustomerRoleController(ICustomerService customerService,
            ILocalizationService localizationService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IArticleService articleService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IStoreService storeService,
            IContributorService contributorService,
            IWorkContext workContext, 
            ICacheManager cacheManager)
		{
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._articleService = articleService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._storeService = storeService;
            this._contributorService = contributorService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
		}

		#endregion 

        #region Utilities

        [NonAction]
        protected virtual CustomerRoleModel PrepareCustomerRoleModel(CustomerRole customerRole)
        {
            var model = customerRole.ToModel();
            var article = _articleService.GetArticleById(customerRole.PurchasedWithArticleId);
            if (article != null)
            {
                model.PurchasedWithArticleName = article.Name;
            }
            return model;
        }

        #endregion

        #region Customer roles

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
			return View();
		}

		[HttpPost]
		public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();
            
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            var gridModel = new DataSourceResult
			{
                Data = customerRoles.Select(PrepareCustomerRoleModel),
                Total = customerRoles.Count()
			};

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var model = new CustomerRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");
		    
            var model = PrepareCustomerRoleModel(customerRole);
            return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new YStoryException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new YStoryException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    if (SystemCustomerRoleNames.Registered.Equals(customerRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithArticleId > 0)
                        throw new YStoryException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.PurchasedWithArticle.Registered"));
                    
                    customerRole = model.ToEntity(customerRole);
                    _customerService.UpdateCustomerRole(customerRole);

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), customerRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id}) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                _customerService.DeleteCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }

		}



        public virtual ActionResult AssociateArticleToCustomerRolePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new CustomerRoleModel.AssociateArticleToCustomerRoleModel();
            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //publishers
            model.AvailablePublishers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var publishers = SelectListHelper.GetPublisherList(_publisherService, _cacheManager, true);
            foreach (var m in publishers)
                model.AvailablePublishers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //contributors
            model.AvailableContributors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var contributors = SelectListHelper.GetContributorList(_contributorService, _cacheManager, true);
            foreach (var v in contributors)
                model.AvailableContributors.Add(v);

            //article types
            model.AvailableArticleTypes = ArticleType.SimpleArticle.ToSelectList(false).ToList();
            model.AvailableArticleTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult AssociateArticleToCustomerRolePopupList(DataSourceRequest command,
            CustomerRoleModel.AssociateArticleToCustomerRoleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            var articles = _articleService.SearchArticles(
                categoryIds: new List<int> { model.SearchCategoryId },
                publisherId: model.SearchPublisherId,
                storeId: model.SearchStoreId,
                contributorId: model.SearchContributorId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult();
            gridModel.Data = articles.Select(x => x.ToModel());
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult AssociateArticleToCustomerRolePopup(string btnId, string articleIdInput,
            string articleNameInput, CustomerRoleModel.AssociateArticleToCustomerRoleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var associatedArticle = _articleService.GetArticleById(model.AssociatedToArticleId);
            if (associatedArticle == null)
                return Content("Cannot load a article");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && associatedArticle.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            ViewBag.RefreshPage = true;
            ViewBag.articleIdInput = articleIdInput;
            ViewBag.articleNameInput = articleNameInput;
            ViewBag.btnId = btnId;
            ViewBag.articleId = associatedArticle.Id;
            ViewBag.articleName = associatedArticle.Name;
            return View(model);
        }

		#endregion
    }
}
