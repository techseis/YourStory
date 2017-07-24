using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.Subscriptions;
 
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.ExportImport.Help;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Seo;
 
using YStory.Services.Stores;
using YStory.Services.Tax;
using YStory.Services.Contributors;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace YStory.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly ICustomerService _customerService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ArticleEditorSettings _articleEditorSettings;
        private readonly IContributorService _contributorService;
        private readonly IArticleTemplateService _articleTemplateService;
        
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IMeasureService _measureService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly SubscriptionSettings _subscriptionSettings;

        #endregion

        #region Ctor

        public ExportManager(ICategoryService categoryService,
            IPublisherService publisherService,
            ICustomerService customerService,
            IArticleAttributeService articleAttributeService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStoreService storeService,
            IWorkContext workContext,
            ArticleEditorSettings articleEditorSettings,
            IContributorService contributorService,
            IArticleTemplateService articleTemplateService,
            
            ITaxCategoryService taxCategoryService,
            IMeasureService measureService,
            CatalogSettings catalogSettings,
            IGenericAttributeService genericAttributeService,
            ICustomerAttributeFormatter customerAttributeFormatter,
            SubscriptionSettings subscriptionSettings)
        {
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._customerService = customerService;
            this._articleAttributeService = articleAttributeService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._articleEditorSettings = articleEditorSettings;
            this._contributorService = contributorService;
            this._articleTemplateService = articleTemplateService;
             
            this._taxCategoryService = taxCategoryService;
            this._measureService = measureService;
            this._catalogSettings = catalogSettings;
            this._genericAttributeService = genericAttributeService;
            this._customerAttributeFormatter = customerAttributeFormatter;
            this._subscriptionSettings = subscriptionSettings;
        }

        #endregion

        #region Utilities

        protected virtual void WriteCategories(XmlWriter xmlWriter, int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            if (categories != null && categories.Any())
            {
                foreach (var category in categories)
                {
                    xmlWriter.WriteStartElement("Category");

                    xmlWriter.WriteString("Id", category.Id);

                    xmlWriter.WriteString("Name", category.Name);
                    xmlWriter.WriteString("Description", category.Description);
                    xmlWriter.WriteString("CategoryTemplateId", category.CategoryTemplateId);
                    xmlWriter.WriteString("MetaKeywords", category.MetaKeywords, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("MetaDescription", category.MetaDescription, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("MetaTitle", category.MetaTitle, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("SeName", category.GetSeName(0), IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("ParentCategoryId", category.ParentCategoryId);
                    xmlWriter.WriteString("PictureId", category.PictureId);
                    xmlWriter.WriteString("PageSize", category.PageSize, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("AllowCustomersToSelectPageSize", category.AllowCustomersToSelectPageSize, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("PageSizeOptions", category.PageSizeOptions, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("PriceRanges", category.PriceRanges, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("ShowOnHomePage", category.ShowOnHomePage, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("IncludeInTopMenu", category.IncludeInTopMenu, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("Published", category.Published, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("Deleted", category.Deleted, true);
                    xmlWriter.WriteString("DisplaySubscription", category.DisplaySubscription);
                    xmlWriter.WriteString("CreatedOnUtc", category.CreatedOnUtc, IgnoreExportCategoryProperty());
                    xmlWriter.WriteString("UpdatedOnUtc", category.UpdatedOnUtc, IgnoreExportCategoryProperty());

                    xmlWriter.WriteStartElement("Articles");
                    var articleCategories = _categoryService.GetArticleCategoriesByCategoryId(category.Id, showHidden: true);
                    foreach (var articleCategory in articleCategories)
                    {
                        var article = articleCategory.Article;
                        if (article != null && !article.Deleted)
                        {
                            xmlWriter.WriteStartElement("ArticleCategory");
                            xmlWriter.WriteString("ArticleCategoryId", articleCategory.Id);
                            xmlWriter.WriteString("ArticleId", articleCategory.ArticleId);
                            xmlWriter.WriteString("ArticleName", article.Name);
                            xmlWriter.WriteString("IsFeaturedArticle", articleCategory.IsFeaturedArticle);
                            xmlWriter.WriteString("DisplaySubscription", articleCategory.DisplaySubscription);
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("SubCategories");
                    WriteCategories(xmlWriter, category.Id);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
            }
        }

        protected virtual void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        protected virtual string GetPictures(int pictureId)
        {
            var picture = _pictureService.GetPictureById(pictureId);
            return _pictureService.GetThumbLocalPath(picture);
        }

        /// <summary>
        /// Returns the list of categories for a article separated by a ";"
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of categories</returns>
        protected virtual string GetCategories(Article article)
        {
            string categoryNames = null;
            foreach (var pc in _categoryService.GetArticleCategoriesByArticleId(article.Id, true))
            {
                categoryNames += pc.Category.Name;
                categoryNames += ";";
            }
            return categoryNames;
        }

        /// <summary>
        /// Returns the list of publisher for a article separated by a ";"
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of publisher</returns>
        protected virtual string GetPublishers(Article article)
        {
            string publisherNames = null;
            foreach (var pm in _publisherService.GetArticlePublishersByArticleId(article.Id, true))
            {
                publisherNames += pm.Publisher.Name;
                publisherNames += ";";
            }
            return publisherNames;
        }

        /// <summary>
        /// Returns the list of article tag for a article separated by a ";"
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of article tag</returns>
        protected virtual string GetArticleTags(Article article)
        {
            string articleTagNames = null;

            foreach (var articleTag in article.ArticleTags)
            {
                articleTagNames += articleTag.Name;
                articleTagNames += ";";
            }
            return articleTagNames;
        }

        /// <summary>
        /// Returns the three first image associated with the article
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>three first image</returns>
        protected virtual string[] GetPictures(Article article)
        {
            //pictures (up to 3 pictures)
            string picture1 = null;
            string picture2 = null;
            string picture3 = null;
            var pictures = _pictureService.GetPicturesByArticleId(article.Id, 3);
            for (var i = 0; i < pictures.Count; i++)
            {
                var pictureLocalPath = _pictureService.GetThumbLocalPath(pictures[i]);
                switch (i)
                {
                    case 0:
                        picture1 = pictureLocalPath;
                        break;
                    case 1:
                        picture2 = pictureLocalPath;
                        break;
                    case 2:
                        picture3 = pictureLocalPath;
                        break;
                }
            }
            return new[] { picture1, picture2, picture3 };
        }
       
        private bool IgnoreExportPoductProperty(Func<ArticleEditorSettings, bool> func)
        {
            var articleAdvancedMode = _workContext.CurrentCustomer.GetAttribute<bool>("article-advanced-mode");
            return !articleAdvancedMode && !func(_articleEditorSettings);
        }

        private bool IgnoreExportCategoryProperty()
        {
            return !_workContext.CurrentCustomer.GetAttribute<bool>("category-advanced-mode");
        }

        private bool IgnoreExportPublisherProperty()
        {
            return !_workContext.CurrentCustomer.GetAttribute<bool>("publisher-advanced-mode");
        }

        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="properties">Class access to the object through its properties</param>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        protected virtual byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                    fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                    
                    //create Headers and format them 
                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++, _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities, fWorksheet: fWorksheet);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private byte[] ExportArticlesToXlsxWithAttributes(PropertyByName<Article>[] properties, IEnumerable<Article> itemsToExport)
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportArticleAttribute>("AttributeId", p => p.AttributeId),
                new PropertyByName<ExportArticleAttribute>("AttributeName", p => p.AttributeName),
                new PropertyByName<ExportArticleAttribute>("AttributeTextPrompt", p => p.AttributeTextPrompt),
                new PropertyByName<ExportArticleAttribute>("AttributeIsRequired", p => p.AttributeIsRequired),
                new PropertyByName<ExportArticleAttribute>("AttributeControlType", p => p.AttributeControlTypeId)
                {
                    DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                },
                new PropertyByName<ExportArticleAttribute>("AttributeDisplaySubscription", p => p.AttributeDisplaySubscription),
                new PropertyByName<ExportArticleAttribute>("ArticleAttributeValueId", p => p.Id),
                new PropertyByName<ExportArticleAttribute>("ValueName", p => p.Name),
                new PropertyByName<ExportArticleAttribute>("AttributeValueType", p => p.AttributeValueTypeId)
                {
                    DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
                },
                new PropertyByName<ExportArticleAttribute>("AssociatedArticleId", p => p.AssociatedArticleId),
                new PropertyByName<ExportArticleAttribute>("ColorSquaresRgb", p => p.ColorSquaresRgb),
                new PropertyByName<ExportArticleAttribute>("ImageSquaresPictureId", p => p.ImageSquaresPictureId),
                new PropertyByName<ExportArticleAttribute>("PriceAdjustment", p => p.PriceAdjustment),
                new PropertyByName<ExportArticleAttribute>("WeightAdjustment", p => p.WeightAdjustment),
                new PropertyByName<ExportArticleAttribute>("Cost", p => p.Cost),
                new PropertyByName<ExportArticleAttribute>("CustomerEntersQty", p => p.CustomerEntersQty),
                new PropertyByName<ExportArticleAttribute>("Quantity", p => p.Quantity),
                new PropertyByName<ExportArticleAttribute>("IsPreSelected", p => p.IsPreSelected),
                new PropertyByName<ExportArticleAttribute>("DisplaySubscription", p => p.DisplaySubscription),
                new PropertyByName<ExportArticleAttribute>("PictureId", p => p.PictureId)
            };

            var attributeManager = new PropertyManager<ExportArticleAttribute>(attributeProperties);

            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(Article).Name);
                    var fpWorksheet = xlPackage.Workbook.Worksheets.Add("DataForArticlesFilters");
                    fpWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                    var faWorksheet = xlPackage.Workbook.Worksheets.Add("DataForArticleAttributesFilters");
                    faWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                    //create Headers and format them 
                    var manager = new PropertyManager<Article>(properties.Where(p => !p.Ignore));
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var item in itemsToExport)
                    {
                        manager.CurrentObject = item;
                        manager.WriteToXlsx(worksheet, row++, _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities, fWorksheet: fpWorksheet);

                        var attributes = item.ArticleAttributeMappings.SelectMany(pam => pam.ArticleAttributeValues.Select(pav => new ExportArticleAttribute
                        {
                            AttributeId = pam.ArticleAttribute.Id,
                            AttributeName = pam.ArticleAttribute.Name,
                            AttributeTextPrompt = pam.TextPrompt,
                            AttributeIsRequired = pam.IsRequired,
                            AttributeControlTypeId = pam.AttributeControlTypeId,
                            AssociatedArticleId = pav.AssociatedArticleId,
                            AttributeDisplaySubscription = pam.DisplaySubscription,
                            Id = pav.Id,
                            Name = pav.Name,
                            AttributeValueTypeId = pav.AttributeValueTypeId,
                            ColorSquaresRgb = pav.ColorSquaresRgb,
                            ImageSquaresPictureId = pav.ImageSquaresPictureId,
                            PriceAdjustment = pav.PriceAdjustment,
                            WeightAdjustment = pav.WeightAdjustment,
                            Cost = pav.Cost,
                            CustomerEntersQty = pav.CustomerEntersQty,
                            Quantity = pav.Quantity,
                            IsPreSelected = pav.IsPreSelected,
                            DisplaySubscription = pav.DisplaySubscription,
                            PictureId = pav.PictureId
                        })).ToList();

                        attributes.AddRange(item.ArticleAttributeMappings.Where(pam => !pam.ArticleAttributeValues.Any()).Select(pam => new ExportArticleAttribute
                        {
                            AttributeId = pam.ArticleAttribute.Id,
                            AttributeName = pam.ArticleAttribute.Name,
                            AttributeTextPrompt = pam.TextPrompt,
                            AttributeIsRequired = pam.IsRequired,
                            AttributeControlTypeId = pam.AttributeControlTypeId
                        }));

                        if (!attributes.Any())
                            continue;

                        attributeManager.WriteCaption(worksheet, SetCaptionStyle, row, ExportArticleAttribute.ProducAttributeCellOffset);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapsed = true;

                        foreach (var exportProducAttribute in attributes)
                        {
                            row++;
                            attributeManager.CurrentObject = exportProducAttribute;
                            attributeManager.WriteToXlsx(worksheet, row, _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities, ExportArticleAttribute.ProducAttributeCellOffset, faWorksheet);
                            worksheet.Row(row).OutlineLevel = 1;
                            worksheet.Row(row).Collapsed = true;
                        }

                        row++;
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private byte[] ExportSubscriptionToXlsxWithArticles(PropertyByName<Subscription>[] properties, IEnumerable<Subscription> itemsToExport)
        {
            var subscriptionItemProperties = new[]
            {
                new PropertyByName<SubscriptionItem>("Name", oi => oi.Article.Name),
                new PropertyByName<SubscriptionItem>("Sku", oi => oi.Article.Sku),
                new PropertyByName<SubscriptionItem>("PriceExclTax", oi => oi.UnitPriceExclTax),
                new PropertyByName<SubscriptionItem>("PriceInclTax", oi => oi.UnitPriceInclTax),
                new PropertyByName<SubscriptionItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<SubscriptionItem>("DiscountExclTax", oi => oi.DiscountAmountExclTax),
                new PropertyByName<SubscriptionItem>("DiscountInclTax", oi => oi.DiscountAmountInclTax),
                new PropertyByName<SubscriptionItem>("TotalExclTax", oi => oi.PriceExclTax),
                new PropertyByName<SubscriptionItem>("TotalInclTax", oi => oi.PriceInclTax)
            };

            var subscriptionItemsManager = new PropertyManager<SubscriptionItem>(subscriptionItemProperties);

            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(Subscription).Name);
                    var fpWorksheet = xlPackage.Workbook.Worksheets.Add("DataForArticlesFilters");
                    fpWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                    //create Headers and format them 
                    var manager = new PropertyManager<Subscription>(properties.Where(p => !p.Ignore));
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var subscription in itemsToExport)
                    {
                        manager.CurrentObject = subscription;
                        manager.WriteToXlsx(worksheet, row++, _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities);
                        
                        //articles
                        var orederItems = subscription.SubscriptionItems.ToList();

                        //a contributor should have access only to his articles
                        if (_workContext.CurrentContributor != null)
                            orederItems = orederItems.Where(p => p.Article.ContributorId == _workContext.CurrentContributor.Id).ToList();

                        if (!orederItems.Any())
                            continue;

                        subscriptionItemsManager.WriteCaption(worksheet, SetCaptionStyle, row, 2);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapsed = true;

                        foreach (var orederItem in orederItems)
                        {
                            row++;
                            subscriptionItemsManager.CurrentObject = orederItem;
                            subscriptionItemsManager.WriteToXlsx(worksheet, row, _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities, 2, fpWorksheet);
                            worksheet.Row(row).OutlineLevel = 1;
                            worksheet.Row(row).Collapsed = true;
                        }

                        row++;
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private string GetCustomCustomerAttributes(Customer customer)
        {
            var selectedCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
            return _customerAttributeFormatter.FormatAttributes(selectedCustomerAttributes, ";");
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Export publisher list to xml
        /// </summary>
        /// <param name="publishers">Publishers</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportPublishersToXml(IList<Publisher> publishers)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Publishers");
            xmlWriter.WriteAttributeString("Version", YStoryVersion.CurrentVersion);
            
            foreach (var publisher in publishers)
            {
                xmlWriter.WriteStartElement("Publisher");

                xmlWriter.WriteString("PublisherId", publisher.Id);
                xmlWriter.WriteString("Name", publisher.Name);
                xmlWriter.WriteString("Description", publisher.Description);
                xmlWriter.WriteString("PublisherTemplateId", publisher.PublisherTemplateId);
                xmlWriter.WriteString("MetaKeywords", publisher.MetaKeywords, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("MetaDescription", publisher.MetaDescription, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("MetaTitle", publisher.MetaTitle, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("SEName", publisher.GetSeName(0), IgnoreExportPublisherProperty());
                xmlWriter.WriteString("PictureId", publisher.PictureId);
                xmlWriter.WriteString("PageSize", publisher.PageSize, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("AllowCustomersToSelectPageSize", publisher.AllowCustomersToSelectPageSize, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("PageSizeOptions", publisher.PageSizeOptions, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("PriceRanges", publisher.PriceRanges, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("Published", publisher.Published, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("Deleted", publisher.Deleted, true);
                xmlWriter.WriteString("DisplaySubscription", publisher.DisplaySubscription);
                xmlWriter.WriteString("CreatedOnUtc", publisher.CreatedOnUtc, IgnoreExportPublisherProperty());
                xmlWriter.WriteString("UpdatedOnUtc", publisher.UpdatedOnUtc, IgnoreExportPublisherProperty());

                xmlWriter.WriteStartElement("Articles");
                var articlePublishers = _publisherService.GetArticlePublishersByPublisherId(publisher.Id, showHidden: true);
                if (articlePublishers != null)
                {
                    foreach (var articlePublisher in articlePublishers)
                    {
                        var article = articlePublisher.Article;
                        if (article != null && !article.Deleted)
                        {
                            xmlWriter.WriteStartElement("ArticlePublisher");
                            xmlWriter.WriteString("ArticlePublisherId", articlePublisher.Id);
                            xmlWriter.WriteString("ArticleId", articlePublisher.ArticleId);
                            xmlWriter.WriteString("ArticleName", article.Name);
                            xmlWriter.WriteString("IsFeaturedArticle", articlePublisher.IsFeaturedArticle);
                            xmlWriter.WriteString("DisplaySubscription", articlePublisher.DisplaySubscription);
                            xmlWriter.WriteEndElement();
                        }
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export publishers to XLSX
        /// </summary>
        /// <param name="publishers">Manufactures</param>
        public virtual byte[] ExportPublishersToXlsx(IEnumerable<Publisher> publishers)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Publisher>("Id", p => p.Id),
                new PropertyByName<Publisher>("Name", p => p.Name),
                new PropertyByName<Publisher>("Description", p => p.Description),
                new PropertyByName<Publisher>("PublisherTemplateId", p => p.PublisherTemplateId),
                new PropertyByName<Publisher>("MetaKeywords", p => p.MetaKeywords, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("MetaDescription", p => p.MetaDescription, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("MetaTitle", p => p.MetaTitle, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("SeName", p => p.GetSeName(0), IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("Picture", p => GetPictures(p.PictureId)),
                new PropertyByName<Publisher>("PageSize", p => p.PageSize, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("PageSizeOptions", p => p.PageSizeOptions, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("PriceRanges", p => p.PriceRanges, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("Published", p => p.Published, IgnoreExportPublisherProperty()),
                new PropertyByName<Publisher>("DisplaySubscription", p => p.DisplaySubscription)
            };

            return ExportToXlsx(properties, publishers);
        }

        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        public virtual string ExportCategoriesToXml()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Categories");
            xmlWriter.WriteAttributeString("Version", YStoryVersion.CurrentVersion);
            WriteCategories(xmlWriter, 0);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        public virtual byte[] ExportCategoriesToXlsx(IEnumerable<Category> categories)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Category>("Id", p => p.Id),
                new PropertyByName<Category>("Name", p => p.Name),
                new PropertyByName<Category>("Description", p => p.Description),
                new PropertyByName<Category>("CategoryTemplateId", p => p.CategoryTemplateId),
                new PropertyByName<Category>("MetaKeywords", p => p.MetaKeywords, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaDescription", p => p.MetaDescription, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaTitle", p => p.MetaTitle, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("SeName", p => p.GetSeName(0), IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                new PropertyByName<Category>("Picture", p => GetPictures(p.PictureId)),
                new PropertyByName<Category>("PageSize", p => p.PageSize, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PriceRanges", p => p.PriceRanges, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ShowOnHomePage", p => p.ShowOnHomePage, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("Published", p => p.Published, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("DisplaySubscription", p => p.DisplaySubscription)
            };
            return ExportToXlsx(properties, categories);
        }

        /// <summary>
        /// Export article list to xml
        /// </summary>
        /// <param name="articles">Articles</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportArticlesToXml(IList<Article> articles)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Articles");
            xmlWriter.WriteAttributeString("Version", YStoryVersion.CurrentVersion);

            foreach (var article in articles)
            {
                xmlWriter.WriteStartElement("Article");

                xmlWriter.WriteString("ArticleId", article.Id, IgnoreExportPoductProperty(p => p.Id));
                xmlWriter.WriteString("ArticleTypeId", article.ArticleTypeId, IgnoreExportPoductProperty(p => p.ArticleType));
                xmlWriter.WriteString("ParentGroupedArticleId", article.ParentGroupedArticleId, IgnoreExportPoductProperty(p => p.ArticleType));
                xmlWriter.WriteString("VisibleIndividually", article.VisibleIndividually, IgnoreExportPoductProperty(p => p.VisibleIndividually));
                xmlWriter.WriteString("Name", article.Name);
                xmlWriter.WriteString("ShortDescription", article.ShortDescription);
                xmlWriter.WriteString("FullDescription", article.FullDescription);
                xmlWriter.WriteString("AdminComment", article.AdminComment, IgnoreExportPoductProperty(p => p.AdminComment));
                //contributor can't change this field
                xmlWriter.WriteString("ContributorId", article.ContributorId, IgnoreExportPoductProperty(p => p.Contributor) || _workContext.CurrentContributor != null);
                xmlWriter.WriteString("ArticleTemplateId", article.ArticleTemplateId, IgnoreExportPoductProperty(p => p.ArticleTemplate));
                xmlWriter.WriteString("ShowOnHomePage", article.ShowOnHomePage, IgnoreExportPoductProperty(p => p.ShowOnHomePage));
                xmlWriter.WriteString("MetaKeywords", article.MetaKeywords, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("MetaDescription", article.MetaDescription, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("MetaTitle", article.MetaTitle, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("SEName", article.GetSeName(0), IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("AllowCustomerReviews", article.AllowCustomerReviews, IgnoreExportPoductProperty(p => p.AllowCustomerReviews));
                xmlWriter.WriteString("SKU", article.Sku);
                xmlWriter.WriteString("PublisherPartNumber", article.PublisherPartNumber, IgnoreExportPoductProperty(p => p.PublisherPartNumber));
                xmlWriter.WriteString("Gtin", article.Gtin, IgnoreExportPoductProperty(p => p.GTIN));
                xmlWriter.WriteString("IsGiftCard", article.IsGiftCard, IgnoreExportPoductProperty(p => p.IsGiftCard));
                xmlWriter.WriteString("OverriddenGiftCardAmount", article.OverriddenGiftCardAmount, IgnoreExportPoductProperty(p => p.IsGiftCard));
                xmlWriter.WriteString("RequireOtherArticles", article.RequireOtherArticles, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart));
                xmlWriter.WriteString("RequiredArticleIds", article.RequiredArticleIds, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart));
                xmlWriter.WriteString("AutomaticallyAddRequiredArticles", article.AutomaticallyAddRequiredArticles, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart));
                xmlWriter.WriteString("HasUserAgreement", article.HasUserAgreement, IgnoreExportPoductProperty(p => p.DownloadableArticle));
                xmlWriter.WriteString("UserAgreementText", article.UserAgreementText, IgnoreExportPoductProperty(p => p.DownloadableArticle));
                xmlWriter.WriteString("IsRecurring", article.IsRecurring, IgnoreExportPoductProperty(p => p.RecurringArticle));
                xmlWriter.WriteString("RecurringCycleLength", article.RecurringCycleLength, IgnoreExportPoductProperty(p => p.RecurringArticle));
                xmlWriter.WriteString("RecurringCyclePeriodId", article.RecurringCyclePeriodId, IgnoreExportPoductProperty(p => p.RecurringArticle));
                xmlWriter.WriteString("RecurringTotalCycles", article.RecurringTotalCycles, IgnoreExportPoductProperty(p => p.RecurringArticle));
                xmlWriter.WriteString("IsRental", article.IsRental, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("RentalPriceLength", article.RentalPriceLength, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("RentalPricePeriodId", article.RentalPricePeriodId, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("IsTaxExempt", article.IsTaxExempt);
                xmlWriter.WriteString("TaxCategoryId", article.TaxCategoryId);
                xmlWriter.WriteString("AllowAddingOnlyExistingAttributeCombinations", article.AllowAddingOnlyExistingAttributeCombinations, IgnoreExportPoductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations));
                xmlWriter.WriteString("NotReturnable", article.NotReturnable, IgnoreExportPoductProperty(p => p.NotReturnable));
                xmlWriter.WriteString("DisableBuyButton", article.DisableBuyButton, IgnoreExportPoductProperty(p => p.DisableBuyButton));
                xmlWriter.WriteString("DisableWishlistButton", article.DisableWishlistButton, IgnoreExportPoductProperty(p => p.DisableWishlistButton));
                xmlWriter.WriteString("AvailableForPreSubscription", article.AvailableForPreSubscription, IgnoreExportPoductProperty(p => p.AvailableForPreSubscription));
                xmlWriter.WriteString("PreSubscriptionAvailabilityStartDateTimeUtc", article.PreSubscriptionAvailabilityStartDateTimeUtc, IgnoreExportPoductProperty(p => p.AvailableForPreSubscription));
                xmlWriter.WriteString("CallForPrice", article.CallForPrice, IgnoreExportPoductProperty(p => p.CallForPrice));
                xmlWriter.WriteString("Price", article.Price);
                xmlWriter.WriteString("OldPrice", article.OldPrice, IgnoreExportPoductProperty(p => p.OldPrice));
                xmlWriter.WriteString("ArticleCost", article.ArticleCost, IgnoreExportPoductProperty(p => p.ArticleCost));
                xmlWriter.WriteString("MarkAsNew", article.MarkAsNew, IgnoreExportPoductProperty(p => p.MarkAsNew));
                xmlWriter.WriteString("MarkAsNewStartDateTimeUtc", article.MarkAsNewStartDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewStartDate));
                xmlWriter.WriteString("MarkAsNewEndDateTimeUtc", article.MarkAsNewEndDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewEndDate));
                xmlWriter.WriteString("Published", article.Published, IgnoreExportPoductProperty(p => p.Published));
                xmlWriter.WriteString("CreatedOnUtc", article.CreatedOnUtc, IgnoreExportPoductProperty(p => p.CreatedOn));
                xmlWriter.WriteString("UpdatedOnUtc", article.UpdatedOnUtc, IgnoreExportPoductProperty(p => p.UpdatedOn));

                 

                if (!IgnoreExportPoductProperty(p => p.ArticleAttributes))
                {
                    xmlWriter.WriteStartElement("ArticleAttributes");
                    var articleAttributMappings =
                        _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
                    foreach (var articleAttributeMapping in articleAttributMappings)
                    {
                        xmlWriter.WriteStartElement("ArticleAttributeMapping");
                        xmlWriter.WriteString("ArticleAttributeMappingId", articleAttributeMapping.Id);
                        xmlWriter.WriteString("ArticleAttributeId", articleAttributeMapping.ArticleAttributeId);
                        xmlWriter.WriteString("ArticleAttributeName", articleAttributeMapping.ArticleAttribute.Name);
                        xmlWriter.WriteString("TextPrompt", articleAttributeMapping.TextPrompt);
                        xmlWriter.WriteString("IsRequired", articleAttributeMapping.IsRequired);
                        xmlWriter.WriteString("AttributeControlTypeId", articleAttributeMapping.AttributeControlTypeId);
                        xmlWriter.WriteString("DisplaySubscription", articleAttributeMapping.DisplaySubscription);
                        //validation rules
                        if (articleAttributeMapping.ValidationRulesAllowed())
                        {
                            if (articleAttributeMapping.ValidationMinLength.HasValue)
                            {
                                xmlWriter.WriteString("ValidationMinLength",
                                    articleAttributeMapping.ValidationMinLength.Value);
                            }
                            if (articleAttributeMapping.ValidationMaxLength.HasValue)
                            {
                                xmlWriter.WriteString("ValidationMaxLength",
                                    articleAttributeMapping.ValidationMaxLength.Value);
                            }
                            if (String.IsNullOrEmpty(articleAttributeMapping.ValidationFileAllowedExtensions))
                            {
                                xmlWriter.WriteString("ValidationFileAllowedExtensions",
                                    articleAttributeMapping.ValidationFileAllowedExtensions);
                            }
                            if (articleAttributeMapping.ValidationFileMaximumSize.HasValue)
                            {
                                xmlWriter.WriteString("ValidationFileMaximumSize",
                                    articleAttributeMapping.ValidationFileMaximumSize.Value);
                            }
                            xmlWriter.WriteString("DefaultValue", articleAttributeMapping.DefaultValue);
                        }
                        //conditions
                        xmlWriter.WriteElementString("ConditionAttributeXml",
                            articleAttributeMapping.ConditionAttributeXml);

                        xmlWriter.WriteStartElement("ArticleAttributeValues");
                        var articleAttributeValues = articleAttributeMapping.ArticleAttributeValues;
                        foreach (var articleAttributeValue in articleAttributeValues)
                        {
                            xmlWriter.WriteStartElement("ArticleAttributeValue");
                            xmlWriter.WriteString("ArticleAttributeValueId", articleAttributeValue.Id);
                            xmlWriter.WriteString("Name", articleAttributeValue.Name);
                            xmlWriter.WriteString("AttributeValueTypeId", articleAttributeValue.AttributeValueTypeId);
                            xmlWriter.WriteString("AssociatedArticleId", articleAttributeValue.AssociatedArticleId);
                            xmlWriter.WriteString("ColorSquaresRgb", articleAttributeValue.ColorSquaresRgb);
                            xmlWriter.WriteString("ImageSquaresPictureId", articleAttributeValue.ImageSquaresPictureId);
                            xmlWriter.WriteString("PriceAdjustment", articleAttributeValue.PriceAdjustment);
                            xmlWriter.WriteString("WeightAdjustment", articleAttributeValue.WeightAdjustment);
                            xmlWriter.WriteString("Cost", articleAttributeValue.Cost);
                            xmlWriter.WriteString("CustomerEntersQty", articleAttributeValue.CustomerEntersQty);
                            xmlWriter.WriteString("Quantity", articleAttributeValue.Quantity);
                            xmlWriter.WriteString("IsPreSelected", articleAttributeValue.IsPreSelected);
                            xmlWriter.WriteString("DisplaySubscription", articleAttributeValue.DisplaySubscription);
                            xmlWriter.WriteString("PictureId", articleAttributeValue.PictureId);
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteStartElement("ArticlePictures");
                var articlePictures = article.ArticlePictures;
                foreach (var articlePicture in articlePictures)
                {
                    xmlWriter.WriteStartElement("ArticlePicture");
                    xmlWriter.WriteString("ArticlePictureId", articlePicture.Id);
                    xmlWriter.WriteString("PictureId", articlePicture.PictureId);
                    xmlWriter.WriteString("DisplaySubscription", articlePicture.DisplaySubscription);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ArticleCategories");
                var articleCategories = _categoryService.GetArticleCategoriesByArticleId(article.Id);
                if (articleCategories != null)
                {
                    foreach (var articleCategory in articleCategories)
                    {
                        xmlWriter.WriteStartElement("ArticleCategory");
                        xmlWriter.WriteString("ArticleCategoryId", articleCategory.Id);
                        xmlWriter.WriteString("CategoryId", articleCategory.CategoryId);
                        xmlWriter.WriteString("IsFeaturedArticle", articleCategory.IsFeaturedArticle);
                        xmlWriter.WriteString("DisplaySubscription", articleCategory.DisplaySubscription);
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                if (!IgnoreExportPoductProperty(p => p.Publishers))
                {
                    xmlWriter.WriteStartElement("ArticlePublishers");
                    var articlePublishers = _publisherService.GetArticlePublishersByArticleId(article.Id);
                    if (articlePublishers != null)
                    {
                        foreach (var articlePublisher in articlePublishers)
                        {
                            xmlWriter.WriteStartElement("ArticlePublisher");
                            xmlWriter.WriteString("ArticlePublisherId", articlePublisher.Id);
                            xmlWriter.WriteString("PublisherId", articlePublisher.PublisherId);
                            xmlWriter.WriteString("IsFeaturedArticle", articlePublisher.IsFeaturedArticle);
                            xmlWriter.WriteString("DisplaySubscription", articlePublisher.DisplaySubscription);
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.SpecificationAttributes))
                {
                    xmlWriter.WriteStartElement("ArticleSpecificationAttributes");
                    var articleSpecificationAttributes = article.ArticleSpecificationAttributes;
                    foreach (var articleSpecificationAttribute in articleSpecificationAttributes)
                    {
                        xmlWriter.WriteStartElement("ArticleSpecificationAttribute");
                        xmlWriter.WriteString("ArticleSpecificationAttributeId", articleSpecificationAttribute.Id);
                        xmlWriter.WriteString("SpecificationAttributeOptionId", articleSpecificationAttribute.SpecificationAttributeOptionId);
                        xmlWriter.WriteString("CustomValue", articleSpecificationAttribute.CustomValue);
                        xmlWriter.WriteString("AllowFiltering", articleSpecificationAttribute.AllowFiltering);
                        xmlWriter.WriteString("ShowOnArticlePage", articleSpecificationAttribute.ShowOnArticlePage);
                        xmlWriter.WriteString("DisplaySubscription", articleSpecificationAttribute.DisplaySubscription);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.ArticleTags))
                {
                    xmlWriter.WriteStartElement("ArticleTags");
                    var articleTags = article.ArticleTags;
                    foreach (var articleTag in articleTags)
                    {
                        xmlWriter.WriteStartElement("ArticleTag");
                        xmlWriter.WriteString("Id", articleTag.Id);
                        xmlWriter.WriteString("Name", articleTag.Name);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export articles to XLSX
        /// </summary>
        /// <param name="articles">Articles</param>
        public virtual byte[] ExportArticlesToXlsx(IEnumerable<Article> articles)
        {
            var properties = new[]
            {
                new PropertyByName<Article>("ArticleType", p => p.ArticleTypeId, IgnoreExportPoductProperty(p => p.ArticleType))
                {
                    DropDownElements = ArticleType.SimpleArticle.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Article>("ParentGroupedArticleId", p => p.ParentGroupedArticleId, IgnoreExportPoductProperty(p => p.ArticleType)),
                new PropertyByName<Article>("VisibleIndividually", p => p.VisibleIndividually, IgnoreExportPoductProperty(p => p.VisibleIndividually)),
                new PropertyByName<Article>("Name", p => p.Name),
                new PropertyByName<Article>("ShortDescription", p => p.ShortDescription),
                new PropertyByName<Article>("FullDescription", p => p.FullDescription),
                //contributor can't change this field
                new PropertyByName<Article>("Contributor", p => p.ContributorId, IgnoreExportPoductProperty(p => p.Contributor) || _workContext.CurrentContributor != null)
                {
                    DropDownElements = _contributorService.GetAllContributors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Contributor).Return(v => v.Name, String.Empty)),
                    AllowBlank = true
                },
                new PropertyByName<Article>("ArticleTemplate", p => p.ArticleTemplateId, IgnoreExportPoductProperty(p => p.ArticleTemplate))
                {
                    DropDownElements = _articleTemplateService.GetAllArticleTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ArticleTemplate).Return(pt => pt.Name, String.Empty)),
                },
                //contributor can't change this field
                new PropertyByName<Article>("ShowOnHomePage", p => p.ShowOnHomePage, IgnoreExportPoductProperty(p => p.ShowOnHomePage) || _workContext.CurrentContributor != null),
                new PropertyByName<Article>("MetaKeywords", p => p.MetaKeywords, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Article>("MetaDescription", p => p.MetaDescription, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Article>("MetaTitle", p => p.MetaTitle, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Article>("SeName", p => p.GetSeName(0), IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Article>("AllowCustomerReviews", p => p.AllowCustomerReviews, IgnoreExportPoductProperty(p => p.AllowCustomerReviews)),
                new PropertyByName<Article>("Published", p => p.Published, IgnoreExportPoductProperty(p => p.Published)),
                new PropertyByName<Article>("SKU", p => p.Sku),
                new PropertyByName<Article>("PublisherPartNumber", p => p.PublisherPartNumber, IgnoreExportPoductProperty(p => p.PublisherPartNumber)),
                new PropertyByName<Article>("Gtin", p => p.Gtin, IgnoreExportPoductProperty(p => p.GTIN)),
                new PropertyByName<Article>("IsGiftCard", p => p.IsGiftCard, IgnoreExportPoductProperty(p => p.IsGiftCard)),
                new PropertyByName<Article>("GiftCardType", p => p.GiftCardTypeId, IgnoreExportPoductProperty(p => p.IsGiftCard))
                {
                    DropDownElements = GiftCardType.Virtual.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Article>("OverriddenGiftCardAmount", p => p.OverriddenGiftCardAmount, IgnoreExportPoductProperty(p => p.IsGiftCard)),
                new PropertyByName<Article>("RequireOtherArticles", p => p.RequireOtherArticles, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart)),
                new PropertyByName<Article>("RequiredArticleIds", p => p.RequiredArticleIds, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart)),
                new PropertyByName<Article>("AutomaticallyAddRequiredArticles", p => p.AutomaticallyAddRequiredArticles, IgnoreExportPoductProperty(p => p.RequireOtherArticlesAddedToTheCart)),
                 
                new PropertyByName<Article>("HasUserAgreement", p => p.HasUserAgreement, IgnoreExportPoductProperty(p => p.DownloadableArticle)),
                new PropertyByName<Article>("UserAgreementText", p => p.UserAgreementText, IgnoreExportPoductProperty(p => p.DownloadableArticle)),
                new PropertyByName<Article>("IsRecurring", p => p.IsRecurring, IgnoreExportPoductProperty(p => p.RecurringArticle)),
                new PropertyByName<Article>("RecurringCycleLength", p => p.RecurringCycleLength, IgnoreExportPoductProperty(p => p.RecurringArticle)),
                new PropertyByName<Article>("RecurringCyclePeriod", p => p.RecurringCyclePeriodId, IgnoreExportPoductProperty(p => p.RecurringArticle))
                {
                    DropDownElements = RecurringArticleCyclePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Article>("RecurringTotalCycles", p => p.RecurringTotalCycles, IgnoreExportPoductProperty(p => p.RecurringArticle)),
                new PropertyByName<Article>("IsRental", p => p.IsRental, IgnoreExportPoductProperty(p => p.IsRental)),
                new PropertyByName<Article>("RentalPriceLength", p => p.RentalPriceLength, IgnoreExportPoductProperty(p => p.IsRental)),
                new PropertyByName<Article>("RentalPricePeriod", p => p.RentalPricePeriodId, IgnoreExportPoductProperty(p => p.IsRental))
                {
                    DropDownElements = RentalPricePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
               
                new PropertyByName<Article>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Article>("TaxCategory", p => p.TaxCategoryId)
                {
                    DropDownElements = _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory).Return(tc => tc.Name, String.Empty)),
                    AllowBlank = true
                },
                  
                new PropertyByName<Article>("AllowAddingOnlyExistingAttributeCombinations", p => p.AllowAddingOnlyExistingAttributeCombinations, IgnoreExportPoductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations)),
                new PropertyByName<Article>("NotReturnable", p => p.NotReturnable, IgnoreExportPoductProperty(p => p.NotReturnable)),
                new PropertyByName<Article>("DisableBuyButton", p => p.DisableBuyButton, IgnoreExportPoductProperty(p => p.DisableBuyButton)),
                new PropertyByName<Article>("DisableWishlistButton", p => p.DisableWishlistButton, IgnoreExportPoductProperty(p => p.DisableWishlistButton)),
                new PropertyByName<Article>("AvailableForPreSubscription", p => p.AvailableForPreSubscription, IgnoreExportPoductProperty(p => p.AvailableForPreSubscription)),
                new PropertyByName<Article>("PreSubscriptionAvailabilityStartDateTimeUtc", p => p.PreSubscriptionAvailabilityStartDateTimeUtc, IgnoreExportPoductProperty(p => p.AvailableForPreSubscription)),
                new PropertyByName<Article>("CallForPrice", p => p.CallForPrice, IgnoreExportPoductProperty(p => p.CallForPrice)),
                new PropertyByName<Article>("Price", p => p.Price),
                new PropertyByName<Article>("OldPrice", p => p.OldPrice, IgnoreExportPoductProperty(p => p.OldPrice)),
                new PropertyByName<Article>("ArticleCost", p => p.ArticleCost, IgnoreExportPoductProperty(p => p.ArticleCost)),
                
                new PropertyByName<Article>("MarkAsNew", p => p.MarkAsNew, IgnoreExportPoductProperty(p => p.MarkAsNew)),
                new PropertyByName<Article>("MarkAsNewStartDateTimeUtc", p => p.MarkAsNewStartDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewStartDate)),
                new PropertyByName<Article>("MarkAsNewEndDateTimeUtc", p => p.MarkAsNewEndDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewEndDate)),
                new PropertyByName<Article>("Categories", GetCategories),
                new PropertyByName<Article>("Publishers", GetPublishers, IgnoreExportPoductProperty(p => p.Publishers)),
                new PropertyByName<Article>("ArticleTags", GetArticleTags, IgnoreExportPoductProperty(p => p.ArticleTags)),
                new PropertyByName<Article>("Picture1", p => GetPictures(p)[0]),
                new PropertyByName<Article>("Picture2", p => GetPictures(p)[1]),
                new PropertyByName<Article>("Picture3", p => GetPictures(p)[2])
            };

            var articleList = articles.ToList();
            var articleAdvancedMode = _workContext.CurrentCustomer.GetAttribute<bool>("article-advanced-mode");

            if (_catalogSettings.ExportImportArticleAttributes)
            {
                if (articleAdvancedMode || _articleEditorSettings.ArticleAttributes)
                    return ExportArticlesToXlsxWithAttributes(properties, articleList);
            }

            return ExportToXlsx(properties, articleList);
        }

        /// <summary>
        /// Export subscription list to xml
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportSubscriptionsToXml(IList<Subscription> subscriptions)
        {
            //a contributor should have access only to part of subscription information
            var ignore = _workContext.CurrentContributor != null;

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Subscriptions");
            xmlWriter.WriteAttributeString("Version", YStoryVersion.CurrentVersion);

            foreach (var subscription in subscriptions)
            {
                xmlWriter.WriteStartElement("Subscription");

                xmlWriter.WriteString("SubscriptionId", subscription.Id);
                xmlWriter.WriteString("SubscriptionGuid", subscription.SubscriptionGuid, ignore);
                xmlWriter.WriteString("StoreId", subscription.StoreId);
                xmlWriter.WriteString("CustomerId", subscription.CustomerId, ignore);
                xmlWriter.WriteString("SubscriptionStatusId", subscription.SubscriptionStatusId, ignore);
                xmlWriter.WriteString("PaymentStatusId", subscription.PaymentStatusId, ignore);
                xmlWriter.WriteString("ShippingStatusId", subscription.ShippingStatusId, ignore);
                xmlWriter.WriteString("CustomerLanguageId", subscription.CustomerLanguageId, ignore);
                xmlWriter.WriteString("CustomerTaxDisplayTypeId", subscription.CustomerTaxDisplayTypeId, ignore);
                xmlWriter.WriteString("CustomerIp", subscription.CustomerIp, ignore);
                xmlWriter.WriteString("SubscriptionSubtotalInclTax", subscription.SubscriptionSubtotalInclTax, ignore);
                xmlWriter.WriteString("SubscriptionSubtotalExclTax", subscription.SubscriptionSubtotalExclTax, ignore);
                xmlWriter.WriteString("SubscriptionSubTotalDiscountInclTax", subscription.SubscriptionSubTotalDiscountInclTax, ignore);
                xmlWriter.WriteString("SubscriptionSubTotalDiscountExclTax", subscription.SubscriptionSubTotalDiscountExclTax, ignore);
                xmlWriter.WriteString("SubscriptionShippingInclTax", subscription.SubscriptionShippingInclTax, ignore);
                xmlWriter.WriteString("SubscriptionShippingExclTax", subscription.SubscriptionShippingExclTax, ignore);
                xmlWriter.WriteString("PaymentMethodAdditionalFeeInclTax", subscription.PaymentMethodAdditionalFeeInclTax, ignore);
                xmlWriter.WriteString("PaymentMethodAdditionalFeeExclTax", subscription.PaymentMethodAdditionalFeeExclTax, ignore);
                xmlWriter.WriteString("TaxRates", subscription.TaxRates, ignore);
                xmlWriter.WriteString("SubscriptionTax", subscription.SubscriptionTax, ignore);
                xmlWriter.WriteString("SubscriptionTotal", subscription.SubscriptionTotal, ignore);
                xmlWriter.WriteString("RefundedAmount", subscription.RefundedAmount, ignore);
                xmlWriter.WriteString("SubscriptionDiscount", subscription.SubscriptionDiscount, ignore);
                xmlWriter.WriteString("CurrencyRate", subscription.CurrencyRate);
                xmlWriter.WriteString("CustomerCurrencyCode", subscription.CustomerCurrencyCode);
                xmlWriter.WriteString("AffiliateId", subscription.AffiliateId, ignore);
                xmlWriter.WriteString("AllowStoringCreditCardNumber", subscription.AllowStoringCreditCardNumber, ignore);
                xmlWriter.WriteString("CardType", subscription.CardType, ignore);
                xmlWriter.WriteString("CardName", subscription.CardName, ignore);
                xmlWriter.WriteString("CardNumber", subscription.CardNumber, ignore);
                xmlWriter.WriteString("MaskedCreditCardNumber", subscription.MaskedCreditCardNumber, ignore);
                xmlWriter.WriteString("CardCvv2", subscription.CardCvv2, ignore);
                xmlWriter.WriteString("CardExpirationMonth", subscription.CardExpirationMonth, ignore);
                xmlWriter.WriteString("CardExpirationYear", subscription.CardExpirationYear, ignore);
                xmlWriter.WriteString("PaymentMethodSystemName", subscription.PaymentMethodSystemName, ignore);
                xmlWriter.WriteString("AuthorizationTransactionId", subscription.AuthorizationTransactionId, ignore);
                xmlWriter.WriteString("AuthorizationTransactionCode", subscription.AuthorizationTransactionCode, ignore);
                xmlWriter.WriteString("AuthorizationTransactionResult", subscription.AuthorizationTransactionResult, ignore);
                xmlWriter.WriteString("CaptureTransactionId", subscription.CaptureTransactionId, ignore);
                xmlWriter.WriteString("CaptureTransactionResult", subscription.CaptureTransactionResult, ignore);
                xmlWriter.WriteString("SubscriptionTransactionId", subscription.SubscriptionTransactionId, ignore);
                xmlWriter.WriteString("PaidDateUtc", subscription.PaidDateUtc == null ? string.Empty : subscription.PaidDateUtc.Value.ToString(), ignore);
                xmlWriter.WriteString("ShippingMethod", subscription.ShippingMethod);
                xmlWriter.WriteString("ShippingRateComputationMethodSystemName", subscription.ShippingRateComputationMethodSystemName, ignore);
                xmlWriter.WriteString("CustomValuesXml", subscription.CustomValuesXml, ignore);
                xmlWriter.WriteString("VatNumber", subscription.VatNumber, ignore);
                xmlWriter.WriteString("Deleted", subscription.Deleted, ignore);
                xmlWriter.WriteString("CreatedOnUtc", subscription.CreatedOnUtc);

                if (_subscriptionSettings.ExportWithArticles)
                {
                    //articles
                    var subscriptionItems = subscription.SubscriptionItems;

                    //a contributor should have access only to his articles
                    if (_workContext.CurrentContributor != null)
                        subscriptionItems = subscriptionItems.Where(oi => oi.Article.ContributorId == _workContext.CurrentContributor.Id).ToList();

                    if (subscriptionItems.Any())
                    {
                        xmlWriter.WriteStartElement("SubscriptionItems");
                        foreach (var subscriptionItem in subscriptionItems)
                        {
                            xmlWriter.WriteStartElement("SubscriptionItem");
                            xmlWriter.WriteString("Id", subscriptionItem.Id);
                            xmlWriter.WriteString("SubscriptionItemGuid", subscriptionItem.SubscriptionItemGuid);
                            xmlWriter.WriteString("Name", subscriptionItem.Article.Name);
                            xmlWriter.WriteString("Sku", subscriptionItem.Article.Sku);
                            xmlWriter.WriteString("PriceExclTax", subscriptionItem.UnitPriceExclTax);
                            xmlWriter.WriteString("PriceInclTax", subscriptionItem.UnitPriceInclTax);
                            xmlWriter.WriteString("Quantity", subscriptionItem.Quantity);
                            xmlWriter.WriteString("DiscountExclTax", subscriptionItem.DiscountAmountExclTax);
                            xmlWriter.WriteString("DiscountInclTax", subscriptionItem.DiscountAmountInclTax);
                            xmlWriter.WriteString("TotalExclTax", subscriptionItem.PriceExclTax);
                            xmlWriter.WriteString("TotalInclTax", subscriptionItem.PriceInclTax);
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }
                }

               
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export subscriptions to XLSX
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        public virtual byte[] ExportSubscriptionsToXlsx(IList<Subscription> subscriptions)
        {
            //a contributor should have access only to part of subscription information
            var ignore = _workContext.CurrentContributor != null;

            //property array
            var properties = new[]
            {
                new PropertyByName<Subscription>("SubscriptionId", p => p.Id),
                new PropertyByName<Subscription>("StoreId", p => p.StoreId),
                new PropertyByName<Subscription>("SubscriptionGuid", p => p.SubscriptionGuid, ignore),
                new PropertyByName<Subscription>("CustomerId", p => p.CustomerId, ignore),
                new PropertyByName<Subscription>("SubscriptionStatusId", p => p.SubscriptionStatusId, ignore),
                new PropertyByName<Subscription>("PaymentStatusId", p => p.PaymentStatusId),
                new PropertyByName<Subscription>("ShippingStatusId", p => p.ShippingStatusId, ignore),
                new PropertyByName<Subscription>("SubscriptionSubtotalInclTax", p => p.SubscriptionSubtotalInclTax, ignore),
                new PropertyByName<Subscription>("SubscriptionSubtotalExclTax", p => p.SubscriptionSubtotalExclTax, ignore),
                new PropertyByName<Subscription>("SubscriptionSubTotalDiscountInclTax", p => p.SubscriptionSubTotalDiscountInclTax, ignore),
                new PropertyByName<Subscription>("SubscriptionSubTotalDiscountExclTax", p => p.SubscriptionSubTotalDiscountExclTax, ignore),
                new PropertyByName<Subscription>("SubscriptionShippingInclTax", p => p.SubscriptionShippingInclTax, ignore),
                new PropertyByName<Subscription>("SubscriptionShippingExclTax", p => p.SubscriptionShippingExclTax, ignore),
                new PropertyByName<Subscription>("PaymentMethodAdditionalFeeInclTax", p => p.PaymentMethodAdditionalFeeInclTax, ignore),
                new PropertyByName<Subscription>("PaymentMethodAdditionalFeeExclTax", p => p.PaymentMethodAdditionalFeeExclTax, ignore),
                new PropertyByName<Subscription>("TaxRates", p => p.TaxRates, ignore),
                new PropertyByName<Subscription>("SubscriptionTax", p => p.SubscriptionTax, ignore),
                new PropertyByName<Subscription>("SubscriptionTotal", p => p.SubscriptionTotal, ignore),
                new PropertyByName<Subscription>("RefundedAmount", p => p.RefundedAmount, ignore),
                new PropertyByName<Subscription>("SubscriptionDiscount", p => p.SubscriptionDiscount, ignore),
                new PropertyByName<Subscription>("CurrencyRate", p => p.CurrencyRate),
                new PropertyByName<Subscription>("CustomerCurrencyCode", p => p.CustomerCurrencyCode),
                new PropertyByName<Subscription>("AffiliateId", p => p.AffiliateId, ignore),
                new PropertyByName<Subscription>("PaymentMethodSystemName", p => p.PaymentMethodSystemName, ignore),
                new PropertyByName<Subscription>("ShippingPickUpInStore", p => p.PickUpInStore, ignore),
                new PropertyByName<Subscription>("ShippingMethod", p => p.ShippingMethod),
                new PropertyByName<Subscription>("ShippingRateComputationMethodSystemName", p => p.ShippingRateComputationMethodSystemName, ignore),
                new PropertyByName<Subscription>("CustomValuesXml", p => p.CustomValuesXml, ignore),
                new PropertyByName<Subscription>("VatNumber", p => p.VatNumber, ignore),
                new PropertyByName<Subscription>("CreatedOnUtc", p => p.CreatedOnUtc.ToOADate()),
                new PropertyByName<Subscription>("BillingFirstName", p => p.BillingAddress.Return(billingAddress => billingAddress.FirstName, String.Empty)),
                new PropertyByName<Subscription>("BillingLastName", p => p.BillingAddress.Return(billingAddress => billingAddress.LastName, String.Empty)),
                new PropertyByName<Subscription>("BillingEmail", p => p.BillingAddress.Return(billingAddress => billingAddress.Email, String.Empty)),
                new PropertyByName<Subscription>("BillingCompany", p => p.BillingAddress.Return(billingAddress => billingAddress.Company, String.Empty)),
                new PropertyByName<Subscription>("BillingCountry", p => p.BillingAddress.Return(billingAddress => billingAddress.Country, null).Return(country => country.Name, String.Empty)),
                new PropertyByName<Subscription>("BillingStateProvince", p => p.BillingAddress.Return(billingAddress => billingAddress.StateProvince, null).Return(stateProvince => stateProvince.Name, String.Empty)),
                new PropertyByName<Subscription>("BillingCity", p => p.BillingAddress.Return(billingAddress => billingAddress.City, String.Empty)),
                new PropertyByName<Subscription>("BillingAddress1", p => p.BillingAddress.Return(billingAddress => billingAddress.Address1, String.Empty)),
                new PropertyByName<Subscription>("BillingAddress2", p => p.BillingAddress.Return(billingAddress => billingAddress.Address2, String.Empty)),
                new PropertyByName<Subscription>("BillingZipPostalCode", p => p.BillingAddress.Return(billingAddress => billingAddress.ZipPostalCode, String.Empty)),
                new PropertyByName<Subscription>("BillingPhoneNumber", p => p.BillingAddress.Return(billingAddress => billingAddress.PhoneNumber, String.Empty)),
                new PropertyByName<Subscription>("BillingFaxNumber", p => p.BillingAddress.Return(billingAddress => billingAddress.FaxNumber, String.Empty)),
                new PropertyByName<Subscription>("ShippingFirstName", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.FirstName, String.Empty)),
                new PropertyByName<Subscription>("ShippingLastName", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.LastName, String.Empty)),
                new PropertyByName<Subscription>("ShippingEmail", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.Email, String.Empty)),
                new PropertyByName<Subscription>("ShippingCompany", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.Company, String.Empty)),
                new PropertyByName<Subscription>("ShippingCountry", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.Country, null).Return(country => country.Name, String.Empty)),
                new PropertyByName<Subscription>("ShippingStateProvince", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.StateProvince, null).Return(stateProvince => stateProvince.Name, String.Empty)),
                new PropertyByName<Subscription>("ShippingCity", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.City, String.Empty)),
                new PropertyByName<Subscription>("ShippingAddress1", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.Address1, String.Empty)),
                new PropertyByName<Subscription>("ShippingAddress2", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.Address2, String.Empty)),
                new PropertyByName<Subscription>("ShippingZipPostalCode", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.ZipPostalCode, String.Empty)),
                new PropertyByName<Subscription>("ShippingPhoneNumber", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.PhoneNumber, String.Empty)),
                new PropertyByName<Subscription>("ShippingFaxNumber", p => p.ShippingAddress.Return(shippingAddress => shippingAddress.FaxNumber, String.Empty))
            };

            return _subscriptionSettings.ExportWithArticles ? ExportSubscriptionToXlsxWithArticles(properties, subscriptions) : ExportToXlsx(properties, subscriptions);
        }

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        public virtual byte[] ExportCustomersToXlsx(IList<Customer> customers)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Customer>("CustomerId", p => p.Id),
                new PropertyByName<Customer>("CustomerGuid", p => p.CustomerGuid),
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username),
                new PropertyByName<Customer>("Password", p => _customerService.GetCurrentPassword(p.Id).Return(password => password.Password, null)),
                new PropertyByName<Customer>("PasswordFormatId", p => _customerService.GetCurrentPassword(p.Id).Return(password => password.PasswordFormatId, 0)),
                new PropertyByName<Customer>("PasswordSalt", p => _customerService.GetCurrentPassword(p.Id).Return(password => password.PasswordSalt, null)),
                new PropertyByName<Customer>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Customer>("AffiliateId", p => p.AffiliateId),
                new PropertyByName<Customer>("ContributorId", p => p.ContributorId),
                new PropertyByName<Customer>("Active", p => p.Active),
                new PropertyByName<Customer>("IsGuest", p => p.IsGuest()),
                new PropertyByName<Customer>("IsRegistered", p => p.IsRegistered()),
                new PropertyByName<Customer>("IsAdministrator", p => p.IsAdmin()),
                new PropertyByName<Customer>("IsForumModerator", p => p.IsForumModerator()),
                new PropertyByName<Customer>("CreatedOnUtc", p => p.CreatedOnUtc),
                //attributes
                new PropertyByName<Customer>("FirstName", p => p.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)),
                new PropertyByName<Customer>("LastName", p => p.GetAttribute<string>(SystemCustomerAttributeNames.LastName)),
                new PropertyByName<Customer>("Gender", p => p.GetAttribute<string>(SystemCustomerAttributeNames.Gender)),
                new PropertyByName<Customer>("Company", p => p.GetAttribute<string>(SystemCustomerAttributeNames.Company)),
                new PropertyByName<Customer>("StreetAddress", p => p.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress)),
                new PropertyByName<Customer>("StreetAddress2", p => p.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2)),
                new PropertyByName<Customer>("ZipPostalCode", p => p.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode)),
                new PropertyByName<Customer>("City", p => p.GetAttribute<string>(SystemCustomerAttributeNames.City)),
                new PropertyByName<Customer>("CountryId", p => p.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)),
                new PropertyByName<Customer>("StateProvinceId", p => p.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId)),
                new PropertyByName<Customer>("Phone", p => p.GetAttribute<string>(SystemCustomerAttributeNames.Phone)),
                new PropertyByName<Customer>("Fax", p => p.GetAttribute<string>(SystemCustomerAttributeNames.Fax)),
                new PropertyByName<Customer>("VatNumber", p => p.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber)),
                new PropertyByName<Customer>("VatNumberStatusId", p => p.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId)),
                new PropertyByName<Customer>("TimeZoneId", p => p.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId)),
                new PropertyByName<Customer>("AvatarPictureId", p => p.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId)),
                new PropertyByName<Customer>("ForumPostCount", p => p.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount)),
                new PropertyByName<Customer>("Signature", p => p.GetAttribute<string>(SystemCustomerAttributeNames.Signature)),
                new PropertyByName<Customer>("CustomCustomerAttributes",  GetCustomCustomerAttributes)
            };

            return ExportToXlsx(properties, customers);
        }

        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportCustomersToXml(IList<Customer> customers)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Customers");
            xmlWriter.WriteAttributeString("Version", YStoryVersion.CurrentVersion);

            foreach (var customer in customers)
            {
                xmlWriter.WriteStartElement("Customer");
                xmlWriter.WriteElementString("CustomerId", null, customer.Id.ToString());
                xmlWriter.WriteElementString("CustomerGuid", null, customer.CustomerGuid.ToString());
                xmlWriter.WriteElementString("Email", null, customer.Email);
                xmlWriter.WriteElementString("Username", null, customer.Username);

                var customerPassword = _customerService.GetCurrentPassword(customer.Id);
                xmlWriter.WriteElementString("Password", null, customerPassword.Return(password => password.Password, null));
                xmlWriter.WriteElementString("PasswordFormatId", null, customerPassword.Return(password => password.PasswordFormatId, 0).ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, customerPassword.Return(password => password.PasswordSalt, null));

                xmlWriter.WriteElementString("IsTaxExempt", null, customer.IsTaxExempt.ToString());
                xmlWriter.WriteElementString("AffiliateId", null, customer.AffiliateId.ToString());
                xmlWriter.WriteElementString("ContributorId", null, customer.ContributorId.ToString());
                xmlWriter.WriteElementString("Active", null, customer.Active.ToString());

                xmlWriter.WriteElementString("IsGuest", null, customer.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, customer.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, customer.IsAdmin().ToString());
                xmlWriter.WriteElementString("IsForumModerator", null, customer.IsForumModerator().ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, customer.CreatedOnUtc.ToString());

                xmlWriter.WriteElementString("FirstName", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName));
                xmlWriter.WriteElementString("LastName", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                xmlWriter.WriteElementString("Gender", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender));
                xmlWriter.WriteElementString("Company", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Company));

                xmlWriter.WriteElementString("CountryId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress));
                xmlWriter.WriteElementString("StreetAddress2", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2));
                xmlWriter.WriteElementString("ZipPostalCode", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode));
                xmlWriter.WriteElementString("City", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.City));
                xmlWriter.WriteElementString("StateProvinceId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId).ToString());
                xmlWriter.WriteElementString("Phone", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone));
                xmlWriter.WriteElementString("Fax", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax));
                xmlWriter.WriteElementString("VatNumber", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber));
                xmlWriter.WriteElementString("VatNumberStatusId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId).ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId));

                foreach (var store in _storeService.GetAllStores())
                {
                    var newsletter = _newsLetterSubscriptionService.GetNewsLetterOrderByEmailAndStoreId(customer.Email, store.Id);
                    bool subscribedToNewsletters = newsletter != null && newsletter.Active;
                    xmlWriter.WriteElementString(string.Format("Newsletter-in-store-{0}", store.Id), null, subscribedToNewsletters.ToString());
                }

                xmlWriter.WriteElementString("AvatarPictureId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId).ToString());
                xmlWriter.WriteElementString("ForumPostCount", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount).ToString());
                xmlWriter.WriteElementString("Signature", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature));

                var selectedCustomerAttributesString = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);

                if (!string.IsNullOrEmpty(selectedCustomerAttributesString))
                {
                    var selectedCustomerAttributes = new StringReader(selectedCustomerAttributesString);
                    var selectedCustomerAttributesXmlReader = XmlReader.Create(selectedCustomerAttributes);
                    xmlWriter.WriteNode(selectedCustomerAttributesXmlReader, false);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions)
        {
            if (subscriptions == null)
                throw new ArgumentNullException("subscriptions");

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(subscription.StoreId);
                sb.Append(Environment.NewLine); //new line
            }
            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(separator);
                sb.Append(state.Name);
                sb.Append(separator);
                sb.Append(state.Abbreviation);
                sb.Append(separator);
                sb.Append(state.Published);
                sb.Append(separator);
                sb.Append(state.DisplaySubscription);
                sb.Append(Environment.NewLine); //new line
            }
            return sb.ToString();
        }

        #endregion
    }
}
