using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Media;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Seo;
using YStory.Services.Stores;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Copy Article service
    /// </summary>
    public partial class CopyArticleService : ICopyArticleService
    {
        #region Fields

        private readonly IArticleService _articleService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CopyArticleService(IArticleService articleService,
            IArticleAttributeService articleAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, 
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICategoryService categoryService, 
            IPublisherService publisherService,
            ISpecificationAttributeService specificationAttributeService,
            IDownloadService downloadService,
            IArticleAttributeParser articleAttributeParser,
            IUrlRecordService urlRecordService, 
            IStoreMappingService storeMappingService)
        {
            this._articleService = articleService;
            this._articleAttributeService = articleAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._specificationAttributeService = specificationAttributeService;
            this._downloadService = downloadService;
            this._articleAttributeParser = articleAttributeParser;
            this._urlRecordService = urlRecordService;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of article with all depended data
        /// </summary>
        /// <param name="article">The article to copy</param>
        /// <param name="newName">The name of article duplicate</param>
        /// <param name="isPublished">A value indicating whether the article duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the article images should be copied</param>
        /// <param name="copyAssociatedArticles">A value indicating whether the copy associated articles</param>
        /// <returns>Article copy</returns>
        public virtual Article CopyArticle(Article article, string newName,
            bool isPublished = true, bool copyImages = true, bool copyAssociatedArticles = true)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (String.IsNullOrEmpty(newName))
                throw new ArgumentException("Article name is required");

            

            var newSku = !String.IsNullOrWhiteSpace(article.Sku)
                ? string.Format(_localizationService.GetResource("Admin.Catalog.Articles.Copy.SKU.New"), article.Sku) :
                article.Sku;
            // article
            var articleCopy = new Article
            {
                ArticleTypeId = article.ArticleTypeId,
                ParentGroupedArticleId = article.ParentGroupedArticleId,
                VisibleIndividually = article.VisibleIndividually,
                Name = newName,
                ShortDescription = article.ShortDescription,
                FullDescription = article.FullDescription,
                ContributorId = article.ContributorId,
                ArticleTemplateId = article.ArticleTemplateId,
                AdminComment = article.AdminComment,
                ShowOnHomePage = article.ShowOnHomePage,
                MetaKeywords = article.MetaKeywords,
                MetaDescription = article.MetaDescription,
                MetaTitle = article.MetaTitle,
                AllowCustomerReviews = article.AllowCustomerReviews,
                LimitedToStores = article.LimitedToStores,
                Sku = newSku,
                PublisherPartNumber = article.PublisherPartNumber,
                Gtin = article.Gtin,
                IsGiftCard = article.IsGiftCard,
              
                OverriddenGiftCardAmount = article.OverriddenGiftCardAmount,
                RequireOtherArticles = article.RequireOtherArticles,
                RequiredArticleIds = article.RequiredArticleIds,
                AutomaticallyAddRequiredArticles = article.AutomaticallyAddRequiredArticles,
          
                HasUserAgreement = article.HasUserAgreement,
                UserAgreementText = article.UserAgreementText,
                IsRecurring = article.IsRecurring,
                RecurringCycleLength = article.RecurringCycleLength,
                RecurringCyclePeriod = article.RecurringCyclePeriod,
                RecurringTotalCycles = article.RecurringTotalCycles,
                IsRental = article.IsRental,
                RentalPriceLength = article.RentalPriceLength,
                RentalPricePeriod = article.RentalPricePeriod,
              
                IsTaxExempt = article.IsTaxExempt,
                TaxCategoryId = article.TaxCategoryId,
              
                AllowAddingOnlyExistingAttributeCombinations = article.AllowAddingOnlyExistingAttributeCombinations,
                NotReturnable = article.NotReturnable,
                DisableBuyButton = article.DisableBuyButton,
                DisableWishlistButton = article.DisableWishlistButton,
                AvailableForPreSubscription = article.AvailableForPreSubscription,
                PreSubscriptionAvailabilityStartDateTimeUtc = article.PreSubscriptionAvailabilityStartDateTimeUtc,
                CallForPrice = article.CallForPrice,
                Price = article.Price,
                OldPrice = article.OldPrice,
                ArticleCost = article.ArticleCost,
                 
                MarkAsNew = article.MarkAsNew,
                MarkAsNewStartDateTimeUtc = article.MarkAsNewStartDateTimeUtc,
                MarkAsNewEndDateTimeUtc = article.MarkAsNewEndDateTimeUtc,
                
                AvailableStartDateTimeUtc = article.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = article.AvailableEndDateTimeUtc,
                DisplaySubscription = article.DisplaySubscription,
                Published = isPublished,
                Deleted = article.Deleted,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            //validate search engine name
            _articleService.InsertArticle(articleCopy);

            //search engine name
            _urlRecordService.SaveSlug(articleCopy, articleCopy.ValidateSeName("", articleCopy.Name, true), 0);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var name = article.GetLocalized(x => x.Name, lang.Id, false, false);
                if (!String.IsNullOrEmpty(name))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.Name, name, lang.Id);

                var shortDescription = article.GetLocalized(x => x.ShortDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(shortDescription))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = article.GetLocalized(x => x.FullDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(fullDescription))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = article.GetLocalized(x => x.MetaKeywords, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaKeywords))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = article.GetLocalized(x => x.MetaDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaDescription))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = article.GetLocalized(x => x.MetaTitle, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaTitle))
                    _localizedEntityService.SaveLocalizedValue(articleCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                _urlRecordService.SaveSlug(articleCopy, articleCopy.ValidateSeName("", name, false), lang.Id);
            }

            //article tags
            foreach (var articleTag in article.ArticleTags)
            {
                articleCopy.ArticleTags.Add(articleTag);
            }
            _articleService.UpdateArticle(articleCopy);

            //article pictures
            //variable to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (copyImages)
            {
                foreach (var articlePicture in article.ArticlePictures)
                {
                    var picture = articlePicture.Picture;
                    var pictureCopy = _pictureService.InsertPicture(
                        _pictureService.LoadPictureBinary(picture),
                        picture.MimeType,
                        _pictureService.GetPictureSeName(newName),
                        picture.AltAttribute,
                        picture.TitleAttribute);
                    _articleService.InsertArticlePicture(new ArticlePicture
                    {
                        ArticleId = articleCopy.Id,
                        PictureId = pictureCopy.Id,
                        DisplaySubscription = articlePicture.DisplaySubscription
                    });
                    originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
                }
            }

           
            _articleService.UpdateArticle(articleCopy);

            // article <-> categories mappings
            foreach (var articleCategory in article.ArticleCategories)
            {
                var articleCategoryCopy = new ArticleCategory
                {
                    ArticleId = articleCopy.Id,
                    CategoryId = articleCategory.CategoryId,
                    IsFeaturedArticle = articleCategory.IsFeaturedArticle,
                    DisplaySubscription = articleCategory.DisplaySubscription
                };

                _categoryService.InsertArticleCategory(articleCategoryCopy);
            }

            // article <-> publishers mappings
            foreach (var articlePublishers in article.ArticlePublishers)
            {
                var articlePublisherCopy = new ArticlePublisher
                {
                    ArticleId = articleCopy.Id,
                    PublisherId = articlePublishers.PublisherId,
                    IsFeaturedArticle = articlePublishers.IsFeaturedArticle,
                    DisplaySubscription = articlePublishers.DisplaySubscription
                };

                _publisherService.InsertArticlePublisher(articlePublisherCopy);
            }

            // article <-> releated articles mappings
            foreach (var relatedArticle in _articleService.GetRelatedArticlesByArticleId1(article.Id, true))
            {
                _articleService.InsertRelatedArticle(
                    new RelatedArticle
                    {
                        ArticleId1 = articleCopy.Id,
                        ArticleId2 = relatedArticle.ArticleId2,
                        DisplaySubscription = relatedArticle.DisplaySubscription
                    });
            }

            // article <-> cross sells mappings
            foreach (var csArticle in _articleService.GetCrossSellArticlesByArticleId1(article.Id, true))
            {
                _articleService.InsertCrossSellArticle(
                    new CrossSellArticle
                    {
                        ArticleId1 = articleCopy.Id,
                        ArticleId2 = csArticle.ArticleId2,
                    });
            }

            // article specifications
            foreach (var articleSpecificationAttribute in article.ArticleSpecificationAttributes)
            {
                var psaCopy = new ArticleSpecificationAttribute
                {
                    ArticleId = articleCopy.Id,
                    AttributeTypeId = articleSpecificationAttribute.AttributeTypeId,
                    SpecificationAttributeOptionId = articleSpecificationAttribute.SpecificationAttributeOptionId,
                    CustomValue = articleSpecificationAttribute.CustomValue,
                    AllowFiltering = articleSpecificationAttribute.AllowFiltering,
                    ShowOnArticlePage = articleSpecificationAttribute.ShowOnArticlePage,
                    DisplaySubscription = articleSpecificationAttribute.DisplaySubscription
                };
                _specificationAttributeService.InsertArticleSpecificationAttribute(psaCopy);
            }

            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(article);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(articleCopy, id);
            }

            //article <-> attributes mappings
            var associatedAttributes = new Dictionary<int, int>();
            var associatedAttributeValues = new Dictionary<int, int>();

            //attribute mapping with condition attributes
            var oldCopyWithConditionAttributes = new List<ArticleAttributeMapping>();

            //all article attribute mapping copies
            var articleAttributeMappingCopies = new Dictionary<int, ArticleAttributeMapping>();

            foreach (var articleAttributeMapping in _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id))
            {
                var articleAttributeMappingCopy = new ArticleAttributeMapping
                {
                    ArticleId = articleCopy.Id,
                    ArticleAttributeId = articleAttributeMapping.ArticleAttributeId,
                    TextPrompt = articleAttributeMapping.TextPrompt,
                    IsRequired = articleAttributeMapping.IsRequired,
                    AttributeControlTypeId = articleAttributeMapping.AttributeControlTypeId,
                    DisplaySubscription = articleAttributeMapping.DisplaySubscription,
                    ValidationMinLength = articleAttributeMapping.ValidationMinLength,
                    ValidationMaxLength = articleAttributeMapping.ValidationMaxLength,
                    ValidationFileAllowedExtensions = articleAttributeMapping.ValidationFileAllowedExtensions,
                    ValidationFileMaximumSize = articleAttributeMapping.ValidationFileMaximumSize,
                    DefaultValue = articleAttributeMapping.DefaultValue
                };
                _articleAttributeService.InsertArticleAttributeMapping(articleAttributeMappingCopy);

                articleAttributeMappingCopies.Add(articleAttributeMappingCopy.Id, articleAttributeMappingCopy);

                if (!string.IsNullOrEmpty(articleAttributeMapping.ConditionAttributeXml))
                {
                    oldCopyWithConditionAttributes.Add(articleAttributeMapping);
                }

                //save associated value (used for combinations copying)
                associatedAttributes.Add(articleAttributeMapping.Id, articleAttributeMappingCopy.Id);

                // article attribute values
                var articleAttributeValues = _articleAttributeService.GetArticleAttributeValues(articleAttributeMapping.Id);
                foreach (var articleAttributeValue in articleAttributeValues)
                {
                    int attributeValuePictureId = 0;
                    if (originalNewPictureIdentifiers.ContainsKey(articleAttributeValue.PictureId))
                    {
                        attributeValuePictureId = originalNewPictureIdentifiers[articleAttributeValue.PictureId];
                    }
                    var attributeValueCopy = new ArticleAttributeValue
                    {
                        ArticleAttributeMappingId = articleAttributeMappingCopy.Id,
                        AttributeValueTypeId = articleAttributeValue.AttributeValueTypeId,
                        AssociatedArticleId = articleAttributeValue.AssociatedArticleId,
                        Name = articleAttributeValue.Name,
                        ColorSquaresRgb = articleAttributeValue.ColorSquaresRgb,
                        PriceAdjustment = articleAttributeValue.PriceAdjustment,
                        WeightAdjustment = articleAttributeValue.WeightAdjustment,
                        Cost = articleAttributeValue.Cost,
                        CustomerEntersQty = articleAttributeValue.CustomerEntersQty,
                        Quantity = articleAttributeValue.Quantity,
                        IsPreSelected = articleAttributeValue.IsPreSelected,
                        DisplaySubscription = articleAttributeValue.DisplaySubscription,
                        PictureId = attributeValuePictureId,
                    };
                    //picture associated to "iamge square" attribute type (if exists)
                    if (articleAttributeValue.ImageSquaresPictureId > 0)
                    {
                        var origImageSquaresPicture = _pictureService.GetPictureById(articleAttributeValue.ImageSquaresPictureId);
                        if (origImageSquaresPicture != null)
                        {
                            //copy the picture
                            var imageSquaresPictureCopy = _pictureService.InsertPicture(
                                _pictureService.LoadPictureBinary(origImageSquaresPicture),
                                origImageSquaresPicture.MimeType,
                                origImageSquaresPicture.SeoFilename,
                                origImageSquaresPicture.AltAttribute,
                                origImageSquaresPicture.TitleAttribute);
                            attributeValueCopy.ImageSquaresPictureId = imageSquaresPictureCopy.Id;
                        }
                    }

                    _articleAttributeService.InsertArticleAttributeValue(attributeValueCopy);

                    //save associated value (used for combinations copying)
                    associatedAttributeValues.Add(articleAttributeValue.Id, attributeValueCopy.Id);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = articleAttributeValue.GetLocalized(x => x.Name, lang.Id, false, false);
                        if (!String.IsNullOrEmpty(name))
                            _localizedEntityService.SaveLocalizedValue(attributeValueCopy, x => x.Name, name, lang.Id);
                    }
                }
            }

            //copy attribute conditions
            foreach (var articleAttributeMapping in oldCopyWithConditionAttributes)
            {
                var oldConditionAttributeMapping = _articleAttributeParser.ParseArticleAttributeMappings(articleAttributeMapping.ConditionAttributeXml).FirstOrDefault();

                if (oldConditionAttributeMapping == null)
                    continue;

                var oldConditionValues = _articleAttributeParser.ParseArticleAttributeValues(articleAttributeMapping.ConditionAttributeXml, oldConditionAttributeMapping.Id);

                if (!oldConditionValues.Any())
                    continue;

                var newAttributeMappingId = associatedAttributes[oldConditionAttributeMapping.Id];
                var newConditionAttributeMapping = articleAttributeMappingCopies[newAttributeMappingId];

                var newConditionAttributeXml = string.Empty;

                foreach (var oldConditionValue in oldConditionValues)
                {
                    newConditionAttributeXml = _articleAttributeParser.AddArticleAttribute(newConditionAttributeXml, newConditionAttributeMapping, associatedAttributeValues[oldConditionValue.Id].ToString());
                }

                var attributeMappingId = associatedAttributes[articleAttributeMapping.Id];
                var conditionAttribute = articleAttributeMappingCopies[attributeMappingId];
                conditionAttribute.ConditionAttributeXml = newConditionAttributeXml;

                _articleAttributeService.UpdateArticleAttributeMapping(conditionAttribute);
            }

            //attribute combinations
            foreach (var combination in _articleAttributeService.GetAllArticleAttributeCombinations(article.Id))
            {
                //generate new AttributesXml according to new value IDs
                string newAttributesXml = "";
                var parsedArticleAttributes = _articleAttributeParser.ParseArticleAttributeMappings(combination.AttributesXml);
                foreach (var oldAttribute in parsedArticleAttributes)
                {
                    if (associatedAttributes.ContainsKey(oldAttribute.Id))
                    {
                        var newAttribute = _articleAttributeService.GetArticleAttributeMappingById(associatedAttributes[oldAttribute.Id]);
                        if (newAttribute != null)
                        {
                            var oldAttributeValuesStr = _articleAttributeParser.ParseValues(combination.AttributesXml, oldAttribute.Id);
                            foreach (var oldAttributeValueStr in oldAttributeValuesStr)
                            {
                                if (newAttribute.ShouldHaveValues())
                                {
                                    //attribute values
                                    int oldAttributeValue = int.Parse(oldAttributeValueStr);
                                    if (associatedAttributeValues.ContainsKey(oldAttributeValue))
                                    {
                                        var newAttributeValue = _articleAttributeService.GetArticleAttributeValueById(associatedAttributeValues[oldAttributeValue]);
                                        if (newAttributeValue != null)
                                        {
                                            newAttributesXml = _articleAttributeParser.AddArticleAttribute(newAttributesXml,
                                                newAttribute, newAttributeValue.Id.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    //just a text
                                    newAttributesXml = _articleAttributeParser.AddArticleAttribute(newAttributesXml,
                                        newAttribute, oldAttributeValueStr);
                                }
                            }
                        }
                    }
                }
                var combinationCopy = new ArticleAttributeCombination
                {
                    ArticleId = articleCopy.Id,
                    AttributesXml = newAttributesXml,
                    StockQuantity = combination.StockQuantity,
                    AllowOutOfStockSubscriptions = combination.AllowOutOfStockSubscriptions,
                    Sku = combination.Sku,
                    PublisherPartNumber = combination.PublisherPartNumber,
                    Gtin = combination.Gtin,
                    OverriddenPrice = combination.OverriddenPrice,
                    NotifyAdminForQuantityBelow = combination.NotifyAdminForQuantityBelow
                };
                _articleAttributeService.InsertArticleAttributeCombination(combinationCopy);

               }

            
            
        
            _articleService.UpdateHasDiscountsApplied(articleCopy);

            //associated articles
            if (copyAssociatedArticles)
            {
                var associatedArticles = _articleService.GetAssociatedArticles(article.Id, showHidden: true);
                foreach (var associatedArticle in associatedArticles)
                {
                    var associatedArticleCopy = CopyArticle(associatedArticle, string.Format("Copy of {0}", associatedArticle.Name),
                        isPublished, copyImages, false);
                    associatedArticleCopy.ParentGroupedArticleId = articleCopy.Id;
                    _articleService.UpdateArticle(articleCopy);
                }
            }

            return articleCopy;
        }

        #endregion
    }
}
