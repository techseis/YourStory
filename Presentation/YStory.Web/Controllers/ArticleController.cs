using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Catalog;
using YStory.Services.Events;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Messages;
using YStory.Services.Subscriptions;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Stores;
using YStory.Web.Factories;
using YStory.Web.Framework;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Security;
using YStory.Web.Framework.Security.Captcha;
using YStory.Web.Infrastructure.Cache;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Controllers
{
    public partial class ArticleController : BasePublicController
    {
        #region Fields

        private readonly IArticleModelFactory _articleModelFactory;
        private readonly IArticleService _articleService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IRecentlyViewedArticlesService _recentlyViewedArticlesService;
        private readonly ICompareArticlesService _compareArticlesService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ISubscriptionReportService _subscriptionReportService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public ArticleController(IArticleModelFactory articleModelFactory,
            IArticleService articleService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IRecentlyViewedArticlesService recentlyViewedArticlesService,
            ICompareArticlesService compareArticlesService,
            IWorkflowMessageService workflowMessageService,
            ISubscriptionReportService subscriptionReportService,
            ISubscriptionService subscriptionService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            ICacheManager cacheManager)
        {
            this._articleModelFactory = articleModelFactory;
            this._articleService = articleService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._recentlyViewedArticlesService = recentlyViewedArticlesService;
            this._compareArticlesService = compareArticlesService;
            this._workflowMessageService = workflowMessageService;
            this._subscriptionReportService = subscriptionReportService;
            this._subscriptionService = subscriptionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._catalogSettings = catalogSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Article details page

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ArticleDetails(int articleId, int updatecartitemid = 0)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!article.Published && !_catalogSettings.AllowViewUnpublishedArticlePage) ||
                //ACL (access control list) 
                !_aclService.Authorize(article) ||
                //Store mapping
                !_storeMappingService.Authorize(article) ||
                //availability dates
                !article.IsAvailable();
            //Check whether the current user has a "Manage articles" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return InvokeHttp404();

            //visible individually?
            if (!article.VisibleIndividually)
            {
                //is this one an associated articles?
                var parentGroupedArticle = _articleService.GetArticleById(article.ParentGroupedArticleId);
                if (parentGroupedArticle == null)
                    return RedirectToRoute("HomePage");

                return RedirectToRoute("Article", new { SeName = parentGroupedArticle.GetSeName() });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Article", new { SeName = article.GetSeName() });
                }
                //is it this article?
                if (article.Id != updatecartitem.ArticleId)
                {
                    return RedirectToRoute("Article", new { SeName = article.GetSeName() });
                }
            }

            //save as recently viewed
            _recentlyViewedArticlesService.AddArticleToRecentlyViewedList(article.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
                _permissionService.Authorize(StandardPermissionProvider.ManageArticles))
            {
                //a contributor should have access only to his articles
                if (_workContext.CurrentContributor == null || _workContext.CurrentContributor.Id == article.ContributorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Article", new { id = article.Id, area = "Admin" }));
                }
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewArticle", _localizationService.GetResource("ActivityLog.PublicStore.ViewArticle"), article.Name);

            //model
            var model = _articleModelFactory.PrepareArticleDetailsModel(article, updatecartitem, false);
            //template
            var articleTemplateViewPath = _articleModelFactory.PrepareArticleTemplateViewPath(article);

            return View(articleTemplateViewPath, model);
        }

        [ChildActionOnly]
        public virtual ActionResult RelatedArticles(int articleId, int? articleThumbPictureSize)
        {
            //load and cache report
            var articleIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.ARTICLES_RELATED_IDS_KEY, articleId, _storeContext.CurrentStore.Id),
                () =>
                    _articleService.GetRelatedArticlesByArticleId1(articleId).Select(x => x.ArticleId2).ToArray()
                    );

            //load articles
            var articles = _articleService.GetArticlesByIds(articleIds);
            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");

            var model = _articleModelFactory.PrepareArticleOverviewModels(articles, true, true, articleThumbPictureSize).ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult ArticlesAlsoPurchased(int articleId, int? articleThumbPictureSize)
        {
            if (!_catalogSettings.ArticlesAlsoPurchasedEnabled)
                return Content("");

            //load and cache report
            var articleIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.ARTICLES_ALSO_PURCHASED_IDS_KEY, articleId, _storeContext.CurrentStore.Id),
                () =>
                    _subscriptionReportService
                    .GetAlsoPurchasedArticlesIds(_storeContext.CurrentStore.Id, articleId, _catalogSettings.ArticlesAlsoPurchasedNumber)
                    );

            //load articles
            var articles = _articleService.GetArticlesByIds(articleIds);
            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");

            var model = _articleModelFactory.PrepareArticleOverviewModels(articles, true, true, articleThumbPictureSize).ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult CrossSellArticles(int? articleThumbPictureSize)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var articles = _articleService.GetCrosssellArticlesByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");


            //Cross-sell articles are dispalyed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingArticle" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            var model = _articleModelFactory.PrepareArticleOverviewModels(articles,
                articleThumbPictureSize: articleThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();

            return PartialView(model);
        }

        #endregion

        #region Recently viewed articles

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult RecentlyViewedArticles()
        {
            if (!_catalogSettings.RecentlyViewedArticlesEnabled)
                return Content("");

            var articles = _recentlyViewedArticlesService.GetRecentlyViewedArticles(_catalogSettings.RecentlyViewedArticlesNumber);

            var model = new List<ArticleOverviewModel>();
            model.AddRange(_articleModelFactory.PrepareArticleOverviewModels(articles));

            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult RecentlyViewedArticlesBlock(int? articleThumbPictureSize, bool? preparePriceModel)
        {
            if (!_catalogSettings.RecentlyViewedArticlesEnabled)
                return Content("");

            var preparePictureModel = articleThumbPictureSize.HasValue;
            var articles = _recentlyViewedArticlesService.GetRecentlyViewedArticles(_catalogSettings.RecentlyViewedArticlesNumber);

            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");

            //prepare model
            var model = new List<ArticleOverviewModel>();
            model.AddRange(_articleModelFactory.PrepareArticleOverviewModels(articles,
                preparePriceModel.GetValueOrDefault(),
                preparePictureModel,
                articleThumbPictureSize));

            return PartialView(model);
        }

        #endregion

        #region New (recently added) articles page

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult NewArticles()
        {
            if (!_catalogSettings.NewArticlesEnabled)
                return Content("");

            var articles = _articleService.SearchArticles(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                subscriptionBy: ArticleSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewArticlesNumber);

            var model = new List<ArticleOverviewModel>();
            model.AddRange(_articleModelFactory.PrepareArticleOverviewModels(articles));

            return View(model);
        }

        public virtual ActionResult NewArticlesRss()
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: New articles", _storeContext.CurrentStore.GetLocalized(x => x.Name)),
                                    "Information about articles",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    string.Format("urn:store:{0}:newArticles", _storeContext.CurrentStore.Id),
                                    DateTime.UtcNow);

            if (!_catalogSettings.NewArticlesEnabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<SyndicationItem>();

            var articles = _articleService.SearchArticles(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                markedAsNewOnly: true,
                subscriptionBy: ArticleSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewArticlesNumber);
            foreach (var article in articles)
            {
                string articleUrl = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");
                string articleName = article.GetLocalized(x => x.Name);
                string articleDescription = article.GetLocalized(x => x.ShortDescription);
                var item = new SyndicationItem(articleName, articleDescription, new Uri(articleUrl), String.Format("urn:store:{0}:newArticles:article:{1}", _storeContext.CurrentStore.Id, article.Id), article.CreatedOnUtc);
                items.Add(item);
                //uncomment below if you want to add RSS enclosure for pictures
                //var picture = _pictureService.GetPicturesByArticleId(article.Id, 1).FirstOrDefault();
                //if (picture != null)
                //{
                //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ArticleDetailsPictureSize);
                //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl)).CreateReader());
                //}

            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Home page bestsellers and articles

        [ChildActionOnly]
        public virtual ActionResult HomepageBestSellers(int? articleThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var report = _cacheManager.Get(string.Format(ModelCacheEventConsumer.HOMEPAGE_BESTSELLERS_IDS_KEY, _storeContext.CurrentStore.Id),
                () => _subscriptionReportService.BestSellersReport(
                    storeId: _storeContext.CurrentStore.Id,
                    pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)
                    .ToList());


            //load articles
            var articles = _articleService.GetArticlesByIds(report.Select(x => x.ArticleId).ToArray());
            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");

            //prepare model
            var model = _articleModelFactory.PrepareArticleOverviewModels(articles, true, true, articleThumbPictureSize).ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult HomepageArticles(int? articleThumbPictureSize)
        {
            var articles = _articleService.GetAllArticlesDisplayedOnHomePage();
            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            if (!articles.Any())
                return Content("");

            var model = _articleModelFactory.PrepareArticleOverviewModels(articles, true, true, articleThumbPictureSize).ToList();
            return PartialView(model);
        }

        #endregion

        #region Article reviews

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ArticleReviews(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted || !article.Published || !article.AllowCustomerReviews)
                return RedirectToRoute("HomePage");

            var model = new ArticleReviewsModel();
            model = _articleModelFactory.PrepareArticleReviewsModel(model, article);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (_catalogSettings.ArticleReviewPossibleOnlyAfterPurchasing &&
                !_subscriptionService.SearchSubscriptions(customerId: _workContext.CurrentCustomer.Id, articleId: articleId, osIds: new List<int> { (int)SubscriptionStatus.Complete }).Any())
                    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ArticleReviewPossibleOnlyAfterPurchasing"));
            
            //default value
            model.AddArticleReview.Rating = _catalogSettings.DefaultArticleRatingValue;
            return View(model);
        }

        [HttpPost, ActionName("ArticleReviews")]
        [PublicAntiForgery]
        [FormValueRequired("add-review")]
        [CaptchaValidator]
        public virtual ActionResult ArticleReviewsAdd(int articleId, ArticleReviewsModel model, bool captchaValid)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted || !article.Published || !article.AllowCustomerReviews)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnArticleReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (_catalogSettings.ArticleReviewPossibleOnlyAfterPurchasing && 
                !_subscriptionService.SearchSubscriptions(customerId: _workContext.CurrentCustomer.Id, articleId: articleId, osIds: new List<int> { (int)SubscriptionStatus.Complete }).Any())
                    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ArticleReviewPossibleOnlyAfterPurchasing"));

            if (ModelState.IsValid)
            {
                //save review
                int rating = model.AddArticleReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultArticleRatingValue;
                bool isApproved = !_catalogSettings.ArticleReviewsMustBeApproved;

                var articleReview = new ArticleReview
                {
                    ArticleId = article.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.AddArticleReview.Title,
                    ReviewText = model.AddArticleReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };
                article.ArticleReviews.Add(articleReview);
                _articleService.UpdateArticle(article);

                //update article totals
                _articleService.UpdateArticleReviewTotals(article);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewArticleReviews)
                    _workflowMessageService.SendArticleReviewNotificationMessage(articleReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddArticleReview", _localizationService.GetResource("ActivityLog.PublicStore.AddArticleReview"), article.Name);

                //raise event
                if (articleReview.IsApproved)
                    _eventPublisher.Publish(new ArticleReviewApprovedEvent(articleReview));

                model = _articleModelFactory.PrepareArticleReviewsModel(model, article);
                model.AddArticleReview.Title = null;
                model.AddArticleReview.ReviewText = null;

                model.AddArticleReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddArticleReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddArticleReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _articleModelFactory.PrepareArticleReviewsModel(model, article);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult SetArticleReviewHelpfulness(int articleReviewId, bool washelpful)
        {
            var articleReview = _articleService.GetArticleReviewById(articleReviewId);
            if (articleReview == null)
                throw new ArgumentException("No article review found with the specified id");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewArticle)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = articleReview.HelpfulYesTotal,
                    TotalNo = articleReview.HelpfulNoTotal
                });
            }

            //customers aren't allowed to vote for their own reviews
            if (articleReview.CustomerId == _workContext.CurrentCustomer.Id)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Reviews.Helpfulness.YourOwnReview"),
                    TotalYes = articleReview.HelpfulYesTotal,
                    TotalNo = articleReview.HelpfulNoTotal
                });
            }

            //delete previous helpfulness
            var prh = articleReview.ArticleReviewHelpfulnessEntries
                .FirstOrDefault(x => x.CustomerId == _workContext.CurrentCustomer.Id);
            if (prh != null)
            {
                //existing one
                prh.WasHelpful = washelpful;
            }
            else
            {
                //insert new helpfulness
                prh = new ArticleReviewHelpfulness
                {
                    ArticleReviewId = articleReview.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    WasHelpful = washelpful,
                };
                articleReview.ArticleReviewHelpfulnessEntries.Add(prh);
            }
            _articleService.UpdateArticle(articleReview.Article);

            //new totals
            articleReview.HelpfulYesTotal = articleReview.ArticleReviewHelpfulnessEntries.Count(x => x.WasHelpful);
            articleReview.HelpfulNoTotal = articleReview.ArticleReviewHelpfulnessEntries.Count(x => !x.WasHelpful);
            _articleService.UpdateArticle(articleReview.Article);

            return Json(new
            {
                Result = _localizationService.GetResource("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = articleReview.HelpfulYesTotal,
                TotalNo = articleReview.HelpfulNoTotal
            });
        }

        public virtual ActionResult CustomerArticleReviews(int? page)
        {
            if (_workContext.CurrentCustomer.IsGuest())
                return new HttpUnauthorizedResult();

            if (!_catalogSettings.ShowArticleReviewsTabOnAccountPage)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _articleModelFactory.PrepareCustomerArticleReviewsModel(page);
            return View(model);
        }

        #endregion

        #region Email a friend

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ArticleEmailAFriend(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted || !article.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("HomePage");

            var model = new ArticleEmailAFriendModel();
            model = _articleModelFactory.PrepareArticleEmailAFriendModel(model, article, false);
            return View(model);
        }

        [HttpPost, ActionName("ArticleEmailAFriend")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        [CaptchaValidator]
        public virtual ActionResult ArticleEmailAFriendSend(ArticleEmailAFriendModel model, bool captchaValid)
        {
            var article = _articleService.GetArticleById(model.ArticleId);
            if (article == null || article.Deleted || !article.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailArticleToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //check whether the current customer is guest and ia allowed to email a friend
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Articles.EmailAFriend.OnlyRegisteredUsers"));
            }
            
            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendArticleEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, article,
                        model.YourEmailAddress, model.FriendEmail,
                        Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model = _articleModelFactory.PrepareArticleEmailAFriendModel(model, article, true);
                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Articles.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _articleModelFactory.PrepareArticleEmailAFriendModel(model, article, true);
            return View(model);
        }

        #endregion

        #region Comparing articles

        [HttpPost]
        public virtual ActionResult AddArticleToCompareList(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null || article.Deleted || !article.Published)
                return Json(new
                {
                    success = false,
                    message = "No article found with the specified ID"
                });

            if (!_catalogSettings.CompareArticlesEnabled)
                return Json(new
                {
                    success = false,
                    message = "Article comparison is disabled"
                });

            _compareArticlesService.AddArticleToCompareList(articleId);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToCompareList", _localizationService.GetResource("ActivityLog.PublicStore.AddToCompareList"), article.Name);

            return Json(new
            {
                success = true,
                message = string.Format(_localizationService.GetResource("Articles.ArticleHasBeenAddedToCompareList.Link"), Url.RouteUrl("CompareArticles"))
                //use the code below (commented) if you want a customer to be automatically redirected to the compare articles page
                //redirect = Url.RouteUrl("CompareArticles"),
            });
        }

        public virtual ActionResult RemoveArticleFromCompareList(int articleId)
        {
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                return RedirectToRoute("HomePage");

            if (!_catalogSettings.CompareArticlesEnabled)
                return RedirectToRoute("HomePage");

            _compareArticlesService.RemoveArticleFromCompareList(articleId);

            return RedirectToRoute("CompareArticles");
        }

        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult CompareArticles()
        {
            if (!_catalogSettings.CompareArticlesEnabled)
                return RedirectToRoute("HomePage");

            var model = new CompareArticlesModel
            {
                IncludeShortDescriptionInCompareArticles = _catalogSettings.IncludeShortDescriptionInCompareArticles,
                IncludeFullDescriptionInCompareArticles = _catalogSettings.IncludeFullDescriptionInCompareArticles,
            };

            var articles = _compareArticlesService.GetComparedArticles();

            //ACL and store mapping
            articles = articles.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            articles = articles.Where(p => p.IsAvailable()).ToList();

            //prepare model
            _articleModelFactory.PrepareArticleOverviewModels(articles, prepareSpecificationAttributes: true)
                .ToList()
                .ForEach(model.Articles.Add);
            return View(model);
        }

        public virtual ActionResult ClearCompareList()
        {
            if (!_catalogSettings.CompareArticlesEnabled)
                return RedirectToRoute("HomePage");

            _compareArticlesService.ClearCompareArticles();

            return RedirectToRoute("CompareArticles");
        }

        #endregion 
    }
}
