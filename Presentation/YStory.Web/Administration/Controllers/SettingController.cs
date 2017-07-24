using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Models.Common;
using YStory.Admin.Models.Settings;
using YStory.Admin.Models.Stores;
using YStory.Core;
using YStory.Core.Configuration;
using YStory.Core.Domain;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Forums;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.News;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Security;
using YStory.Core.Domain.Seo;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Services;
using YStory.Services.Common;
using YStory.Services.Configuration;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Security;
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Web.Framework;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;
using YStory.Web.Framework.Security;
using YStory.Web.Framework.Security.Captcha;
using YStory.Web.Framework.Themes;

namespace YStory.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
	{
		#region Fields

        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly YStoryConfig _config;
        #endregion

        #region Ctor

        public SettingController(ISettingService settingService,
            ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            IAddressService addressService, 
            ITaxCategoryService taxCategoryService,
            ICurrencyService currencyService,
            IPictureService pictureService, 
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            ISubscriptionService subscriptionService,
            IEncryptionService encryptionService,
            IThemeProvider themeProvider,
            ICustomerService customerService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IFulltextService fulltextService, 
            IMaintenanceService maintenanceService,
            IStoreService storeService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService,
            IReturnRequestService returnRequestService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            YStoryConfig config)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._taxCategoryService = taxCategoryService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._subscriptionService = subscriptionService;
            this._encryptionService = encryptionService;
            this._themeProvider = themeProvider;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._returnRequestService = returnRequestService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._config = config;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(ReturnRequestReason rrr, ReturnRequestReasonModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rrr,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(ReturnRequestAction rra, ReturnRequestActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rra,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public virtual ActionResult Mode(string modeName = "settings-advanced-mode")
        {
            var model = new ModeModel()
            {
                ModeName = modeName,
                Enabled = _workContext.CurrentCustomer.GetAttribute<bool>(modeName)
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult StoreScopeConfiguration()
        {
            var allStores = _storeService.GetAllStores();
            if (allStores.Count < 2)
                return Content("");

            var model = new StoreScopeConfigurationModel();
            foreach (var s in allStores)
            {
                model.Stores.Add(new StoreModel
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
            model.StoreId = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            return PartialView(model);
        }
        public virtual ActionResult ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
        {
            var store = _storeService.GetStoreById(storeid);
            if (store != null || storeid == 0)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration, storeid);
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

        public virtual ActionResult Blog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var blogSettings = _settingService.LoadSetting<BlogSettings>(storeScope);
            var model = blogSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.Enabled, storeScope);
                model.PostsPageSize_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.PostsPageSize, storeScope);
                model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);
                model.NotifyAboutNewBlogComments_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.NotifyAboutNewBlogComments, storeScope);
                model.NumberOfTags_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.NumberOfTags, storeScope);
                model.ShowHeaderRssUrl_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.ShowHeaderRssUrl, storeScope);
                model.BlogCommentsMustBeApproved_OverrideForStore = _settingService.SettingExists(blogSettings, x => x.BlogCommentsMustBeApproved, storeScope);
            }

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Blog(BlogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var blogSettings = _settingService.LoadSetting<BlogSettings>(storeScope);
            blogSettings = model.ToEntity(blogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.NotifyAboutNewBlogComments, model.NotifyAboutNewBlogComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.NumberOfTags, model.NumberOfTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogSettings, x => x.BlogCommentsMustBeApproved, model.BlogCommentsMustBeApproved_OverrideForStore, storeScope, false);
            _settingService.SaveSetting(blogSettings, x => x.ShowBlogCommentsPerStore, clearCache: false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Blog");
        }




        public virtual ActionResult Contributor()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contributorSettings = _settingService.LoadSetting<ContributorSettings>(storeScope);
            var model = contributorSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ContributorsBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.ContributorsBlockItemsToDisplay, storeScope);
                model.ShowContributorOnArticleDetailsPage_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.ShowContributorOnArticleDetailsPage, storeScope);
                model.AllowCustomersToContactContributors_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.AllowCustomersToContactContributors, storeScope);
                model.AllowCustomersToApplyForContributorAccount_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.AllowCustomersToApplyForContributorAccount, storeScope);
                model.AllowSearchByContributor_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.AllowSearchByContributor, storeScope);
                model.AllowContributorsToEditInfo_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.AllowContributorsToEditInfo, storeScope);
                model.NotifyStoreOwnerAboutContributorInformationChange_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.NotifyStoreOwnerAboutContributorInformationChange, storeScope);
                model.MaximumArticleNumber_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.MaximumArticleNumber, storeScope);
                model.AllowContributorsToImportArticles_OverrideForStore = _settingService.SettingExists(contributorSettings, x => x.AllowContributorsToImportArticles, storeScope);
            }

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Contributor(ContributorSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contributorSettings = _settingService.LoadSetting<ContributorSettings>(storeScope);
            contributorSettings = model.ToEntity(contributorSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.ContributorsBlockItemsToDisplay, model.ContributorsBlockItemsToDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.ShowContributorOnArticleDetailsPage, model.ShowContributorOnArticleDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.AllowCustomersToContactContributors, model.AllowCustomersToContactContributors_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.AllowCustomersToApplyForContributorAccount, model.AllowCustomersToApplyForContributorAccount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.AllowSearchByContributor, model.AllowSearchByContributor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.AllowContributorsToEditInfo, model.AllowContributorsToEditInfo_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.NotifyStoreOwnerAboutContributorInformationChange, model.NotifyStoreOwnerAboutContributorInformationChange_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.MaximumArticleNumber, model.MaximumArticleNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(contributorSettings, x => x.AllowContributorsToImportArticles, model.AllowContributorsToImportArticles_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Contributor");
        }




        public virtual ActionResult Forum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            var model = forumSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ForumsEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumsEnabled, storeScope);
                model.RelativeDateTimeFormattingEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeScope);
                model.ShowCustomersPostCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ShowCustomersPostCount, storeScope);
                model.AllowGuestsToCreatePosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreatePosts, storeScope);
                model.AllowGuestsToCreateTopics_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreateTopics, storeScope);
                model.AllowCustomersToEditPosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToEditPosts, storeScope);
                model.AllowCustomersToDeletePosts_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToDeletePosts, storeScope);
                model.AllowPostVoting_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowPostVoting, storeScope);
                model.MaxVotesPerDay_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.MaxVotesPerDay, storeScope);
                model.AllowCustomersToManageSubscriptions_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeScope);
                model.TopicsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.TopicsPageSize, storeScope);
                model.PostsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.PostsPageSize, storeScope);
                model.ForumEditor_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumEditor, storeScope);
                model.SignaturesEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.SignaturesEnabled, storeScope);
                model.AllowPrivateMessages_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.AllowPrivateMessages, storeScope);
                model.ShowAlertForPM_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ShowAlertForPM, storeScope);
                model.NotifyAboutPrivateMessages_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.NotifyAboutPrivateMessages, storeScope);
                model.ActiveDiscussionsFeedEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeScope);
                model.ActiveDiscussionsFeedCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedCount, storeScope);
                model.ForumFeedsEnabled_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumFeedsEnabled, storeScope);
                model.ForumFeedCount_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ForumFeedCount, storeScope);
                model.SearchResultsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.SearchResultsPageSize, storeScope);
                model.ActiveDiscussionsPageSize_OverrideForStore = _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsPageSize, storeScope);
            }
            model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Forum(ForumSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            forumSettings = model.ToEntity(forumSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumsEnabled, model.ForumsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.RelativeDateTimeFormattingEnabled, model.RelativeDateTimeFormattingEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ShowCustomersPostCount, model.ShowCustomersPostCount_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowGuestsToCreatePosts, model.AllowGuestsToCreatePosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowGuestsToCreateTopics, model.AllowGuestsToCreateTopics_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToEditPosts, model.AllowCustomersToEditPosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToDeletePosts, model.AllowCustomersToDeletePosts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowPostVoting, model.AllowPostVoting_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.MaxVotesPerDay, model.MaxVotesPerDay_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowCustomersToManageSubscriptions, model.AllowCustomersToManageSubscriptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.TopicsPageSize, model.TopicsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumEditor, model.ForumEditor_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.SignaturesEnabled, model.SignaturesEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.AllowPrivateMessages, model.AllowPrivateMessages_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ShowAlertForPM, model.ShowAlertForPM_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.NotifyAboutPrivateMessages, model.NotifyAboutPrivateMessages_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsFeedEnabled, model.ActiveDiscussionsFeedEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsFeedCount, model.ActiveDiscussionsFeedCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumFeedsEnabled, model.ForumFeedsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ForumFeedCount, model.ForumFeedCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.SearchResultsPageSize, model.SearchResultsPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(forumSettings, x => x.ActiveDiscussionsPageSize, model.ActiveDiscussionsPageSize_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Forum");
        }




        public virtual ActionResult News()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
            var model = newsSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.Enabled, storeScope);
                model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeScope);
                model.NotifyAboutNewNewsComments_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.NotifyAboutNewNewsComments, storeScope);
                model.ShowNewsOnMainPage_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.ShowNewsOnMainPage, storeScope);
                model.MainPageNewsCount_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.MainPageNewsCount, storeScope);
                model.NewsArchivePageSize_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.NewsArchivePageSize, storeScope);
                model.ShowHeaderRssUrl_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.ShowHeaderRssUrl, storeScope);
                model.NewsCommentsMustBeApproved_OverrideForStore = _settingService.SettingExists(newsSettings, x => x.NewsCommentsMustBeApproved, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult News(NewsSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
            newsSettings = model.ToEntity(newsSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.NotifyAboutNewNewsComments, model.NotifyAboutNewNewsComments_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.ShowNewsOnMainPage, model.ShowNewsOnMainPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.MainPageNewsCount, model.MainPageNewsCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.NewsArchivePageSize, model.NewsArchivePageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(newsSettings, x => x.NewsCommentsMustBeApproved, model.NewsCommentsMustBeApproved_OverrideForStore, storeScope, false);
            _settingService.SaveSetting(newsSettings, x => x.ShowNewsCommentsPerStore, clearCache: false);

            //now clear settings cache
            _settingService.ClearCache();


            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("News");
        }



 

        public virtual ActionResult Tax()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var taxSettings = _settingService.LoadSetting<TaxSettings>(storeScope);
            var model = taxSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.PricesIncludeTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PricesIncludeTax, storeScope);
                model.AllowCustomersToSelectTaxDisplayType_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeScope);
                model.TaxDisplayType_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.TaxDisplayType, storeScope);
                model.DisplayTaxSuffix_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DisplayTaxSuffix, storeScope);
                model.DisplayTaxRates_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DisplayTaxRates, storeScope);
                model.HideZeroTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.HideZeroTax, storeScope);
                model.HideTaxInSubscriptionSummary_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.HideTaxInSubscriptionSummary, storeScope);
                model.ForceTaxExclusionFromSubscriptionSubtotal_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ForceTaxExclusionFromSubscriptionSubtotal, storeScope);
                model.DefaultTaxCategoryId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DefaultTaxCategoryId, storeScope);
                model.TaxBasedOn_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.TaxBasedOn, storeScope);
                model.TaxBasedOnPickupPointAddress_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeScope);
                model.DefaultTaxAddress_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.DefaultTaxAddressId, storeScope);
                model.ShippingIsTaxable_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingIsTaxable, storeScope);
                model.ShippingPriceIncludesTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingPriceIncludesTax, storeScope);
                model.ShippingTaxClassId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.ShippingTaxClassId, storeScope);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeScope);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeScope);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeScope);
                model.EuVatEnabled_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatEnabled, storeScope);
                model.EuVatShopCountryId_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatShopCountryId, storeScope);
                model.EuVatAllowVatExemption_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatAllowVatExemption, storeScope);
                model.EuVatUseWebService_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatUseWebService, storeScope);
                model.EuVatAssumeValid_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatAssumeValid, storeScope);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = _settingService.SettingExists(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeScope);
            }

            model.TaxBasedOnValues = taxSettings.TaxBasedOn.ToSelectList();
            model.TaxDisplayTypeValues = taxSettings.TaxDisplayType.ToSelectList();

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.TaxCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"), Value = "0" });
            foreach (var tc in taxCategories)
                model.TaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
            model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == taxSettings.PaymentMethodAdditionalFeeTaxClassId });

            //EU VAT countries
            model.EuVatShopCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.EuVatShopCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id == taxSettings.EuVatShopCountryId });

            //default tax address
            var defaultAddress = taxSettings.DefaultTaxAddressId > 0
                                     ? _addressService.GetAddressById(taxSettings.DefaultTaxAddressId)
                                     : null;
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel();
            else
                model.DefaultTaxAddress = new AddressModel();

            model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (defaultAddress != null && c.Id == defaultAddress.CountryId) });

            var states = defaultAddress != null && defaultAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(defaultAddress.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == defaultAddress.StateProvinceId) });
            }
            else
                model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.DefaultTaxAddress.CountryEnabled = true;
            model.DefaultTaxAddress.StateProvinceEnabled = true;
            model.DefaultTaxAddress.ZipPostalCodeEnabled = true;
            model.DefaultTaxAddress.ZipPostalCodeRequired = true;

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Tax(TaxSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var taxSettings = _settingService.LoadSetting<TaxSettings>(storeScope);
            taxSettings = model.ToEntity(taxSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PricesIncludeTax, model.PricesIncludeTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, model.AllowCustomersToSelectTaxDisplayType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.TaxDisplayType, model.TaxDisplayType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.DisplayTaxSuffix, model.DisplayTaxSuffix_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.DisplayTaxRates, model.DisplayTaxRates_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.HideZeroTax, model.HideZeroTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.HideTaxInSubscriptionSummary, model.HideTaxInSubscriptionSummary_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ForceTaxExclusionFromSubscriptionSubtotal, model.ForceTaxExclusionFromSubscriptionSubtotal_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.DefaultTaxCategoryId, model.DefaultTaxCategoryId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.TaxBasedOn, model.TaxBasedOn_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.TaxBasedOnPickupPointAddress, model.TaxBasedOnPickupPointAddress_OverrideForStore, storeScope, false);


            if (model.DefaultTaxAddress_OverrideForStore || storeScope == 0)
            {
                //update address
                var addressId = _settingService.SettingExists(taxSettings, x => x.DefaultTaxAddressId, storeScope) ?
                    taxSettings.DefaultTaxAddressId : 0;
                var originAddress = _addressService.GetAddressById(addressId) ??
                    new Address
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                //update ID manually (in case we're in multi-store configuration mode it'll be set to the shared one)
                model.DefaultTaxAddress.Id = addressId;
                originAddress = model.DefaultTaxAddress.ToEntity(originAddress);
                if (originAddress.Id > 0)
                    _addressService.UpdateAddress(originAddress);
                else
                    _addressService.InsertAddress(originAddress);
                taxSettings.DefaultTaxAddressId = originAddress.Id;

                _settingService.SaveSetting(taxSettings, x => x.DefaultTaxAddressId, storeScope, false);
            }
            else if (storeScope > 0)
                _settingService.DeleteSetting(taxSettings, x => x.DefaultTaxAddressId, storeScope);

            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingIsTaxable, model.ShippingIsTaxable_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingPriceIncludesTax, model.ShippingPriceIncludesTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.ShippingTaxClassId, model.ShippingTaxClassId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatEnabled, model.EuVatEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatShopCountryId, model.EuVatShopCountryId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatAllowVatExemption, model.EuVatAllowVatExemption_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatUseWebService, model.EuVatUseWebService_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatAssumeValid, model.EuVatAssumeValid_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Tax");
        }




        public virtual ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            var model = catalogSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllowViewUnpublishedArticlePage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowViewUnpublishedArticlePage, storeScope);
                model.DisplayDiscontinuedMessageForUnpublishedArticles_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedArticles, storeScope);
                model.ShowSkuOnArticleDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnArticleDetailsPage, storeScope);
                model.ShowSkuOnCatalogPages_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnCatalogPages, storeScope);
                model.ShowPublisherPartNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowPublisherPartNumber, storeScope);
                model.ShowGtin_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowGtin, storeScope);
                model.ShowFreeShippingNotification_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowFreeShippingNotification, storeScope);
                model.AllowArticleSorting_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowArticleSorting, storeScope);
                model.AllowArticleViewModeChanging_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowArticleViewModeChanging, storeScope);
                model.ShowArticlesFromSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowArticlesFromSubcategories, storeScope);
                model.ShowCategoryArticleNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryArticleNumber, storeScope);
                model.ShowCategoryArticleNumberIncludingSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryArticleNumberIncludingSubcategories, storeScope);
                model.CategoryBreadcrumbEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeScope);
                model.ShowShareButton_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowShareButton, storeScope);
                model.PageShareCode_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PageShareCode, storeScope);
                model.ArticleReviewsMustBeApproved_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticleReviewsMustBeApproved, storeScope);
                model.AllowAnonymousUsersToReviewArticle_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToReviewArticle, storeScope);
                model.ArticleReviewPossibleOnlyAfterPurchasing_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticleReviewPossibleOnlyAfterPurchasing, storeScope);
                model.NotifyStoreOwnerAboutNewArticleReviews_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NotifyStoreOwnerAboutNewArticleReviews, storeScope);
                model.EmailAFriendEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.EmailAFriendEnabled, storeScope);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeScope);
                model.RecentlyViewedArticlesNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedArticlesNumber, storeScope);
                model.RecentlyViewedArticlesEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedArticlesEnabled, storeScope);
                model.NewArticlesNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewArticlesNumber, storeScope);
                model.NewArticlesEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewArticlesEnabled, storeScope);
                model.CompareArticlesEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CompareArticlesEnabled, storeScope);
                model.ShowBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowBestsellersOnHomepage, storeScope);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeScope);
                model.SearchPageArticlesPerPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageArticlesPerPage, storeScope);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeScope);
                model.SearchPagePageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPagePageSizeOptions, storeScope);
                model.ArticleSearchAutoCompleteEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticleSearchAutoCompleteEnabled, storeScope);
                model.ArticleSearchAutoCompleteNumberOfArticles_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticleSearchAutoCompleteNumberOfArticles, storeScope);
                model.ShowArticleImagesInSearchAutoComplete_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowArticleImagesInSearchAutoComplete, storeScope);
                model.ArticleSearchTermMinimumLength_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticleSearchTermMinimumLength, storeScope);
                model.ArticlesAlsoPurchasedEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticlesAlsoPurchasedEnabled, storeScope);
                model.ArticlesAlsoPurchasedNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticlesAlsoPurchasedNumber, storeScope);
                model.NumberOfArticleTags_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfArticleTags, storeScope);
                model.ArticlesByTagPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticlesByTagPageSize, storeScope);
                model.ArticlesByTagAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticlesByTagAllowCustomersToSelectPageSize, storeScope);
                model.ArticlesByTagPageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ArticlesByTagPageSizeOptions, storeScope);
                model.IncludeShortDescriptionInCompareArticles_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeShortDescriptionInCompareArticles, storeScope);
                model.IncludeFullDescriptionInCompareArticles_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeFullDescriptionInCompareArticles, storeScope);
                model.PublishersBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PublishersBlockItemsToDisplay, storeScope);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope);
                model.DisplayTaxShippingInfoArticleDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoArticleDetailsPage, storeScope);
                model.DisplayTaxShippingInfoArticleBoxes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoArticleBoxes, storeScope);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope);
                model.DisplayTaxShippingInfoSubscriptionDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoSubscriptionDetailsPage, storeScope);
                model.ShowArticleReviewsPerStore_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowArticleReviewsPerStore, storeScope);
                model.ShowArticleReviewsOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowArticleReviewsTabOnAccountPage, storeScope);
                model.ArticleReviewsPageSizeOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x=> x.ArticleReviewsPageSizeOnAccountPage, storeScope);
                model.ExportImportArticleAttributes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ExportImportArticleAttributes, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            catalogSettings = model.ToEntity(catalogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowViewUnpublishedArticlePage, model.AllowViewUnpublishedArticlePage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedArticles, model.DisplayDiscontinuedMessageForUnpublishedArticles_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowSkuOnArticleDetailsPage, model.ShowSkuOnArticleDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowSkuOnCatalogPages, model.ShowSkuOnCatalogPages_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowPublisherPartNumber, model.ShowPublisherPartNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowGtin, model.ShowGtin_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowFreeShippingNotification, model.ShowFreeShippingNotification_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowArticleSorting, model.AllowArticleSorting_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowArticleViewModeChanging, model.AllowArticleViewModeChanging_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowArticlesFromSubcategories, model.ShowArticlesFromSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryArticleNumber, model.ShowCategoryArticleNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryArticleNumberIncludingSubcategories, model.ShowCategoryArticleNumberIncludingSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CategoryBreadcrumbEnabled, model.CategoryBreadcrumbEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowShareButton, model.ShowShareButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.PageShareCode, model.PageShareCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleReviewsMustBeApproved, model.ArticleReviewsMustBeApproved_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToReviewArticle, model.AllowAnonymousUsersToReviewArticle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleReviewPossibleOnlyAfterPurchasing, model.ArticleReviewPossibleOnlyAfterPurchasing_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NotifyStoreOwnerAboutNewArticleReviews, model.NotifyStoreOwnerAboutNewArticleReviews_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.EmailAFriendEnabled, model.EmailAFriendEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedArticlesNumber, model.RecentlyViewedArticlesNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedArticlesEnabled, model.RecentlyViewedArticlesEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewArticlesNumber, model.NewArticlesNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewArticlesEnabled, model.NewArticlesEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CompareArticlesEnabled, model.CompareArticlesEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowBestsellersOnHomepage, model.ShowBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfBestsellersOnHomepage, model.NumberOfBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageArticlesPerPage, model.SearchPageArticlesPerPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPagePageSizeOptions, model.SearchPagePageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleSearchAutoCompleteEnabled, model.ArticleSearchAutoCompleteEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleSearchAutoCompleteNumberOfArticles, model.ArticleSearchAutoCompleteNumberOfArticles_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowArticleImagesInSearchAutoComplete, model.ShowArticleImagesInSearchAutoComplete_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleSearchTermMinimumLength, model.ArticleSearchTermMinimumLength_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticlesAlsoPurchasedEnabled, model.ArticlesAlsoPurchasedEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticlesAlsoPurchasedNumber, model.ArticlesAlsoPurchasedNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfArticleTags, model.NumberOfArticleTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticlesByTagPageSize, model.ArticlesByTagPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticlesByTagAllowCustomersToSelectPageSize, model.ArticlesByTagAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticlesByTagPageSizeOptions, model.ArticlesByTagPageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeShortDescriptionInCompareArticles, model.IncludeShortDescriptionInCompareArticles_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeFullDescriptionInCompareArticles, model.IncludeFullDescriptionInCompareArticles_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.PublishersBlockItemsToDisplay, model.PublishersBlockItemsToDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoFooter, model.DisplayTaxShippingInfoFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoArticleDetailsPage, model.DisplayTaxShippingInfoArticleDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoArticleBoxes, model.DisplayTaxShippingInfoArticleBoxes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, model.DisplayTaxShippingInfoWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoSubscriptionDetailsPage, model.DisplayTaxShippingInfoSubscriptionDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowArticleReviewsPerStore, model.ShowArticleReviewsPerStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowArticleReviewsTabOnAccountPage, model.ShowArticleReviewsOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ArticleReviewsPageSizeOnAccountPage, model.ArticleReviewsPageSizeOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ExportImportArticleAttributes, model.ExportImportArticleAttributes_OverrideForStore, storeScope, false);
            
            //now settings not overridable per store
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreFeaturedArticles, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreAcl, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreStoreLimitations, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.CacheArticlePrices, 0, false);


            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("Catalog");
        }

        #region Sort options

        [HttpPost]
        public virtual ActionResult SortOptionsList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var catalogSettings = _settingService.LoadSetting<CatalogSettings>();
            var model = new List<SortOptionModel>();
            foreach (int option in Enum.GetValues(typeof(ArticleSortingEnum)))
            {
                int value;
                model.Add(new SortOptionModel()
                {
                    Id = option,
                    Name = ((ArticleSortingEnum)option).GetLocalizedEnum(_localizationService, _workContext),
                    IsActive = !catalogSettings.ArticleSortingEnumDisabled.Contains(option),
                    DisplaySubscription = catalogSettings.ArticleSortingEnumDisplaySubscription.TryGetValue(option, out value) ? value : option
                });
            }
            var gridModel = new DataSourceResult
            {
                Data = model.OrderBy(option => option.DisplaySubscription),
                Total = model.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult SortOptionUpdate(SortOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);

            catalogSettings.ArticleSortingEnumDisplaySubscription[model.Id] = model.DisplaySubscription;
            if (model.IsActive && catalogSettings.ArticleSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ArticleSortingEnumDisabled.Remove(model.Id);
            if (!model.IsActive && !catalogSettings.ArticleSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ArticleSortingEnumDisabled.Add(model.Id);


            _settingService.SaveSetting(catalogSettings);

            return new NullJsonResult();
        }

        #endregion

        public virtual ActionResult RewardPoints()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var rewardPointsSettings = _settingService.LoadSetting<RewardPointsSettings>(storeScope);
            var model = rewardPointsSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.Enabled, storeScope);
                model.ExchangeRate_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.ExchangeRate, storeScope);
                model.MinimumRewardPointsToUse_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeScope);
                model.PointsForRegistration_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
                model.PointsForPurchases_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeScope) ||
                    _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Points, storeScope);
                model.ActivationDelay_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.ActivationDelay, storeScope);
                model.DisplayHowMuchWillBeEarned_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeScope);
                model.PointsForRegistration_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeScope);
                model.PageSize_OverrideForStore = _settingService.SettingExists(rewardPointsSettings, x => x.PageSize, storeScope);
            }
            var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope); 
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            model.ActivatePointsImmediately = model.ActivationDelay <= 0;

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult RewardPoints(RewardPointsSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var rewardPointsSettings = _settingService.LoadSetting<RewardPointsSettings>(storeScope);
                rewardPointsSettings = model.ToEntity(rewardPointsSettings);

                if (model.ActivatePointsImmediately)
                    rewardPointsSettings.ActivationDelay = 0;

                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.ExchangeRate, model.ExchangeRate_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.MinimumRewardPointsToUse, model.MinimumRewardPointsToUse_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForRegistration, model.PointsForRegistration_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Amount, model.PointsForPurchases_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PointsForPurchases_Points, model.PointsForPurchases_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.ActivationDelay, model.ActivationDelay_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.ActivationDelayPeriodId, model.ActivationDelay_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, model.DisplayHowMuchWillBeEarned_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(rewardPointsSettings, x => x.PageSize, model.PageSize_OverrideForStore, storeScope, false);
                _settingService.SaveSetting(rewardPointsSettings, x => x.PointsAccumulatedForAllStores, 0, false);

                //now clear settings cache
                _settingService.ClearCache();

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            }
            else
            {
                //If we got this far, something failed, redisplay form
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        ErrorNotification(error.ErrorMessage);
            }
            return RedirectToAction("RewardPoints");
        }




        public virtual ActionResult Subscription()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();



            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var subscriptionSettings = _settingService.LoadSetting<SubscriptionSettings>(storeScope);
            var model = subscriptionSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.IsReSubscriptionAllowed_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.IsReSubscriptionAllowed, storeScope);
                model.MinSubscriptionSubtotalAmount_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.MinSubscriptionSubtotalAmount, storeScope);
                model.MinSubscriptionSubtotalAmountIncludingTax_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.MinSubscriptionSubtotalAmountIncludingTax, storeScope);
                model.MinSubscriptionTotalAmount_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.MinSubscriptionTotalAmount, storeScope);
                model.AutoUpdateSubscriptionTotalsOnEditingSubscription_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.AutoUpdateSubscriptionTotalsOnEditingSubscription, storeScope);
                model.AnonymousCheckoutAllowed_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.AnonymousCheckoutAllowed, storeScope);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.TermsOfServiceOnShoppingCartPage, storeScope);
                model.TermsOfServiceOnSubscriptionConfirmPage_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.TermsOfServiceOnSubscriptionConfirmPage, storeScope);
                model.OnePageCheckoutEnabled_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.OnePageCheckoutEnabled, storeScope);
                model.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab, storeScope);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.DisableBillingAddressCheckoutStep, storeScope);
                model.DisableSubscriptionCompletedPage_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.DisableSubscriptionCompletedPage, storeScope);
                model.AttachPdfInvoiceToSubscriptionPlacedEmail_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionPlacedEmail, storeScope);
                model.AttachPdfInvoiceToSubscriptionPaidEmail_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionPaidEmail, storeScope);
                model.AttachPdfInvoiceToSubscriptionCompletedEmail_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionCompletedEmail, storeScope);
                model.ReturnRequestsEnabled_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.ReturnRequestsEnabled, storeScope);
                model.ReturnRequestsAllowFiles_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.ReturnRequestsAllowFiles, storeScope);
                model.ReturnRequestNumberMask_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.ReturnRequestNumberMask, storeScope);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.NumberOfDaysReturnRequestAvailable, storeScope);
                model.CustomSubscriptionNumberMask_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.CustomSubscriptionNumberMask, storeScope);
                model.ExportWithArticles_OverrideForStore = _settingService.SettingExists(subscriptionSettings, x => x.ExportWithArticles, storeScope);
            }

            var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //subscription ident
            model.SubscriptionIdent = _maintenanceService.GetTableIdent<Subscription>();

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult Subscription(SubscriptionSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var subscriptionSettings = _settingService.LoadSetting<SubscriptionSettings>(storeScope);
                subscriptionSettings = model.ToEntity(subscriptionSettings);

                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.IsReSubscriptionAllowed, model.IsReSubscriptionAllowed_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.MinSubscriptionSubtotalAmount, model.MinSubscriptionSubtotalAmount_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.MinSubscriptionSubtotalAmountIncludingTax, model.MinSubscriptionSubtotalAmountIncludingTax_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.MinSubscriptionTotalAmount, model.MinSubscriptionTotalAmount_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.AutoUpdateSubscriptionTotalsOnEditingSubscription, model.AutoUpdateSubscriptionTotalsOnEditingSubscription_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.AnonymousCheckoutAllowed, model.AnonymousCheckoutAllowed_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.TermsOfServiceOnShoppingCartPage, model.TermsOfServiceOnShoppingCartPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.TermsOfServiceOnSubscriptionConfirmPage, model.TermsOfServiceOnSubscriptionConfirmPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.OnePageCheckoutEnabled, model.OnePageCheckoutEnabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab, model.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.DisableBillingAddressCheckoutStep, model.DisableBillingAddressCheckoutStep_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.DisableSubscriptionCompletedPage, model.DisableSubscriptionCompletedPage_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionPlacedEmail, model.AttachPdfInvoiceToSubscriptionPlacedEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionPaidEmail, model.AttachPdfInvoiceToSubscriptionPaidEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.AttachPdfInvoiceToSubscriptionCompletedEmail, model.AttachPdfInvoiceToSubscriptionCompletedEmail_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.ReturnRequestsEnabled, model.ReturnRequestsEnabled_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.ReturnRequestsAllowFiles, model.ReturnRequestsAllowFiles_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.ReturnRequestNumberMask, model.ReturnRequestNumberMask_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.NumberOfDaysReturnRequestAvailable, model.NumberOfDaysReturnRequestAvailable_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.CustomSubscriptionNumberMask, model.CustomSubscriptionNumberMask_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(subscriptionSettings, x => x.ExportWithArticles, model.ExportWithArticles_OverrideForStore, storeScope, false);
                _settingService.SaveSetting(subscriptionSettings, x => x.ActivateGiftCardsAfterCompletingSubscription, 0, false);
                _settingService.SaveSetting(subscriptionSettings, x => x.DeactivateGiftCardsAfterCancellingSubscription, 0, false);
                _settingService.SaveSetting(subscriptionSettings, x => x.DeactivateGiftCardsAfterDeletingSubscription, 0, false);
                _settingService.SaveSetting(subscriptionSettings, x => x.CompleteSubscriptionWhenDelivered, 0, false);
               
                //now clear settings cache
                _settingService.ClearCache();
                
                //subscription ident
                if (model.SubscriptionIdent.HasValue)
                {
                    try
                    {
                        _maintenanceService.SetTableIdent<Subscription>(model.SubscriptionIdent.Value);
                    }
                    catch (Exception exc)
                    {
                        ErrorNotification(exc.Message);
                    }
                }

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            }
            else
            {
                //If we got this far, something failed, redisplay form
                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        ErrorNotification(error.ErrorMessage);
            }

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("Subscription");
        }

        #region Return request reasons

        public virtual ActionResult ReturnRequestReasonList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the subscription settings page

            //select "return request" tab
            SaveSelectedTabName("tab-returnrequest");
            return RedirectToAction("Subscription", "Setting");
        }
        [HttpPost]
        public virtual ActionResult ReturnRequestReasonList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var reasons = _returnRequestService.GetAllReturnRequestReasons();
            var gridModel = new DataSourceResult
            {
                Data = reasons.Select(x => x.ToModel()),
                Total = reasons.Count
            };
            return Json(gridModel);
        }
        //create
        public virtual ActionResult ReturnRequestReasonCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new ReturnRequestReasonModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var rrr = model.ToEntity();
                _returnRequestService.InsertReturnRequestReason(rrr);
                //locales
                UpdateLocales(rrr, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.Added"));
                return continueEditing ? RedirectToAction("ReturnRequestReasonEdit", new { id = rrr.Id }) : RedirectToAction("ReturnRequestReasonList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public virtual ActionResult ReturnRequestReasonEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(id);
            if (rrr == null)
                //No reason found with the specified id
                return RedirectToAction("ReturnRequestReasonList");

            var model = rrr.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = rrr.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(model.Id);
            if (rrr == null)
                //No reason found with the specified id
                return RedirectToAction("ReturnRequestReasonList");

            if (ModelState.IsValid)
            {
                rrr = model.ToEntity(rrr);
                _returnRequestService.UpdateReturnRequestReason(rrr);
                //locales
                UpdateLocales(rrr, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ReturnRequestReasonEdit", new { id = rrr.Id });
                }
                return RedirectToAction("ReturnRequestReasonList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //delete
        [HttpPost]
        public virtual ActionResult ReturnRequestReasonDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rrr = _returnRequestService.GetReturnRequestReasonById(id);
            _returnRequestService.DeleteReturnRequestReason(rrr);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.Deleted"));
            return RedirectToAction("ReturnRequestReasonList");
        }

        #endregion

        #region Return request actions

        public virtual ActionResult ReturnRequestActionList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the subscription settings page

            //select "return request" tab
            SaveSelectedTabName("tab-returnrequest");
            return RedirectToAction("Subscription", "Setting");
        }
        [HttpPost]
        public virtual ActionResult ReturnRequestActionList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var actions = _returnRequestService.GetAllReturnRequestActions();
            var gridModel = new DataSourceResult
            {
                Data = actions.Select(x => x.ToModel()),
                Total = actions.Count
            };
            return Json(gridModel);
        }
        //create
        public virtual ActionResult ReturnRequestActionCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new ReturnRequestActionModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var rra = model.ToEntity();
                _returnRequestService.InsertReturnRequestAction(rra);
                //locales
                UpdateLocales(rra, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestActions.Added"));
                return continueEditing ? RedirectToAction("ReturnRequestActionEdit", new { id = rra.Id }) : RedirectToAction("ReturnRequestActionList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //edit
        public virtual ActionResult ReturnRequestActionEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(id);
            if (rra == null)
                //No action found with the specified id
                return RedirectToAction("ReturnRequestActionList");

            var model = rra.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = rra.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(model.Id);
            if (rra == null)
                //No action found with the specified id
                return RedirectToAction("ReturnRequestActionList");

            if (ModelState.IsValid)
            {
                rra = model.ToEntity(rra);
                _returnRequestService.UpdateReturnRequestAction(rra);
                //locales
                UpdateLocales(rra, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestActions.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ReturnRequestActionEdit", new { id = rra.Id });
                }
                return RedirectToAction("ReturnRequestActionList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        //delete
        [HttpPost]
        public virtual ActionResult ReturnRequestActionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var rra = _returnRequestService.GetReturnRequestActionById(id);
            _returnRequestService.DeleteReturnRequestAction(rra);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Subscription.ReturnRequestActions.Deleted"));
            return RedirectToAction("ReturnRequestActionList");
        }

        #endregion





        public virtual ActionResult ShoppingCart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shoppingCartSettings = _settingService.LoadSetting<ShoppingCartSettings>(storeScope);
            var model = shoppingCartSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DisplayCartAfterAddingArticle_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.DisplayCartAfterAddingArticle, storeScope);
                model.DisplayWishlistAfterAddingArticle_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.DisplayWishlistAfterAddingArticle, storeScope);
                model.MaximumShoppingCartItems_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeScope);
                model.MaximumWishlistItems_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MaximumWishlistItems, storeScope);
                model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeScope);
                model.MoveItemsFromWishlistToCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeScope);
                model.CartsSharedBetweenStores_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeScope);
                model.ShowArticleImagesOnShoppingCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowArticleImagesOnShoppingCart, storeScope);
                model.ShowArticleImagesOnWishList_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowArticleImagesOnWishList, storeScope);
                model.ShowDiscountBox_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowDiscountBox, storeScope);
                model.ShowGiftCardBox_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowGiftCardBox, storeScope);
                model.CrossSellsNumber_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.CrossSellsNumber, storeScope);
                model.EmailWishlistEnabled_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.EmailWishlistEnabled, storeScope);
                model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeScope);
                model.MiniShoppingCartEnabled_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeScope);
                model.ShowArticleImagesInMiniShoppingCart_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.ShowArticleImagesInMiniShoppingCart, storeScope);
                model.MiniShoppingCartArticleNumber_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartArticleNumber, storeScope);
                model.AllowCartItemEditing_OverrideForStore = _settingService.SettingExists(shoppingCartSettings, x => x.AllowCartItemEditing, storeScope);
            }
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult ShoppingCart(ShoppingCartSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var shoppingCartSettings = _settingService.LoadSetting<ShoppingCartSettings>(storeScope);
            shoppingCartSettings = model.ToEntity(shoppingCartSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.DisplayCartAfterAddingArticle, model.DisplayCartAfterAddingArticle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.DisplayWishlistAfterAddingArticle, model.DisplayWishlistAfterAddingArticle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MaximumShoppingCartItems, model.MaximumShoppingCartItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MaximumWishlistItems, model.MaximumWishlistItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, model.MoveItemsFromWishlistToCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.CartsSharedBetweenStores, model.CartsSharedBetweenStores_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowArticleImagesOnShoppingCart, model.ShowArticleImagesOnShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowArticleImagesOnWishList, model.ShowArticleImagesOnWishList_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowDiscountBox, model.ShowDiscountBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowGiftCardBox, model.ShowGiftCardBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.CrossSellsNumber, model.CrossSellsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.EmailWishlistEnabled, model.EmailWishlistEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MiniShoppingCartEnabled, model.MiniShoppingCartEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.ShowArticleImagesInMiniShoppingCart, model.ShowArticleImagesInMiniShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.MiniShoppingCartArticleNumber, model.MiniShoppingCartArticleNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shoppingCartSettings, x => x.AllowCartItemEditing, model.AllowCartItemEditing_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();
            

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("ShoppingCart");
        }




        public virtual ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            var model = mediaSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AvatarPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, storeScope);
                model.ArticleThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ArticleThumbPictureSize, storeScope);
                model.ArticleDetailsPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ArticleDetailsPictureSize, storeScope);
                model.ArticleThumbPictureSizeOnArticleDetailsPage_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ArticleThumbPictureSizeOnArticleDetailsPage, storeScope);
                model.AssociatedArticlePictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AssociatedArticlePictureSize, storeScope);
                model.CategoryThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CategoryThumbPictureSize, storeScope);
                model.PublisherThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.PublisherThumbPictureSize, storeScope);
                model.ContributorThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ContributorThumbPictureSize, storeScope);
                model.CartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CartThumbPictureSize, storeScope);
                model.MiniCartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope);
                model.MaximumImageSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, storeScope);
                model.MultipleThumbDirectories_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MultipleThumbDirectories, storeScope);
                model.DefaultImageQuality_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, storeScope);
                model.ImportArticleImagesUsingHash_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ImportArticleImagesUsingHash, storeScope);
                model.DefaultPictureZoomEnabled_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultPictureZoomEnabled, storeScope);
            }
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            mediaSettings = model.ToEntity(mediaSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ArticleThumbPictureSize, model.ArticleThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ArticleDetailsPictureSize, model.ArticleDetailsPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ArticleThumbPictureSizeOnArticleDetailsPage, model.ArticleThumbPictureSizeOnArticleDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AssociatedArticlePictureSize, model.AssociatedArticlePictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.PublisherThumbPictureSize, model.PublisherThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ContributorThumbPictureSize, model.ContributorThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ImportArticleImagesUsingHash, model.ImportArticleImagesUsingHash_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultPictureZoomEnabled, model.DefaultPictureZoomEnabled_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            
            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public virtual ActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }



        public virtual ActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            //merge settings
            var model = new CustomerUserSettingsModel();
            model.CustomerSettings = customerSettings.ToModel();
            model.AddressSettings = addressSettings.ToModel();

            model.DateTimeSettings.AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            customerSettings = model.CustomerSettings.ToEntity(customerSettings);
            _settingService.SaveSetting(customerSettings);

            addressSettings = model.AddressSettings.ToEntity(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("CustomerUser");
        }






        public virtual ActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            model.ActiveStoreScopeConfiguration = storeScope;
            //store information
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            model.StoreInformationSettings.StoreClosed = storeInformationSettings.StoreClosed;
            //themes
            model.StoreInformationSettings.DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme;
            model.StoreInformationSettings.AvailableStoreThemes = _themeProvider
                .GetThemeConfigurations()
                .Select(x => new GeneralCommonSettingsModel.StoreInformationSettingsModel.ThemeConfigurationModel
                {
                    ThemeTitle = x.ThemeTitle,
                    ThemeName = x.ThemeName,
                    PreviewImageUrl = x.PreviewImageUrl,
                    PreviewText = x.PreviewText,
                    SupportRtl = x.SupportRtl,
                    Selected = x.ThemeName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                })
                .ToList();
            model.StoreInformationSettings.AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme;
            model.StoreInformationSettings.LogoPictureId = storeInformationSettings.LogoPictureId;
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            model.StoreInformationSettings.FacebookLink = storeInformationSettings.FacebookLink;
            model.StoreInformationSettings.TwitterLink = storeInformationSettings.TwitterLink;
            model.StoreInformationSettings.YoutubeLink = storeInformationSettings.YoutubeLink;
            model.StoreInformationSettings.GooglePlusLink = storeInformationSettings.GooglePlusLink;
            //contact us
            model.StoreInformationSettings.SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm;
            model.StoreInformationSettings.UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm;
            //sitemap
            model.StoreInformationSettings.SitemapEnabled = commonSettings.SitemapEnabled;
            model.StoreInformationSettings.SitemapIncludeCategories = commonSettings.SitemapIncludeCategories;
            model.StoreInformationSettings.SitemapIncludePublishers = commonSettings.SitemapIncludePublishers;
            model.StoreInformationSettings.SitemapIncludeArticles = commonSettings.SitemapIncludeArticles;

            //override settings
            if (storeScope > 0)
            {
                model.StoreInformationSettings.StoreClosed_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeScope);
                model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);
                model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope);
                model.StoreInformationSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.LogoPictureId, storeScope);
                model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);
                model.StoreInformationSettings.FacebookLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeScope);
                model.StoreInformationSettings.TwitterLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeScope);
                model.StoreInformationSettings.YoutubeLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeScope);
                model.StoreInformationSettings.GooglePlusLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.GooglePlusLink, storeScope);
                model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
                model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
                model.StoreInformationSettings.SitemapEnabled_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapEnabled, storeScope);
                model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeCategories, storeScope);
                model.StoreInformationSettings.SitemapIncludePublishers_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludePublishers, storeScope);
                model.StoreInformationSettings.SitemapIncludeArticles_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeArticles, storeScope);
            }

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList();
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.GenerateArticleMetaDescription = seoSettings.GenerateArticleMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.WwwRequirement = (int)seoSettings.WwwRequirement;
            model.SeoSettings.WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList();
            model.SeoSettings.EnableJsBundling = seoSettings.EnableJsBundling;
            model.SeoSettings.EnableCssBundling = seoSettings.EnableCssBundling;
            model.SeoSettings.TwitterMetaTags = seoSettings.TwitterMetaTags;
            model.SeoSettings.OpenGraphMetaTags = seoSettings.OpenGraphMetaTags;
            model.SeoSettings.CustomHeadTags = seoSettings.CustomHeadTags;
            //override settings
            if (storeScope > 0)
            {
                model.SeoSettings.PageTitleSeparator_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator, storeScope);
                model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment, storeScope);
                model.SeoSettings.DefaultTitle_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultTitle, storeScope);
                model.SeoSettings.DefaultMetaKeywords_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords, storeScope);
                model.SeoSettings.DefaultMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription, storeScope);
                model.SeoSettings.GenerateArticleMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.GenerateArticleMetaDescription, storeScope);
                model.SeoSettings.ConvertNonWesternChars_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars, storeScope);
                model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled, storeScope);
                model.SeoSettings.WwwRequirement_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.WwwRequirement, storeScope);
                model.SeoSettings.EnableJsBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableJsBundling, storeScope);
                model.SeoSettings.EnableCssBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableCssBundling, storeScope);
                model.SeoSettings.TwitterMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags, storeScope);
                model.SeoSettings.OpenGraphMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags, storeScope);
                model.SeoSettings.CustomHeadTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CustomHeadTags, storeScope);
            }
            
            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.ForceSslForAllPages = securitySettings.ForceSslForAllPages;
            model.SecuritySettings.EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea;
            model.SecuritySettings.EnableXsrfProtectionForPublicStore = securitySettings.EnableXsrfProtectionForPublicStore;
            model.SecuritySettings.HoneypotEnabled = securitySettings.HoneypotEnabled;

            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            model.CaptchaSettings = captchaSettings.ToModel();
            model.CaptchaSettings.AvailableReCaptchaVersions = ReCaptchaVersion.Version1.ToSelectList(false).ToList();

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            model.PdfSettings.LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled;
            model.PdfSettings.LogoPictureId = pdfSettings.LogoPictureId;
            model.PdfSettings.DisablePdfInvoicesForPendingSubscriptions = pdfSettings.DisablePdfInvoicesForPendingSubscriptions;
            model.PdfSettings.InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1;
            model.PdfSettings.InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2;
            //override settings
            if (storeScope > 0)
            {
                model.PdfSettings.LetterPageSizeEnabled_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LetterPageSizeEnabled, storeScope);
                model.PdfSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LogoPictureId, storeScope);
                model.PdfSettings.DisablePdfInvoicesForPendingSubscriptions_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.DisablePdfInvoicesForPendingSubscriptions, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn1, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn2, storeScope);
            }

            //localization
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            model.LocalizationSettings.AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage;
            model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup;
            model.LocalizationSettings.LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup;

            //full-text support
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchMode = (int)commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();
            
            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem = displayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayNewArticlesMenuItem = displayDefaultMenuItemSettings.DisplayNewArticlesMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayArticleSearchMenuItem = displayDefaultMenuItemSettings.DisplayArticleSearchMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem = displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayNewArticlesMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayNewArticlesMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayArticleSearchMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayArticleSearchMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeScope);


            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //store information settings
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.GooglePlusLink = model.StoreInformationSettings.GooglePlusLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
            //sitemap
            commonSettings.SitemapEnabled = model.StoreInformationSettings.SitemapEnabled;
            commonSettings.SitemapIncludeCategories = model.StoreInformationSettings.SitemapIncludeCategories;
            commonSettings.SitemapIncludePublishers = model.StoreInformationSettings.SitemapIncludePublishers;
            commonSettings.SitemapIncludeArticles = model.StoreInformationSettings.SitemapIncludeArticles;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.AllowCustomerToSelectTheme, model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.GooglePlusLink, model.StoreInformationSettings.GooglePlusLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapEnabled, model.StoreInformationSettings.SitemapEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeCategories, model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludePublishers, model.StoreInformationSettings.SitemapIncludePublishers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeArticles, model.StoreInformationSettings.SitemapIncludeArticles_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();



            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.GenerateArticleMetaDescription = model.SeoSettings.GenerateArticleMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.EnableJsBundling = model.SeoSettings.EnableJsBundling;
            seoSettings.EnableCssBundling = model.SeoSettings.EnableCssBundling;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
            seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultTitle, model.SeoSettings.DefaultTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaKeywords, model.SeoSettings.DefaultMetaKeywords_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaDescription, model.SeoSettings.DefaultMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.GenerateArticleMetaDescription, model.SeoSettings.GenerateArticleMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableJsBundling, model.SeoSettings.EnableJsBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableCssBundling, model.SeoSettings.EnableCssBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();



            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtectionForAdminArea = model.SecuritySettings.EnableXsrfProtectionForAdminArea;
            securitySettings.EnableXsrfProtectionForPublicStore = model.SecuritySettings.EnableXsrfProtectionForPublicStore;
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            _settingService.SaveSetting(securitySettings);

            //captcha settings
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            captchaSettings = model.CaptchaSettings.ToEntity(captchaSettings);
            _settingService.SaveSetting(captchaSettings);
            if (captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));
            }

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
            pdfSettings.DisablePdfInvoicesForPendingSubscriptions = model.PdfSettings.DisablePdfInvoicesForPendingSubscriptions;
            pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
            pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LetterPageSizeEnabled, model.PdfSettings.LetterPageSizeEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LogoPictureId, model.PdfSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.DisablePdfInvoicesForPendingSubscriptions, model.PdfSettings.DisablePdfInvoicesForPendingSubscriptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn1, model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn2, model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore, storeScope, false);
           
            //now clear settings cache
            _settingService.ClearCache();




            //localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                //clear cached values of routes
                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
            }
            localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
            localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
            _settingService.SaveSetting(localizationSettings);

            //full-text
            commonSettings.FullTextMode = (FulltextSearchMode)model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(commonSettings);

            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            displayDefaultMenuItemSettings.DisplayHomePageMenuItem = model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            displayDefaultMenuItemSettings.DisplayNewArticlesMenuItem = model.DisplayDefaultMenuItemSettings.DisplayNewArticlesMenuItem;
            displayDefaultMenuItemSettings.DisplayArticleSearchMenuItem = model.DisplayDefaultMenuItemSettings.DisplayArticleSearchMenuItem;
            displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem = model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem;
            displayDefaultMenuItemSettings.DisplayBlogMenuItem = model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem;
            displayDefaultMenuItemSettings.DisplayForumsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem;
            displayDefaultMenuItemSettings.DisplayContactUsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayNewArticlesMenuItem, model.DisplayDefaultMenuItemSettings.DisplayNewArticlesMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayArticleSearchMenuItem, model.DisplayDefaultMenuItemSettings.DisplayArticleSearchMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public virtual ActionResult ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new YStoryException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new YStoryException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update encrypted subscription info
                var subscriptions = _subscriptionService.SearchSubscriptions();
                foreach (var subscription in subscriptions)
                {
                    string decryptedCardType = _encryptionService.DecryptText(subscription.CardType, oldEncryptionPrivateKey);
                    string decryptedCardName = _encryptionService.DecryptText(subscription.CardName, oldEncryptionPrivateKey);
                    string decryptedCardNumber = _encryptionService.DecryptText(subscription.CardNumber, oldEncryptionPrivateKey);
                    string decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(subscription.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                    string decryptedCardCvv2 = _encryptionService.DecryptText(subscription.CardCvv2, oldEncryptionPrivateKey);
                    string decryptedCardExpirationMonth = _encryptionService.DecryptText(subscription.CardExpirationMonth, oldEncryptionPrivateKey);
                    string decryptedCardExpirationYear = _encryptionService.DecryptText(subscription.CardExpirationYear, oldEncryptionPrivateKey);

                    string encryptedCardType = _encryptionService.EncryptText(decryptedCardType, newEncryptionPrivateKey);
                    string encryptedCardName = _encryptionService.EncryptText(decryptedCardName, newEncryptionPrivateKey);
                    string encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, newEncryptionPrivateKey);
                    string encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                    string encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, newEncryptionPrivateKey);
                    string encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                    string encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, newEncryptionPrivateKey);

                    subscription.CardType = encryptedCardType;
                    subscription.CardName = encryptedCardName;
                    subscription.CardNumber = encryptedCardNumber;
                    subscription.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
                    subscription.CardCvv2 = encryptedCardCvv2;
                    subscription.CardExpirationMonth = encryptedCardExpirationMonth;
                    subscription.CardExpirationYear = encryptedCardExpirationYear;
                    _subscriptionService.UpdateSubscription(subscription);
                }

                //update password information
                //optimization - load only passwords with PasswordFormat.Encrypted
                var customerPasswords = _customerService.GetCustomerPasswords(passwordFormat: PasswordFormat.Encrypted);
                foreach (var customerPassword in customerPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(customerPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    customerPassword.Password = encryptedPassword;
                    _customerService.UpdateCustomerPassword(customerPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public virtual ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            try
            {
                if (! _fulltextService.IsFullTextSupported())
                    throw new YStoryException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                if (commonSettings.UseFullTextSearch)
                {
                    _fulltextService.DisableFullText();

                    commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                {
                    _fulltextService.EnableFullText();

                    commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            
            return RedirectToAction("GeneralCommon");
        }




        //all settings
        public virtual ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)] 
        public virtual ActionResult AllSettings(DataSourceRequest command, AllSettingsListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var query = _settingService.GetAllSettings().AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchSettingName))
                query = query.Where(s => s.Name.ToLowerInvariant().Contains(model.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(model.SearchSettingValue))
                query = query.Where(s => s.Value.ToLowerInvariant().Contains(model.SearchSettingValue.ToLowerInvariant()));

            var settings = query.ToList()
                .Select(x =>
                            {
                                string storeName;
                                if (x.StoreId == 0)
                                {
                                    storeName = _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                                }
                                else
                                {
                                    var store = _storeService.GetStoreById(x.StoreId);
                                    storeName = store != null ? store.Name : "Unknown";
                                }
                                var settingModel = new SettingModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Value = x.Value,
                                    Store = storeName,
                                    StoreId = x.StoreId
                                };
                                return settingModel;
                            })
                .AsQueryable();

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }
        [HttpPost]
        public virtual ActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            var storeId = model.StoreId;

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeId)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            var storeId = model.StoreId;
            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }

        //action displaying notification (warning) to a store owner about a lot of traffic 
        //between the Redis server and the application when LoadAllLocaleRecordsOnStartup seetting is set
        [ValidateInput(false)]
        public ActionResult RedisCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
        {
            //LoadAllLocaleRecordsOnStartup is set and Redis cache is used, so display warning
            if (_config.RedisCachingEnabled && loadAllLocaleRecordsOnStartup)
                return Json(new { Result = _localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning") }, JsonRequestBehavior.AllowGet);

            return Json(new { Result = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
