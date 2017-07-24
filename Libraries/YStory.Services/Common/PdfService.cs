// RTL Support provided by Credo inc (www.credo.co.il  ||   info@credo.co.il)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Subscriptions;
 
using YStory.Core.Domain.Tax;
using YStory.Core.Html;
using YStory.Services.Catalog;
using YStory.Services.Configuration;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Stores;

namespace YStory.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPaymentService _paymentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IPictureService _pictureService;
        private readonly IArticleService _articleService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingContext;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService, 
            ILanguageService languageService,
            IWorkContext workContext,
            ISubscriptionService subscriptionService,
            IPaymentService paymentService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService, 
            IMeasureService measureService,
            IPictureService pictureService,
            IArticleService articleService, 
            IArticleAttributeParser articleAttributeParser,
            IStoreService storeService,
            IStoreContext storeContext,
            ISettingService settingContext,
            IAddressAttributeFormatter addressAttributeFormatter,
            CatalogSettings catalogSettings, 
            CurrencySettings currencySettings,
            MeasureSettings measureSettings,
            PdfSettings pdfSettings,
            TaxSettings taxSettings,
            AddressSettings addressSettings)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._workContext = workContext;
            this._subscriptionService = subscriptionService;
            this._paymentService = paymentService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._pictureService = pictureService;
            this._articleService = articleService;
            this._articleAttributeParser = articleAttributeParser;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._settingContext = settingContext;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._currencySettings = currencySettings;
            this._catalogSettings = catalogSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._taxSettings = taxSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get font
        /// </summary>
        /// <returns>Font</returns>
        protected virtual Font GetFont()
        {
            //yourStory supports unicode characters
            //yourStory uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            return GetFont(_pdfSettings.FontFileName);
        }
        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="fontFileName">Font file name</param>
        /// <returns>Font</returns>
        protected virtual Font GetFont(string fontFileName)
        {
            if (fontFileName == null)
                throw new ArgumentNullException("fontFileName");

            string fontPath = Path.Combine(CommonHelper.MapPath("~/App_Data/Pdf/"), fontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        /// <summary>
        /// Get font direction
        /// </summary>
        /// <param name="lang">Language</param>
        /// <returns>Font direction</returns>
        protected virtual int GetDirection(Language lang)
        {
            return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
        }

        /// <summary>
        /// Get element alignment
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="isOpposite">Is opposite?</param>
        /// <returns>Element alignment</returns>
        protected virtual int GetAlignment(Language lang, bool isOpposite = false)
        {
            //if we need the element to be opposite, like logo etc`.
            if (!isOpposite)
                return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
            
            return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Print an subscription to PDF
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an subscription</param>
        /// <param name="contributorId">Contributor identifier to limit articles; 0 to to print all articles. If specified, then totals won't be printed</param>
        /// <returns>A path of generated file</returns>
        public virtual string PrintSubscriptionToPdf(Subscription subscription, int languageId = 0, int contributorId = 0)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            string fileName = string.Format("subscription_{0}_{1}.pdf", subscription.SubscriptionGuid, CommonHelper.GenerateRandomDigitCode(4));
            string filePath = Path.Combine(CommonHelper.MapPath("~/content/files/ExportImport"), fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var subscriptions = new List<Subscription>();
                subscriptions.Add(subscription);
                PrintSubscriptionsToPdf(fileStream, subscriptions, languageId, contributorId);
            }
            return filePath;
        }

        /// <summary>
        /// Print subscriptions to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an subscription</param>
        /// <param name="contributorId">Contributor identifier to limit articles; 0 to to print all articles. If specified, then totals won't be printed</param>
        public virtual void PrintSubscriptionsToPdf(Stream stream, IList<Subscription> subscriptions, int languageId = 0, int contributorId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (subscriptions == null)
                throw new ArgumentNullException("subscriptions");

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }


            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            int ordCount = subscriptions.Count;
            int ordNum = 0;

            foreach (var subscription in subscriptions)
            {
                //by default _pdfSettings contains settings for the current active store
                //and we need PdfSettings for the store which was used to place an subscription
                //so let's load it based on a store of the current subscription
                var pdfSettingsByStore = _settingContext.LoadSetting<PdfSettings>(subscription.StoreId);


                var lang = _languageService.GetLanguageById(languageId == 0 ? subscription.CustomerLanguageId : languageId);
                if (lang == null || !lang.Published)
                    lang = _workContext.WorkingLanguage;

                #region Header

                //logo
                var logoPicture = _pictureService.GetPictureById(pdfSettingsByStore.LogoPictureId);
                var logoExists = logoPicture != null;

                //header
                var headerTable = new PdfPTable(logoExists ? 2 : 1);
                headerTable.RunDirection = GetDirection(lang);
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                //store info
                var store = _storeService.GetStoreById(subscription.StoreId) ?? _storeContext.CurrentStore;
                var anchor = new Anchor(store.Url.Trim(new [] { '/' }), font);
                anchor.Reference = store.Url;

                var cellHeader = new PdfPCell(new Phrase(String.Format(_localizationService.GetResource("PDFInvoice.Subscription#", lang.Id), subscription.CustomSubscriptionNumber), titleFont));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Phrase.Add(new Phrase(anchor));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Phrase.Add(new Phrase(String.Format(_localizationService.GetResource("PDFInvoice.SubscriptionDate", lang.Id), _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc).ToString("D", new CultureInfo(lang.LanguageCulture))), font));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                cellHeader.Border = Rectangle.NO_BORDER;

                headerTable.AddCell(cellHeader);

                if (logoExists)
                    if (lang.Rtl)
                        headerTable.SetWidths(new[] { 0.2f, 0.8f });
                    else
                        headerTable.SetWidths(new[] { 0.8f, 0.2f });
                headerTable.WidthPercentage = 100f;

                //logo               
                if (logoExists)
                {
                    var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                    var logo = Image.GetInstance(logoFilePath);
                    logo.Alignment = GetAlignment(lang, true);
                    logo.ScaleToFit(65f, 65f);

                    var cellLogo = new PdfPCell();
                    cellLogo.Border = Rectangle.NO_BORDER;
                    cellLogo.AddElement(logo);
                    headerTable.AddCell(cellLogo);
                }
                doc.Add(headerTable); 

                #endregion

                #region Addresses

                var addressTable = new PdfPTable(2);
                addressTable.RunDirection = GetDirection(lang);
                addressTable.DefaultCell.Border = Rectangle.NO_BORDER;
                addressTable.WidthPercentage = 100f;
                addressTable.SetWidths(new[] { 50, 50 });

                //billing info
                var billingAddress = new PdfPTable(1);
                billingAddress.DefaultCell.Border = Rectangle.NO_BORDER;
                billingAddress.RunDirection = GetDirection(lang);

                billingAddress.AddCell(new Paragraph(_localizationService.GetResource("PDFInvoice.BillingInformation", lang.Id), titleFont));

                if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(subscription.BillingAddress.Company))
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), subscription.BillingAddress.Company), font));

                billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), subscription.BillingAddress.FirstName + " " + subscription.BillingAddress.LastName), font));
                if (_addressSettings.PhoneEnabled)
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), subscription.BillingAddress.PhoneNumber), font));
                if (_addressSettings.FaxEnabled && !String.IsNullOrEmpty(subscription.BillingAddress.FaxNumber))
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), subscription.BillingAddress.FaxNumber), font));
                if (_addressSettings.StreetAddressEnabled)
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), subscription.BillingAddress.Address1), font));
                if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(subscription.BillingAddress.Address2))
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), subscription.BillingAddress.Address2), font));
                if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                    billingAddress.AddCell(new Paragraph("   " + String.Format("{0}, {1} {2}", subscription.BillingAddress.City, subscription.BillingAddress.StateProvince != null ? subscription.BillingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", subscription.BillingAddress.ZipPostalCode), font));
                if (_addressSettings.CountryEnabled && subscription.BillingAddress.Country != null)
                    billingAddress.AddCell(new Paragraph("   " + subscription.BillingAddress.Country.GetLocalized(x => x.Name, lang.Id), font));

                //VAT number
                if (!String.IsNullOrEmpty(subscription.VatNumber))
                    billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.VATNumber", lang.Id), subscription.VatNumber), font));

                //custom attributes
                var customBillingAddressAttributes = _addressAttributeFormatter.FormatAttributes( subscription.BillingAddress.CustomAttributes);
                if (!String.IsNullOrEmpty(customBillingAddressAttributes))
                {
                    //TODO: we should add padding to each line (in case if we have sevaral custom address attributes)
                    billingAddress.AddCell(new Paragraph("   " + HtmlHelper.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true), font));
                }



                //contributors payment details
                if (contributorId == 0)
                {
                    //payment method
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(subscription.PaymentMethodSystemName);
                    string paymentMethodStr = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, lang.Id) : subscription.PaymentMethodSystemName;
                    if (!String.IsNullOrEmpty(paymentMethodStr))
                    {
                        billingAddress.AddCell(new Paragraph(" "));
                        billingAddress.AddCell(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.PaymentMethod", lang.Id), paymentMethodStr), font));
                        billingAddress.AddCell(new Paragraph());
                    }

                    //custom values
                    var customValues = subscription.DeserializeCustomValues();
                    if (customValues != null)
                    {
                        foreach (var item in customValues)
                        {
                            billingAddress.AddCell(new Paragraph(" "));
                            billingAddress.AddCell(new Paragraph("   " + item.Key + ": " + item.Value, font));
                            billingAddress.AddCell(new Paragraph());
                        }
                    }
                }
                addressTable.AddCell(billingAddress);

                //shipping info
                var shippingAddress = new PdfPTable(1);
                shippingAddress.DefaultCell.Border = Rectangle.NO_BORDER;
                shippingAddress.RunDirection = GetDirection(lang);

                 

                doc.Add(addressTable);
                doc.Add(new Paragraph(" "));

                #endregion

                #region Articles

                //articles
                var articlesHeader = new PdfPTable(1);
                articlesHeader.RunDirection = GetDirection(lang);
                articlesHeader.WidthPercentage = 100f;
                var cellArticles = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.Article(s)", lang.Id), titleFont));
                cellArticles.Border = Rectangle.NO_BORDER;
                articlesHeader.AddCell(cellArticles);
                doc.Add(articlesHeader);
                doc.Add(new Paragraph(" "));


                var subscriptionItems = subscription.SubscriptionItems;

                var articlesTable = new PdfPTable(_catalogSettings.ShowSkuOnArticleDetailsPage ? 5 : 4);
                articlesTable.RunDirection = GetDirection(lang);
                articlesTable.WidthPercentage = 100f;
                if (lang.Rtl)
                {
                    articlesTable.SetWidths(_catalogSettings.ShowSkuOnArticleDetailsPage
                        ? new[] {15, 10, 15, 15, 45}
                        : new[] {20, 10, 20, 50});
                }
                else
                {
                    articlesTable.SetWidths(_catalogSettings.ShowSkuOnArticleDetailsPage
                        ? new[] {45, 15, 15, 10, 15}
                        : new[] {50, 20, 10, 20});
                }

                //article name
                var cellArticleItem = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ArticleName", lang.Id), font));
                cellArticleItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                articlesTable.AddCell(cellArticleItem);

                //SKU
                if (_catalogSettings.ShowSkuOnArticleDetailsPage)
                {
                    cellArticleItem = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.SKU", lang.Id), font));
                    cellArticleItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                    articlesTable.AddCell(cellArticleItem);
                }

                //price
                cellArticleItem = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ArticlePrice", lang.Id), font));
                cellArticleItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                articlesTable.AddCell(cellArticleItem);

                //qty
                cellArticleItem = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ArticleQuantity", lang.Id), font));
                cellArticleItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                articlesTable.AddCell(cellArticleItem);

                //total
                cellArticleItem = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ArticleTotal", lang.Id), font));
                cellArticleItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                articlesTable.AddCell(cellArticleItem);

                foreach (var subscriptionItem in subscriptionItems)
                {
                    var p = subscriptionItem.Article;

                    //a contributor should have access only to his articles
                    if (contributorId > 0 && p.ContributorId != contributorId)
                        continue;

                    var pAttribTable = new PdfPTable(1);
                    pAttribTable.RunDirection = GetDirection(lang);
                    pAttribTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    
                    //article name
                    string name = p.GetLocalized(x => x.Name, lang.Id);
                    pAttribTable.AddCell(new Paragraph(name, font));
                    cellArticleItem.AddElement(new Paragraph(name, font));
                    //attributes
                    if (!String.IsNullOrEmpty(subscriptionItem.AttributeDescription))
                    {
                        var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(subscriptionItem.AttributeDescription, true, true), attributesFont);
                        pAttribTable.AddCell(attributesParagraph);
                    }
                    //rental info
                    if (subscriptionItem.Article.IsRental)
                    {
                        var rentalStartDate = subscriptionItem.RentalStartDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalStartDateUtc.Value) : "";
                        var rentalEndDate = subscriptionItem.RentalEndDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalEndDateUtc.Value) : "";
                        var rentalInfo = string.Format(_localizationService.GetResource("Subscription.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);

                        var rentalInfoParagraph = new Paragraph(rentalInfo, attributesFont);
                        pAttribTable.AddCell(rentalInfoParagraph);
                    }
                    articlesTable.AddCell(pAttribTable);

                    //SKU
                    if (_catalogSettings.ShowSkuOnArticleDetailsPage)
                    {
                        var sku = p.FormatSku(subscriptionItem.AttributesXml, _articleAttributeParser);
                        cellArticleItem = new PdfPCell(new Phrase(sku ?? String.Empty, font));
                        cellArticleItem.HorizontalAlignment = Element.ALIGN_CENTER;
                        articlesTable.AddCell(cellArticleItem);
                    }

                    //price
                    string unitPrice;
                    if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                    {
                        //including tax
                        var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceInclTax, subscription.CurrencyRate);
                        unitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, true);
                    }
                    else
                    {
                        //excluding tax
                        var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceExclTax, subscription.CurrencyRate);
                        unitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, false);
                    }
                    cellArticleItem = new PdfPCell(new Phrase(unitPrice, font));
                    cellArticleItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    articlesTable.AddCell(cellArticleItem);

                    //qty
                    cellArticleItem = new PdfPCell(new Phrase(subscriptionItem.Quantity.ToString(), font));
                    cellArticleItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    articlesTable.AddCell(cellArticleItem);

                    //total
                    string subTotal; 
                    if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                    {
                        //including tax
                        var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceInclTax, subscription.CurrencyRate);
                        subTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, true);
                    }
                    else
                    {
                        //excluding tax
                        var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceExclTax, subscription.CurrencyRate);
                        subTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, false);
                    }
                    cellArticleItem = new PdfPCell(new Phrase(subTotal, font));
                    cellArticleItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    articlesTable.AddCell(cellArticleItem);
                }
                doc.Add(articlesTable);

                #endregion

                #region Checkout attributes

                //contributors cannot see checkout attributes
                if (contributorId == 0 && !String.IsNullOrEmpty(subscription.CheckoutAttributeDescription))
                {
                    doc.Add(new Paragraph(" "));
                    var attribTable = new PdfPTable(1);
                    attribTable.RunDirection = GetDirection(lang);
                    attribTable.WidthPercentage = 100f;

                    string attributes = HtmlHelper.ConvertHtmlToPlainText(subscription.CheckoutAttributeDescription, true, true);
                    var cCheckoutAttributes = new PdfPCell(new Phrase(attributes, font));
                    cCheckoutAttributes.Border = Rectangle.NO_BORDER;
                    cCheckoutAttributes.HorizontalAlignment = Element.ALIGN_RIGHT;
                    attribTable.AddCell(cCheckoutAttributes);
                    doc.Add(attribTable);
                }

                #endregion

                #region Totals

                //contributors cannot see totals
                if (contributorId == 0)
                {
                    //subtotal
                    var totalsTable = new PdfPTable(1);
                    totalsTable.RunDirection = GetDirection(lang);
                    totalsTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    totalsTable.WidthPercentage = 100f;

                    //subscription subtotal
                    if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionSubtotal)
                    {
                        //including tax

                        var subscriptionSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalInclTax, subscription.CurrencyRate);
                        string subscriptionSubtotalInclTaxStr = _priceFormatter.FormatPrice(subscriptionSubtotalInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, true);

                        var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), subscriptionSubtotalInclTaxStr), font));
                        p.HorizontalAlignment = Element.ALIGN_RIGHT;
                        p.Border = Rectangle.NO_BORDER;
                        totalsTable.AddCell(p);
                    }
                    else
                    {
                        //excluding tax

                        var subscriptionSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalExclTax, subscription.CurrencyRate);
                        string subscriptionSubtotalExclTaxStr = _priceFormatter.FormatPrice(subscriptionSubtotalExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, false);

                        var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), subscriptionSubtotalExclTaxStr), font));
                        p.HorizontalAlignment = Element.ALIGN_RIGHT;
                        p.Border = Rectangle.NO_BORDER;
                        totalsTable.AddCell(p);
                    }

                    //discount (applied to subscription subtotal)
                    if (subscription.SubscriptionSubTotalDiscountExclTax > decimal.Zero)
                    {
                        //subscription subtotal
                        if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionSubtotal)
                        {
                            //including tax

                            var subscriptionSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountInclTax, subscription.CurrencyRate);
                            string subscriptionSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, true);

                            var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), subscriptionSubTotalDiscountInCustomerCurrencyStr), font));
                            p.HorizontalAlignment = Element.ALIGN_RIGHT;
                            p.Border = Rectangle.NO_BORDER;
                            totalsTable.AddCell(p);
                        }
                        else
                        {
                            //excluding tax

                            var subscriptionSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountExclTax, subscription.CurrencyRate);
                            string subscriptionSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, false);

                            var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), subscriptionSubTotalDiscountInCustomerCurrencyStr), font));
                            p.HorizontalAlignment = Element.ALIGN_RIGHT;
                            p.Border = Rectangle.NO_BORDER;
                            totalsTable.AddCell(p);
                        }
                    }

                     

                    //payment fee
                    if (subscription.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                    {
                        if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeInclTax, subscription.CurrencyRate);
                            string paymentMethodAdditionalFeeInclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, true);

                            var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeInclTaxStr), font));
                            p.HorizontalAlignment = Element.ALIGN_RIGHT;
                            p.Border = Rectangle.NO_BORDER;
                            totalsTable.AddCell(p);
                        }
                        else
                        {
                            //excluding tax
                            var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeExclTax, subscription.CurrencyRate);
                            string paymentMethodAdditionalFeeExclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, lang, false);

                            var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeExclTaxStr), font));
                            p.HorizontalAlignment = Element.ALIGN_RIGHT;
                            p.Border = Rectangle.NO_BORDER;
                            totalsTable.AddCell(p);
                        }
                    }

                    //tax
                    string taxStr = string.Empty;
                    var taxRates = new SortedDictionary<decimal, decimal>();
                    bool displayTax = true;
                    bool displayTaxRates = true;
                    if (_taxSettings.HideTaxInSubscriptionSummary && subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                    {
                        displayTax = false;
                    }
                    else
                    {
                        if (subscription.SubscriptionTax == 0 && _taxSettings.HideZeroTax)
                        {
                            displayTax = false;
                            displayTaxRates = false;
                        }
                        else
                        {
                            taxRates = subscription.TaxRatesDictionary;

                            displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                            displayTax = !displayTaxRates;

                            var subscriptionTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTax, subscription.CurrencyRate);
                            taxStr = _priceFormatter.FormatPrice(subscriptionTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, lang);
                        }
                    }
                    if (displayTax)
                    {
                        var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Tax", lang.Id), taxStr), font));
                        p.HorizontalAlignment = Element.ALIGN_RIGHT;
                        p.Border = Rectangle.NO_BORDER;
                        totalsTable.AddCell(p);
                    }
                    if (displayTaxRates)
                    {
                        foreach (var item in taxRates)
                        {
                            string taxRate = String.Format(_localizationService.GetResource("PDFInvoice.TaxRate", lang.Id), _priceFormatter.FormatTaxRate(item.Key));
                            string taxValue = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value, subscription.CurrencyRate), true, subscription.CustomerCurrencyCode, false, lang);

                            var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", taxRate, taxValue), font));
                            p.HorizontalAlignment = Element.ALIGN_RIGHT;
                            p.Border = Rectangle.NO_BORDER;
                            totalsTable.AddCell(p);
                        }
                    }

                    //discount (applied to subscription total)
                    if (subscription.SubscriptionDiscount > decimal.Zero)
                    {
                        var subscriptionDiscountInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionDiscount, subscription.CurrencyRate);
                        string subscriptionDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-subscriptionDiscountInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, lang);

                        var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), subscriptionDiscountInCustomerCurrencyStr), font));
                        p.HorizontalAlignment = Element.ALIGN_RIGHT;
                        p.Border = Rectangle.NO_BORDER;
                        totalsTable.AddCell(p);
                    }

                     

                    //reward points
                    if (subscription.RedeemedRewardPointsEntry != null)
                    {
                        string rpTitle = string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", lang.Id), -subscription.RedeemedRewardPointsEntry.Points);
                        string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(subscription.RedeemedRewardPointsEntry.UsedAmount, subscription.CurrencyRate)), true, subscription.CustomerCurrencyCode, false, lang);

                        var p = new PdfPCell(new Paragraph(String.Format("{0} {1}", rpTitle, rpAmount), font));
                        p.HorizontalAlignment = Element.ALIGN_RIGHT;
                        p.Border = Rectangle.NO_BORDER;
                        totalsTable.AddCell(p);
                    }

                    //subscription total
                    var subscriptionTotalInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTotal, subscription.CurrencyRate);
                    string subscriptionTotalStr = _priceFormatter.FormatPrice(subscriptionTotalInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, lang);


                    var pTotal = new PdfPCell(new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.SubscriptionTotal", lang.Id), subscriptionTotalStr), titleFont));
                    pTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pTotal.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(pTotal);

                    doc.Add(totalsTable);
                }

                #endregion

                #region Subscription notes

                if (pdfSettingsByStore.RenderSubscriptionNotes)
                {
                    var subscriptionNotes = subscription.SubscriptionNotes
                        .Where(on => on.DisplayToCustomer)
                        .OrderByDescending(on => on.CreatedOnUtc)
                        .ToList();
                    if (subscriptionNotes.Any())
                    { 
                        var notesHeader = new PdfPTable(1);
                        notesHeader.RunDirection = GetDirection(lang);
                        notesHeader.WidthPercentage = 100f;
                        var cellSubscriptionNote = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.SubscriptionNotes", lang.Id), titleFont));
                        cellSubscriptionNote.Border = Rectangle.NO_BORDER;
                        notesHeader.AddCell(cellSubscriptionNote);
                        doc.Add(notesHeader);
                        doc.Add(new Paragraph(" "));

                        var notesTable = new PdfPTable(2);
                        notesTable.RunDirection = GetDirection(lang);
                        if (lang.Rtl)
                        {
                            notesTable.SetWidths(new[] {70, 30});
                        }
                        else
                        {
                            notesTable.SetWidths(new[] {30, 70});
                        }
                        notesTable.WidthPercentage = 100f;

                        //created on
                        cellSubscriptionNote = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.SubscriptionNotes.CreatedOn", lang.Id), font));
                        cellSubscriptionNote.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cellSubscriptionNote.HorizontalAlignment = Element.ALIGN_CENTER;
                        notesTable.AddCell(cellSubscriptionNote);

                        //note
                        cellSubscriptionNote = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.SubscriptionNotes.Note", lang.Id), font));
                        cellSubscriptionNote.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cellSubscriptionNote.HorizontalAlignment = Element.ALIGN_CENTER;
                        notesTable.AddCell(cellSubscriptionNote);

                        foreach (var subscriptionNote in subscriptionNotes)
                        {
                            cellSubscriptionNote = new PdfPCell(new Phrase(_dateTimeHelper.ConvertToUserTime(subscriptionNote.CreatedOnUtc, DateTimeKind.Utc).ToString(), font));
                            cellSubscriptionNote.HorizontalAlignment = Element.ALIGN_LEFT;
                            notesTable.AddCell(cellSubscriptionNote);

                            cellSubscriptionNote = new PdfPCell(new Phrase(HtmlHelper.ConvertHtmlToPlainText(subscriptionNote.FormatSubscriptionNoteText(), true, true), font));
                            cellSubscriptionNote.HorizontalAlignment = Element.ALIGN_LEFT;
                            notesTable.AddCell(cellSubscriptionNote);

                            //should we display a link to downloadable files here?
                            //I think, no. Onyway, PDFs are printable documents and links (files) are useful here
                        }
                        doc.Add(notesTable);
                    }
                }

                #endregion

                #region Footer

                if (!String.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) || !String.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2))
                {
                    var column1Lines = String.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
                        new List<string>() :
                        pdfSettingsByStore.InvoiceFooterTextColumn1
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    var column2Lines = String.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
                        new List<string>() :
                        pdfSettingsByStore.InvoiceFooterTextColumn2
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    if (column1Lines.Any() || column2Lines.Any())
                    {
                        var totalLines = Math.Max(column1Lines.Count, column2Lines.Count);
                        const float margin = 43;

                        //if you have really a lot of lines in the footer, then replace 9 with 10 or 11
                        int footerHeight = totalLines * 9;
                        var directContent = pdfWriter.DirectContent;
                        directContent.MoveTo(pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight);
                        directContent.LineTo(pageSize.GetRight(margin), pageSize.GetBottom(margin) + footerHeight);
                        directContent.Stroke();


                        var footerTable = new PdfPTable(2);
                        footerTable.WidthPercentage = 100f;
                        footerTable.SetTotalWidth(new float[] { 250, 250 });
                        footerTable.RunDirection = GetDirection(lang);

                        //column 1
                        if (column1Lines.Any())
                        {
                            var column1 = new PdfPCell(new Phrase());
                            column1.Border = Rectangle.NO_BORDER;
                            column1.HorizontalAlignment = Element.ALIGN_LEFT;
                            foreach (var footerLine in column1Lines)
                            {
                                column1.Phrase.Add(new Phrase(footerLine, font));
                                column1.Phrase.Add(new Phrase(Environment.NewLine));
                            }
                            footerTable.AddCell(column1);
                        }
                        else
                        {
                            var column = new PdfPCell(new Phrase(" "));
                            column.Border = Rectangle.NO_BORDER;
                            footerTable.AddCell(column);
                        }

                        //column 2
                        if (column2Lines.Any())
                        {
                            var column2 = new PdfPCell(new Phrase());
                            column2.Border = Rectangle.NO_BORDER;
                            column2.HorizontalAlignment = Element.ALIGN_LEFT;
                            foreach (var footerLine in column2Lines)
                            {
                                column2.Phrase.Add(new Phrase(footerLine, font));
                                column2.Phrase.Add(new Phrase(Environment.NewLine));
                            }
                            footerTable.AddCell(column2);
                        }
                        else
                        {
                            var column = new PdfPCell(new Phrase(" "));
                            column.Border = Rectangle.NO_BORDER;
                            footerTable.AddCell(column);
                        }

                        footerTable.WriteSelectedRows(0, totalLines, pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight, directContent);
                    }
                }

                #endregion

                ordNum++;
                if (ordNum < ordCount)
                {
                    doc.NewPage();
                }
            }
            doc.Close();
        }
       

        /// <summary>
        /// Print articles to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="articles">Articles</param>
        public virtual void PrintArticlesToPdf(Stream stream, IList<Article> articles)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (articles == null)
                throw new ArgumentNullException("articles");

            var lang = _workContext.WorkingLanguage;

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();

            int articleNumber = 1;
            int prodCount = articles.Count;

            foreach (var article in articles)
            {
                string articleName = article.GetLocalized(x => x.Name, lang.Id);
                string articleDescription = article.GetLocalized(x => x.FullDescription, lang.Id);

                var articleTable = new PdfPTable(1);
                articleTable.WidthPercentage = 100f;
                articleTable.DefaultCell.Border = Rectangle.NO_BORDER;
                if (lang.Rtl)
                {
                    articleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                }

                articleTable.AddCell(new Paragraph(String.Format("{0}. {1}", articleNumber, articleName), titleFont));
                articleTable.AddCell(new Paragraph(" "));
                articleTable.AddCell(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(articleDescription, decode: true)), font));
                articleTable.AddCell(new Paragraph(" "));

                if (article.ArticleType == ArticleType.SimpleArticle)
                {
                    //simple article
                    //render its properties such as price, weight, etc
                    var priceStr = string.Format("{0} {1}", article.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
                    if (article.IsRental)
                        priceStr = _priceFormatter.FormatRentalArticlePeriod(article, priceStr);
                    articleTable.AddCell(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFArticleCatalog.Price", lang.Id), priceStr), font));
                    articleTable.AddCell(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFArticleCatalog.SKU", lang.Id), article.Sku), font));

                    
                    articleTable.AddCell(new Paragraph(" "));
                }
                var pictures = _pictureService.GetPicturesByArticleId(article.Id);
                if (pictures.Any())
                {
                    var table = new PdfPTable(2);
                    table.WidthPercentage = 100f;
                    if (lang.Rtl)
                    {
                        table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    }

                    foreach (var pic in pictures)
                    {
                        var picBinary = _pictureService.LoadPictureBinary(pic);
                        if (picBinary != null && picBinary.Length > 0)
                        {
                            var pictureLocalPath = _pictureService.GetThumbLocalPath(pic, 200, false);
                            var cell = new PdfPCell(Image.GetInstance(pictureLocalPath));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                        }
                    }

                    if (pictures.Count % 2 > 0)
                    {
                        var cell = new PdfPCell(new Phrase(" "));
                        cell.Border = Rectangle.NO_BORDER;
                        table.AddCell(cell);
                    }

                    articleTable.AddCell(table);
                    articleTable.AddCell(new Paragraph(" "));
                }


                if (article.ArticleType == ArticleType.GroupedArticle)
                {
                    //grouped article. render its associated articles
                    int pvNum = 1;
                    foreach (var associatedArticle in _articleService.GetAssociatedArticles(article.Id, showHidden: true))
                    {
                        articleTable.AddCell(new Paragraph(String.Format("{0}-{1}. {2}", articleNumber, pvNum, associatedArticle.GetLocalized(x => x.Name, lang.Id)), font));
                        articleTable.AddCell(new Paragraph(" "));

                        //uncomment to render associated article description
                        //string apDescription = associatedArticle.GetLocalized(x => x.ShortDescription, lang.Id);
                        //if (!String.IsNullOrEmpty(apDescription))
                        //{
                        //    articleTable.AddCell(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(apDescription)), font));
                        //    articleTable.AddCell(new Paragraph(" "));
                        //}

                        //uncomment to render associated article picture
                        //var apPicture = _pictureService.GetPicturesByArticleId(associatedArticle.Id).FirstOrDefault();
                        //if (apPicture != null)
                        //{
                        //    var picBinary = _pictureService.LoadPictureBinary(apPicture);
                        //    if (picBinary != null && picBinary.Length > 0)
                        //    {
                        //        var pictureLocalPath = _pictureService.GetThumbLocalPath(apPicture, 200, false);
                        //        articleTable.AddCell(Image.GetInstance(pictureLocalPath));
                        //    }
                        //}

                        articleTable.AddCell(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFArticleCatalog.Price", lang.Id), associatedArticle.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode), font));
                        articleTable.AddCell(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFArticleCatalog.SKU", lang.Id), associatedArticle.Sku), font));

                       
                        articleTable.AddCell(new Paragraph(" "));

                        pvNum++;
                    }
                }

                doc.Add(articleTable);

                articleNumber++;

                if (articleNumber <= prodCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        #endregion
    }
}