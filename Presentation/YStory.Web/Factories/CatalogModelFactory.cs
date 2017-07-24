using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Forums;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Contributors;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Events;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Stores;
using YStory.Services.Topics;
using YStory.Services.Contributors;
using YStory.Web.Framework.Events;
using YStory.Web.Infrastructure.Cache;
using YStory.Web.Models.Catalog;
using YStory.Web.Models.Media;

namespace YStory.Web.Factories
{
    public partial class CatalogModelFactory : ICatalogModelFactory
    {
        #region Fields

        private readonly IArticleModelFactory _articleModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IArticleService _articleService;
        private readonly IContributorService _contributorService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IPublisherTemplateService _publisherTemplateService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IArticleTagService _articleTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicService _topicService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISearchTermService _searchTermService;
        private readonly HttpContextBase _httpContext;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ContributorSettings _contributorSettings;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICacheManager _cacheManager;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;

        #endregion

        #region Constructors

        public CatalogModelFactory(IArticleModelFactory articleModelFactory,
            ICategoryService categoryService, 
            IPublisherService publisherService,
            IArticleService articleService, 
            IContributorService contributorService,
            ICategoryTemplateService categoryTemplateService,
            IPublisherTemplateService publisherTemplateService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IPictureService pictureService, 
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper, 
            ISpecificationAttributeService specificationAttributeService,
            IArticleTagService articleTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ITopicService topicService,
            IEventPublisher eventPublisher,
            ISearchTermService searchTermService,
            HttpContextBase httpContext,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            ContributorSettings contributorSettings,
            BlogSettings blogSettings,
            ForumSettings  forumSettings,
            ICacheManager cacheManager,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings)
        {
            this._articleModelFactory = articleModelFactory;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleService = articleService;
            this._contributorService = contributorService;
            this._categoryTemplateService = categoryTemplateService;
            this._publisherTemplateService = publisherTemplateService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._articleTagService = articleTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._topicService = topicService;
            this._eventPublisher = eventPublisher;
            this._searchTermService = searchTermService;
            this._httpContext = httpContext;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._contributorSettings = contributorSettings;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._cacheManager = cacheManager;
            this._displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <returns>List of child category identifiers</returns>
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY, 
                parentCategoryId, 
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), 
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        #endregion

        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        public virtual void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            var allDisabled = _catalogSettings.ArticleSortingEnumDisabled.Count == Enum.GetValues(typeof(ArticleSortingEnum)).Length;
            pagingFilteringModel.AllowArticleSorting = _catalogSettings.AllowArticleSorting && !allDisabled;

            var activeOptions = Enum.GetValues(typeof(ArticleSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ArticleSortingEnumDisabled)
                .Select((idOption) =>
                {
                    int subscription;
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ArticleSortingEnumDisplaySubscription.TryGetValue(idOption, out subscription) ? subscription : idOption);
                })
                .OrderBy(x => x.Value);
            if (command.OrderBy == null)
                command.OrderBy = allDisabled ? 0 : activeOptions.First().Key;

            if (pagingFilteringModel.AllowArticleSorting)
            {
                foreach (var option in activeOptions)
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + (option.Key).ToString(), null);

