using System;
using System.Text;
using System.Web;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Html;
using YStory.Services.Directory;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Tax;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article attribute formatter
    /// </summary>
    public partial class ArticleAttributeFormatter : IArticleAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public ArticleAttributeFormatter(IWorkContext workContext,
            IArticleAttributeService articleAttributeService,
            IArticleAttributeParser articleAttributeParser,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ITaxService taxService,
            IPriceFormatter priceFormatter,
            IDownloadService downloadService,
            IWebHelper webHelper,
            IPriceCalculationService priceCalculationService,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._articleAttributeService = articleAttributeService;
            this._articleAttributeParser = articleAttributeParser;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
            this._priceCalculationService = priceCalculationService;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Article article, string attributesXml)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributes(article, attributesXml, customer);
        }
        
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderArticleAttributes">A value indicating whether to render article attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Article article, string attributesXml,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderArticleAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();

            //attributes
            if (renderArticleAttributes)
            {
                foreach (var attribute in _articleAttributeParser.ParseArticleAttributeMappings(attributesXml))
                {
                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _articleAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = attribute.ArticleAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = HttpUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = string.Format("{0}: {1}", attributeName, HtmlHelper.FormatText(value, false, true, false, false, false, false));
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid downloadGuid;
                                Guid.TryParse(value, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = string.Format("{0}{1}", download.Filename ?? download.DownloadGuid.ToString(), download.Extension);

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = HttpUtility.HtmlEncode(fileName);

                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    var attributeText = allowHyperlinks ? string.Format("<a href=\"{0}download/getfileupload/?downloadId={1}\" class=\"fileuploadattribute\">{2}</a>",
                                        _webHelper.GetStoreLocation(false), download.DownloadGuid, fileName) : fileName;

                                    var attributeName = attribute.ArticleAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = HttpUtility.HtmlEncode(attributeName);

                                    formattedAttribute = string.Format("{0}: {1}", attributeName, attributeText);
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = string.Format("{0}: {1}", attribute.ArticleAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), value);

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = HttpUtility.HtmlEncode(formattedAttribute);
                            }

                            if (!string.IsNullOrEmpty(formattedAttribute))
                            {
                                if (result.Length > 0)
                                    result.Append(serapator);
                                result.Append(formattedAttribute);
                            }
                        }
                    }
                    //article attribute values
                    else
                    {
                        foreach (var attributeValue in _articleAttributeParser.ParseArticleAttributeValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Format("{0}: {1}",
                                attribute.ArticleAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id),
                                attributeValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));

                            if (renderPrices)
                            {
                                decimal taxRate;
                                var attributeValuePriceAdjustment = _priceCalculationService.GetArticleAttributeValuePriceAdjustment(attributeValue);
                                var priceAdjustmentBase = _taxService.GetArticlePrice(article, attributeValuePriceAdjustment, customer, out taxRate);
                                var priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                                if (priceAdjustmentBase > 0)
                                    formattedAttribute += string.Format(" [+{0}]", _priceFormatter.FormatPrice(priceAdjustment, false, false));
                                else if (priceAdjustmentBase < decimal.Zero)
                                    formattedAttribute += string.Format(" [-{0}]", _priceFormatter.FormatPrice(-priceAdjustment, false, false));
                            }

                            //display quantity
                            if (_shoppingCartSettings.RenderAssociatedAttributeValueQuantity && attributeValue.AttributeValueType == AttributeValueType.AssociatedToArticle)
                            {
                                //render only when more than 1
                                if (attributeValue.Quantity > 1)
                                    formattedAttribute += string.Format(_localizationService.GetResource("ArticleAttributes.Quantity"), attributeValue.Quantity);
                            }

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = HttpUtility.HtmlEncode(formattedAttribute);

                            if (!string.IsNullOrEmpty(formattedAttribute))
                            {
                                if (result.Length > 0)
                                    result.Append(serapator);
                                result.Append(formattedAttribute);
                            }
                        }
                    }
                }
            }

             
            return result.ToString();
        }
    }
}
