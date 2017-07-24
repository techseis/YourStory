using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using YStory.Admin.Infrastructure.Cache;
using YStory.Admin.Models.Home;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Catalog;
using YStory.Services.Configuration;
using YStory.Services.Customers;
using YStory.Services.Subscriptions;
using YStory.Services.Security;

namespace YStory.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IArticleService _articleService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICustomerService _customerService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings, 
            ISettingService settingService,
            IPermissionService permissionService,
            IArticleService articleService,
            ISubscriptionService subscriptionService,
            ICustomerService customerService,
            IReturnRequestService returnRequestService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._storeContext = storeContext;
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._articleService = articleService;
            this._subscriptionService = subscriptionService;
            this._customerService = customerService;
            this._returnRequestService = returnRequestService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            var model = new DashboardModel();
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult YourStoryNews()
        {
            try
            {
                string feedUrl = string.Format("http://www.yourStory.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}",
                    YStoryVersion.CurrentVersion, 
                    Request.Url.IsLoopback,
                    _adminAreaSettings.HideAdvertisementsOnAdminArea,
                    _storeContext.CurrentStore.Url)
                    .ToLowerInvariant();

                var rssData = _cacheManager.Get(ModelCacheEventConsumer.OFFICIAL_NEWS_MODEL_KEY, () =>
                {
                    //specify timeout (5 secs)
                    var request = WebRequest.Create(feedUrl);
                    request.Timeout = 5000;
                    using (var response = request.GetResponse())
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        return SyndicationFeed.Load(reader);
                    }
                });
                
                var model = new YourStoryNewsModel()
                {
                    HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
                };
                for (int i = 0; i < rssData.Items.Count(); i++)
                {
                    var item = rssData.Items.ElementAt(i);
                    var newsItem = new YourStoryNewsModel.NewsDetailsModel()
                    {
                        Title = item.Title.Text,
                        Summary = item.Summary.Text,
                        Url = item.Links.Any() ? item.Links.First().Uri.OriginalString : null,
                        PublishDate = item.PublishDate
                    };
                    model.Items.Add(newsItem);

                    //has new items?
                    if (i == 0)
                    {
                        var firstRequest = String.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                        if (_adminAreaSettings.LastNewsTitleAdminArea != newsItem.Title)
                        {
                            _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                            _settingService.SaveSetting(_adminAreaSettings);

                            if (!firstRequest)
                            {
                                //new item
                                model.HasNewItems = true;
                            }
                        }
                    }
                }
                return PartialView(model);
            }
            catch (Exception)
            {
                return Content("");
            }
        }

        [HttpPost]
        public virtual ActionResult YourStoryNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_adminAreaSettings);
            return Content("Setting changed");
        }

        [ChildActionOnly]
        public virtual ActionResult CommonStatistics()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageSubscriptions) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return Content("");

            //a contributor doesn't have access to this report
            if (_workContext.CurrentContributor != null)
                return Content("");

            var model = new CommonStatisticsModel();

            model.NumberOfSubscriptions = _subscriptionService.SearchSubscriptions(
                pageIndex: 0, 
                pageSize: 1).TotalCount;

            model.NumberOfCustomers = _customerService.GetAllCustomers(
                customerRoleIds: new [] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id }, 
                pageIndex: 0, 
                pageSize: 1).TotalCount;

            model.NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(
                rs: ReturnRequestStatus.Pending, 
                pageIndex: 0, 
                pageSize:1).TotalCount;

           

            return PartialView(model);
        }

        #endregion
    }
}
