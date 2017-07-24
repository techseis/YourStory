using System;
using System.Linq;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Contributors;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Security;
using YStory.Services.Stores;
using YStory.Services.Contributors;
using YStory.Web.Factories;
using YStory.Web.Framework.Security;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Controllers
{
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly IArticleModelFactory _articleModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IArticleService _articleService;
        private readonly IContributorService _contributorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IArticleTagService _articleTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ContributorSettings _contributorSettings;

        #endregion

        #region Constructors

        public CatalogController(ICatalogModelFactory catalogModelFactory,
            IArticleModelFactory articleModelFactory,
            ICategoryService categoryService, 
            IPublisherService publisherService,
            IArticleService articleService, 
            IContributorService contributorService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IArticleTagService articleTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService, 
            ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            ContributorSettings contributorSettings)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._articleModelFactory = articleModelFactory;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleService = articleService;
            this._contributorService = contributorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._articleTagService = articleTagService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._contributorSettings = contributorSettings;
        }

        #endregion

        #region Categories
        
        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                SystemCustomerAttributeNames.LastContinueShoppingPage, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = "Admin" }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            //model
            var model = _catalogModelFactory.PrepareCategoryModel(category, command);

            //template
            var templateViewPath = _catalogModelFactory.PrepareCategoryTemplateViewPath(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        [ChildActionOnly]
        public virtual ActionResult CategoryNavigation(int currentCategoryId, int currentArticleId)
        {
            var model = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentArticleId);
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult TopMenu()
        {
            var model = _catalogModelFactory.PrepareTopMenuModel();
            return PartialView(model);
        }
        
        [ChildActionOnly]
        public virtual ActionResult HomepageCategories()
        {
            var model = _catalogModelFactory.PrepareHomepageCategoryModels();
            if (!model.Any())
                return Content("");

            return PartialView(model);
        }

        #endregion

        #region Publishers

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Publisher(int publisherId, CatalogPagingFilteringModel command)
        {
            var publisher = _publisherService.GetPublisherById(publisherId);
            if (publisher == null || publisher.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !publisher.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(publisher) ||
                //Store mapping
                !_storeMappingService.Authorize(publisher);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                SystemCustomerAttributeNames.LastContinueShoppingPage, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManagePublishers))
                DisplayEditLink(Url.Action("Edit", "Publisher", new { id = publisher.Id, area = "Admin" }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewPublisher", _localizationService.GetResource("ActivityLog.PublicStore.ViewPublisher"), publisher.Name);

            //model
            var model = _catalogModelFactory.PreparePublisherModel(publisher, command);
            
            //template
            var templateViewPath = _catalogModelFactory.PreparePublisherTemplateViewPath(publisher.PublisherTemplateId);
            return View(templateViewPath, model);
        }

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult PublisherAll()
        {
            var model = _catalogModelFactory.PreparePublisherAllModels();
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult PublisherNavigation(int currentPublisherId)
        {
            if (_catalogSettings.PublishersBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PreparePublisherNavigationModel(currentPublisherId);

            if (!model.Publishers.Any())
                return Content("");
            
            return PartialView(model);
        }

        #endregion

        #region Contributors

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Contributor(int contributorId, CatalogPagingFilteringModel command)
        {
            var contributor = _contributorService.GetContributorById(contributorId);
            if (contributor == null || contributor.Deleted || !contributor.Active)
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                DisplayEditLink(Url.Action("Edit", "Contributor", new { id = contributor.Id, area = "Admin" }));

            //model
            var model = _catalogModelFactory.PrepareContributorModel(contributor, command);

            return View(model);
        }

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ContributorAll()
        {
            //we don't allow viewing of contributors if "contributors" block is hidden
            if (_contributorSettings.ContributorsBlockItemsToDisplay == 0)
                return RedirectToRoute("HomePage");

            var model = _catalogModelFactory.PrepareContributorAllModels();
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult ContributorNavigation()
        {
            if (_contributorSettings.ContributorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareContributorNavigationModel();
            if (!model.Contributors.Any())
                return Content("");
            
            return PartialView(model);
        }

        #endregion

        #region Article tags
        
        [ChildActionOnly]
        public virtual ActionResult PopularArticleTags()
        {
            var model = _catalogModelFactory.PreparePopularArticleTagsModel();

            if (!model.Tags.Any())
                return Content("");
            
            return PartialView(model);
        }

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ArticlesByTag(int articleTagId, CatalogPagingFilteringModel command)
        {
            var articleTag = _articleTagService.GetArticleTagById(articleTagId);
            if (articleTag == null)
                return InvokeHttp404();

            var model = _catalogModelFactory.PrepareArticlesByTagModel(articleTag, command);
            return View(model);
        }

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ArticleTagsAll()
        {
            var model = _catalogModelFactory.PrepareArticleTagsAllModel();
            return View(model);
        }

        #endregion

        #region Searching

        [YStoryHttpsRequirement(SslRequirement.No)]
        [ValidateInput(false)]
        public virtual ActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult SearchBox()
        {
            var model = _catalogModelFactory.PrepareSearchBoxModel();
            return PartialView(model);
        }

        [ValidateInput(false)]
        public virtual ActionResult SearchTermAutoComplete(string term)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ArticleSearchTermMinimumLength)
                return Content("");

            //articles
            var articleNumber = _catalogSettings.ArticleSearchAutoCompleteNumberOfArticles > 0 ?
                _catalogSettings.ArticleSearchAutoCompleteNumberOfArticles : 10;

            var articles = _articleService.SearchArticles(
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: articleNumber);

            var models =  _articleModelFactory.PrepareArticleOverviewModels(articles, false, _catalogSettings.ShowArticleImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              articleurl = Url.RouteUrl("Article", new { SeName = p.SeName }),
                              articlepictureurl = p.DefaultPictureModel.ImageUrl
                          })
                          .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
