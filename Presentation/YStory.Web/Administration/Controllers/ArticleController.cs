using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Helpers;
using YStory.Admin.Infrastructure.Cache;
using YStory.Admin.Models.Catalog;
using YStory.Admin.Models.Subscriptions;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Services;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Configuration;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.ExportImport;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Services.Contributors;
using YStory.Web.Framework;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class ArticleController : BaseAdminController
    {
        #region Fields

        private readonly IArticleService _articleService;
        private readonly IArticleTemplateService _articleTemplateService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPictureService _pictureService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICopyArticleService _copyArticleService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IContributorService _contributorService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IArticleAttributeFormatter _articleAttributeFormatter;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly ISettingService _settingService;
        private readonly TaxSettings _taxSettings;
        private readonly ContributorSettings _contributorSettings;

        #endregion

        #region Constructors

        public ArticleController(IArticleService articleService,
            IArticleTemplateService articleTemplateService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ISpecificationAttributeService specificationAttributeService,
            IPictureService pictureService,
            ITaxCategoryService taxCategoryService,
            IArticleTagService articleTagService,
            ICopyArticleService copyArticleService,
            IPdfService pdfService,
            IExportManager exportManager,
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            ISubscriptionService subscriptionService,
            IStoreMappingService storeMappingService,
            IContributorService contributorService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            ICacheManager cacheManager,
            IDateTimeHelper dateTimeHelper,
            IArticleAttributeService articleAttributeService,
            IShoppingCartService shoppingCartService,
            IArticleAttributeFormatter articleAttributeFormatter,
            IArticleAttributeParser articleAttributeParser,
            IDownloadService downloadService,
            ISettingService settingService,
            TaxSettings taxSettings,
            ContributorSettings contributorSettings)
        {
            this._articleService = articleService;
            this._articleTemplateService = articleTemplateService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._specificationAttributeService = specificationAttributeService;
            this._pictureService = pictureService;
            this._taxCategoryService = taxCategoryService;
            this._articleTagService = articleTagService;
            this._copyArticleService = copyArticleService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._subscriptionService = subscriptionService;
            this._storeMappingService = storeMappingService;
            this._contributorService = contributorService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._articleAttributeService = articleAttributeService;
            this._shoppingCartService = shoppingCartService;
            this._articleAttributeFormatter = articleAttributeFormatter;
            this._articleAttributeParser = articleAttributeParser;
            this._downloadService = downloadService;
            this._settingService = settingService;
            this._taxSettings = taxSettings;
            this._contributorSettings = contributorSettings;
        }

        #endregion 

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Article article, ArticleModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.FullDescription,
                                                               localized.FullDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.MetaKeywords,
                                                               localized.MetaKeywords,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.MetaDescription,
                                                               localized.MetaDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(article,
                                                               x => x.MetaTitle,
                                                               localized.MetaTitle,
                                                               localized.LanguageId);

                //search engine name
                var seName = article.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(article, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(ArticleTag articleTag, ArticleTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(articleTag,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(ArticleAttributeValue pav, ArticleModel.ArticleAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pav,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Article article)
        {
            foreach (var pp in article.ArticlePictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(article.Name));
        }

        [NonAction]
        protected virtual void PrepareAclModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && article != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(article).ToList();

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
        protected virtual void SaveArticleAcl(Article article, ArticleModel model)
        {
            article.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(article);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(article, customerRole.Id);
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
        protected virtual void PrepareStoresMappingModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && article != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(article).ToList();

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
        protected virtual void SaveStoreMappings(Article article, ArticleModel model)
        {
            article.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(article);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(article, store.Id);
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

        [NonAction]
        protected virtual void PrepareCategoryMappingModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && article != null)
                model.SelectedCategoryIds = _categoryService.GetArticleCategoriesByArticleId(article.Id, true).Select(c => c.CategoryId).ToList();

            var allCategories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in allCategories)
            {
                c.Selected = model.SelectedCategoryIds.Contains(int.Parse(c.Value));
                model.AvailableCategories.Add(c);
            }
        }

        [NonAction]
        protected virtual void SaveCategoryMappings(Article article, ArticleModel model)
        {
            var existingArticleCategories = _categoryService.GetArticleCategoriesByArticleId(article.Id, true);

            //delete categories
            foreach (var existingArticleCategory in existingArticleCategories)
                if (!model.SelectedCategoryIds.Contains(existingArticleCategory.CategoryId))
                    _categoryService.DeleteArticleCategory(existingArticleCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
                if (existingArticleCategories.FindArticleCategory(article.Id, categoryId) == null)
                {
                    //find next display subscription
                    var displaySubscription = 1;
                    var existingCategoryMapping = _categoryService.GetArticleCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displaySubscription = existingCategoryMapping.Max(x => x.DisplaySubscription) + 1;
                    _categoryService.InsertArticleCategory(new ArticleCategory
                    {
                        ArticleId = article.Id,
                        CategoryId = categoryId,
                        DisplaySubscription = displaySubscription
                    });
                }
        }

        [NonAction]
        protected virtual void PreparePublisherMappingModel(ArticleModel model, Article article, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && article != null)
                model.SelectedPublisherIds = _publisherService.GetArticlePublishersByArticleId(article.Id, true).Select(c => c.PublisherId).ToList();

            var allPublishers = SelectListHelper.GetPublisherList(_publisherService, _cacheManager, true);
            foreach (var m in allPublishers)
            {
                m.Selected = model.SelectedPublisherIds.Contains(int.Parse(m.Value));
                model.AvailablePublishers.Add(m);
            }
        }

        [NonAction]
        protected virtual void SavePublisherMappings(Article article, ArticleModel model)
        {
            var existingArticlePublishers = _publisherService.GetArticlePublishersByArticleId(article.Id, true);

            //delete publishers
            foreach (var existingArticlePublisher in existingArticlePublishers)
                if (!model.SelectedPublisherIds.Contains(existingArticlePublisher.PublisherId))
                    _publisherService.DeleteArticlePublisher(existingArticlePublisher);

            //add publishers
            foreach (var publisherId in model.SelectedPublisherIds)
                if (existingArticlePublishers.FindArticlePublisher(article.Id, publisherId) == null)
                {
                    //find next display subscription
                    var displaySubscription = 1;
                    var existingPublisherMapping = _publisherService.GetArticlePublishersByPublisherId(publisherId, showHidden: true);
                    if (existingPublisherMapping.Any())
                        displaySubscription = existingPublisherMapping.Max(x => x.DisplaySubscription) + 1;
                    _publisherService.InsertArticlePublisher(new ArticlePublisher()
                    {
                        ArticleId = article.Id,
                        PublisherId = publisherId,
                        DisplaySubscription = displaySubscription
                    });
                }
        }

      

        [NonAction]
        protected virtual void PrepareAddArticleAttributeCombinationModel(AddArticleAttributeCombinationModel model, Article article)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (article == null)
                throw new ArgumentNullException("article");

            model.ArticleId = article.Id;
            model.StockQuantity = 10000;
            model.NotifyAdminForQuantityBelow = 1;

            var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id)
                //ignore non-combinable attributes for combinations
                .Where(x => !x.IsNonCombinable())
                .ToList();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddArticleAttributeCombinationModel.ArticleAttributeModel
                {
                    Id = attribute.Id,
                    ArticleAttributeId = attribute.ArticleAttributeId,
                    Name = attribute.ArticleAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddArticleAttributeCombinationModel.ArticleAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                model.ArticleAttributes.Add(attributeModel);
            }
        }

        [NonAction]
        protected virtual string[] ParseArticleTags(string articleTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(articleTags))
            {
                string[] values = articleTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val1 in values)
                    if (!String.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }
        
        [NonAction]
        protected virtual void PrepareArticleModel(ArticleModel model, Article article, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (article != null)
            {
                var parentGroupedArticle = _articleService.GetArticleById(article.ParentGroupedArticleId);
                if (parentGroupedArticle != null)
                {
                    model.AssociatedToArticleId = article.ParentGroupedArticleId;
                    model.AssociatedToArticleName = parentGroupedArticle.Name;
                }

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(article.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(article.UpdatedOnUtc, DateTimeKind.Utc);
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.BaseDimensionIn = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId).Name;
            
            //little performance hack here
            //there's no need to load attributes when creating a new article
            //anyway they're not used (you need to save a article before you map them)
            if (article != null)
            {
                //article attributes
                foreach (var articleAttribute in _articleAttributeService.GetAllArticleAttributes())
                {
                    model.AvailableArticleAttributes.Add(new SelectListItem
                    {
                        Text = articleAttribute.Name,
                        Value = articleAttribute.Id.ToString()
                    });
                }

                //specification attributes
                model.AddSpecificationAttributeModel.AvailableAttributes = _cacheManager
                    .Get(ModelCacheEventConsumer.SPEC_ATTRIBUTES_MODEL_KEY, () =>
                    {
                        var availableSpecificationAttributes = new List<SelectListItem>();
                        foreach (var sa in _specificationAttributeService.GetSpecificationAttributes())
                        {
                            availableSpecificationAttributes.Add(new SelectListItem
                            {
                                Text = sa.Name,
                                Value = sa.Id.ToString()
                            });
                        }
                        return availableSpecificationAttributes;
                    });

                //options of preselected specification attribute
                if (model.AddSpecificationAttributeModel.AvailableAttributes.Any())
                {
                    var selectedAttributeId = int.Parse(model.AddSpecificationAttributeModel.AvailableAttributes.First().Value);
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(selectedAttributeId))
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem
                        {
                            Text = sao.Name,
                            Value = sao.Id.ToString()
                        });
                }
                //default specs values
                model.AddSpecificationAttributeModel.ShowOnArticlePage = true;
            }


            //copy article
            if (article != null)
            {
                model.CopyArticleModel.Id = article.Id;
                model.CopyArticleModel.Name = string.Format(_localizationService.GetResource("Admin.Catalog.Articles.Copy.Name.New"), article.Name);
                model.CopyArticleModel.Published = true;
                model.CopyArticleModel.CopyImages = true;
            }

            //templates
            var templates = _articleTemplateService.GetAllArticleTemplates();
            foreach (var template in templates)
            {
                model.AvailableArticleTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
            //supported article types
            foreach (var articleType in ArticleType.SimpleArticle.ToSelectList(false).ToList())
            {
                var articleTypeId = int.Parse(articleType.Value);
                model.ArticlesTypesSupportedByArticleTemplates.Add(articleTypeId, new List<SelectListItem>());
                foreach (var template in templates)
                {
                    if (String.IsNullOrEmpty(template.IgnoredArticleTypes) ||
                        !((IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredArticleTypes)).Contains(articleTypeId))
                    {
                        model.ArticlesTypesSupportedByArticleTemplates[articleTypeId].Add(new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        });
                    }
                }
            }

            //contributors
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            model.AvailableContributors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Articles.Fields.Contributor.None"),
                Value = "0"
            });
            var contributors = SelectListHelper.GetContributorList(_contributorService, _cacheManager, true);
            foreach (var v in contributors)
                model.AvailableContributors.Add(v);

            //delivery dates
            model.AvailableDeliveryDates.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Articles.Fields.DeliveryDate.None"),
                Value = "0"
            });
            

            //article availability ranges
            model.AvailableArticleAvailabilityRanges.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Articles.Fields.ArticleAvailabilityRange.None"),
                Value = "0"
            });
           

             

            //article tags
            if (article != null)
            {
                var result = new StringBuilder();
                for (int i = 0; i < article.ArticleTags.Count; i++)
                {
                    var pt = article.ArticleTags.ToList()[i];
                    result.Append(pt.Name);
                    if (i != article.ArticleTags.Count - 1)
                        result.Append(", ");
                }
                model.ArticleTags = result.ToString();
            }

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"), Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = article != null && !setPredefinedValues && tc.Id == article.TaxCategoryId });

            //baseprice units
            var measureWeights = _measureService.GetAllMeasureWeights();
            foreach (var mw in measureWeights)
                model.AvailableBasepriceUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = article != null });
            foreach (var mw in measureWeights)
                model.AvailableBasepriceBaseUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = article != null  });

           

            //default values
            if (setPredefinedValues)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.RentalPriceLength = 1;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.SubscriptionMinimumQuantity = 1;
                model.SubscriptionMaximumQuantity = 10000;
                model.TaxCategoryId = _taxSettings.DefaultTaxCategoryId;
                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.AllowCustomerReviews = true;
                model.Published = true;
                model.VisibleIndividually = true;
            }

            //editor settings
            var articleEditorSettings = _settingService.LoadSetting<ArticleEditorSettings>();
            model.ArticleEditorSettingsModel = articleEditorSettings.ToModel();
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

       
        #endregion

        #region Methods

        #region Article list / create / edit / delete

        //list articles
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleListModel();
            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            model.AllowContributorsToImportArticles = _contributorSettings.AllowContributorsToImportArticles;

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

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Articles.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Articles.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Articles.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ArticleList(DataSourceRequest command, ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var articles = _articleService.SearchArticles(
                categoryIds: categoryIds,
                publisherId: model.SearchPublisherId,
                storeId: model.SearchStoreId,
                contributorId: model.SearchContributorId,
                warehouseId: model.SearchWarehouseId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult();
            gridModel.Data = articles.Select(x =>
            {
                var articleModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                articleModel.FullDescription = "";

                //picture
                var defaultArticlePicture = _pictureService.GetPicturesByArticleId(x.Id, 1).FirstOrDefault();
                articleModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultArticlePicture, 75, true);
                //article type
                articleModel.ArticleTypeName = x.ArticleType.GetLocalizedEnum(_localizationService, _workContext);
                //friendly stock qantity
            
                return articleModel;
            });
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-article-by-sku")]
        public virtual ActionResult GoToSku(ArticleListModel model)
        {
            string sku = model.GoDirectlyToSku;

            //try to load a article entity
            var article = _articleService.GetArticleBySku(sku);

            //if not found, then try to load a article attribute combination
            if (article == null)
            {
                var combination = _articleAttributeService.GetArticleAttributeCombinationBySku(sku);
                if (combination != null)
                {
                    article = combination.Article;
                }
            }

            if (article != null)
                return RedirectToAction("Edit", "Article", new { id = article.Id });

            //not found
            return List();
        }

        //create article
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //validate maximum number of articles per contributor
            if (_contributorSettings.MaximumArticleNumber > 0 &&
                _workContext.CurrentContributor != null &&
                _articleService.GetNumberOfArticlesByContributorId(_workContext.CurrentContributor.Id) >= _contributorSettings.MaximumArticleNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Articles.ExceededMaximumNumber"), _contributorSettings.MaximumArticleNumber));
                return RedirectToAction("List");
            }

            var model = new ArticleModel();

            PrepareArticleModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            PrepareStoresMappingModel(model, null, false);
            PrepareCategoryMappingModel(model, null, false);
            PreparePublisherMappingModel(model, null, false);
            

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(ArticleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //validate maximum number of articles per contributor
            if (_contributorSettings.MaximumArticleNumber > 0 &&
                _workContext.CurrentContributor != null &&
                _articleService.GetNumberOfArticlesByContributorId(_workContext.CurrentContributor.Id) >= _contributorSettings.MaximumArticleNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Articles.ExceededMaximumNumber"), _contributorSettings.MaximumArticleNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a contributor should have access only to his articles
                if (_workContext.CurrentContributor != null)
                {
                    model.ContributorId = _workContext.CurrentContributor.Id;
                }

                //contributors cannot edit "Show on home page" property
                if (_workContext.CurrentContributor != null && model.ShowOnHomePage)
                {
                    model.ShowOnHomePage = false;
                }

                //article
                var article = model.ToEntity();
                article.CreatedOnUtc = DateTime.UtcNow;
                article.UpdatedOnUtc = DateTime.UtcNow;
                _articleService.InsertArticle(article);
                //search engine name
                model.SeName = article.ValidateSeName(model.SeName, article.Name, true);
                _urlRecordService.SaveSlug(article, model.SeName, 0);
                //locales
                UpdateLocales(article, model);
                //categories
                SaveCategoryMappings(article, model);
                //publishers
                SavePublisherMappings(article, model);
                //ACL (customer roles)
                SaveArticleAcl(article, model);
                //stores
               
                //tags
                _articleTagService.UpdateArticleTags(article, ParseArticleTags(model.ArticleTags));
              

               

                //activity log
                _customerActivityService.InsertActivity("AddNewArticle", _localizationService.GetResource("ActivityLog.AddNewArticle"), article.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Articles.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = article.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareArticleModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            PrepareStoresMappingModel(model, null, true);
            PrepareCategoryMappingModel(model, null, true);
            PreparePublisherMappingModel(model, null, true);
           

            return View(model);
        }

        //edit article
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(id);
            if (article == null || article.Deleted)
                //No article found with the specified id
                return RedirectToAction("List");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            var model = article.ToModel();
            PrepareArticleModel(model, article, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = article.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = article.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.FullDescription = article.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.MetaKeywords = article.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = article.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = article.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = article.GetSeName(languageId, false, false);
            });

            PrepareAclModel(model, article, false);
            PrepareStoresMappingModel(model, article, false);
            PrepareCategoryMappingModel(model, article, false);
            PreparePublisherMappingModel(model, article, false);
             

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ArticleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(model.Id);

            if (article == null || article.Deleted)
                //No article found with the specified id
                return RedirectToAction("List");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            

            if (ModelState.IsValid)
            {
                //a contributor should have access only to his articles
                if (_workContext.CurrentContributor != null)
                {
                    model.ContributorId = _workContext.CurrentContributor.Id;
                }

                //we do not validate maximum number of articles per contributor when editing existing articles (only during creation of new articles)

                //contributors cannot edit "Show on home page" property
                if (_workContext.CurrentContributor != null && model.ShowOnHomePage != article.ShowOnHomePage)
                {
                    model.ShowOnHomePage = article.ShowOnHomePage;
                }
                //some previously used values
                var prevTotalStockQuantity = article.GetTotalStockQuantity();
                

                //article
                article = model.ToEntity(article);

                article.UpdatedOnUtc = DateTime.UtcNow;
                _articleService.UpdateArticle(article);
                //search engine name
                model.SeName = article.ValidateSeName(model.SeName, article.Name, true);
                _urlRecordService.SaveSlug(article, model.SeName, 0);
                //locales
                UpdateLocales(article, model);
                //tags
                _articleTagService.UpdateArticleTags(article, ParseArticleTags(model.ArticleTags));
           
                //categories
                SaveCategoryMappings(article, model);
                //publishers
                SavePublisherMappings(article, model);
                //ACL (customer roles)
                SaveArticleAcl(article, model);
                //stores
                SaveStoreMappings(article, model);
               
                //picture seo names
                UpdatePictureSeoNames(article);
  

                //activity log
                _customerActivityService.InsertActivity("EditArticle", _localizationService.GetResource("ActivityLog.EditArticle"), article.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Articles.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = article.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareArticleModel(model, article, false, true);
            PrepareAclModel(model, article, true);
            PrepareStoresMappingModel(model, article, true);
            PrepareCategoryMappingModel(model, article, true);
            PreparePublisherMappingModel(model, article, true);
           

            return View(model);
        }

        //delete article
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(id);
            if (article == null)
                //No article found with the specified id
                return RedirectToAction("List");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            _articleService.DeleteArticle(article);

            //activity log
            _customerActivityService.InsertActivity("DeleteArticle", _localizationService.GetResource("ActivityLog.DeleteArticle"), article.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Articles.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _articleService.DeleteArticles(_articleService.GetArticlesByIds(selectedIds.ToArray()).Where(p => _workContext.CurrentContributor == null || p.ContributorId == _workContext.CurrentContributor.Id).ToList());
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult CopyArticle(ArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var copyModel = model.CopyArticleModel;
            try
            {
                var originalArticle = _articleService.GetArticleById(copyModel.Id);

                //a contributor should have access only to his articles
                if (_workContext.CurrentContributor != null && originalArticle.ContributorId != _workContext.CurrentContributor.Id)
                    return RedirectToAction("List");

                var newArticle = _copyArticleService.CopyArticle(originalArticle,
                    copyModel.Name, copyModel.Published, copyModel.CopyImages);
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Articles.Copied"));
                return RedirectToAction("Edit", new { id = newArticle.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        #endregion

        #region Required articles

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult LoadArticleFriendlyNames(string articleIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return Json(new { Text = result });

            if (!String.IsNullOrWhiteSpace(articleIds))
            {
                var ids = new List<int>();
                var rangeArray = articleIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                foreach (string str1 in rangeArray)
                {
                    int tmp1;
                    if (int.TryParse(str1, out tmp1))
                        ids.Add(tmp1);
                }

                var articles = _articleService.GetArticlesByIds(ids.ToArray());
                for (int i = 0; i <= articles.Count - 1; i++)
                {
                    result += articles[i].Name;
                    if (i != articles.Count - 1)
                        result += ", ";
                }
            }

            return Json(new { Text = result });
        }

        public virtual ActionResult RequiredArticleAddPopup(string btnId, string articleIdsInput)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleModel.AddRequiredArticleModel();
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


            ViewBag.articleIdsInput = articleIdsInput;
            ViewBag.btnId = btnId;

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult RequiredArticleAddPopupList(DataSourceRequest command, ArticleModel.AddRequiredArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
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

        #endregion

        #region Related articles

        [HttpPost]
        public virtual ActionResult RelatedArticleList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            var relatedArticles = _articleService.GetRelatedArticlesByArticleId1(articleId, true);
            var relatedArticlesModel = relatedArticles
                .Select(x => new ArticleModel.RelatedArticleModel
                {
                    Id = x.Id,
                    //ArticleId1 = x.ArticleId1,
                    ArticleId2 = x.ArticleId2,
                    Article2Name = _articleService.GetArticleById(x.ArticleId2).Name,
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = relatedArticlesModel,
                Total = relatedArticlesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult RelatedArticleUpdate(ArticleModel.RelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(model.Id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(relatedArticle.ArticleId1);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            relatedArticle.DisplaySubscription = model.DisplaySubscription;
            _articleService.UpdateRelatedArticle(relatedArticle);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult RelatedArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var relatedArticle = _articleService.GetRelatedArticleById(id);
            if (relatedArticle == null)
                throw new ArgumentException("No related article found with the specified id");

            var articleId = relatedArticle.ArticleId1;

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            _articleService.DeleteRelatedArticle(relatedArticle);

            return new NullJsonResult();
        }

        public virtual ActionResult RelatedArticleAddPopup(int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleModel.AddRelatedArticleModel();
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
        public virtual ActionResult RelatedArticleAddPopupList(DataSourceRequest command, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
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
        public virtual ActionResult RelatedArticleAddPopup(string btnId, string formId, ArticleModel.AddRelatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                            continue;

                        var existingRelatedArticles = _articleService.GetRelatedArticlesByArticleId1(model.ArticleId);
                        if (existingRelatedArticles.FindRelatedArticle(model.ArticleId, id) == null)
                        {
                            _articleService.InsertRelatedArticle(
                                new RelatedArticle
                                {
                                    ArticleId1 = model.ArticleId,
                                    ArticleId2 = id,
                                    DisplaySubscription = 1
                                });
                        }
                    }
                }
            }

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Cross-sell articles

        [HttpPost]
        public virtual ActionResult CrossSellArticleList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            var crossSellArticles = _articleService.GetCrossSellArticlesByArticleId1(articleId, true);
            var crossSellArticlesModel = crossSellArticles
                .Select(x => new ArticleModel.CrossSellArticleModel
                {
                    Id = x.Id,
                    //ArticleId1 = x.ArticleId1,
                    ArticleId2 = x.ArticleId2,
                    Article2Name = _articleService.GetArticleById(x.ArticleId2).Name,
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = crossSellArticlesModel,
                Total = crossSellArticlesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult CrossSellArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var crossSellArticle = _articleService.GetCrossSellArticleById(id);
            if (crossSellArticle == null)
                throw new ArgumentException("No cross-sell article found with the specified id");

            var articleId = crossSellArticle.ArticleId1;

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            _articleService.DeleteCrossSellArticle(crossSellArticle);

            return new NullJsonResult();
        }

        public virtual ActionResult CrossSellArticleAddPopup(int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleModel.AddCrossSellArticleModel();
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
        public virtual ActionResult CrossSellArticleAddPopupList(DataSourceRequest command, ArticleModel.AddCrossSellArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
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
        public virtual ActionResult CrossSellArticleAddPopup(string btnId, string formId, ArticleModel.AddCrossSellArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                            continue;

                        var existingCrossSellArticles = _articleService.GetCrossSellArticlesByArticleId1(model.ArticleId);
                        if (existingCrossSellArticles.FindCrossSellArticle(model.ArticleId, id) == null)
                        {
                            _articleService.InsertCrossSellArticle(
                                new CrossSellArticle
                                {
                                    ArticleId1 = model.ArticleId,
                                    ArticleId2 = id,
                                });
                        }
                    }
                }
            }

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Associated articles

        [HttpPost]
        public virtual ActionResult AssociatedArticleList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            //a contributor should have access only to his articles
            var contributorId = 0;
            if (_workContext.CurrentContributor != null)
            {
                contributorId = _workContext.CurrentContributor.Id;
            }

            var associatedArticles = _articleService.GetAssociatedArticles(parentGroupedArticleId: articleId,
                contributorId: contributorId,
                showHidden: true);
            var associatedArticlesModel = associatedArticles
                .Select(x => new ArticleModel.AssociatedArticleModel
                {
                    Id = x.Id,
                    ArticleName = x.Name,
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = associatedArticlesModel,
                Total = associatedArticlesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult AssociatedArticleUpdate(ArticleModel.AssociatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var associatedArticle = _articleService.GetArticleById(model.Id);
            if (associatedArticle == null)
                throw new ArgumentException("No associated article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && associatedArticle.ContributorId != _workContext.CurrentContributor.Id)
            {
                return Content("This is not your article");
            }

            associatedArticle.DisplaySubscription = model.DisplaySubscription;
            _articleService.UpdateArticle(associatedArticle);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult AssociatedArticleDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(id);
            if (article == null)
                throw new ArgumentException("No associated article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            article.ParentGroupedArticleId = 0;
            _articleService.UpdateArticle(article);

            return new NullJsonResult();
        }

        public virtual ActionResult AssociatedArticleAddPopup(int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleModel.AddAssociatedArticleModel();
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
        public virtual ActionResult AssociatedArticleAddPopupList(DataSourceRequest command, ArticleModel.AddAssociatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
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
            gridModel.Data = articles.Select(x =>
            {
                var articleModel = x.ToModel();
                //display already associated articles
                var parentGroupedArticle = _articleService.GetArticleById(x.ParentGroupedArticleId);
                if (parentGroupedArticle != null)
                {
                    articleModel.AssociatedToArticleId = x.ParentGroupedArticleId;
                    articleModel.AssociatedToArticleName = parentGroupedArticle.Name;
                }
                return articleModel;
            });
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult AssociatedArticleAddPopup(string btnId, string formId, ArticleModel.AddAssociatedArticleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (model.SelectedArticleIds != null)
            {
                foreach (int id in model.SelectedArticleIds)
                {
                    var article = _articleService.GetArticleById(id);
                    if (article != null)
                    {
                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                            continue;

                        article.ParentGroupedArticleId = model.ArticleId;
                        _articleService.UpdateArticle(article);
                    }
                }
            }

            //a contributor should have access only to his articles
            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Article pictures

        [ValidateInput(false)]
        public virtual ActionResult ArticlePictureAdd(int pictureId, int displaySubscription,
            string overrideAltAttribute, string overrideTitleAttribute,
            int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List");

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(article.Name));

            _articleService.InsertArticlePicture(new ArticlePicture
            {
                PictureId = pictureId,
                ArticleId = articleId,
                DisplaySubscription = displaySubscription,
            });

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult ArticlePictureList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            var articlePictures = _articleService.GetArticlePicturesByArticleId(articleId);
            var articlePicturesModel = articlePictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new ArticleModel.ArticlePictureModel
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        DisplaySubscription = x.DisplaySubscription
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = articlePicturesModel,
                Total = articlePicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticlePictureUpdate(ArticleModel.ArticlePictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articlePicture = _articleService.GetArticlePictureById(model.Id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articlePicture.ArticleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            var picture = _pictureService.GetPictureById(articlePicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            articlePicture.DisplaySubscription = model.DisplaySubscription;
            _articleService.UpdateArticlePicture(articlePicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticlePictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articlePicture = _articleService.GetArticlePictureById(id);
            if (articlePicture == null)
                throw new ArgumentException("No article picture found with the specified id");

            var articleId = articlePicture.ArticleId;

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }
            var pictureId = articlePicture.PictureId;
            _articleService.DeleteArticlePicture(articlePicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Article specification attributes

        [ValidateInput(false)]
        public virtual ActionResult ArticleSpecificationAttributeAdd(int attributeTypeId, int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnArticlePage,
            int displaySubscription, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return RedirectToAction("List");
                }
            }

            //we allow filtering only for "Option" attribute type
            if (attributeTypeId != (int)SpecificationAttributeType.Option)
            {
                allowFiltering = false;
            }
            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
            {
                customValue = null;
            }

            var psa = new ArticleSpecificationAttribute
            {
                AttributeTypeId = attributeTypeId,
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                ArticleId = articleId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnArticlePage = showOnArticlePage,
                DisplaySubscription = displaySubscription,
            };
            _specificationAttributeService.InsertArticleSpecificationAttribute(psa);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult ArticleSpecAttrList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            var articlerSpecs = _specificationAttributeService.GetArticleSpecificationAttributes(articleId);

            var articlerSpecsModel = articlerSpecs
                .Select(x =>
                {
                    var psaModel = new ArticleSpecificationAttributeModel
                    {
                        Id = x.Id,
                        AttributeTypeId = x.AttributeTypeId,
                        AttributeTypeName = x.AttributeType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeId = x.SpecificationAttributeOption.SpecificationAttribute.Id,
                        AttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                        AllowFiltering = x.AllowFiltering,
                        ShowOnArticlePage = x.ShowOnArticlePage,
                        DisplaySubscription = x.DisplaySubscription
                    };
                    switch (x.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            psaModel.ValueRaw = HttpUtility.HtmlEncode(x.SpecificationAttributeOption.Name);
                            psaModel.SpecificationAttributeOptionId = x.SpecificationAttributeOptionId;
                            break;
                        case SpecificationAttributeType.CustomText:
                            psaModel.ValueRaw = HttpUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            //do not encode?
                            //psaModel.ValueRaw = x.CustomValue;
                            psaModel.ValueRaw = HttpUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            psaModel.ValueRaw = x.CustomValue;
                            break;
                        default:
                            break;
                    }
                    return psaModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = articlerSpecsModel,
                Total = articlerSpecsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticleSpecAttrUpdate(ArticleSpecificationAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetArticleSpecificationAttributeById(model.Id);
            if (psa == null)
                return Content("No article specification attribute found with the specified id");

            var articleId = psa.Article.Id;

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            //we allow filtering and change option only for "Option" attribute type
            if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
            {
                psa.AllowFiltering = model.AllowFiltering;
                int specificationAttributeOptionId;
                if (int.TryParse(model.ValueRaw, out specificationAttributeOptionId))
                    psa.SpecificationAttributeOptionId = specificationAttributeOptionId;
            }

            psa.ShowOnArticlePage = model.ShowOnArticlePage;
            psa.DisplaySubscription = model.DisplaySubscription;
            _specificationAttributeService.UpdateArticleSpecificationAttribute(psa);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticleSpecAttrDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetArticleSpecificationAttributeById(id);
            if (psa == null)
                throw new ArgumentException("No specification attribute found with the specified id");

            var articleId = psa.ArticleId;

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                var article = _articleService.GetArticleById(articleId);
                if (article != null && article.ContributorId != _workContext.CurrentContributor.Id)
                {
                    return Content("This is not your article");
                }
            }

            _specificationAttributeService.DeleteArticleSpecificationAttribute(psa);

            return new NullJsonResult();
        }

        #endregion

        #region Article tags

        public virtual ActionResult ArticleTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleTags))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult ArticleTags(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleTags))
                return AccessDeniedKendoGridJson();

            var tags = _articleTagService.GetAllArticleTags()
                //subscription by article count
                .OrderByDescending(x => _articleTagService.GetArticleCount(x.Id, 0))
                .Select(x => new ArticleTagModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ArticleCount = _articleTagService.GetArticleCount(x.Id, 0)
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = tags.PagedForCommand(command),
                Total = tags.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticleTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleTags))
                return AccessDeniedView();

            var tag = _articleTagService.GetArticleTagById(id);
            if (tag == null)
                throw new ArgumentException("No article tag found with the specified id");
            _articleTagService.DeleteArticleTag(tag);

            return new NullJsonResult();
        }

        //edit
        public virtual ActionResult EditArticleTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleTags))
                return AccessDeniedView();

            var articleTag = _articleTagService.GetArticleTagById(id);
            if (articleTag == null)
                //No article tag found with the specified id
                return RedirectToAction("List");

            var model = new ArticleTagModel
            {
                Id = articleTag.Id,
                Name = articleTag.Name,
                ArticleCount = _articleTagService.GetArticleCount(articleTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = articleTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult EditArticleTag(string btnId, string formId, ArticleTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticleTags))
                return AccessDeniedView();

            var articleTag = _articleTagService.GetArticleTagById(model.Id);
            if (articleTag == null)
                //No article tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                articleTag.Name = model.Name;
                _articleTagService.UpdateArticleTag(articleTag);
                //locales
                UpdateLocales(articleTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with subscription

        [HttpPost]
        public virtual ActionResult PurchasedWithSubscriptions(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var subscriptions = _subscriptionService.SearchSubscriptions(
                articleId: articleId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = subscriptions.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new SubscriptionModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        SubscriptionStatus = x.SubscriptionStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        
                        CustomerEmail = x.BillingAddress.Email,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomSubscriptionNumber = x.CustomSubscriptionNumber
                    };
                }),
                Total = subscriptions.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual ActionResult DownloadCatalogAsPdf(ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var articles = _articleService.SearchArticles(
                categoryIds: categoryIds,
                publisherId: model.SearchPublisherId,
                storeId: model.SearchStoreId,
                contributorId: model.SearchContributorId,
                warehouseId: model.SearchWarehouseId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintArticlesToPdf(stream, articles);
                    bytes = stream.ToArray();
                }
                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual ActionResult ExportXmlAll(ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var articles = _articleService.SearchArticles(
                categoryIds: categoryIds,
                publisherId: model.SearchPublisherId,
                storeId: model.SearchStoreId,
                contributorId: model.SearchContributorId,
                warehouseId: model.SearchWarehouseId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                var xml = _exportManager.ExportArticlesToXml(articles);
                return new XmlDownloadResult(xml, "articles.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articles = new List<Article>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                articles.AddRange(_articleService.GetArticlesByIds(ids));
            }
            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                articles = articles.Where(p => p.ContributorId == _workContext.CurrentContributor.Id).ToList();
            }

            var xml = _exportManager.ExportArticlesToXml(articles);
            return new XmlDownloadResult(xml, "articles.xml");
        }


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual ActionResult ExportExcelAll(ArticleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                model.SearchContributorId = _workContext.CurrentContributor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var articles = _articleService.SearchArticles(
                categoryIds: categoryIds,
                publisherId: model.SearchPublisherId,
                storeId: model.SearchStoreId,
                contributorId: model.SearchContributorId,
                warehouseId: model.SearchWarehouseId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                showHidden: true,
                overridePublished: overridePublished
            );
            try
            {
                var bytes = _exportManager.ExportArticlesToXlsx(articles);

                return File(bytes, MimeTypes.TextXlsx, "articles.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articles = new List<Article>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                articles.AddRange(_articleService.GetArticlesByIds(ids));
            }
            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
            {
                articles = articles.Where(p => p.ContributorId == _workContext.CurrentContributor.Id).ToList();
            }

            var bytes = _exportManager.ExportArticlesToXlsx(articles);

            return File(bytes, MimeTypes.TextXlsx, "articles.xlsx");
        }

        [HttpPost]
        public virtual ActionResult ImportExcel()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            
            if (_workContext.CurrentContributor != null && !_contributorSettings.AllowContributorsToImportArticles)
                //a contributor can not import articles
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportArticlesFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Articles.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        #endregion

         

        #region Bulk editing

        public virtual ActionResult BulkEdit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new BulkEditListModel();
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

            //article types
            model.AvailableArticleTypes = ArticleType.SimpleArticle.ToSelectList(false).ToList();
            model.AvailableArticleTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult BulkEditSelect(DataSourceRequest command, BulkEditListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            int contributorId = 0;
            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null)
                contributorId = _workContext.CurrentContributor.Id;

            var articles = _articleService.SearchArticles(categoryIds: new List<int> { model.SearchCategoryId },
                publisherId: model.SearchPublisherId,
                contributorId: contributorId,
                articleType: model.SearchArticleTypeId > 0 ? (ArticleType?)model.SearchArticleTypeId : null,
                keywords: model.SearchArticleName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult();
            gridModel.Data = articles.Select(x =>
            {
                var articleModel = new BulkEditArticleModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Sku = x.Sku,
                    OldPrice = x.OldPrice,
                    Price = x.Price,
                    
                    Published = x.Published
                };

               

                return articleModel;
            });
            gridModel.Total = articles.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult BulkEditUpdate(IEnumerable<BulkEditArticleModel> articles)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (articles != null)
            {
                foreach (var pModel in articles)
                {
                    //update
                    var article = _articleService.GetArticleById(pModel.Id);
                    if (article != null)
                    {
                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                            continue;

                        var prevTotalStockQuantity = article.GetTotalStockQuantity();
                    

                        article.Name = pModel.Name;
                        article.Sku = pModel.Sku;
                        article.Price = pModel.Price;
                        article.OldPrice = pModel.OldPrice;
                       
                        article.Published = pModel.Published;
                        article.UpdatedOnUtc = DateTime.UtcNow;
                        _articleService.UpdateArticle(article);

                       
                    }
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult BulkEditDelete(IEnumerable<BulkEditArticleModel> articles)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            if (articles != null)
            {
                foreach (var pModel in articles)
                {
                    //delete
                    var article = _articleService.GetArticleById(pModel.Id);
                    if (article != null)
                    {
                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                            continue;

                        _articleService.DeleteArticle(article);
                    }
                }
            }
            return new NullJsonResult();
        }

        #endregion

         

        #region Article attributes

        [HttpPost]
        public virtual ActionResult ArticleAttributeMappingList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(articleId);
            var attributesModel = attributes
                .Select(x =>
                {
                    var attributeModel = new ArticleModel.ArticleAttributeMappingModel
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        ArticleAttribute = _articleAttributeService.GetArticleAttributeById(x.ArticleAttributeId).Name,
                        ArticleAttributeId = x.ArticleAttributeId,
                        TextPrompt = x.TextPrompt,
                        IsRequired = x.IsRequired,
                        AttributeControlType = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeControlTypeId = x.AttributeControlTypeId,
                        DisplaySubscription = x.DisplaySubscription
                    };


                    if (x.ShouldHaveValues())
                    {
                        attributeModel.ShouldHaveValues = true;
                        attributeModel.TotalValues = x.ArticleAttributeValues.Count;
                    }

                    if (x.ValidationRulesAllowed())
                    {
                        var validationRules = new StringBuilder(string.Empty);
                        attributeModel.ValidationRulesAllowed = true;
                        if (x.ValidationMinLength != null)
                            validationRules.AppendFormat("{0}: {1}<br />",
                                _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.MinLength"),
                                x.ValidationMinLength);
                        if (x.ValidationMaxLength != null)
                            validationRules.AppendFormat("{0}: {1}<br />",
                                _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.MaxLength"),
                                x.ValidationMaxLength);
                        if (!string.IsNullOrEmpty(x.ValidationFileAllowedExtensions))
                            validationRules.AppendFormat("{0}: {1}<br />",
                                _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                                HttpUtility.HtmlEncode(x.ValidationFileAllowedExtensions));
                        if (x.ValidationFileMaximumSize != null)
                            validationRules.AppendFormat("{0}: {1}<br />",
                                _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.FileMaximumSize"),
                                x.ValidationFileMaximumSize);
                        if (!string.IsNullOrEmpty(x.DefaultValue))
                            validationRules.AppendFormat("{0}: {1}<br />",
                                _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.DefaultValue"),
                                HttpUtility.HtmlEncode(x.DefaultValue));
                        attributeModel.ValidationRulesString = validationRules.ToString();
                    }


                    //currenty any attribute can have condition. why not?
                    attributeModel.ConditionAllowed = true;
                    var conditionAttribute = _articleAttributeParser.ParseArticleAttributeMappings(x.ConditionAttributeXml).FirstOrDefault();
                    var conditionValue = _articleAttributeParser.ParseArticleAttributeValues(x.ConditionAttributeXml).FirstOrDefault();
                    if (conditionAttribute != null && conditionValue != null)
                        attributeModel.ConditionString = string.Format("{0}: {1}",
                            HttpUtility.HtmlEncode(conditionAttribute.ArticleAttribute.Name),
                            HttpUtility.HtmlEncode(conditionValue.Name));
                    else
                        attributeModel.ConditionString = string.Empty;
                    return attributeModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = attributesModel,
                Total = attributesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeMappingInsert(ArticleModel.ArticleAttributeMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(model.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
            {
                return Content("This is not your article");
            }

            //ensure this attribute is not mapped yet
            if (_articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id).Any(x => x.ArticleAttributeId == model.ArticleAttributeId))
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.AlreadyExists") });
            }

            //insert mapping
            var articleAttributeMapping = new ArticleAttributeMapping
            {
                ArticleId = model.ArticleId,
                ArticleAttributeId = model.ArticleAttributeId,
                TextPrompt = model.TextPrompt,
                IsRequired = model.IsRequired,
                AttributeControlTypeId = model.AttributeControlTypeId,
                DisplaySubscription = model.DisplaySubscription
            };
            _articleAttributeService.InsertArticleAttributeMapping(articleAttributeMapping);

            //predefined values
            var predefinedValues = _articleAttributeService.GetPredefinedArticleAttributeValues(model.ArticleAttributeId);
            foreach (var predefinedValue in predefinedValues)
            {
                var pav = new ArticleAttributeValue
                {
                    ArticleAttributeMappingId = articleAttributeMapping.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = predefinedValue.Name,
                    PriceAdjustment = predefinedValue.PriceAdjustment,
                    WeightAdjustment = predefinedValue.WeightAdjustment,
                    Cost = predefinedValue.Cost,
                    IsPreSelected = predefinedValue.IsPreSelected,
                    DisplaySubscription = predefinedValue.DisplaySubscription
                };
                _articleAttributeService.InsertArticleAttributeValue(pav);
                //locales
                var languages = _languageService.GetAllLanguages(true);
                //localization
                foreach (var lang in languages)
                {
                    var name = predefinedValue.GetLocalized(x => x.Name, lang.Id, false, false);
                    if (!String.IsNullOrEmpty(name))
                        _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeMappingUpdate(ArticleModel.ArticleAttributeMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(model.Id);
            if (articleAttributeMapping == null)
                throw new ArgumentException("No article attribute mapping found with the specified id");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            articleAttributeMapping.ArticleAttributeId = model.ArticleAttributeId;
            articleAttributeMapping.TextPrompt = model.TextPrompt;
            articleAttributeMapping.IsRequired = model.IsRequired;
            articleAttributeMapping.AttributeControlTypeId = model.AttributeControlTypeId;
            articleAttributeMapping.DisplaySubscription = model.DisplaySubscription;
            _articleAttributeService.UpdateArticleAttributeMapping(articleAttributeMapping);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeMappingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(id);
            if (articleAttributeMapping == null)
                throw new ArgumentException("No article attribute mapping found with the specified id");

            var articleId = articleAttributeMapping.ArticleId;
            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");


            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            _articleAttributeService.DeleteArticleAttributeMapping(articleAttributeMapping);

            return new NullJsonResult();
        }

        #endregion

        #region Article attributes. Validation rules

        public virtual ActionResult ArticleAttributeValidationRulesPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(id);
            if (articleAttributeMapping == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            var model = new ArticleModel.ArticleAttributeMappingModel
            {
                //prepare only used properties
                Id = articleAttributeMapping.Id,
                ValidationRulesAllowed = articleAttributeMapping.ValidationRulesAllowed(),
                AttributeControlTypeId = articleAttributeMapping.AttributeControlTypeId,
                ValidationMinLength = articleAttributeMapping.ValidationMinLength,
                ValidationMaxLength = articleAttributeMapping.ValidationMaxLength,
                ValidationFileAllowedExtensions = articleAttributeMapping.ValidationFileAllowedExtensions,
                ValidationFileMaximumSize = articleAttributeMapping.ValidationFileMaximumSize,
                DefaultValue = articleAttributeMapping.DefaultValue,
            };
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult ArticleAttributeValidationRulesPopup(string btnId, string formId, ArticleModel.ArticleAttributeMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(model.Id);
            if (articleAttributeMapping == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            if (ModelState.IsValid)
            {
                articleAttributeMapping.ValidationMinLength = model.ValidationMinLength;
                articleAttributeMapping.ValidationMaxLength = model.ValidationMaxLength;
                articleAttributeMapping.ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions;
                articleAttributeMapping.ValidationFileMaximumSize = model.ValidationFileMaximumSize;
                articleAttributeMapping.DefaultValue = model.DefaultValue;
                _articleAttributeService.UpdateArticleAttributeMapping(articleAttributeMapping);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model.ValidationRulesAllowed = articleAttributeMapping.ValidationRulesAllowed();
            model.AttributeControlTypeId = articleAttributeMapping.AttributeControlTypeId;
            return View(model);
        }

        #endregion

        #region Article attributes. Condition

        public virtual ActionResult ArticleAttributeConditionPopup(string btnId, string formId, int articleAttributeMappingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(articleAttributeMappingId);
            if (articleAttributeMapping == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            var model = new ArticleAttributeConditionModel();
            model.ArticleAttributeMappingId = articleAttributeMapping.Id;
            model.EnableCondition = !String.IsNullOrEmpty(articleAttributeMapping.ConditionAttributeXml);


            //pre-select attribute and values
            var selectedPva = _articleAttributeParser
                .ParseArticleAttributeMappings(articleAttributeMapping.ConditionAttributeXml)
                .FirstOrDefault();

            var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id)
                //ignore non-combinable attributes (should have selectable values)
                .Where(x => x.CanBeUsedAsCondition())
                //ignore this attribute (it cannot depend on itself)
                .Where(x => x.Id != articleAttributeMapping.Id)
                .ToList();
            foreach (var attribute in attributes)
            {
                var attributeModel = new ArticleAttributeConditionModel.ArticleAttributeModel
                {
                    Id = attribute.Id,
                    ArticleAttributeId = attribute.ArticleAttributeId,
                    Name = attribute.ArticleAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ArticleAttributeConditionModel.ArticleAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }

                    //pre-select attribute and value
                    if (selectedPva != null && attribute.Id == selectedPva.Id)
                    {
                        //attribute
                        model.SelectedArticleAttributeId = selectedPva.Id;

                        //values
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                                {
                                    if (!String.IsNullOrEmpty(articleAttributeMapping.ConditionAttributeXml))
                                    {
                                        //clear default selection
                                        foreach (var item in attributeModel.Values)
                                            item.IsPreSelected = false;

                                        //select new values
                                        var selectedValues = _articleAttributeParser.ParseArticleAttributeValues(articleAttributeMapping.ConditionAttributeXml);
                                        foreach (var attributeValue in selectedValues)
                                            foreach (var item in attributeModel.Values)
                                                if (attributeValue.Id == item.Id)
                                                    item.IsPreSelected = true;
                                    }
                                }
                                break;
                            case AttributeControlType.ReadonlyCheckboxes:
                            case AttributeControlType.TextBox:
                            case AttributeControlType.MultilineTextbox:
                            case AttributeControlType.Datepicker:
                            case AttributeControlType.FileUpload:
                            default:
                                //these attribute types are supported as conditions
                                break;
                        }
                    }
                }

                model.ArticleAttributes.Add(attributeModel);
            }

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeConditionPopup(string btnId, string formId,
            ArticleAttributeConditionModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping =
                _articleAttributeService.GetArticleAttributeMappingById(model.ArticleAttributeMappingId);
            if (articleAttributeMapping == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            string attributesXml = null;
            if (model.EnableCondition)
            {
                var attribute = _articleAttributeService.GetArticleAttributeMappingById(model.SelectedArticleAttributeId);
                if (attribute != null)
                {
                    string controlId = string.Format("article_attribute_{0}", attribute.Id);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                var ctrlAttributes = form[controlId];
                                if (!String.IsNullOrEmpty(ctrlAttributes))
                                {
                                    int selectedAttributeId = int.Parse(ctrlAttributes);
                                    if (selectedAttributeId > 0)
                                    {
                                        attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                    }
                                    else
                                    {
                                        //for conditions we should empty values save even when nothing is selected
                                        //otherwise "attributesXml" will be empty
                                        //hence we won't be able to find a selected attribute
                                        attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, "");
                                    }
                                }
                                else
                                {
                                    //for conditions we should empty values save even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                        attribute, "");
                                }
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var cblAttributes = form[controlId];
                                if (!String.IsNullOrEmpty(cblAttributes))
                                {
                                    bool anyValueSelected = false;
                                    foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        int selectedAttributeId = int.Parse(item);
                                        if (selectedAttributeId > 0)
                                        {
                                            attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                                attribute, selectedAttributeId.ToString());
                                            anyValueSelected = true;
                                        }
                                    }
                                    if (!anyValueSelected)
                                    {
                                        //for conditions we should save empty values even when nothing is selected
                                        //otherwise "attributesXml" will be empty
                                        //hence we won't be able to find a selected attribute
                                        attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, "");
                                    }
                                }
                                else
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, "");
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //these attribute types are supported as conditions
                            break;
                    }
                }
            }
            articleAttributeMapping.ConditionAttributeXml = attributesXml;
            _articleAttributeService.UpdateArticleAttributeMapping(articleAttributeMapping);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Article attribute values

        //list
        public virtual ActionResult EditAttributeValues(int articleAttributeMappingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(articleAttributeMappingId);
            if (articleAttributeMapping == null)
                throw new ArgumentException("No article attribute mapping found with the specified id");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            var model = new ArticleModel.ArticleAttributeValueListModel
            {
                ArticleName = article.Name,
                ArticleId = articleAttributeMapping.ArticleId,
                ArticleAttributeName = articleAttributeMapping.ArticleAttribute.Name,
                ArticleAttributeMappingId = articleAttributeMapping.Id,
            };

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeValueList(int articleAttributeMappingId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(articleAttributeMappingId);
            if (articleAttributeMapping == null)
                throw new ArgumentException("No article attribute mapping found with the specified id");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var values = _articleAttributeService.GetArticleAttributeValues(articleAttributeMappingId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x =>
                {
                    Article associatedArticle = null;
                    if (x.AttributeValueType == AttributeValueType.AssociatedToArticle)
                    {
                        associatedArticle = _articleService.GetArticleById(x.AssociatedArticleId);
                    }
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (String.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = _pictureService.GetPictureUrl(null, 1, true);
                    return new ArticleModel.ArticleAttributeValueModel
                    {
                        Id = x.Id,
                        ArticleAttributeMappingId = x.ArticleAttributeMappingId,
                        AttributeValueTypeId = x.AttributeValueTypeId,
                        AttributeValueTypeName = x.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                        AssociatedArticleId = x.AssociatedArticleId,
                        AssociatedArticleName = associatedArticle != null ? associatedArticle.Name : "",
                        Name = x.ArticleAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                        ColorSquaresRgb = x.ColorSquaresRgb,
                        ImageSquaresPictureId = x.ImageSquaresPictureId,
                        PriceAdjustment = x.PriceAdjustment,
                        PriceAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.PriceAdjustment.ToString("G29") : "",
                        WeightAdjustment = x.WeightAdjustment,
                        WeightAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.WeightAdjustment.ToString("G29") : "",
                        Cost = x.Cost,
                        CustomerEntersQty = x.CustomerEntersQty,
                        Quantity = x.Quantity,
                        IsPreSelected = x.IsPreSelected,
                        DisplaySubscription = x.DisplaySubscription,
                        PictureId = x.PictureId,
                        PictureThumbnailUrl = pictureThumbnailUrl
                    };
                }),
                Total = values.Count()
            };

            return Json(gridModel);
        }

        //create
        public virtual ActionResult ArticleAttributeValueCreatePopup(int articleAttributeMappingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(articleAttributeMappingId);
            if (articleAttributeMapping == null)
                throw new ArgumentException("No article attribute mapping found with the specified id");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            var model = new ArticleModel.ArticleAttributeValueModel();
            model.ArticleAttributeMappingId = articleAttributeMappingId;

            //color squares
            model.DisplayColorSquaresRgb = articleAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";
            //image squares
            model.DisplayImageSquaresPicture = articleAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares;

            //default qantity for associated article
            model.Quantity = 1;

            //locales
            AddLocales(_languageService, model.Locales);

            //pictures
            model.ArticlePictureModels = _articleService.GetArticlePicturesByArticleId(article.Id)
                .Select(x => new ArticleModel.ArticlePictureModel
                {
                    Id = x.Id,
                    ArticleId = x.ArticleId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeValueCreatePopup(string btnId, string formId, ArticleModel.ArticleAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var articleAttributeMapping = _articleAttributeService.GetArticleAttributeMappingById(model.ArticleAttributeMappingId);
            if (articleAttributeMapping == null)
                //No article attribute found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(articleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            if (articleAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (articleAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                var pav = new ArticleAttributeValue
                {
                    ArticleAttributeMappingId = model.ArticleAttributeMappingId,
                    AttributeValueTypeId = model.AttributeValueTypeId,
                    AssociatedArticleId = model.AssociatedArticleId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    ImageSquaresPictureId = model.ImageSquaresPictureId,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    CustomerEntersQty = model.CustomerEntersQty,
                    Quantity = model.CustomerEntersQty ? 1 : model.Quantity,
                    IsPreSelected = model.IsPreSelected,
                    DisplaySubscription = model.DisplaySubscription,
                    PictureId = model.PictureId,
                };

                _articleAttributeService.InsertArticleAttributeValue(pav);
                UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form


            //pictures
            model.ArticlePictureModels = _articleService.GetArticlePicturesByArticleId(article.Id)
                .Select(x => new ArticleModel.ArticlePictureModel
                {
                    Id = x.Id,
                    ArticleId = x.ArticleId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            var associatedArticle = _articleService.GetArticleById(model.AssociatedArticleId);
            model.AssociatedArticleName = associatedArticle != null ? associatedArticle.Name : "";

            return View(model);
        }

        //edit
        public virtual ActionResult ArticleAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var pav = _articleAttributeService.GetArticleAttributeValueById(id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(pav.ArticleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            var associatedArticle = _articleService.GetArticleById(pav.AssociatedArticleId);

            var model = new ArticleModel.ArticleAttributeValueModel
            {
                ArticleAttributeMappingId = pav.ArticleAttributeMappingId,
                AttributeValueTypeId = pav.AttributeValueTypeId,
                AttributeValueTypeName = pav.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                AssociatedArticleId = pav.AssociatedArticleId,
                AssociatedArticleName = associatedArticle != null ? associatedArticle.Name : "",
                Name = pav.Name,
                ColorSquaresRgb = pav.ColorSquaresRgb,
                DisplayColorSquaresRgb = pav.ArticleAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                ImageSquaresPictureId = pav.ImageSquaresPictureId,
                DisplayImageSquaresPicture = pav.ArticleAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                PriceAdjustment = pav.PriceAdjustment,
                WeightAdjustment = pav.WeightAdjustment,
                Cost = pav.Cost,
                CustomerEntersQty = pav.CustomerEntersQty,
                Quantity = pav.Quantity,
                IsPreSelected = pav.IsPreSelected,
                DisplaySubscription = pav.DisplaySubscription,
                PictureId = pav.PictureId
            };
            if (model.DisplayColorSquaresRgb && String.IsNullOrEmpty(model.ColorSquaresRgb))
            {
                model.ColorSquaresRgb = "#000000";
            }
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = pav.GetLocalized(x => x.Name, languageId, false, false);
            });
            //pictures
            model.ArticlePictureModels = _articleService.GetArticlePicturesByArticleId(article.Id)
                .Select(x => new ArticleModel.ArticlePictureModel
                {
                    Id = x.Id,
                    ArticleId = x.ArticleId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeValueEditPopup(string btnId, string formId, ArticleModel.ArticleAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var pav = _articleAttributeService.GetArticleAttributeValueById(model.Id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Article");

            var article = _articleService.GetArticleById(pav.ArticleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            if (pav.ArticleAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (pav.ArticleAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                pav.AttributeValueTypeId = model.AttributeValueTypeId;
                pav.AssociatedArticleId = model.AssociatedArticleId;
                pav.Name = model.Name;
                pav.ColorSquaresRgb = model.ColorSquaresRgb;
                pav.ImageSquaresPictureId = model.ImageSquaresPictureId;
                pav.PriceAdjustment = model.PriceAdjustment;
                pav.WeightAdjustment = model.WeightAdjustment;
                pav.Cost = model.Cost;
                pav.CustomerEntersQty = model.CustomerEntersQty;
                pav.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                pav.IsPreSelected = model.IsPreSelected;
                pav.DisplaySubscription = model.DisplaySubscription;
                pav.PictureId = model.PictureId;
                _articleAttributeService.UpdateArticleAttributeValue(pav);

                UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form

            //pictures
            model.ArticlePictureModels = _articleService.GetArticlePicturesByArticleId(article.Id)
                .Select(x => new ArticleModel.ArticlePictureModel
                {
                    Id = x.Id,
                    ArticleId = x.ArticleId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplaySubscription = x.DisplaySubscription
                })
                .ToList();

            var associatedArticle = _articleService.GetArticleById(model.AssociatedArticleId);
            model.AssociatedArticleName = associatedArticle != null ? associatedArticle.Name : "";

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult ArticleAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var pav = _articleAttributeService.GetArticleAttributeValueById(id);
            if (pav == null)
                throw new ArgumentException("No article attribute value found with the specified id");

            var article = _articleService.GetArticleById(pav.ArticleAttributeMapping.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            _articleAttributeService.DeleteArticleAttributeValue(pav);

            return new NullJsonResult();
        }





        public virtual ActionResult AssociateArticleToAttributeValuePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var model = new ArticleModel.ArticleAttributeValueModel.AssociateArticleToAttributeValueModel();
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
        public virtual ActionResult AssociateArticleToAttributeValuePopupList(DataSourceRequest command,
            ArticleModel.ArticleAttributeValueModel.AssociateArticleToAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
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
        public virtual ActionResult AssociateArticleToAttributeValuePopup(string articleIdInput,
            string articleNameInput, ArticleModel.ArticleAttributeValueModel.AssociateArticleToAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var associatedArticle = _articleService.GetArticleById(model.AssociatedToArticleId);
            if (associatedArticle == null)
                return Content("Cannot load a article");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && associatedArticle.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            model.IsLoggedInAsContributor = _workContext.CurrentContributor != null;
            ViewBag.RefreshPage = true;
            ViewBag.articleIdInput = articleIdInput;
            ViewBag.articleNameInput = articleNameInput;
            ViewBag.articleId = associatedArticle.Id;
            ViewBag.articleName = associatedArticle.Name;
            return View(model);
        }

        //action displaying notification (warning) to a store owner when associating some article
        [ValidateInput(false)]
        public virtual ActionResult AssociatedArticleGetWarnings(int articleId)
        {
            var associatedArticle = _articleService.GetArticleById(articleId);
            if (associatedArticle != null)
            {
                //attributes
                if (associatedArticle.ArticleAttributeMappings.Any())
                {
                    if (associatedArticle.ArticleAttributeMappings.Any(attribute => attribute.IsRequired))
                        return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AssociatedArticle.HasRequiredAttributes") }, JsonRequestBehavior.AllowGet);

                    return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AssociatedArticle.HasAttributes") }, JsonRequestBehavior.AllowGet);
                }
                
               

               
            }

            return Json(new { Result = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Article attribute combinations

        [HttpPost]
        public virtual ActionResult ArticleAttributeCombinationList(DataSourceRequest command, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedKendoGridJson();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var combinations = _articleAttributeService.GetAllArticleAttributeCombinations(articleId);
            var combinationsModel = combinations
                .Select(x =>
                {
                    var pacModel = new ArticleModel.ArticleAttributeCombinationModel
                    {
                        Id = x.Id,
                        ArticleId = x.ArticleId,
                        AttributesXml = _articleAttributeFormatter.FormatAttributes(x.Article, x.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false),
                        StockQuantity = x.StockQuantity,
                        AllowOutOfStockSubscriptions = x.AllowOutOfStockSubscriptions,
                        Sku = x.Sku,
                        PublisherPartNumber = x.PublisherPartNumber,
                        Gtin = x.Gtin,
                        OverriddenPrice = x.OverriddenPrice,
                        NotifyAdminForQuantityBelow = x.NotifyAdminForQuantityBelow
                    };
                    //warnings
                    var warnings = _shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                        ShoppingCartType.ShoppingCart, x.Article, 1, x.AttributesXml, true);
                    for (int i = 0; i < warnings.Count; i++)
                    {
                        pacModel.Warnings += warnings[i];
                        if (i != warnings.Count - 1)
                            pacModel.Warnings += "<br />";
                    }

                    return pacModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = combinationsModel,
                Total = combinationsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeCombinationUpdate(ArticleModel.ArticleAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var combination = _articleAttributeService.GetArticleAttributeCombinationById(model.Id);
            if (combination == null)
                throw new ArgumentException("No article attribute combination found with the specified id");

            var article = _articleService.GetArticleById(combination.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var previousSrockQuantity = combination.StockQuantity;

            combination.StockQuantity = model.StockQuantity;
            combination.AllowOutOfStockSubscriptions = model.AllowOutOfStockSubscriptions;
            combination.Sku = model.Sku;
            combination.PublisherPartNumber = model.PublisherPartNumber;
            combination.Gtin = model.Gtin;
            combination.OverriddenPrice = model.OverriddenPrice;
            combination.NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow;
            _articleAttributeService.UpdateArticleAttributeCombination(combination);

        
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ArticleAttributeCombinationDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var combination = _articleAttributeService.GetArticleAttributeCombinationById(id);
            if (combination == null)
                throw new ArgumentException("No article attribute combination found with the specified id");

            var article = _articleService.GetArticleById(combination.ArticleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            _articleAttributeService.DeleteArticleAttributeCombination(combination);

            return new NullJsonResult();
        }

        public virtual ActionResult AddAttributeCombinationPopup(string btnId, string formId, int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                //No article found with the specified id
                return RedirectToAction("List", "Article");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            var model = new AddArticleAttributeCombinationModel();
            PrepareAddArticleAttributeCombinationModel(model, article);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult AddAttributeCombinationPopup(string btnId, string formId, int articleId,
            AddArticleAttributeCombinationModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                //No article found with the specified id
                return RedirectToAction("List", "Article");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return RedirectToAction("List", "Article");

            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            //attributes
            string attributesXml = "";
            var warnings = new List<string>();

            #region Article attributes

            var attributes = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id)
                //ignore non-combinable attributes for combinations
                .Where(x => !x.IsNonCombinable())
                .ToList();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("article_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _articleAttributeService.GetArticleAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var httpPostedFile = this.Request.Files[controlId];
                            if ((httpPostedFile != null) && (!String.IsNullOrEmpty(httpPostedFile.FileName)))
                            {
                                var fileSizeOk = true;
                                if (attribute.ValidationFileMaximumSize.HasValue)
                                {
                                    //compare in bytes
                                    var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                                    if (httpPostedFile.ContentLength > maxFileSizeBytes)
                                    {
                                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value));
                                        fileSizeOk = false;
                                    }
                                }
                                if (fileSizeOk)
                                {
                                    //save an uploaded file
                                    var download = new Download
                                    {
                                        DownloadGuid = Guid.NewGuid(),
                                        UseDownloadUrl = false,
                                        DownloadUrl = "",
                                        DownloadBinary = httpPostedFile.GetDownloadBits(),
                                        ContentType = httpPostedFile.ContentType,
                                        Filename = Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                        Extension = Path.GetExtension(httpPostedFile.FileName),
                                        IsNew = true
                                    };
                                    _downloadService.InsertDownload(download);
                                    //save attribute
                                    attributesXml = _articleAttributeParser.AddArticleAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in attributes)
            {
                var conditionMet = _articleAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _articleAttributeParser.RemoveArticleAttribute(attributesXml, attribute);
                }
            }

            #endregion

            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                ShoppingCartType.ShoppingCart, article, 1, attributesXml, true));
            if (!warnings.Any())
            {
                //save combination
                var combination = new ArticleAttributeCombination
                {
                    ArticleId = article.Id,
                    AttributesXml = attributesXml,
                    StockQuantity = model.StockQuantity,
                    AllowOutOfStockSubscriptions = model.AllowOutOfStockSubscriptions,
                    Sku = model.Sku,
                    PublisherPartNumber = model.PublisherPartNumber,
                    Gtin = model.Gtin,
                    OverriddenPrice = model.OverriddenPrice,
                    NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow,
                };
                _articleAttributeService.InsertArticleAttributeCombination(combination);

              
                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            PrepareAddArticleAttributeCombinationModel(model, article);
            model.Warnings = warnings;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult GenerateAllAttributeCombinations(int articleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            var article = _articleService.GetArticleById(articleId);
            if (article == null)
                throw new ArgumentException("No article found with the specified id");

            //a contributor should have access only to his articles
            if (_workContext.CurrentContributor != null && article.ContributorId != _workContext.CurrentContributor.Id)
                return Content("This is not your article");

            var allAttributesXml = _articleAttributeParser.GenerateAllCombinations(article, true);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = _articleAttributeParser.FindArticleAttributeCombination(article, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, article, 1, attributesXml, true));
                if (warnings.Count != 0)
                    continue;

                //save combination
                var combination = new ArticleAttributeCombination
                {
                    ArticleId = article.Id,
                    AttributesXml = attributesXml,
                    StockQuantity = 0,
                    AllowOutOfStockSubscriptions = false,
                    Sku = null,
                    PublisherPartNumber = null,
                    Gtin = null,
                    OverriddenPrice = null,
                    NotifyAdminForQuantityBelow = 1
                };
                _articleAttributeService.InsertArticleAttributeCombination(combination);
            }
            return Json(new { Success = true });
        }

        #endregion

        #region Article editor settings

        [HttpPost]
        public virtual ActionResult SaveArticleEditorSettings(ArticleModel model, string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();

            //contributors cannot manage these settings
            if (_workContext.CurrentContributor != null)
                return RedirectToAction("List");
            
            var articleEditorSettings = _settingService.LoadSetting<ArticleEditorSettings>();
            articleEditorSettings = model.ArticleEditorSettingsModel.ToEntity(articleEditorSettings);
            _settingService.SaveSetting(articleEditorSettings);

            //article list
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("List");
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("List");
            return Redirect(returnUrl);
        }

        #endregion

         

        #endregion
    }
}