                    var sortValue = ((ArticleSortingEnum)option.Key).GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = option.Key == command.OrderBy
                    });
                }
            }
        }

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        public virtual void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowArticleViewModeChanging = _catalogSettings.AllowArticleViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowArticleViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }

        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        public virtual void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Category model</returns>
        public virtual CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = category.GetLocalized(x => x.Name),
                Description = category.GetLocalized(x => x.Description),
                MetaKeywords = category.GetLocalized(x => x.MetaKeywords),
                MetaDescription = category.GetLocalized(x => x.MetaDescription),
                MetaTitle = category.GetLocalized(x => x.MetaTitle),
                SeName = category.GetSeName(),
            };

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                category.AllowCustomersToSelectPageSize, 
                category.PageSizeOptions, 
                category.PageSize);

            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }


            //category breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                string breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_BREADCRUMB_KEY,
                    category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                model.CategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                    category
                    .GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService)
                    .Select(catBr => new CategoryModel
                    {
                        Id = catBr.Id,
                        Name = catBr.GetLocalized(x => x.Name),
                        SeName = catBr.GetSeName()
                    })
                    .ToList()
                );
            }


            
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //subcategories
            string subCategoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                category.Id,
                pictureSize,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());
            model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
                _categoryService.GetAllCategoriesByParentCategoryId(category.Id)
                .Select(x =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        Description = x.GetLocalized(y => y.Description)
                    };

                    //prepare picture model
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };
                        return pictureModel;
                    });

                    return subCatModel;
                })
                .ToList()
            );




            //featured articles
            if (!_catalogSettings.IgnoreFeaturedArticles)
            {
                //We cache a value indicating whether we have featured articles
                IPagedList<Article> featuredArticles = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HAS_FEATURED_ARTICLES_KEY, category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), _storeContext.CurrentStore.Id);
                var hasFeaturedArticlesCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedArticlesCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load articles and cache the result (true/false)
                    featuredArticles = _articleService.SearchArticles(
                       categoryIds: new List<int> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredArticles: true);
                    hasFeaturedArticlesCache = featuredArticles.TotalCount > 0;
                    _cacheManager.Set(cacheKey, hasFeaturedArticlesCache, 60);
                }
                if (hasFeaturedArticlesCache.Value && featuredArticles == null)
                {
                    //cache indicates that the category has featured articles
                    //let's load them
                    featuredArticles = _articleService.SearchArticles(
                       categoryIds: new List<int> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredArticles: true);
                }
                if (featuredArticles != null)
                {
                    model.FeaturedArticles = _articleModelFactory.PrepareArticleOverviewModels(featuredArticles).ToList();
                }
            }


            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowArticlesFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }
            //articles
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds;
            var articles = _articleService.SearchArticles(out filterableSpecificationAttributeOptionIds,
                true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredArticles:_catalogSettings.IncludeFeaturedArticlesInNormalLists ? null : (bool?)false,
                priceMin:minPriceConverted, 
                priceMax:maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                subscriptionBy: (ArticleSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Articles = _articleModelFactory.PrepareArticleOverviewModels(articles).ToList();

            model.PagingFilteringContext.LoadPagedList(articles);

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds != null ? filterableSpecificationAttributeOptionIds.ToArray() : null, 
                _specificationAttributeService, 
                _webHelper, 
                _workContext,
                _cacheManager);
            
            return model;
        }

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        public virtual string PrepareCategoryTemplateViewPath(int templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _categoryTemplateService.GetCategoryTemplateById(templateId);
                if (template == null)
                    template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return templateViewPath;
        }

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentArticleId">Current article identifier</param>
        /// <returns>Category navigation model</returns>
        public virtual CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId, int currentArticleId)
        {
            //get active category
            int activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentArticleId > 0)
            {
                //article details page
                var articleCategories = _categoryService.GetArticleCategoriesByArticleId(currentArticleId);
                if (articleCategories.Any())
                    activeCategoryId = articleCategories[0].CategoryId;
            }

            var cachedCategoriesModel = PrepareCategorySimpleModels();
            var model = new CategoryNavigationModel
            {
                CurrentCategoryId = activeCategoryId,
                Categories = cachedCategoriesModel
            };

            return model;
        }

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        public virtual TopMenuModel PrepareTopMenuModel()
        {
            //categories
            var cachedCategoriesModel = PrepareCategorySimpleModels();

            //top menu topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY, 
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new TopMenuModel.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName()
                })
                .ToList()
            );
            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
                NewArticlesEnabled = _catalogSettings.NewArticlesEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                DisplayHomePageMenuItem = _displayDefaultMenuItemSettings.DisplayHomePageMenuItem,
                DisplayNewArticlesMenuItem = _displayDefaultMenuItemSettings.DisplayNewArticlesMenuItem,
                DisplayArticleSearchMenuItem = _displayDefaultMenuItemSettings.DisplayArticleSearchMenuItem,
                DisplayCustomerInfoMenuItem = _displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
                DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem
            };
            return model;
        }

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        public virtual List<CategoryModel> PrepareHomepageCategoryModels()
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), 
                pictureSize,
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id, 
                _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(category =>
                {
                    var catModel = new CategoryModel
                    {
                        Id = category.Id,
                        Name = category.GetLocalized(x => x.Name),
                        Description = category.GetLocalized(x => x.Description),
                        MetaKeywords = category.GetLocalized(x => x.MetaKeywords),
                        MetaDescription = category.GetLocalized(x => x.MetaDescription),
                        MetaTitle = category.GetLocalized(x => x.MetaTitle),
                        SeName = category.GetSeName(),
                    };

                    //prepare picture model
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, category.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(category.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                        };
                        return pictureModel;
                    });

                    return catModel;
                })
                .ToList()
            );

            return model;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels()
        {
            //load and cache them
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once
                allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //number of articles in each category
                if (_catalogSettings.ShowCategoryArticleNumber)
                {
                    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_ARTICLES_MODEL_KEY,
                        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id,
                        category.Id);
                    categoryModel.NumberOfArticles = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>();
                        categoryIds.Add(category.Id);
                        //include subcategories
                        if (_catalogSettings.ShowCategoryArticleNumberIncludingSubcategories)
                            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                        return _articleService.GetNumberOfArticlesInCategory(categoryIds, _storeContext.CurrentStore.Id);
                    });
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        #endregion

        #region Publishers

        /// <summary>
        /// Prepare publisher model
        /// </summary>
        /// <param name="publisher">Publisher identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Publisher model</returns>
        public virtual PublisherModel PreparePublisherModel(Publisher publisher, CatalogPagingFilteringModel command)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            var model = new PublisherModel
            {
                Id = publisher.Id,
                Name = publisher.GetLocalized(x => x.Name),
                Description = publisher.GetLocalized(x => x.Description),
                MetaKeywords = publisher.GetLocalized(x => x.MetaKeywords),
                MetaDescription = publisher.GetLocalized(x => x.MetaDescription),
                MetaTitle = publisher.GetLocalized(x => x.MetaTitle),
                SeName = publisher.GetSeName(),
            };

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                publisher.AllowCustomersToSelectPageSize,
                publisher.PageSizeOptions,
                publisher.PageSize);


            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(publisher.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, publisher.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }



            //featured articles
            if (!_catalogSettings.IgnoreFeaturedArticles)
            {
                IPagedList<Article> featuredArticles = null;

                //We cache a value indicating whether we have featured articles
                string cacheKey = string.Format(ModelCacheEventConsumer.PUBLISHER_HAS_FEATURED_ARTICLES_KEY, 
                    publisher.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                var hasFeaturedArticlesCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedArticlesCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load articles and cache the result (true/false)
                    featuredArticles = _articleService.SearchArticles(
                       publisherId: publisher.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredArticles: true);
                    hasFeaturedArticlesCache = featuredArticles.TotalCount > 0;
                    _cacheManager.Set(cacheKey, hasFeaturedArticlesCache, 60);
                }
                if (hasFeaturedArticlesCache.Value && featuredArticles == null)
                {
                    //cache indicates that the publisher has featured articles
                    //let's load them
                    featuredArticles = _articleService.SearchArticles(
                       publisherId: publisher.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredArticles: true);
                }
                if (featuredArticles != null)
                {
                    model.FeaturedArticles = _articleModelFactory.PrepareArticleOverviewModels(featuredArticles).ToList();
                }
            }



            //articles
            IList<int> filterableSpecificationAttributeOptionIds;
            var articles = _articleService.SearchArticles(out filterableSpecificationAttributeOptionIds, true,
                publisherId: publisher.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredArticles: _catalogSettings.IncludeFeaturedArticlesInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted, 
                priceMax: maxPriceConverted,
                subscriptionBy: (ArticleSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Articles = _articleModelFactory.PrepareArticleOverviewModels(articles).ToList();

            model.PagingFilteringContext.LoadPagedList(articles);

            return model;
        }

        /// <summary>
        /// Prepare publisher template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Publisher template view path</returns>
        public virtual string PreparePublisherTemplateViewPath(int templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.PUBLISHER_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _publisherTemplateService.GetPublisherTemplateById(templateId);
                if (template == null)
                    template = _publisherTemplateService.GetAllPublisherTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return templateViewPath;
        }

        /// <summary>
        /// Prepare publisher all models
        /// </summary>
        /// <returns>List of publisher models</returns>
        public virtual List<PublisherModel> PreparePublisherAllModels()
        {
            var model = new List<PublisherModel>();
            var publishers = _publisherService.GetAllPublishers(storeId: _storeContext.CurrentStore.Id);
            foreach (var publisher in publishers)
            {
                var modelMan = new PublisherModel
                {
                    Id = publisher.Id,
                    Name = publisher.GetLocalized(x => x.Name),
                    Description = publisher.GetLocalized(x => x.Description),
                    MetaKeywords = publisher.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = publisher.GetLocalized(x => x.MetaDescription),
                    MetaTitle = publisher.GetLocalized(x => x.MetaTitle),
                    SeName = publisher.GetSeName(),
                };

                //prepare picture model
                int pictureSize = _mediaSettings.PublisherThumbPictureSize;
                var publisherPictureCacheKey = string.Format(ModelCacheEventConsumer.PUBLISHER_PICTURE_MODEL_KEY, publisher.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                modelMan.PictureModel = _cacheManager.Get(publisherPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(publisher.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Publisher.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Publisher.ImageAlternateTextFormat"), modelMan.Name)
                    };
                    return pictureModel;
                });
                model.Add(modelMan);
            }

            return model;
        }

        /// <summary>
        /// Prepare publisher navigation model
        /// </summary>
        /// <param name="currentPublisherId">Current publisher identifier</param>
        /// <returns>Publisher navigation model</returns>
        public virtual PublisherNavigationModel PreparePublisherNavigationModel(int currentPublisherId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.PUBLISHER_NAVIGATION_MODEL_KEY, 
                currentPublisherId, 
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var currentPublisher = _publisherService.GetPublisherById(currentPublisherId);

                var publishers = _publisherService.GetAllPublishers(storeId: _storeContext.CurrentStore.Id,
                    pageSize: _catalogSettings.PublishersBlockItemsToDisplay);
                var model = new PublisherNavigationModel
                {
                    TotalPublishers = publishers.TotalCount
                };

                foreach (var publisher in publishers)
                {
                    var modelMan = new PublisherBriefInfoModel
                    {
                        Id = publisher.Id,
                        Name = publisher.GetLocalized(x => x.Name),
                        SeName = publisher.GetSeName(),
                        IsActive = currentPublisher != null && currentPublisher.Id == publisher.Id,
                    };
                    model.Publishers.Add(modelMan);
                }
                return model;
            });
            
            
            return cachedModel;
        }

        #endregion

        #region Contributors

        /// <summary>
        /// Prepare contributor model
        /// </summary>
        /// <param name="contributor">Contributor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Contributor model</returns>
        public virtual ContributorModel PrepareContributorModel(Contributor contributor, CatalogPagingFilteringModel command)
        {
            if (contributor == null)
                throw new ArgumentNullException("contributor");

            var model = new ContributorModel
            {
                Id = contributor.Id,
                Name = contributor.GetLocalized(x => x.Name),
                Description = contributor.GetLocalized(x => x.Description),
                MetaKeywords = contributor.GetLocalized(x => x.MetaKeywords),
                MetaDescription = contributor.GetLocalized(x => x.MetaDescription),
                MetaTitle = contributor.GetLocalized(x => x.MetaTitle),
                SeName = contributor.GetSeName(),
                AllowCustomersToContactContributors = _contributorSettings.AllowCustomersToContactContributors
            };



            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                contributor.AllowCustomersToSelectPageSize,
                contributor.PageSizeOptions,
                contributor.PageSize);

            //articles
            IList<int> filterableSpecificationAttributeOptionIds;
            var articles = _articleService.SearchArticles(out filterableSpecificationAttributeOptionIds, true,
                contributorId: contributor.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                subscriptionBy: (ArticleSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Articles = _articleModelFactory.PrepareArticleOverviewModels(articles).ToList();

            model.PagingFilteringContext.LoadPagedList(articles);

            return model;
        }

        /// <summary>
        /// Prepare contributor all models
        /// </summary>
        /// <returns>List of contributor models</returns>
        public virtual List<ContributorModel> PrepareContributorAllModels()
        {
            var model = new List<ContributorModel>();
            var contributors = _contributorService.GetAllContributors();
            foreach (var contributor in contributors)
            {
                var contributorModel = new ContributorModel
                {
                    Id = contributor.Id,
                    Name = contributor.GetLocalized(x => x.Name),
                    Description = contributor.GetLocalized(x => x.Description),
                    MetaKeywords = contributor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = contributor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = contributor.GetLocalized(x => x.MetaTitle),
                    SeName = contributor.GetSeName(),
                    AllowCustomersToContactContributors = _contributorSettings.AllowCustomersToContactContributors
                };

                //prepare picture model
                int pictureSize = _mediaSettings.ContributorThumbPictureSize;
                var pictureCacheKey = string.Format(ModelCacheEventConsumer.CONTRIBUTOR_PICTURE_MODEL_KEY, contributor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                contributorModel.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(contributor.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Contributor.ImageLinkTitleFormat"), contributorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Contributor.ImageAlternateTextFormat"), contributorModel.Name)
                    };
                    return pictureModel;
                });
                model.Add(contributorModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare contributor navigation model
        /// </summary>
        /// <returns>Contributor navigation model</returns>
        public virtual ContributorNavigationModel PrepareContributorNavigationModel()
        {
            string cacheKey = ModelCacheEventConsumer.CONTRIBUTOR_NAVIGATION_MODEL_KEY;
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var contributors = _contributorService.GetAllContributors(pageSize: _contributorSettings.ContributorsBlockItemsToDisplay);
                var model = new ContributorNavigationModel
                {
                    TotalContributors = contributors.TotalCount
                };

                foreach (var contributor in contributors)
                {
                    model.Contributors.Add(new ContributorBriefInfoModel
                    {
                        Id = contributor.Id,
                        Name = contributor.GetLocalized(x => x.Name),
                        SeName = contributor.GetSeName(),
                    });
                }
                return model;
            });
            
            return cachedModel;
        }

        #endregion

        #region Article tags

        /// <summary>
        /// Prepare popular article tags model
        /// </summary>
        /// <returns>Article tags model</returns>
        public virtual PopularArticleTagsModel PreparePopularArticleTagsModel()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.ARTICLETAG_POPULAR_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new PopularArticleTagsModel();

                //get all tags
                var allTags = _articleTagService
                    .GetAllArticleTags()
                    //filter by current store
                    .Where(x => _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                    //subscription by article count
                    .OrderByDescending(x => _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id))
                    .ToList();

                var tags = allTags
                    .Take(_catalogSettings.NumberOfArticleTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => x.GetLocalized(y => y.Name)).ToList();

                model.TotalTags = allTags.Count;

                foreach (var tag in tags)
                    model.Tags.Add(new ArticleTagModel
                    {
                        Id = tag.Id,
                        Name = tag.GetLocalized(y => y.Name),
                        SeName = tag.GetSeName(),
                        ArticleCount = _articleTagService.GetArticleCount(tag.Id, _storeContext.CurrentStore.Id)
                    });
                return model;
            });

            return cachedModel;
        }

        /// <summary>
        /// Prepare articles by tag model
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Articles by tag model</returns>
        public virtual ArticlesByTagModel PrepareArticlesByTagModel(ArticleTag articleTag, CatalogPagingFilteringModel command)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            var model = new ArticlesByTagModel
            {
                Id = articleTag.Id,
                TagName = articleTag.GetLocalized(y => y.Name),
                TagSeName = articleTag.GetSeName()
            };


            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.ArticlesByTagAllowCustomersToSelectPageSize,
                _catalogSettings.ArticlesByTagPageSizeOptions,
                _catalogSettings.ArticlesByTagPageSize);


            //articles
            var articles = _articleService.SearchArticles(
                storeId: _storeContext.CurrentStore.Id,
                articleTagId: articleTag.Id,
                visibleIndividuallyOnly: true,
                subscriptionBy: (ArticleSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Articles = _articleModelFactory.PrepareArticleOverviewModels(articles).ToList();

            model.PagingFilteringContext.LoadPagedList(articles);
            return model;
        }

        /// <summary>
        /// Prepare article tags all model
        /// </summary>
        /// <returns>Popular article tags model</returns>
        public virtual PopularArticleTagsModel PrepareArticleTagsAllModel()
        {
            var model = new PopularArticleTagsModel();
            model.Tags = _articleTagService
                .GetAllArticleTags()
                //filter by current store
                .Where(x => _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                //sort by name
                .OrderBy(x => x.GetLocalized(y => y.Name))
                .Select(x =>
                {
                    var ptModel = new ArticleTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        ArticleCount = _articleTagService.GetArticleCount(x.Id, _storeContext.CurrentStore.Id)
                    };
                    return ptModel;
                })
                .ToList();
            return model;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Search model</returns>
        public virtual SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions,
                _catalogSettings.SearchPageArticlesPerPage);


            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY, 
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), 
                _storeContext.CurrentStore.Id); 
            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                foreach (var c in allCategories)
                {
                    //generate full category name (breadcrumb)
                    string categoryBreadcrumb= "";
                    var breadcrumb = c.GetCategoryBreadCrumb(allCategories, _aclService, _storeMappingService);
                    for (int i = 0; i <= breadcrumb.Count - 1; i++)
                    {
                        categoryBreadcrumb += breadcrumb[i].GetLocalized(x => x.Name);
                        if (i != breadcrumb.Count - 1)
                            categoryBreadcrumb += " >> ";
                    }
                    categoriesModel.Add(new SearchModel.CategoryModel
                    {
                        Id = c.Id,
                        Breadcrumb = categoryBreadcrumb
                    });
                }
                return categoriesModel;
            });
            if (categories.Any())
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                    {
                         Value = "0",
                         Text = _localizationService.GetResource("Common.All")
                    });
                //all other categories
                foreach (var c in categories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var publishers = _publisherService.GetAllPublishers(storeId: _storeContext.CurrentStore.Id);
            if (publishers.Any())
            {
                model.AvailablePublishers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in publishers)
                    model.AvailablePublishers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.GetLocalized(x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            model.asv = _contributorSettings.AllowSearchByContributor;
            if (model.asv)
            {
                var contributors = _contributorService.GetAllContributors();
                if (contributors.Any())
                {
                    model.AvailableContributors.Add(new SelectListItem
                    {
                        Value = "0",
                        Text = _localizationService.GetResource("Common.All")
                    });
                    foreach (var contributor in contributors)
                        model.AvailableContributors.Add(new SelectListItem
                        {
                            Value = contributor.Id.ToString(),
                            Text = contributor.GetLocalized(x => x.Name),
                            Selected = model.vid == contributor.Id
                        });
                }
            }

            IPagedList<Article> articles = new PagedList<Article>(new List<Article>(), 0, 1);
            var isSearchTermSpecified = false;
            try
            {
                // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
                isSearchTermSpecified = _httpContext.Request.Params["q"] != null;
            }
            catch
            {
                //the "A potentially dangerous Request.QueryString value was detected from the client" exception could be thrown here when some wrong char is specified (e.g. <)
                //although we [ValidateInput(false)] attribute here we try to access "Request.Params" directly
                //that's why we do not re-throw it

                //just ensure that some search term is specified (0 length is not supported inthis case)
                isSearchTermSpecified = !String.IsNullOrEmpty(searchTerms);
            }
            if (isSearchTermSpecified)
            {
                if (searchTerms.Length < _catalogSettings.ArticleSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ArticleSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    int publisherId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    int contributorId = 0;
                    if (model.adv)
                    {
                        //advanced search
                        var categoryId = model.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId));
                            }
                        }

                        publisherId = model.mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.pf))
                        {
                            decimal minPrice;
                            if (decimal.TryParse(model.pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(model.pt))
                        {
                            decimal maxPrice;
                            if (decimal.TryParse(model.pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        if (model.asv)
                            contributorId = model.vid;

                        searchInDescriptions = model.sid;
                    }
                    
                    //var searchInArticleTags = false;
                    var searchInArticleTags = searchInDescriptions;

                    //articles
                    articles = _articleService.SearchArticles(
                        categoryIds: categoryIds,
                        publisherId: publisherId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchArticleTags: searchInArticleTags,
                        languageId: _workContext.WorkingLanguage.Id,
                        subscriptionBy: (ArticleSortingEnum)command.OrderBy,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize,
                        contributorId: contributorId);
                    model.Articles = _articleModelFactory.PrepareArticleOverviewModels(articles).ToList();

                    model.NoResults = !model.Articles.Any();

                    //search term statistics
                    if (!String.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = _storeContext.CurrentStore.Id,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ArticleSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        PublisherId = publisherId,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id,
                        ContributorId = contributorId
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(articles);
            return model;
        }

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>Search box model</returns>
        public virtual SearchBoxModel PrepareSearchBoxModel()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.ArticleSearchAutoCompleteEnabled,
                ShowArticleImagesInSearchAutoComplete = _catalogSettings.ShowArticleImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.ArticleSearchTermMinimumLength
            };
            return model;
        }

        #endregion
    }
}
