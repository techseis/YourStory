using System;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Models.Catalog;
using YStory.Core.Domain.Catalog;
using YStory.Services.Catalog;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Security;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class ArticleAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IArticleService _articleService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ArticleAttributeController(IArticleService articleService,
            IArticleAttributeService articleAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._articleService = articleService;
            this._articleAttributeService = articleAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(ArticleAttribute articleAttribute, ArticleAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(articleAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(articleAttribute,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(PredefinedArticleAttributeValue ppav, PredefinedArticleAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(ppav,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion
        
        #region Methods

        #region Attribute list / create / edit / delete

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var articleAttributes = _articleAttributeService
                .GetAllArticleAttributes(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = articleAttributes.Select(x => x.ToModel()),
                Total = articleAttributes.TotalCount
            };

            return Json(gridModel);
        }
        
        //create
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new ArticleAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(ArticleAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var articleAttribute = model.ToEntity();
                _articleAttributeService.InsertArticleAttribute(articleAttribute);
                UpdateLocales(articleAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewArticleAttribute", _localizationService.GetResource("ActivityLog.AddNewArticleAttribute"), articleAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = articleAttribute.Id });
                }
                return RedirectToAction("List");

            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var articleAttribute = _articleAttributeService.GetArticleAttributeById(id);
            if (articleAttribute == null)
                //No article attribute found with the specified id
                return RedirectToAction("List");

            var model = articleAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = articleAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = articleAttribute.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ArticleAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var articleAttribute = _articleAttributeService.GetArticleAttributeById(model.Id);
            if (articleAttribute == null)
                //No article attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                articleAttribute = model.ToEntity(articleAttribute);
                _articleAttributeService.UpdateArticleAttribute(articleAttribute);

                UpdateLocales(articleAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditArticleAttribute", _localizationService.GetResource("ActivityLog.EditArticleAttribute"), articleAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = articleAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var articleAttribute = _articleAttributeService.GetArticleAttributeById(id);
            if (articleAttribute == null)
                //No article attribute found with the specified id
                return RedirectToAction("List");

            _articleAttributeService.DeleteArticleAttribute(articleAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteArticleAttribute", _localizationService.GetResource("ActivityLog.DeleteArticleAttribute"), articleAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ArticleAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Used by articles

        //used by articles
        [HttpPost]
        public virtual ActionResult UsedByArticles(DataSourceRequest command, int articleAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var articles = _articleService.GetArticlesByArticleAtributeId(
                articleAttributeId: articleAttributeId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = articles.Select(x =>
                {
                    return new ArticleAttributeModel.UsedByArticleModel
                    {
                        Id = x.Id,
                        ArticleName = x.Name,
                        Published = x.Published
                    };
                }),
                Total = articles.TotalCount
            };

            return Json(gridModel);
        }
        
        #endregion

        #region Predefined values

        [HttpPost]
        public virtual ActionResult PredefinedArticleAttributeValueList(int articleAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var values = _articleAttributeService.GetPredefinedArticleAttributeValues(articleAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x =>
                {
                    return new PredefinedArticleAttributeValueModel
                    {
                        Id = x.Id,
                        ArticleAttributeId = x.ArticleAttributeId,
                        Name = x.Name,
                        PriceAdjustment = x.PriceAdjustment,
                        PriceAdjustmentStr = x.PriceAdjustment.ToString("G29"),
                        WeightAdjustment = x.WeightAdjustment,
                        WeightAdjustmentStr = x.WeightAdjustment.ToString("G29"),
                        Cost = x.Cost,
                        IsPreSelected = x.IsPreSelected,
                        DisplaySubscription = x.DisplaySubscription
                    };
                }),
                Total = values.Count()
            };

            return Json(gridModel);
        }

        //create
        public virtual ActionResult PredefinedArticleAttributeValueCreatePopup(int articleAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var articleAttribute = _articleAttributeService.GetArticleAttributeById(articleAttributeId);
            if (articleAttribute == null)
                throw new ArgumentException("No article attribute found with the specified id");

            var model = new PredefinedArticleAttributeValueModel();
            model.ArticleAttributeId = articleAttributeId;

            //locales
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult PredefinedArticleAttributeValueCreatePopup(string btnId, string formId, PredefinedArticleAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var articleAttribute = _articleAttributeService.GetArticleAttributeById(model.ArticleAttributeId);
            if (articleAttribute == null)
                throw new ArgumentException("No article attribute found with the specified id");

            if (ModelState.IsValid)
            {
                var ppav = new PredefinedArticleAttributeValue
                {
                    ArticleAttributeId = model.ArticleAttributeId,
                    Name = model.Name,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    IsPreSelected = model.IsPreSelected,
                    DisplaySubscription = model.DisplaySubscription
                };

                _articleAttributeService.InsertPredefinedArticleAttributeValue(ppav);
                UpdateLocales(ppav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult PredefinedArticleAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _articleAttributeService.GetPredefinedArticleAttributeValueById(id);
            if (ppav == null)
                throw new ArgumentException("No article attribute value found with the specified id");

            var model = new PredefinedArticleAttributeValueModel
            {
                ArticleAttributeId = ppav.ArticleAttributeId,
                Name = ppav.Name,
                PriceAdjustment = ppav.PriceAdjustment,
                WeightAdjustment = ppav.WeightAdjustment,
                Cost = ppav.Cost,
                IsPreSelected = ppav.IsPreSelected,
                DisplaySubscription = ppav.DisplaySubscription
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = ppav.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult PredefinedArticleAttributeValueEditPopup(string btnId, string formId, PredefinedArticleAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _articleAttributeService.GetPredefinedArticleAttributeValueById(model.Id);
            if (ppav == null)
                throw new ArgumentException("No article attribute value found with the specified id");

            if (ModelState.IsValid)
            {
                ppav.Name = model.Name;
                ppav.PriceAdjustment = model.PriceAdjustment;
                ppav.WeightAdjustment = model.WeightAdjustment;
                ppav.Cost = model.Cost;
                ppav.IsPreSelected = model.IsPreSelected;
                ppav.DisplaySubscription = model.DisplaySubscription;
                _articleAttributeService.UpdatePredefinedArticleAttributeValue(ppav);

                UpdateLocales(ppav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult PredefinedArticleAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _articleAttributeService.GetPredefinedArticleAttributeValueById(id);
            if (ppav == null)
                throw new ArgumentException("No predefined article attribute value found with the specified id");

            _articleAttributeService.DeletePredefinedArticleAttributeValue(ppav);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}
