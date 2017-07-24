using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Helpers;
using YStory.Admin.Models.Catalog;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Services;
using YStory.Services.Catalog;
using YStory.Services.Customers;
using YStory.Services.ExportImport;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Stores;
using YStory.Services.Contributors;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class PublisherController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IPublisherTemplateService _publisherTemplateService;
        private readonly IArticleService _articleService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IContributorService _contributorService;
        private readonly IAclService _aclService; 
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public PublisherController(ICategoryService categoryService, 
            IPublisherService publisherService,
            IPublisherTemplateService publisherTemplateService,
            IArticleService articleService,
            ICustomerService customerService, 
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService, 
            IPictureService pictureService,
            ILanguageService languageService, 
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService, 
            IExportManager exportManager,
            ICustomerActivityService customerActivityService, 
            IContributorService contributorService,
            IAclService aclService,
            IPermissionService permissionService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager, 
            ICacheManager cacheManager)
        {
            this._categoryService = categoryService;
            this._publisherTemplateService = publisherTemplateService;
            this._publisherService = publisherService;
            this._articleService = articleService;
            this._customerService = customerService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._contributorService = contributorService;
            this._aclService = aclService;
            this._permissionService = permissionService;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Publisher publisher, PublisherModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(publisher,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(publisher,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(publisher,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(publisher,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(publisher,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = publisher.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(publisher, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Publisher publisher)
        {
            var picture = _pictureService.GetPictureById(publisher.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(publisher.Name));
        }

        [NonAction]
        protected virtual void PrepareTemplatesModel(PublisherModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _publisherTemplateService.GetAllPublisherTemplates();
            foreach (var template in templates)
            {
                model.AvailablePublisherTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        

        [NonAction]
        protected virtual void PrepareAclModel(PublisherModel model, Publisher publisher, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && publisher != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(publisher).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SavePublisherAcl(Publisher publisher, PublisherModel model)
        {
            publisher.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(publisher);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(publisher, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(PublisherModel model, Publisher publisher, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && publisher != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(publisher).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Publisher publisher, PublisherModel model)
        {
            publisher.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(publisher);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(publisher, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region List

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var model = new PublisherListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, PublisherListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedKendoGridJson();

            var publishers = _publisherService.GetAllPublishers(model.SearchPublisherName,
                model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = publishers.Select(x => x.ToModel()),
                Total = publishers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var model = new PublisherModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //templates
            PrepareTemplatesModel(model);
           
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.PageSize = _catalogSettings.DefaultPublisherPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultPublisherPageSizeOptions;
            model.Published = true;
            model.AllowCustomersToSelectPageSize = true;
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(PublisherModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var publisher = model.ToEntity();
                publisher.CreatedOnUtc = DateTime.UtcNow;
                publisher.UpdatedOnUtc = DateTime.UtcNow;
                _publisherService.InsertPublisher(publisher);
                //search engine name
                model.SeName = publisher.ValidateSeName(model.SeName, publisher.Name, true);
                _urlRecordService.SaveSlug(publisher, model.SeName, 0);
                //locales
                UpdateLocales(publisher, model);
               
                _publisherService.UpdatePublisher(publisher);
                //update picture seo file name
                UpdatePictureSeoNames(publisher);
                //ACL (customer roles)
                SavePublisherAcl(publisher, model);
                //Stores
                SaveStoreMappings(publisher, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewPublisher", _localizationService.GetResource("ActivityLog.AddNewPublisher"), publisher.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Publishers.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = publisher.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
           
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);

            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var publisher = _publisherService.GetPublisherById(id);
            if (publisher == null || publisher.Deleted)
                //No publisher found with the specified id
                return RedirectToAction("List");

            var model = publisher.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = publisher.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = publisher.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = publisher.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = publisher.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = publisher.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = publisher.GetSeName(languageId, false, false);
            });
            //templates
            PrepareTemplatesModel(model);
           
            //ACL
            PrepareAclModel(model, publisher, false);
            //Stores
            PrepareStoresMappingModel(model, publisher, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(PublisherModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var publisher = _publisherService.GetPublisherById(model.Id);
            if (publisher == null || publisher.Deleted)
                //No publisher found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = publisher.PictureId;
                publisher = model.ToEntity(publisher);
                publisher.UpdatedOnUtc = DateTime.UtcNow;
                _publisherService.UpdatePublisher(publisher);
                //search engine name
                model.SeName = publisher.ValidateSeName(model.SeName, publisher.Name, true);
                _urlRecordService.SaveSlug(publisher, model.SeName, 0);
                //locales
                UpdateLocales(publisher, model);
               
                _publisherService.UpdatePublisher(publisher);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != publisher.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(publisher);
                //ACL
                SavePublisherAcl(publisher, model);
                //Stores
                SaveStoreMappings(publisher, model);

                //activity log
                _customerActivityService.InsertActivity("EditPublisher", _localizationService.GetResource("ActivityLog.EditPublisher"), publisher.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Publishers.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = publisher.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
           
            //ACL
            PrepareAclModel(model, publisher, true);
            //Stores
            PrepareStoresMappingModel(model, publisher, true);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var publisher = _publisherService.GetPublisherById(id);
            if (publisher == null)
                //No publisher found with the specified id
                return RedirectToAction("List");

            _publisherService.DeletePublisher(publisher);

            //activity log
            _customerActivityService.InsertActivity("DeletePublisher", _localizationService.GetResource("ActivityLog.DeletePublisher"), publisher.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Publishers.Deleted"));
            return RedirectToAction("List");
        }
        
        #endregion

        #region Export / Import

        public virtual ActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            try
            {
                var publishers = _publisherService.GetAllPublishers(showHidden: true);
                var xml = _exportManager.ExportPublishersToXml(publishers);
                return new XmlDownloadResult(xml, "publishers.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual ActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportPublishersToXlsx(_publisherService.GetAllPublishers(showHidden: true).Where(p=>!p.Deleted));
                 
                return File(bytes, MimeTypes.TextXlsx, "publishers.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ImportFromXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            //a contributor cannot import publishers
            if (_workContext.CurrentContributor != null)
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportPublishersFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Publishers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Articles

        [HttpPost]
        public virtual ActionResult ArticleList(DataSourceRequest command, int publisherId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedKendoGridJson();

            var articlePublishers = _publisherService.GetArticlePublishersByPublisherId(publisherId,
                command.Page - 1, command.PageSize, true);

            var gridModel = new DataSourceResult
            {
                Data = articlePublishers
                .Select(x => new PublisherModel.PublisherArticleModel
                {
                    Id = x.Id,
                    PublisherId = x.PublisherId,
                    ArticleId = x.ArticleId,
                    ArticleName = _articleService.GetArticleById(x.ArticleId).Name,
                    IsFeaturedArticle = x.IsFeaturedArticle,
                    DisplaySubscription = x.DisplaySubscription
                }),
                Total = articlePublishers.TotalCount
            };


            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticleUpdate(PublisherModel.PublisherArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var articlePublisher = _publisherService.GetArticlePublisherById(model.Id);
            if (articlePublisher == null)
                throw new ArgumentException("No article publisher mapping found with the specified id");

            articlePublisher.IsFeaturedArticle = model.IsFeaturedArticle;
            articlePublisher.DisplaySubscription = model.DisplaySubscription;
            _publisherService.UpdateArticlePublisher(articlePublisher);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var articlePublisher = _publisherService.GetArticlePublisherById(id);
            if (articlePublisher == null)
                throw new ArgumentException("No article publisher mapping found with the specified id");

            //var publisherId = articlePublisher.PublisherId;
            _publisherService.DeleteArticlePublisher(articlePublisher);

            return new NullJsonResult();
        }

        public virtual ActionResult ArticleAddPopup(int publisherId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            var model = new PublisherModel.AddPublisherArticleModel();
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
        public virtual ActionResult ArticleAddPopupList(DataSourceRequest command, PublisherModel.AddPublisherArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedKendoGridJson();

            var gridModel = new DataSourceResult();
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
            gridModel.Data = articles.Select(x => x.ToModel());
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }
        
        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult ArticleAddPopup(string btnId, string formId, PublisherModel.AddPublisherArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        var existingArticlepublishers = _publisherService.GetArticlePublishersByPublisherId(model.PublisherId, showHidden: true);
                        if (existingArticlepublishers.FindArticlePublisher(id, model.PublisherId) == null)
                        {
                            _publisherService.InsertArticlePublisher(
                                new ArticlePublisher
                                {
                                    PublisherId = model.PublisherId,
                                    ArticleId = id,
                                    IsFeaturedArticle = false,
                                    DisplaySubscription = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        
    }
}
