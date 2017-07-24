using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Services.Catalog;
using YStory.Services.Directory;
using YStory.Services.ExportImport.Help;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Tax;
using YStory.Services.Contributors;
using OfficeOpenXml;

namespace YStory.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly IArticleService _articleService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IEncryptionService _encryptionService;
        private readonly IDataProvider _dataProvider;
        private readonly MediaSettings _mediaSettings;
        private readonly IContributorService _contributorService;
        private readonly IArticleTemplateService _articleTemplateService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IMeasureService _measureService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IArticleTagService _articleTagService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ContributorSettings _contributorSettings;

        #endregion

        #region Ctor

        public ImportManager(IArticleService articleService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IEncryptionService encryptionService,
            IDataProvider dataProvider,
            MediaSettings mediaSettings,
            IContributorService contributorService,
            IArticleTemplateService articleTemplateService,
            ITaxCategoryService taxCategoryService,
            IMeasureService measureService,
            IArticleAttributeService articleAttributeService,
            CatalogSettings catalogSettings,
            IArticleTagService articleTagService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            ContributorSettings contributorSettings)
        {
            this._articleService = articleService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._storeContext = storeContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._encryptionService = encryptionService;
            this._dataProvider = dataProvider;
            this._mediaSettings = mediaSettings;
            this._contributorService = contributorService;
            this._articleTemplateService = articleTemplateService;
            this._taxCategoryService = taxCategoryService;
            this._measureService = measureService;
            this._articleAttributeService = articleAttributeService;
            this._catalogSettings = catalogSettings;
            this._articleTagService = articleTagService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._contributorSettings = contributorSettings;
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == MimeTypes.ApplicationOctetStream)
                mimeType = MimeTypes.ImageJpeg;

            return mimeType;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected virtual Picture LoadPicture(string picturePath, string name, int? picId = null)
        {
            if (String.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = File.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing article pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);

                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType,
                _pictureService.GetPictureSeName(name));
            return newPicture;
        }

        protected virtual void ImportArticleImagesUsingServices(IList<ArticlePictureMetadata> articlePictureMetadata)
        {
            foreach (var article in articlePictureMetadata)
            {
                foreach (var picturePath in new[] { article.Picture1Path, article.Picture2Path, article.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = File.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!article.IsNew)
                    {
                        //compare with existing article pictures
                        var existingPictures = _pictureService.GetPicturesByArticleId(article.ArticleItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;
                    var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(article.ArticleItem.Name));
                    article.ArticleItem.ArticlePictures.Add(new ArticlePicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplaySubscription = 1,
                    });
                    _articleService.UpdateArticle(article.ArticleItem);
                }
            }
        }

        protected virtual void ImportArticleImagesUsingHash(IList<ArticlePictureMetadata> articlePictureMetadata, IList<Article> allArticlesBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash() - 1;
            var articlesImagesIds = _articleService.GetArticlesImagesIds(allArticlesBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(articlesImagesIds.SelectMany(p => p.Value).ToArray());

            foreach (var article in articlePictureMetadata)
            {
                foreach (var picturePath in new[] { article.Picture1Path, article.Picture2Path, article.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = File.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!article.IsNew)
                    {
                        var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                        var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                        var imagesIds = articlesImagesIds.ContainsKey(article.ArticleItem.Id)
                            ? articlesImagesIds[article.ArticleItem.Id]
                            : new int[0];

                        pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key)).Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                    }

                    if (pictureAlreadyExists)
                        continue;
                    var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(article.ArticleItem.Name));
                    article.ArticleItem.ArticlePictures.Add(new ArticlePicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplaySubscription = 1,
                    });
                    _articleService.UpdateArticle(article.ArticleItem);
                }
            }
        }

        protected virtual IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[1, poz];

                    if (cell == null || cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Import articles from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportArticlesFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new YStoryException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Article>(worksheet);
                
                var manager = new PropertyManager<Article>(properties);

                var attributProperties = new[]
                   {
                        new PropertyByName<ExportArticleAttribute>("AttributeId"),
                        new PropertyByName<ExportArticleAttribute>("AttributeName"),
                        new PropertyByName<ExportArticleAttribute>("AttributeTextPrompt"),
                        new PropertyByName<ExportArticleAttribute>("AttributeIsRequired"),
                        new PropertyByName<ExportArticleAttribute>("AttributeControlType")
                        {
                            DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                        },
                        new PropertyByName<ExportArticleAttribute>("AttributeDisplaySubscription"), 
                        new PropertyByName<ExportArticleAttribute>("ArticleAttributeValueId"),
                        new PropertyByName<ExportArticleAttribute>("ValueName"),
                        new PropertyByName<ExportArticleAttribute>("AttributeValueType")
                        {
                            DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
                        },
                        new PropertyByName<ExportArticleAttribute>("AssociatedArticleId"),
                        new PropertyByName<ExportArticleAttribute>("ColorSquaresRgb"),
                        new PropertyByName<ExportArticleAttribute>("ImageSquaresPictureId"),
                        new PropertyByName<ExportArticleAttribute>("PriceAdjustment"),
                        new PropertyByName<ExportArticleAttribute>("WeightAdjustment"),
                        new PropertyByName<ExportArticleAttribute>("Cost"),
                        new PropertyByName<ExportArticleAttribute>("CustomerEntersQty"),
                        new PropertyByName<ExportArticleAttribute>("Quantity"),
                        new PropertyByName<ExportArticleAttribute>("IsPreSelected"),
                        new PropertyByName<ExportArticleAttribute>("DisplaySubscription"),
                        new PropertyByName<ExportArticleAttribute>("PictureId")
                    };

                var managerArticleAttribute = new PropertyManager<ExportArticleAttribute>(attributProperties);

                var endRow = 2;
                var allCategoriesNames = new List<string>();
                var allSku = new List<string>();

                var tempProperty = manager.GetProperty("Categories");
                var categoryCellNum = tempProperty.Return(p => p.PropertySubscriptionPosition, -1);
                
                tempProperty = manager.GetProperty("SKU");
                var skuCellNum = tempProperty.Return(p => p.PropertySubscriptionPosition, -1);

                var allPublishersNames = new List<string>();
                tempProperty = manager.GetProperty("Publishers");
                var publisherCellNum = tempProperty.Return(p => p.PropertySubscriptionPosition, -1);

                manager.SetSelectList("ArticleType", ArticleType.SimpleArticle.ToSelectList(useLocalization: false));
                manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod", RecurringArticleCyclePeriod.Days.ToSelectList(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

                manager.SetSelectList("Contributor", _contributorService.GetAllContributors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Contributor).Return(v => v.Name, String.Empty)));
                manager.SetSelectList("ArticleTemplate", _articleTemplateService.GetAllArticleTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ArticleTemplate).Return(pt => pt.Name, String.Empty)));
                manager.SetSelectList("TaxCategory", _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory).Return(tc => tc.Name, String.Empty)));
                manager.SetSelectList("BasepriceUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p =>(p as MeasureWeight).Return(mw => mw.Name, String.Empty)));
                manager.SetSelectList("BasepriceBaseUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));

                var allAttributeIds = new List<int>();
                var attributeIdCellNum = managerArticleAttribute.GetProperty("AttributeId").PropertySubscriptionPosition + ExportArticleAttribute.ProducAttributeCellOffset;

                var countArticlesInFile = 0;

                //find end of data
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[endRow, property.PropertySubscriptionPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    if (new[] { 1, 2 }.Select(cellNum => worksheet.Cells[endRow, cellNum]).All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString())) && worksheet.Row(endRow).OutlineLevel == 0)
                    {
                        var cellValue = worksheet.Cells[endRow, attributeIdCellNum].Value;
                        try
                        {
                            var aid = cellValue.Return(Convert.ToInt32, -1);

                            var articleAttribute = _articleAttributeService.GetArticleAttributeById(aid);

                            if (articleAttribute != null)
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                        catch (FormatException)
                        {
                            if (cellValue.Return(cv => cv.ToString(), String.Empty) == "AttributeId")
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                    }

                    if (worksheet.Row(endRow).OutlineLevel != 0)
                    {
                        managerArticleAttribute.ReadFromXlsx(worksheet, endRow, ExportArticleAttribute.ProducAttributeCellOffset);
                        if (!managerArticleAttribute.IsCaption)
                        {
                            var aid = worksheet.Cells[endRow, attributeIdCellNum].Value.Return(Convert.ToInt32, -1);
                            allAttributeIds.Add(aid);
                        }

                        endRow++;
                        continue;
                    }

                    if (categoryCellNum > 0)
                    { 
                        var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!categoryIds.IsEmpty())
                            allCategoriesNames.AddRange(categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    if (skuCellNum > 0)
                    {
                        var sku = worksheet.Cells[endRow, skuCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!sku.IsEmpty())
                            allSku.Add(sku);
                    }

                    if (publisherCellNum > 0)
                    { 
                        var publisherIds = worksheet.Cells[endRow, publisherCellNum].Value.Return(p => p.ToString(), string.Empty);
                        if (!publisherIds.IsEmpty())
                            allPublishersNames.AddRange(publisherIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    //counting the number of articles
                    countArticlesInFile += 1;

                    endRow++;
                }

                //performance optimization, the check for the existence of the categories in one SQL request
                var notExistingCategories = _categoryService.GetNotExistingCategories(allCategoriesNames.ToArray());
                if (notExistingCategories.Any())
                {
                    throw new ArgumentException(string.Format("The following category name(s) don't exist - {0}", string.Join(", ", notExistingCategories)));
                }

                //performance optimization, the check for the existence of the publishers in one SQL request
                var notExistingPublishers = _publisherService.GetNotExistingPublishers(allPublishersNames.ToArray());
                if (notExistingPublishers.Any())
                {
                    throw new ArgumentException(string.Format("The following publisher name(s) don't exist - {0}", string.Join(", ", notExistingPublishers)));
                }

                //performance optimization, the check for the existence of the article attributes in one SQL request
                var notExistingArticleAttributes = _articleAttributeService.GetNotExistingAttributes(allAttributeIds.ToArray());
                if (notExistingArticleAttributes.Any())
                {
                    throw new ArgumentException(string.Format("The following article attribute ID(s) don't exist - {0}", string.Join(", ", notExistingArticleAttributes)));
                }

                //performance optimization, load all articles by SKU in one SQL request
                var allArticlesBySku = _articleService.GetArticlesBySku(allSku.ToArray(), _workContext.CurrentContributor.Return(v=>v.Id, 0));

                //validate maximum number of articles per contributor
                if (_contributorSettings.MaximumArticleNumber > 0 &&
                    _workContext.CurrentContributor != null)
                {
                    var newArticlesCount = countArticlesInFile - allArticlesBySku.Count;
                    if(_articleService.GetNumberOfArticlesByContributorId(_workContext.CurrentContributor.Id) + newArticlesCount > _contributorSettings.MaximumArticleNumber)
                        throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Articles.ExceededMaximumNumber"), _contributorSettings.MaximumArticleNumber));
                }

                //performance optimization, load all categories IDs for articles in one SQL request
                var allArticlesCategoryIds = _categoryService.GetArticleCategoryIds(allArticlesBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all categories in one SQL request
                var allCategories = _categoryService.GetAllCategories(showHidden: true);

                //performance optimization, load all publishers IDs for articles in one SQL request
                var allArticlesPublisherIds = _publisherService.GetArticlePublisherIds(allArticlesBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all publishers in one SQL request
                var allPublishers = _publisherService.GetAllPublishers(showHidden: true);

                //article to import images
                var articlePictureMetadata = new List<ArticlePictureMetadata>();

                Article lastLoadedArticle = null;

                for (var iRow = 2; iRow < endRow; iRow++)
                {
                    //imports article attributes
                    if (worksheet.Row(iRow).OutlineLevel != 0)
                    {
                        if (_catalogSettings.ExportImportArticleAttributes)
                        {
                            managerArticleAttribute.ReadFromXlsx(worksheet, iRow,
                                ExportArticleAttribute.ProducAttributeCellOffset);
                            if (lastLoadedArticle == null || managerArticleAttribute.IsCaption)
                                continue;

                            var articleAttributeId = managerArticleAttribute.GetProperty("AttributeId").IntValue;
                            var attributeControlTypeId = managerArticleAttribute.GetProperty("AttributeControlType").IntValue;
                            
                            var articleAttributeValueId = managerArticleAttribute.GetProperty("ArticleAttributeValueId").IntValue;
                            var associatedArticleId = managerArticleAttribute.GetProperty("AssociatedArticleId").IntValue;
                            var valueName = managerArticleAttribute.GetProperty("ValueName").StringValue;
                            var attributeValueTypeId = managerArticleAttribute.GetProperty("AttributeValueType").IntValue;
                            var colorSquaresRgb = managerArticleAttribute.GetProperty("ColorSquaresRgb").StringValue;
                            var imageSquaresPictureId = managerArticleAttribute.GetProperty("ImageSquaresPictureId").IntValue;
                            var priceAdjustment = managerArticleAttribute.GetProperty("PriceAdjustment").DecimalValue;
                            var weightAdjustment = managerArticleAttribute.GetProperty("WeightAdjustment").DecimalValue;
                            var cost = managerArticleAttribute.GetProperty("Cost").DecimalValue;
                            var customerEntersQty = managerArticleAttribute.GetProperty("CustomerEntersQty").BooleanValue;
                            var quantity = managerArticleAttribute.GetProperty("Quantity").IntValue;
                            var isPreSelected = managerArticleAttribute.GetProperty("IsPreSelected").BooleanValue;
                            var displaySubscription = managerArticleAttribute.GetProperty("DisplaySubscription").IntValue;
                            var pictureId = managerArticleAttribute.GetProperty("PictureId").IntValue;
                            var textPrompt = managerArticleAttribute.GetProperty("AttributeTextPrompt").StringValue;
                            var isRequired = managerArticleAttribute.GetProperty("AttributeIsRequired").BooleanValue;
                            var attributeDisplaySubscription = managerArticleAttribute.GetProperty("AttributeDisplaySubscription").IntValue;

                            var articleAttributeMapping = lastLoadedArticle.ArticleAttributeMappings.FirstOrDefault(pam => pam.ArticleAttributeId == articleAttributeId);
                            
                            if (articleAttributeMapping == null)
                            {
                                //insert mapping
                                articleAttributeMapping = new ArticleAttributeMapping
                                {
                                    ArticleId = lastLoadedArticle.Id,
                                    ArticleAttributeId = articleAttributeId,
                                    TextPrompt = textPrompt,
                                    IsRequired = isRequired,
                                    AttributeControlTypeId = attributeControlTypeId,
                                    DisplaySubscription = attributeDisplaySubscription
                                };
                                _articleAttributeService.InsertArticleAttributeMapping(articleAttributeMapping);
                            }
                            else
                            {
                                articleAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                                articleAttributeMapping.TextPrompt = textPrompt;
                                articleAttributeMapping.IsRequired = isRequired;
                                articleAttributeMapping.DisplaySubscription = attributeDisplaySubscription;
                                _articleAttributeService.UpdateArticleAttributeMapping(articleAttributeMapping);
                            }

                            var pav = _articleAttributeService.GetArticleAttributeValueById(articleAttributeValueId);

                            var attributeControlType = (AttributeControlType) attributeControlTypeId;

                            if (pav == null)
                            {
                                switch (attributeControlType)
                                {
                                    case AttributeControlType.Datepicker:
                                    case AttributeControlType.FileUpload:
                                    case AttributeControlType.MultilineTextbox:
                                    case AttributeControlType.TextBox:
                                        continue;
                                }

                                pav = new ArticleAttributeValue
                                {
                                    ArticleAttributeMappingId = articleAttributeMapping.Id,
                                    AttributeValueType = (AttributeValueType) attributeValueTypeId,
                                    AssociatedArticleId = associatedArticleId,
                                    Name = valueName,
                                    PriceAdjustment = priceAdjustment,
                                    WeightAdjustment = weightAdjustment,
                                    Cost = cost,
                                    IsPreSelected = isPreSelected,
                                    DisplaySubscription = displaySubscription,
                                    ColorSquaresRgb = colorSquaresRgb,
                                    ImageSquaresPictureId = imageSquaresPictureId,
                                    CustomerEntersQty = customerEntersQty,
                                    Quantity = quantity,
                                    PictureId = pictureId
                                };

                                _articleAttributeService.InsertArticleAttributeValue(pav);
                            }
                            else
                            {
                                pav.AttributeValueTypeId = attributeValueTypeId;
                                pav.AssociatedArticleId = associatedArticleId;
                                pav.Name = valueName;
                                pav.ColorSquaresRgb = colorSquaresRgb;
                                pav.ImageSquaresPictureId = imageSquaresPictureId;
                                pav.PriceAdjustment = priceAdjustment;
                                pav.WeightAdjustment = weightAdjustment;
                                pav.Cost = cost;
                                pav.CustomerEntersQty = customerEntersQty;
                                pav.Quantity = quantity;
                                pav.IsPreSelected = isPreSelected;
                                pav.DisplaySubscription = displaySubscription;
                                pav.PictureId = pictureId;

                                _articleAttributeService.UpdateArticleAttributeValue(pav);
                            }
                        }
                        continue;
                    }

                    manager.ReadFromXlsx(worksheet, iRow);

                    var article = skuCellNum > 0 ? allArticlesBySku.FirstOrDefault(p => p.Sku == manager.GetProperty("SKU").StringValue) : null;

                    var isNew = article == null;

                    article = article ?? new Article();

                   

                    if (isNew)
                        article.CreatedOnUtc = DateTime.UtcNow;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "ArticleType":
                                article.ArticleTypeId = property.IntValue;
                                break;
                            case "ParentGroupedArticleId":
                                article.ParentGroupedArticleId = property.IntValue;
                                break;
                            case "VisibleIndividually":
                                article.VisibleIndividually = property.BooleanValue;
                                break;
                            case "Name":
                                article.Name = property.StringValue;
                                break;
                            case "ShortDescription":
                                article.ShortDescription = property.StringValue;
                                break;
                            case "FullDescription":
                                article.FullDescription = property.StringValue;
                                break;
                            case "Contributor":
                                //contributor can't change this field
                                if (_workContext.CurrentContributor == null)
                                    article.ContributorId = property.IntValue;
                                break;
                            case "ArticleTemplate":
                                article.ArticleTemplateId = property.IntValue;
                                break;
                            case "ShowOnHomePage":
                                //contributor can't change this field
                                if (_workContext.CurrentContributor == null)
                                    article.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "MetaKeywords":
                                article.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                article.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                article.MetaTitle = property.StringValue;
                                break;
                            case "AllowCustomerReviews":
                                article.AllowCustomerReviews = property.BooleanValue;
                                break;
                            case "Published":
                                article.Published = property.BooleanValue;
                                break;
                            case "SKU":
                                article.Sku = property.StringValue;
                                break;
                            case "PublisherPartNumber":
                                article.PublisherPartNumber = property.StringValue;
                                break;
                            case "Gtin":
                                article.Gtin = property.StringValue;
                                break;
                            case "IsGiftCard":
                                article.IsGiftCard = property.BooleanValue;
                                break;
                            case "GiftCardType":
                                article.GiftCardTypeId = property.IntValue;
                                break;
                            case "OverriddenGiftCardAmount":
                                article.OverriddenGiftCardAmount = property.DecimalValue;
                                break;
                            case "RequireOtherArticles":
                                article.RequireOtherArticles = property.BooleanValue;
                                break;
                            case "RequiredArticleIds":
                                article.RequiredArticleIds = property.StringValue;
                                break;
                            case "AutomaticallyAddRequiredArticles":
                                article.AutomaticallyAddRequiredArticles = property.BooleanValue;
                                break;
                           
                            case "HasUserAgreement":
                                article.HasUserAgreement = property.BooleanValue;
                                break;
                            case "UserAgreementText":
                                article.UserAgreementText = property.StringValue;
                                break;
                            case "IsRecurring":
                                article.IsRecurring = property.BooleanValue;
                                break;
                            case "RecurringCycleLength":
                                article.RecurringCycleLength = property.IntValue;
                                break;
                            case "RecurringCyclePeriod":
                                article.RecurringCyclePeriodId = property.IntValue;
                                break;
                            case "RecurringTotalCycles":
                                article.RecurringTotalCycles = property.IntValue;
                                break;
                            case "IsRental":
                                article.IsRental = property.BooleanValue;
                                break;
                            case "RentalPriceLength":
                                article.RentalPriceLength = property.IntValue;
                                break;
                            case "RentalPricePeriod":
                                article.RentalPricePeriodId = property.IntValue;
                                break;
                              
                            case "IsTaxExempt":
                                article.IsTaxExempt = property.BooleanValue;
                                break;
                            case "TaxCategory":
                                article.TaxCategoryId = property.IntValue;
                                break;
                              
                            case "AllowAddingOnlyExistingAttributeCombinations":
                                article.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                                break;
                            case "NotReturnable":
                                article.NotReturnable = property.BooleanValue;
                                break;
                            case "DisableBuyButton":
                                article.DisableBuyButton = property.BooleanValue;
                                break;
                            case "DisableWishlistButton":
                                article.DisableWishlistButton = property.BooleanValue;
                                break;
                            case "AvailableForPreSubscription":
                                article.AvailableForPreSubscription = property.BooleanValue;
                                break;
                            case "PreSubscriptionAvailabilityStartDateTimeUtc":
                                article.PreSubscriptionAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CallForPrice":
                                article.CallForPrice = property.BooleanValue;
                                break;
                            case "Price":
                                article.Price = property.DecimalValue;
                                break;
                            case "OldPrice":
                                article.OldPrice = property.DecimalValue;
                                break;
                            case "ArticleCost":
                                article.ArticleCost = property.DecimalValue;
                                break;
                            
                            case "MarkAsNew":
                                article.MarkAsNew = property.BooleanValue;
                                break;
                            case "MarkAsNewStartDateTimeUtc":
                                article.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "MarkAsNewEndDateTimeUtc":
                                article.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                                break;
                          
                        }
                    }

                    //set some default default values if not specified
                    if (isNew && properties.All(p => p.PropertyName != "ArticleType"))
                        article.ArticleType = ArticleType.SimpleArticle;
                    if (isNew && properties.All(p => p.PropertyName != "VisibleIndividually"))
                        article.VisibleIndividually = true;
                    if (isNew && properties.All(p => p.PropertyName != "Published"))
                        article.Published = true;

                    //sets the current contributor for the new article
                    if (isNew && _workContext.CurrentContributor != null)
                        article.ContributorId = _workContext.CurrentContributor.Id;
                    
                    article.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _articleService.InsertArticle(article);
                    }
                    else
                    {
                        _articleService.UpdateArticle(article);
                    }

                   
                    tempProperty = manager.GetProperty("SeName");
                    if (tempProperty != null)
                    {
                        var seName = tempProperty.StringValue;
                        //search engine name
                        _urlRecordService.SaveSlug(article, article.ValidateSeName(seName, article.Name, true), 0);
                    }

                    tempProperty = manager.GetProperty("Categories");

                    if (tempProperty != null)
                    { 
                        var categoryNames = tempProperty.StringValue;

                        //category mappings
                        var categories = isNew || !allArticlesCategoryIds.ContainsKey(article.Id) ? new int[0] : allArticlesCategoryIds[article.Id];
                        var importedCategories = categoryNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allCategories.First(c => c.Name == x.Trim()).Id).ToList();
                        foreach (var categoryId in importedCategories)
                        {
                            if (categories.Any(c => c == categoryId))
                                continue;
                       
                            var articleCategory = new ArticleCategory
                            {
                                ArticleId = article.Id,
                                CategoryId = categoryId,
                                IsFeaturedArticle = false,
                                DisplaySubscription = 1
                            };
                            _categoryService.InsertArticleCategory(articleCategory);
                        }

                        //delete article categories
                        var deletedArticleCategories = categories.Where(categoryId => !importedCategories.Contains(categoryId))
                                .Select(categoryId => article.ArticleCategories.First(pc => pc.CategoryId == categoryId));
                        foreach (var deletedArticleCategory in deletedArticleCategories)
                        {
                            _categoryService.DeleteArticleCategory(deletedArticleCategory);
                        }
                    }

                    tempProperty = manager.GetProperty("Publishers");
                    if (tempProperty != null)
                    {
                        var publisherNames = tempProperty.StringValue;

                        //publisher mappings
                        var publishers = isNew || !allArticlesPublisherIds.ContainsKey(article.Id) ? new int[0] : allArticlesPublisherIds[article.Id];
                        var importedPublishers = publisherNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allPublishers.First(m => m.Name == x.Trim()).Id).ToList();
                        foreach (var publisherId in importedPublishers)
                        {
                            if (publishers.Any(c => c == publisherId))
                                continue;

                            var articlePublisher = new ArticlePublisher
                            {
                                ArticleId = article.Id,
                                PublisherId = publisherId,
                                IsFeaturedArticle = false,
                                DisplaySubscription = 1
                            };
                            _publisherService.InsertArticlePublisher(articlePublisher);
                        }

                        //delete article publishers
                        var deletedArticlesPublishers = publishers.Where(publisherId => !importedPublishers.Contains(publisherId))
                                .Select(publisherId => article.ArticlePublishers.First(pc => pc.PublisherId == publisherId));
                        foreach (var deletedArticlePublisher in deletedArticlesPublishers)
                        {
                            _publisherService.DeleteArticlePublisher(deletedArticlePublisher);
                        }
                    }

                    tempProperty = manager.GetProperty("ArticleTags");
                    if (tempProperty != null)
                    {
                        var articleTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

                        //article tag mappings
                        _articleTagService.UpdateArticleTags(article, articleTags);
                    }

                    var picture1 = manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty);
                    var picture2 = manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty);
                    var picture3 = manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty);

                    articlePictureMetadata.Add(new ArticlePictureMetadata
                    {
                        ArticleItem = article,
                        Picture1Path = picture1,
                        Picture2Path = picture2,
                        Picture3Path = picture3,
                        IsNew = isNew
                    });

                    lastLoadedArticle = article;
 
                    //_articleService.UpdateHasDiscountsApplied(article);
                }
               
                if (_mediaSettings.ImportArticleImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash() > 0)
                    ImportArticleImagesUsingHash(articlePictureMetadata, allArticlesBySku);
                else
                    ImportArticleImagesUsingServices(articlePictureMetadata);

                //activity log
                _customerActivityService.InsertActivity("ImportArticles", _localizationService.GetResource("ActivityLog.ImportArticles"), countArticlesInFile);
            }
        }
        
        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        public virtual int ImportNewsletterSubscribersFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    string email;
                    bool isActive = true;
                    int storeId = _storeContext.CurrentStore.Id;
                    //parse
                    if (tmp.Length == 1)
                    {
                        //"email" only
                        email = tmp[0].Trim();
                    }
                    else if (tmp.Length == 2)
                    {
                        //"email" and "active" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                    }
                    else if (tmp.Length == 3)
                    {
                        //"email" and "active" and "storeId" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                        storeId = Int32.Parse(tmp[2].Trim());
                    }
                    else
                        throw new YStoryException("Wrong file format");

                    //import
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterOrderByEmailAndStoreId(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        public virtual int ImportStatesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new YStoryException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    bool published = Boolean.Parse(tmp[3].Trim());
                    int displaySubscription = Int32.Parse(tmp[4].Trim());

                    var country = _countryService.GetCountryByTwoLetterIsoCode(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplaySubscription = displaySubscription;
                        _stateProvinceService.UpdateStateProvince(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplaySubscription = displaySubscription,
                        };
                        _stateProvinceService.InsertStateProvince(state);
                    }
                    count++;
                }
            }

            //activity log
            _customerActivityService.InsertActivity("ImportStates", _localizationService.GetResource("ActivityLog.ImportStates"), count);

            return count;
        }

        /// <summary>
        /// Import publishers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportPublishersFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new YStoryException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Publisher>(worksheet);

                var manager = new PropertyManager<Publisher>(properties);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertySubscriptionPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var publisher = _publisherService.GetPublisherById(manager.GetProperty("Id").IntValue);

                    var isNew = publisher == null;

                    publisher = publisher ?? new Publisher();

                    if (isNew)
                    {
                        publisher.CreatedOnUtc = DateTime.UtcNow;

                        //default values
                        publisher.PageSize = _catalogSettings.DefaultPublisherPageSize;
                        publisher.PageSizeOptions = _catalogSettings.DefaultPublisherPageSizeOptions;
                        publisher.Published = true;
                        publisher.AllowCustomersToSelectPageSize = true;
                    }

                    var seName = string.Empty;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                publisher.Name = property.StringValue;
                                break;
                            case "Description":
                                publisher.Description = property.StringValue;
                                break;
                            case "PublisherTemplateId":
                                publisher.PublisherTemplateId = property.IntValue;
                                break;
                            case "MetaKeywords":
                                publisher.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                publisher.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                publisher.MetaTitle = property.StringValue;
                                break;
                            case "Picture":
                                var picture = LoadPicture(manager.GetProperty("Picture").StringValue, publisher.Name,
                                    isNew ? null : (int?) publisher.PictureId);

                                if (picture != null)
                                    publisher.PictureId = picture.Id;

                                break;
                            case "PageSize":
                                publisher.PageSize = property.IntValue;
                                break;
                            case "AllowCustomersToSelectPageSize":
                                publisher.AllowCustomersToSelectPageSize = property.BooleanValue;
                                break;
                            case "PageSizeOptions":
                                publisher.PageSizeOptions = property.StringValue;
                                break;
                            case "PriceRanges":
                                publisher.PriceRanges = property.StringValue;
                                break;
                            case "Published":
                                publisher.Published = property.BooleanValue;
                                break;
                            case "DisplaySubscription":
                                publisher.DisplaySubscription = property.IntValue;
                                break;
                            case "SeName":
                                seName = property.StringValue;
                                break;
                        }
                    }

                    publisher.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _publisherService.InsertPublisher(publisher);
                    else
                        _publisherService.UpdatePublisher(publisher);

                    //search engine name
                    if (setSeName)
                        _urlRecordService.SaveSlug(publisher, publisher.ValidateSeName(seName, publisher.Name, true), 0);

                    iRow++;
                }

                //activity log
                _customerActivityService.InsertActivity("ImportPublishers", _localizationService.GetResource("ActivityLog.ImportPublishers"), iRow - 2);
            }
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportCategoriesFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new YStoryException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Category>(worksheet);

                var manager = new PropertyManager<Category>(properties);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertySubscriptionPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var category = _categoryService.GetCategoryById(manager.GetProperty("Id").IntValue);

                    var isNew = category == null;

                    category = category ?? new Category();

                    if (isNew)
                    {
                        category.CreatedOnUtc = DateTime.UtcNow;
                        //default values
                        category.PageSize = _catalogSettings.DefaultCategoryPageSize;
                        category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                        category.Published = true;
                        category.IncludeInTopMenu = true;
                        category.AllowCustomersToSelectPageSize = true;
                    }

                    var seName = string.Empty;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                category.Name = property.StringValue;
                                break;
                            case "Description":
                                category.Description = property.StringValue;
                                break;
                            case "CategoryTemplateId":
                                category.CategoryTemplateId = property.IntValue;
                                break;
                            case "MetaKeywords":
                                category.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                category.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                category.MetaTitle = property.StringValue;
                                break;
                            case "ParentCategoryId":
                                category.ParentCategoryId = property.IntValue;
                                break;
                            case "Picture":
                                var picture = LoadPicture(manager.GetProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                                if (picture != null)
                                    category.PictureId = picture.Id;
                                break;
                            case "PageSize":
                                category.PageSize = property.IntValue;
                                break;
                            case "AllowCustomersToSelectPageSize":
                                category.AllowCustomersToSelectPageSize = property.BooleanValue;
                                break;
                            case "PageSizeOptions":
                                category.PageSizeOptions = property.StringValue;
                                break;
                            case "PriceRanges":
                                category.PriceRanges = property.StringValue;
                                break;
                            case "ShowOnHomePage":
                                category.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "IncludeInTopMenu":
                                category.IncludeInTopMenu = property.BooleanValue;
                                break;
                            case "Published":
                                category.Published = property.BooleanValue;
                                break;
                            case "DisplaySubscription":
                                category.DisplaySubscription = property.IntValue;
                                break;
                            case "SeName":
                                seName = property.StringValue;
                                break;
                        }
                    }

                    category.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _categoryService.InsertCategory(category);
                    else
                        _categoryService.UpdateCategory(category);

                    //search engine name
                    if (setSeName)
                        _urlRecordService.SaveSlug(category, category.ValidateSeName(seName, category.Name, true), 0);

                    iRow++;
                }

                //activity log
                _customerActivityService.InsertActivity("ImportCategories", _localizationService.GetResource("ActivityLog.ImportCategories"), iRow - 2);
            }
        }

        #endregion

        #region Nested classes

        protected class ArticlePictureMetadata
        {
            public Article ArticleItem { get; set; }
            public string Picture1Path { get; set; }
            public string Picture2Path { get; set; }
            public string Picture3Path { get; set; }
            public bool IsNew { get; set; }
        }

        #endregion
    }
}
