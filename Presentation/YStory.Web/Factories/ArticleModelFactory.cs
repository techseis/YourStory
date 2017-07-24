using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Seo;
using YStory.Core.Domain.Contributors;
using YStory.Services.Catalog;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Services.Contributors;
using YStory.Web.Framework.Security.Captcha;
using YStory.Web.Infrastructure.Cache;
using YStory.Web.Models.Catalog;
using YStory.Web.Models.Common;
using YStory.Web.Models.Media;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the article model factory
    /// </summary>
    public partial class ArticleModelFactory : IArticleModelFactory
    {
        #region Fields
        
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IArticleService _articleService;
        private readonly IContributorService _contributorService;
        private readonly IArticleTemplateService _articleTemplateService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IArticleTagService _articleTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ContributorSettings _contributorSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public ArticleModelFactory(ISpecificationAttributeService specificationAttributeService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IArticleService articleService,
            IContributorService contributorService,
            IArticleTemplateService articleTemplateService,
            IArticleAttributeService articleAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            IArticleTagService articleTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            IArticleAttributeParser articleAttributeParser,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            ContributorSettings contributorSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager)
        {
            this._specificationAttributeService = specificationAttributeService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleService = articleService;
            this._contributorService = contributorService;
            this._articleTemplateService = articleTemplateService;
            this._articleAttributeService = articleAttributeService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._articleTagService = articleTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._articleAttributeParser = articleAttributeParser;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._contributorSettings = contributorSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._seoSettings = seoSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the article review overview model
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Article review overview model</returns>
        protected virtual ArticleReviewOverviewModel PrepareArticleReviewOverviewModel(Article article)
        {
            ArticleReviewOverviewModel articleReview;

            if (_catalogSettings.ShowArticleReviewsPerStore)
            {
                string cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_REVIEWS_MODEL_KEY, article.Id, _storeContext.CurrentStore.Id);

                articleReview = _cacheManager.Get(cacheKey, () =>
                {
                    return new ArticleReviewOverviewModel
                    {
                        RatingSum = article.ArticleReviews
                                .Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                                .Sum(pr => pr.Rating),
                        TotalReviews = article
                                .ArticleReviews
                                .Count(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                    };
                });
            }
            else
            {
                articleReview = new ArticleReviewOverviewModel()
                {
                    RatingSum = article.ApprovedRatingSum,
                    TotalReviews = article.ApprovedTotalReviews
                };
            }
            if (articleReview != null)
            {
                articleReview.ArticleId = article.Id;
                articleReview.AllowCustomerReviews = article.AllowCustomerReviews;
            }
            return articleReview;
        }

        /// <summary>
        /// Prepare the article overview price model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Article overview price model</returns>
        protected virtual ArticleOverviewModel.ArticlePriceModel PrepareArticleOverviewPriceModel(Article article, bool forceRedirectionAfterAddingToCart = false)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var priceModel = new ArticleOverviewModel.ArticlePriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (article.ArticleType)
            {
                case ArticleType.GroupedArticle:
                {
                    #region Grouped article

                    var associatedArticles = _articleService.GetAssociatedArticles(article.Id,
                        _storeContext.CurrentStore.Id);

                    //add to cart button (ignore "DisableBuyButton" property for grouped articles)
                    priceModel.DisableBuyButton =
                        !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                    //add to wishlist button (ignore "DisableWishlistButton" property for grouped articles)
                    priceModel.DisableWishlistButton =
                        !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                        !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                    //compare articles
                    priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareArticlesEnabled;
                    switch (associatedArticles.Count)
                    {
                        case 0:
                        {
                            //no associated articles
                        }
                            break;
                        default:
                        {
                            //we have at least one associated article
                            //compare articles
                            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareArticlesEnabled;
                            //priceModel.AvailableForPreSubscription = false;

                            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                            {
                                //find a minimum possible price
                                decimal? minPossiblePrice = null;
                                Article minPriceArticle = null;
                                foreach (var associatedArticle in associatedArticles)
                                {
                                    var tmpMinPossiblePrice = _priceCalculationService.GetFinalPrice(associatedArticle, _workContext.CurrentCustomer);

                                   

                                    if (!minPossiblePrice.HasValue || tmpMinPossiblePrice < minPossiblePrice.Value)
                                    {
                                        minPriceArticle = associatedArticle;
                                        minPossiblePrice = tmpMinPossiblePrice;
                                    }
                                }
                                if (minPriceArticle != null)
                                {
                                    if (minPriceArticle.CallForPrice)
                                    {
                                        priceModel.OldPrice = null;
                                        priceModel.Price = _localizationService.GetResource("Articles.CallForPrice");
                                    }
                                    else if (minPossiblePrice.HasValue)
                                    {
                                        //calculate prices
                                        decimal taxRate;
                                        decimal finalPriceBase = _taxService.GetArticlePrice(minPriceArticle, minPossiblePrice.Value, out taxRate);
                                        decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                        priceModel.OldPrice = null;
                                        priceModel.Price = String.Format(_localizationService.GetResource("Articles.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                        priceModel.PriceValue = finalPrice;

                                        //PAngV baseprice (used in Germany)
                                        priceModel.BasePricePAngV = article.FormatBasePrice(finalPrice,
                                            _localizationService, _measureService, _currencyService, _workContext,
                                            _priceFormatter);
                                    }
                                    else
                                    {
                                        //Actually it's not possible (we presume that minimalPrice always has a value)
                                        //We never should get here
                                        Debug.WriteLine("Cannot calculate minPrice for article #{0}", article.Id);
                                    }
                                }
                            }
                            else
                            {
                                //hide prices
                                priceModel.OldPrice = null;
                                priceModel.Price = null;
                            }
                        }
                            break;
                    }

                    #endregion
                }
                    break;
                case ArticleType.SimpleArticle:
                default:
                {
                    #region Simple article

                    //add to cart button
                    priceModel.DisableBuyButton = article.DisableBuyButton ||
                                                  !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                                                  !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                    //add to wishlist button
                    priceModel.DisableWishlistButton = article.DisableWishlistButton ||
                                                       !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                                                       !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);
                    //compare articles
                    priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareArticlesEnabled;

                    //rental
                    priceModel.IsRental = article.IsRental;

                    //pre-subscription
                    if (article.AvailableForPreSubscription)
                    {
                        priceModel.AvailableForPreSubscription = !article.PreSubscriptionAvailabilityStartDateTimeUtc.HasValue ||
                                                          article.PreSubscriptionAvailabilityStartDateTimeUtc.Value >=
                                                          DateTime.UtcNow;
                        priceModel.PreSubscriptionAvailabilityStartDateTimeUtc = article.PreSubscriptionAvailabilityStartDateTimeUtc;
                    }

                    //prices
                    if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                    {
                         
                            if (article.CallForPrice)
                            {
                                //call for price
                                priceModel.OldPrice = null;
                                priceModel.Price = _localizationService.GetResource("Articles.CallForPrice");
                            }
                            else
                            {
                                //prices
                                var minPossiblePrice = _priceCalculationService.GetFinalPrice(article, _workContext.CurrentCustomer);

                                

                                decimal taxRate;
                                decimal oldPriceBase = _taxService.GetArticlePrice(article, article.OldPrice, out taxRate);
                                decimal finalPriceBase = _taxService.GetArticlePrice(article, minPossiblePrice, out taxRate);

                                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                
                              
                                
                                if (article.IsRental)
                                {
                                    //rental article
                                    priceModel.OldPrice = _priceFormatter.FormatRentalArticlePeriod(article,
                                        priceModel.OldPrice);
                                    priceModel.Price = _priceFormatter.FormatRentalArticlePeriod(article,
                                        priceModel.Price);
                                }


                                //property for German market
                                //we display tax/shipping info only with "shipping enabled" for this article
                                //we also ensure this it's not free shipping
                                priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoArticleBoxes
                                                                     ;


                                //PAngV baseprice (used in Germany)
                                priceModel.BasePricePAngV = article.FormatBasePrice(finalPrice,
                                    _localizationService, _measureService, _currencyService, _workContext,
                                    _priceFormatter);
                            }
                        }
                   

                    #endregion
                }
                    break;
            }

            return priceModel;
        }

        /// <summary>
        /// Prepare the article overview picture model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="articleThumbPictureSize">Article thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>Picture model</returns>
        protected virtual PictureModel PrepareArticleOverviewPictureModel(Article article, int? articleThumbPictureSize = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var articleName = article.GetLocalized(x => x.Name);
            //If a size has been set in the view, we use it in priority
            int pictureSize = articleThumbPictureSize.HasValue
                ? articleThumbPictureSize.Value
                : _mediaSettings.ArticleThumbPictureSize;

            //prepare picture model
            var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_DEFAULTPICTURE_MODEL_KEY,
                article.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore.Id);

            PictureModel defaultPictureModel = _cacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByArticleId(article.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                };
                //"title" attribute
                pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                    ? picture.TitleAttribute
                    : string.Format(_localizationService.GetResource("Media.Article.ImageLinkTitleFormat"), articleName);
                //"alt" attribute
                pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                    ? picture.AltAttribute
                    : string.Format(_localizationService.GetResource("Media.Article.ImageAlternateTextFormat"),
                        articleName);

                return pictureModel;
            });

            return defaultPictureModel;
        }

        /// <summary>
        /// Prepare the article breadcrumb model
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Article breadcrumb model</returns>
        protected virtual ArticleDetailsModel.ArticleBreadcrumbModel PrepareArticleBreadcrumbModel(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_BREADCRUMB_MODEL_KEY,
                    article.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var breadcrumbModel = new ArticleDetailsModel.ArticleBreadcrumbModel
                {
                    Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                    ArticleId = article.Id,
                    ArticleName = article.GetLocalized(x => x.Name),
                    ArticleSeName = article.GetSeName()
                };
                var articleCategories = _categoryService.GetArticleCategoriesByArticleId(article.Id);
                if (articleCategories.Any())
                {
                    var category = articleCategories[0].Category;
                    if (category != null)
                    {
                        foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                        {
                            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                            {
                                Id = catBr.Id,
                                Name = catBr.GetLocalized(x => x.Name),
                                SeName = catBr.GetSeName(),
                                IncludeInTopMenu = catBr.IncludeInTopMenu
                            });
                        }
                    }
                }
                return breadcrumbModel;
            });
            return cachedModel;
        }

        /// <summary>
        /// Prepare the article tag models
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of article tag model</returns>
        protected virtual IList<ArticleTagModel> PrepareArticleTagModels(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var articleTagsCacheKey = string.Format(ModelCacheEventConsumer.ARTICLETAG_BY_ARTICLE_MODEL_KEY, article.Id, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(articleTagsCacheKey, () =>
                article.ArticleTags
                //filter by store
                .Where(x => _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                .Select(x => new ArticleTagModel
                {
                    Id = x.Id,
                    Name = x.GetLocalized(y => y.Name),
                    SeName = x.GetSeName(),
                    ArticleCount = _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id)
                })
                .ToList());

            return model;
        }

        /// <summary>
        /// Prepare the article price model
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Article price model</returns>
        protected virtual ArticleDetailsModel.ArticlePriceModel PrepareArticlePriceModel(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var model = new ArticleDetailsModel.ArticlePriceModel();

            model.ArticleId = article.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                 
                    if (article.CallForPrice)
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        decimal taxRate;
                        decimal oldPriceBase = _taxService.GetArticlePrice(article, article.OldPrice, out taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetArticlePrice(article, _priceCalculationService.GetFinalPrice(article, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetArticlePrice(article, _priceCalculationService.GetFinalPrice(article, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this article
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoArticleDetailsPage
                             ;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = article.FormatBasePrice(finalPriceWithDiscountBase,
                            _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);

                        //currency code
                        model.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (article.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.RentalPrice = _priceFormatter.FormatRentalArticlePeriod(article, priceStr);
                        }
                    }
                 
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.Price = null;
            }

            return model;
        }

        /// <summary>
        /// Prepare the article add to cart model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>Article add to cart model</returns>
        protected virtual ArticleDetailsModel.AddToCartModel PrepareArticleAddToCartModel(Article article, ShoppingCartItem updatecartitem)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var model = new ArticleDetailsModel.AddToCartModel();


            model.ArticleId = article.Id;
            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : 0;
            //allowed quantities
            var allowedQuantities = article.ParseAllowedQuantities();
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = article.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = article.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-subscription
            if (article.AvailableForPreSubscription)
            {
                model.AvailableForPreSubscription = !article.PreSubscriptionAvailabilityStartDateTimeUtc.HasValue ||
                    article.PreSubscriptionAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreSubscriptionAvailabilityStartDateTimeUtc = article.PreSubscriptionAvailabilityStartDateTimeUtc;
            }
            //rental
            model.IsRental = article.IsRental;

            

            return model;
        }

        /// <summary>
        /// Prepare the article attribute models
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>List of article attribute model</returns>
        protected virtual IList<ArticleDetailsModel.ArticleAttributeModel> PrepareArticleAttributeModels(Article article, ShoppingCartItem updatecartitem)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //performance optimization
            //We cache a value indicating whether a article has attributes
            IList<ArticleAttributeMapping> articleAttributeMapping = null;
            string cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_HAS_ARTICLE_ATTRIBUTES_KEY, article.Id);
            var hasArticleAttributesCache = _cacheManager.Get<bool?>(cacheKey);
            if (!hasArticleAttributesCache.HasValue)
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
                hasArticleAttributesCache = articleAttributeMapping.Any();
                _cacheManager.Set(cacheKey, hasArticleAttributesCache, 60);
            }
            if (hasArticleAttributesCache.Value && articleAttributeMapping == null)
            {
                //cache indicates that the article has attributes
                //let's load them
                articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
            }
            if (articleAttributeMapping == null)
            {
                articleAttributeMapping = new List<ArticleAttributeMapping>();
            }

            var model = new List<ArticleDetailsModel.ArticleAttributeModel>();

            foreach (var attribute in articleAttributeMapping)
            {
                var attributeModel = new ArticleDetailsModel.ArticleAttributeModel
                {
                    Id = attribute.Id,
                    ArticleId = article.Id,
                    ArticleAttributeId = attribute.ArticleAttributeId,
                    Name = attribute.ArticleAttribute.GetLocalized(x => x.Name),
                    Description = attribute.ArticleAttribute.GetLocalized(x => x.Description),
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                    HasCondition = !String.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ArticleDetailsModel.ArticleAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal taxRate;
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetArticleAttributeValuePriceAdjustment(attributeValue);
                            decimal priceAdjustmentBase = _taxService.GetArticlePrice(article, attributeValuePriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //"image square" picture (with with "image squares" attribute type only)
                        if (attributeValue.ImageSquaresPictureId > 0)
                        {
                            var articleAttributeImageSquarePictureCacheKey = string.Format(ModelCacheEventConsumer.ARTICLEATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY,
                                   attributeValue.ImageSquaresPictureId,
                                   _webHelper.IsCurrentConnectionSecured(),
                                   _storeContext.CurrentStore.Id);
                            valueModel.ImageSquaresPictureModel = _cacheManager.Get(articleAttributeImageSquarePictureCacheKey, () =>
                            {
                                var imageSquaresPicture = _pictureService.GetPictureById(attributeValue.ImageSquaresPictureId);
                                if (imageSquaresPicture != null)
                                {
                                    return new PictureModel
                                    {
                                        FullSizeImageUrl = _pictureService.GetPictureUrl(imageSquaresPicture),
                                        ImageUrl = _pictureService.GetPictureUrl(imageSquaresPicture, _mediaSettings.ImageSquarePictureSize)
                                    };
                                }
                                return new PictureModel();
                            });
                        }

                        //picture of a article attribute value
                        valueModel.PictureId = attributeValue.PictureId;
                    }

                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _articleAttributeParser.ParseArticleAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                            {
                                                item.IsPreSelected = true;

                                                //set customer entered quantity
                                                if (attributeValue.CustomerEntersQty)
                                                    item.Quantity = attributeValue.Quantity;
                                            }
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //values are already pre-set

                                //set customer entered quantity
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    foreach (var attributeValue in _articleAttributeParser.ParseArticleAttributeValues(updatecartitem.AttributesXml)
                                        .Where(value => value.CustomerEntersQty))
                                    {
                                        var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
                                        if (item != null)
                                            item.Quantity = attributeValue.Quantity;
                                    }
                                }
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _articleAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _articleAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Any())
                                {
                                    DateTime selectedDate;
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                           DateTimeStyles.None, out selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }

                            }
                            break;
                        case AttributeControlType.FileUpload:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = _articleAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                                    Guid downloadGuid;
                                    Guid.TryParse(downloadGuidStr, out downloadGuid);
                                    var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                    if (download != null)
                                        attributeModel.DefaultValue = download.DownloadGuid.ToString();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                model.Add(attributeModel);
            }

            return model;
        }

   

        /// <summary>
        /// Prepare the article publisher models
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of publisher brief info model</returns>
        protected virtual IList<PublisherBriefInfoModel> PrepareArticlePublisherModels(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            string publishersCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_PUBLISHERS_MODEL_KEY,
                     article.Id,
                     _workContext.WorkingLanguage.Id,
                     string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                     _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(publishersCacheKey,
                () => _publisherService.GetArticlePublishersByArticleId(article.Id)
                    .Select(pm =>
                    {
                        var publisher = pm.Publisher;
                        var modelMan = new PublisherBriefInfoModel
                        {
                            Id = publisher.Id,
                            Name = publisher.GetLocalized(x => x.Name),
                            SeName = publisher.GetSeName()
                        };
                        return modelMan;
                    })
                    .ToList()
                );

            return model;
        }

        /// <summary>
        /// Prepare the article details picture model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="isAssociatedArticle">Whether the article is associated</param>
        /// <returns>Picture model for the default picture and list of picture models for all article pictures</returns>
        protected virtual dynamic PrepareArticleDetailsPictureModel(Article article, bool isAssociatedArticle = false)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //default picture size
            var defaultPictureSize = isAssociatedArticle ?
                _mediaSettings.AssociatedArticlePictureSize :
                _mediaSettings.ArticleDetailsPictureSize;

            //prepare picture models
            var articlePicturesCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_DETAILS_PICTURES_MODEL_KEY, article.Id, defaultPictureSize, isAssociatedArticle, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var cachedPictures = _cacheManager.Get(articlePicturesCacheKey, () =>
            {
                var articleName = article.GetLocalized(x => x.Name);

                var pictures = _pictureService.GetPicturesByArticleId(article.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize, !isAssociatedArticle),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 0, !isAssociatedArticle)
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(_localizationService.GetResource("Media.Article.ImageLinkTitleFormat.Details"), articleName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(_localizationService.GetResource("Media.Article.ImageAlternateTextFormat.Details"), articleName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, defaultPictureSize, !isAssociatedArticle),
                        ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ArticleThumbPictureSizeOnArticleDetailsPage),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Article.ImageLinkTitleFormat.Details"), articleName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Article.ImageAlternateTextFormat.Details"), articleName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Article.ImageLinkTitleFormat.Details"), articleName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Article.ImageAlternateTextFormat.Details"), articleName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            return cachedPictures;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the article template view path
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>View path</returns>
        public virtual string PrepareArticleTemplateViewPath(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var templateCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_TEMPLATE_MODEL_KEY, article.ArticleTemplateId);
            var articleTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _articleTemplateService.GetArticleTemplateById(article.ArticleTemplateId);
                if (template == null)
                    template = _articleTemplateService.GetAllArticleTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return articleTemplateViewPath;
        }

        /// <summary>
        /// Prepare the article overview models
        /// </summary>
        /// <param name="articles">Collection of articles</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="articleThumbPictureSize">Article thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of article overview model</returns>
        public virtual IEnumerable<ArticleOverviewModel> PrepareArticleOverviewModels(IEnumerable<Article> articles,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? articleThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (articles == null)
                throw new ArgumentNullException("articles");

            var models = new List<ArticleOverviewModel>();
            foreach (var article in articles)
            {
                var model = new ArticleOverviewModel
                {
                    Id = article.Id,
                    Name = article.GetLocalized(x => x.Name),
                    ShortDescription = article.GetLocalized(x => x.ShortDescription),
                    FullDescription = article.GetLocalized(x => x.FullDescription),
                    SeName = article.GetSeName(),
                    Sku = article.Sku,
                    ArticleType = article.ArticleType,
                    MarkAsNew = article.MarkAsNew &&
                        (!article.MarkAsNewStartDateTimeUtc.HasValue || article.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!article.MarkAsNewEndDateTimeUtc.HasValue || article.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                //price
                if (preparePriceModel)
                {
                    model.ArticlePrice = PrepareArticleOverviewPriceModel(article, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.DefaultPictureModel = PrepareArticleOverviewPictureModel(article, articleThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.SpecificationAttributeModels = PrepareArticleSpecificationModel(article);
                }

                //reviews
                model.ReviewOverviewModel = PrepareArticleReviewOverviewModel(article);

                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Prepare the article details model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedArticle">Whether the article is associated</param>
        /// <returns>Article details model</returns>
        public virtual ArticleDetailsModel PrepareArticleDetailsModel(Article article,
            ShoppingCartItem updatecartitem = null, bool isAssociatedArticle = false)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            //standard properties
            var model = new ArticleDetailsModel
            {
                Id = article.Id,
                Name = article.GetLocalized(x => x.Name),
                ShortDescription = article.GetLocalized(x => x.ShortDescription),
                FullDescription = article.GetLocalized(x => x.FullDescription),
                MetaKeywords = article.GetLocalized(x => x.MetaKeywords),
                MetaDescription = article.GetLocalized(x => x.MetaDescription),
                MetaTitle = article.GetLocalized(x => x.MetaTitle),
                SeName = article.GetSeName(),
                ArticleType = article.ArticleType,
                ShowSku = _catalogSettings.ShowSkuOnArticleDetailsPage,
                Sku = article.Sku,
                ShowPublisherPartNumber = _catalogSettings.ShowPublisherPartNumber,
                
                PublisherPartNumber = article.PublisherPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = article.Gtin,
                
                DisplayDiscontinuedMessage = !article.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedArticles
            };

            //automatically generate article description?
            if (_seoSettings.GenerateArticleMetaDescription && String.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            
          

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare articles
            model.CompareArticlesEnabled = _catalogSettings.CompareArticlesEnabled;
            //store name
            model.CurrentStoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name);

            //contributor details
            if (_contributorSettings.ShowContributorOnArticleDetailsPage)
            {
                var contributor = _contributorService.GetContributorById(article.ContributorId);
                if (contributor != null && !contributor.Deleted && contributor.Active)
                {
                    model.ShowContributor = true;

                    model.ContributorModel = new ContributorBriefInfoModel
                    {
                        Id = contributor.Id,
                        Name = contributor.GetLocalized(x => x.Name),
                        SeName = contributor.GetSeName(),
                    };
                }
            }

            //page sharing
            if (_catalogSettings.ShowShareButton && !String.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }
                model.PageShareCode = shareCode;
            }

         

            //breadcrumb
            //do not prepare this model for the associated articles. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedArticle)
            {
                model.Breadcrumb = PrepareArticleBreadcrumbModel(article);
            }

            //article tags
            //do not prepare this model for the associated articles. anyway it's not used
            if (!isAssociatedArticle)
            {
                model.ArticleTags = PrepareArticleTagModels(article);
            }
            
           //pictures
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            var pictureModels = PrepareArticleDetailsPictureModel(article, isAssociatedArticle);
            model.DefaultPictureModel = pictureModels.DefaultPictureModel;
            model.PictureModels = pictureModels.PictureModels;

            //price
            model.ArticlePrice = PrepareArticlePriceModel(article);

            //'Add to cart' model
            model.AddToCart = PrepareArticleAddToCartModel(article, updatecartitem);

           

            //article attributes
            model.ArticleAttributes = PrepareArticleAttributeModels(article, updatecartitem);
            
            //article specifications
            //do not prepare this model for the associated articles. anyway it's not used
            if (!isAssociatedArticle)
            {
                model.ArticleSpecifications = PrepareArticleSpecificationModel(article);
            }

            //article review overview
            model.ArticleReviewOverview = PrepareArticleReviewOverviewModel(article);

         

            //publishers
            //do not prepare this model for the associated articles. anywway it's not used
            if (!isAssociatedArticle)
            {
                model.ArticlePublishers = PrepareArticlePublisherModels(article);
            }

            //rental articles
            if (article.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //associated articles
            if (article.ArticleType == ArticleType.GroupedArticle)
            {
                //ensure no circular references
                if (!isAssociatedArticle)
                {
                    var associatedArticles = _articleService.GetAssociatedArticles(article.Id, _storeContext.CurrentStore.Id);
                    foreach (var associatedArticle in associatedArticles)
                        model.AssociatedArticles.Add(PrepareArticleDetailsModel(associatedArticle, null, true));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the article reviews model
        /// </summary>
        /// <param name="model">Article reviews model</param>
        /// <param name="article">Article</param>
        /// <returns>Article reviews model</returns>
        public virtual ArticleReviewsModel PrepareArticleReviewsModel(ArticleReviewsModel model, Article article)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (article == null)
                throw new ArgumentNullException("article");

            model.ArticleId = article.Id;
            model.ArticleName = article.GetLocalized(x => x.Name);
            model.ArticleSeName = article.GetSeName();

            var articleReviews = _catalogSettings.ShowArticleReviewsPerStore
                ? article.ArticleReviews.Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id).OrderBy(pr => pr.CreatedOnUtc)
                : article.ArticleReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in articleReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ArticleReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = customer.FormatUserName(),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ArticleReviewHelpfulnessModel
                    {
                        ArticleReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddArticleReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewArticle || !_workContext.CurrentCustomer.IsGuest();
            model.AddArticleReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnArticleReviewPage;

            return model;
        }

        /// <summary>
        /// Prepare the customer article reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer article reviews model</returns>
        public virtual CustomerArticleReviewsModel PrepareCustomerArticleReviewsModel(int? page)
        {
            var pageSize = _catalogSettings.ArticleReviewsPageSizeOnAccountPage;
            int pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var list = _articleService.GetAllArticleReviews(customerId: _workContext.CurrentCustomer.Id, 
                approved: null, 
                pageIndex: pageIndex, 
                pageSize: pageSize);

            var articleReviews = new List<CustomerArticleReviewModel>();

            foreach (var review in list)
            {
                var article = review.Article;
                var articleReviewModel = new CustomerArticleReviewModel
                {
                    Title = review.Title,
                    ArticleId = article.Id,
                    ArticleName = article.GetLocalized(p => p.Name),
                    ArticleSeName = article.GetSeName(),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc).ToString("g")
                };

                if (_catalogSettings.ArticleReviewsMustBeApproved)
                {
                    articleReviewModel.ApprovalStatus = review.IsApproved
                        ? _localizationService.GetResource("Account.CustomerArticleReviews.ApprovalStatus.Approved")
                        : _localizationService.GetResource("Account.CustomerArticleReviews.ApprovalStatus.Pending");
                }
                articleReviews.Add(articleReviewModel);
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerArticleReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerArticleReviewsModel.CustomerArticleReviewsRouteValues { page = pageIndex }
            };

            var model = new CustomerArticleReviewsModel
            {
                ArticleReviews = articleReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the article email a friend model
        /// </summary>
        /// <param name="model">Article email a friend model</param>
        /// <param name="article">Article</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>article email a friend model</returns>
        public virtual ArticleEmailAFriendModel PrepareArticleEmailAFriendModel(ArticleEmailAFriendModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (article == null)
                throw new ArgumentNullException("article");

            model.ArticleId = article.Id;
            model.ArticleName = article.GetLocalized(x => x.Name);
            model.ArticleSeName = article.GetSeName();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailArticleToFriendPage;
            if (!excludeProperties)
            {
                model.YourEmailAddress = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the article specification models
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of article specification model</returns>
        public virtual IList<ArticleSpecificationModel> PrepareArticleSpecificationModel(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            string cacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_SPECS_MODEL_KEY, article.Id, _workContext.WorkingLanguage.Id);
            return _cacheManager.Get(cacheKey, () =>
                _specificationAttributeService.GetArticleSpecificationAttributes(article.Id, 0, null, true)
                .Select(psa =>
                {
                    var m = new ArticleSpecificationModel
                    {
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                        ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = psa.CustomValue;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                            break;
                        default:
                            break;
                    }
                    return m;
                }).ToList()
            );
        }

        #endregion
    }
}
