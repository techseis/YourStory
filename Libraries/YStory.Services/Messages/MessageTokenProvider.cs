using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using YStory.Core;
using YStory.Core.Domain;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Forums;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.News;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
 
using YStory.Core.Domain.Stores;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Contributors;
using YStory.Core.Html;
using YStory.Core.Infrastructure;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Events;
using YStory.Services.Forums;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Subscriptions;
using YStory.Services.Payments;
using YStory.Services.Seo;
 
using YStory.Services.Stores;

namespace YStory.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly IDownloadService _downloadService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPaymentService _paymentService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;

        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
      
        private readonly PaymentSettings _paymentSettings;

        private readonly IEventPublisher _eventPublisher;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter, 
            ICurrencyService currencyService,
            IWorkContext workContext,
            IDownloadService downloadService,
            ISubscriptionService subscriptionService,
            IPaymentService paymentService,
            IStoreService storeService,
            IStoreContext storeContext,
            IArticleAttributeParser articleAttributeParser,
            IAddressAttributeFormatter addressAttributeFormatter,
            ICustomerAttributeFormatter customerAttributeFormatter,
            MessageTemplatesSettings templatesSettings,
            CatalogSettings catalogSettings,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            
            PaymentSettings paymentSettings,
            IEventPublisher eventPublisher,
            StoreInformationSettings storeInformationSettings)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._downloadService = downloadService;
            this._subscriptionService = subscriptionService;
            this._paymentService = paymentService;
            this._articleAttributeParser = articleAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._customerAttributeFormatter = customerAttributeFormatter;
            this._storeService = storeService;
            this._storeContext = storeContext;

            this._templatesSettings = templatesSettings;
            this._catalogSettings = catalogSettings;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
           
            this._paymentSettings = paymentSettings;
            this._eventPublisher = eventPublisher;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Allowed tokens

        private Dictionary<string, IEnumerable<string>> _allowedTokens;
        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //store tokens
                _allowedTokens.Add(TokenGroupNames.StoreTokens, new[]
                {
                    "%Store.Name%",
                    "%Store.URL%",
                    "%Store.Email%",
                    "%Store.CompanyName%",
                    "%Store.CompanyAddress%",
                    "%Store.CompanyPhoneNumber%",
                    "%Store.CompanyVat%",
                    "%Facebook.URL%",
                    "%Twitter.URL%",
                    "%YouTube.URL%",
                    "%GooglePlus.URL%"
                });

                //customer tokens
                _allowedTokens.Add(TokenGroupNames.CustomerTokens, new[]
                {
                    "%Customer.Email%",
                    "%Customer.Username%",
                    "%Customer.FullName%",
                    "%Customer.FirstName%",
                    "%Customer.LastName%",
                    "%Customer.VatNumber%",
                    "%Customer.VatNumberStatus%",
                    "%Customer.CustomAttributes%",
                    "%Customer.PasswordRecoveryURL%",
                    "%Customer.AccountActivationURL%",
                    "%Customer.EmailRevalidationURL%",
                    "%Wishlist.URLForCustomer%"
                });

                //subscription tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionTokens, new[]
                {
                    "%Subscription.SubscriptionNumber%",
                    "%Subscription.CustomerFullName%",
                    "%Subscription.CustomerEmail%",
                    "%Subscription.BillingFirstName%",
                    "%Subscription.BillingLastName%",
                    "%Subscription.BillingPhoneNumber%",
                    "%Subscription.BillingEmail%",
                    "%Subscription.BillingFaxNumber%",
                    "%Subscription.BillingCompany%",
                    "%Subscription.BillingAddress1%",
                    "%Subscription.BillingAddress2%",
                    "%Subscription.BillingCity%",
                    "%Subscription.BillingStateProvince%",
                    "%Subscription.BillingZipPostalCode%",
                    "%Subscription.BillingCountry%",
                    "%Subscription.BillingCustomAttributes%",
                    "%Subscription.Shippable%",
                    "%Subscription.ShippingMethod%",
                    "%Subscription.ShippingFirstName%",
                    "%Subscription.ShippingLastName%",
                    "%Subscription.ShippingPhoneNumber%",
                    "%Subscription.ShippingEmail%",
                    "%Subscription.ShippingFaxNumber%",
                    "%Subscription.ShippingCompany%",
                    "%Subscription.ShippingAddress1%",
                    "%Subscription.ShippingAddress2%",
                    "%Subscription.ShippingCity%",
                    "%Subscription.ShippingStateProvince%",
                    "%Subscription.ShippingZipPostalCode%",
                    "%Subscription.ShippingCountry%",
                    "%Subscription.ShippingCustomAttributes%",
                    "%Subscription.PaymentMethod%",
                    "%Subscription.VatNumber%",
                    "%Subscription.CustomValues%",
                    "%Subscription.Article(s)%",
                    "%Subscription.CreatedOn%",
                    "%Subscription.SubscriptionURLForCustomer%"
                });

                //shipment tokens
                _allowedTokens.Add(TokenGroupNames.ShipmentTokens, new[]
                {
                    "%Shipment.ShipmentNumber%",
                    "%Shipment.TrackingNumber%",
                    "%Shipment.TrackingNumberURL%",
                    "%Shipment.Article(s)%",
                    "%Shipment.URLForCustomer%"
                });

                //refunded subscription tokens
                _allowedTokens.Add(TokenGroupNames.RefundedSubscriptionTokens, new[]
                {
                    "%Subscription.AmountRefunded%"
                });

                //subscription note tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionNoteTokens, new[]
                {
                    "%Subscription.NewNoteText%",
                    "%Subscription.SubscriptionNoteAttachmentUrl%"
                });

                //recurring payment tokens
                _allowedTokens.Add(TokenGroupNames.RecurringPaymentTokens, new[]
                {
                    "%RecurringPayment.ID%",
                    "%RecurringPayment.CancelAfterFailedPayment%",
                    "%RecurringPayment.RecurringPaymentType%"
                });

                //newsletter subscription tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionTokens, new[]
                {
                    "%NewsLetterSubscription.Email%",
                    "%NewsLetterSubscription.ActivationUrl%",
                    "%NewsLetterSubscription.DeactivationUrl%"
                });

                //article tokens
                _allowedTokens.Add(TokenGroupNames.ArticleTokens, new[]
                {
                    "%Article.ID%",
                    "%Article.Name%",
                    "%Article.ShortDescription%",
                    "%Article.ArticleURLForCustomer%",
                    "%Article.SKU%",
                    "%Article.StockQuantity%"
                });

                //return request tokens
                _allowedTokens.Add(TokenGroupNames.ReturnRequestTokens, new[]
                {
                    "%ReturnRequest.CustomNumber%",
                    "%ReturnRequest.SubscriptionId%",
                    "%ReturnRequest.Article.Quantity%",
                    "%ReturnRequest.Article.Name%",
                    "%ReturnRequest.Reason%",
                    "%ReturnRequest.RequestedAction%",
                    "%ReturnRequest.CustomerComment%",
                    "%ReturnRequest.StaffNotes%",
                    "%ReturnRequest.Status%"
                });

                //forum tokens
                _allowedTokens.Add(TokenGroupNames.ForumTokens, new[]
                {
                    "%Forums.ForumURL%",
                    "%Forums.ForumName%"
                });

                //forum topic tokens
                _allowedTokens.Add(TokenGroupNames.ForumTopicTokens, new[]
                {
                    "%Forums.TopicURL%",
                    "%Forums.TopicName%"
                });

                //forum post tokens
                _allowedTokens.Add(TokenGroupNames.ForumPostTokens, new[]
                {
                    "%Forums.PostAuthor%",
                    "%Forums.PostBody%"
                });

                //private message tokens
                _allowedTokens.Add(TokenGroupNames.PrivateMessageTokens, new[]
                {
                    "%PrivateMessage.Subject%",
                    "%PrivateMessage.Text%"
                });

                //contributor tokens
                _allowedTokens.Add(TokenGroupNames.ContributorTokens, new[]
                {
                    "%Contributor.Name%",
                    "%Contributor.Email%"
                });

                //gift card tokens
                _allowedTokens.Add(TokenGroupNames.GiftCardTokens, new[]
                {
                    "%GiftCard.SenderName%",
                    "%GiftCard.SenderEmail%",
                    "%GiftCard.RecipientName%",
                    "%GiftCard.RecipientEmail%",
                    "%GiftCard.Amount%",
                    "%GiftCard.CouponCode%",
                    "%GiftCard.Message%"
                });

                //article review tokens
                _allowedTokens.Add(TokenGroupNames.ArticleReviewTokens, new[]
                {
                    "%ArticleReview.ArticleName%"
                });

                //attribute combination tokens
                _allowedTokens.Add(TokenGroupNames.AttributeCombinationTokens, new[]
                {
                    "%AttributeCombination.Formatted%",
                    "%AttributeCombination.SKU%",
                    "%AttributeCombination.StockQuantity%"
                });

                //blog comment tokens
                _allowedTokens.Add(TokenGroupNames.BlogCommentTokens, new[]
                {
                    "%BlogComment.BlogPostTitle%"
                });

                //news comment tokens
                _allowedTokens.Add(TokenGroupNames.NewsCommentTokens, new[]
                {
                    "%NewsComment.NewsTitle%"
                });

                //article back in stock tokens
                _allowedTokens.Add(TokenGroupNames.ArticleBackInStockTokens, new[]
                {
                    "%BackInStockSubscription.ArticleName%",
                    "%BackInStockSubscription.ArticleUrl%"
                });

                //email a friend tokens
                _allowedTokens.Add(TokenGroupNames.EmailAFriendTokens, new[]
                {
                    "%EmailAFriend.PersonalMessage%",
                    "%EmailAFriend.Email%"
                });

                //wishlist to friend tokens
                _allowedTokens.Add(TokenGroupNames.WishlistToFriendTokens, new[]
                {
                    "%Wishlist.PersonalMessage%",
                    "%Wishlist.Email%"
                });

                //VAT validation tokens
                _allowedTokens.Add(TokenGroupNames.VatValidation, new[]
                {
                    "%VatValidationResult.Name%",
                    "%VatValidationResult.Address%"
                });

                //contact us tokens
                _allowedTokens.Add(TokenGroupNames.ContactUs, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                //contact contributor tokens
                _allowedTokens.Add(TokenGroupNames.ContactContributor, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                return _allowedTokens;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="contributorId">Contributor identifier (used to limit articles by contributor</param>
        /// <returns>HTML table of articles</returns>
        protected virtual string ArticleListToHtmlTable(Subscription subscription, int languageId, int contributorId)
        {
            string result;

            var language = _languageService.GetLanguageById(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table bsubscription=\"0\" style=\"width:100%;\">");

            #region Articles
            sb.AppendLine(string.Format("<tr style=\"background-color:{0};text-align:center;\">", _templatesSettings.Color1));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Name", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Price", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Quantity", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Total", languageId)));
            sb.AppendLine("</tr>");

            var table = subscription.SubscriptionItems.ToList();
            for (int i = 0; i <= table.Count - 1; i++)
            {
                var subscriptionItem = table[i];
                var article = subscriptionItem.Article;
                if (article == null)
                    continue;

                if (contributorId > 0 && article.ContributorId != contributorId)
                    continue;

                sb.AppendLine(string.Format("<tr style=\"background-color: {0};text-align: center;\">", _templatesSettings.Color2));
                //article name
                string articleName = article.GetLocalized(x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + HttpUtility.HtmlEncode(articleName));
                //add download link
                if (_downloadService.IsDownloadAllowed(subscriptionItem))
                {
                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                    string downloadUrl = string.Format("{0}download/getdownload/{1}", GetStoreUrl(subscription.StoreId), subscriptionItem.SubscriptionItemGuid);
                    string downloadLink = string.Format("<a class=\"link\" href=\"{0}\">{1}</a>", downloadUrl, _localizationService.GetResource("Messages.Subscription.Article(s).Download", languageId));
                    sb.AppendLine("<br />");
                    sb.AppendLine(downloadLink);
                }
                //add download link
                if (_downloadService.IsLicenseDownloadAllowed(subscriptionItem))
                {
                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                    string licenseUrl = string.Format("{0}download/getlicense/{1}", GetStoreUrl(subscription.StoreId), subscriptionItem.SubscriptionItemGuid);
                    string licenseLink = string.Format("<a class=\"link\" href=\"{0}\">{1}</a>", licenseUrl, _localizationService.GetResource("Messages.Subscription.Article(s).License", languageId));
                    sb.AppendLine("<br />");
                    sb.AppendLine(licenseLink);
                }
                //attributes
                if (!String.IsNullOrEmpty(subscriptionItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(subscriptionItem.AttributeDescription);
                }
                //rental info
                if (subscriptionItem.Article.IsRental)
                {
                    var rentalStartDate = subscriptionItem.RentalStartDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = subscriptionItem.RentalEndDateUtc.HasValue ? subscriptionItem.Article.FormatRentalDate(subscriptionItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(_localizationService.GetResource("Subscription.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }
                //sku
                if (_catalogSettings.ShowSkuOnArticleDetailsPage)
                {
                    var sku = article.FormatSku(subscriptionItem.AttributesXml, _articleAttributeParser);
                    if (!String.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(_localizationService.GetResource("Messages.Subscription.Article(s).SKU", languageId), HttpUtility.HtmlEncode(sku)));
                    }
                }
                sb.AppendLine("</td>");

                string unitPriceStr;
                if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceInclTax, subscription.CurrencyRate);
                    unitPriceStr = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.UnitPriceExclTax, subscription.CurrencyRate);
                    unitPriceStr = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                }
                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: right;\">{0}</td>", unitPriceStr));

                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: center;\">{0}</td>", subscriptionItem.Quantity));

                string priceStr; 
                if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceInclTax, subscription.CurrencyRate);
                    priceStr = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscriptionItem.PriceExclTax, subscription.CurrencyRate);
                    priceStr = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                }
                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: right;\">{0}</td>", priceStr));

                sb.AppendLine("</tr>");
            }
            #endregion

            if (contributorId == 0)
            {
                //we render checkout attributes and totals only for store owners (hide for contributors)
            
                #region Checkout Attributes

                if (!String.IsNullOrEmpty(subscription.CheckoutAttributeDescription))
                {
                    sb.AppendLine("<tr><td style=\"text-align:right;\" colspan=\"1\">&nbsp;</td><td colspan=\"3\" style=\"text-align:right\">");
                    sb.AppendLine(subscription.CheckoutAttributeDescription);
                    sb.AppendLine("</td></tr>");
                }

                #endregion

                #region Totals

                //subtotal
                string cusSubTotal;
                bool displaySubTotalDiscount = false;
                string cusSubTotalDiscount = string.Empty;
                if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromSubscriptionSubtotal)
                {
                    //including tax

                    //subtotal
                    var subscriptionSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalInclTax, subscription.CurrencyRate);
                    cusSubTotal = _priceFormatter.FormatPrice(subscriptionSubtotalInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                    //discount (applied to subscription subtotal)
                    var subscriptionSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountInclTax, subscription.CurrencyRate);
                    if (subscriptionSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    {
                        cusSubTotalDiscount = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                        displaySubTotalDiscount = true;
                    }
                }
                else
                {
                    //exсluding tax

                    //subtotal
                    var subscriptionSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubtotalExclTax, subscription.CurrencyRate);
                    cusSubTotal = _priceFormatter.FormatPrice(subscriptionSubtotalExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                    //discount (applied to subscription subtotal)
                    var subscriptionSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionSubTotalDiscountExclTax, subscription.CurrencyRate);
                    if (subscriptionSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    {
                        cusSubTotalDiscount = _priceFormatter.FormatPrice(-subscriptionSubTotalDiscountExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                        displaySubTotalDiscount = true;
                    }
                }
                
                //shipping, payment method fee
                string cusShipTotal;
                string cusPaymentMethodAdditionalFee;
                var taxRates = new SortedDictionary<decimal, decimal>();
                string cusTaxTotal = string.Empty;
                string cusDiscount = string.Empty;
                string cusTotal; 
                if (subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax

                    //shipping
                    var subscriptionShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionShippingInclTax, subscription.CurrencyRate);
                    cusShipTotal = _priceFormatter.FormatShippingPrice(subscriptionShippingInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                    //payment method additional fee
                    var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeInclTax, subscription.CurrencyRate);
                    cusPaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, true);
                }
                else
                {
                    //excluding tax

                    //shipping
                    var subscriptionShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionShippingExclTax, subscription.CurrencyRate);
                    cusShipTotal = _priceFormatter.FormatShippingPrice(subscriptionShippingExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                    //payment method additional fee
                    var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.PaymentMethodAdditionalFeeExclTax, subscription.CurrencyRate);
                    cusPaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, language, false);
                }

               

                //payment method fee
                bool displayPaymentMethodFee = subscription.PaymentMethodAdditionalFeeExclTax > decimal.Zero;

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInSubscriptionSummary && subscription.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
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
                        taxRates = new SortedDictionary<decimal, decimal>();
                        foreach (var tr in subscription.TaxRatesDictionary)
                            taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, subscription.CurrencyRate));

                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        var subscriptionTaxInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTax, subscription.CurrencyRate);
                        string taxStr = _priceFormatter.FormatPrice(subscriptionTaxInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, language);
                        cusTaxTotal = taxStr;
                    }
                }

                //discount
                bool displayDiscount = false;
                if (subscription.SubscriptionDiscount > decimal.Zero)
                {
                    var subscriptionDiscountInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionDiscount, subscription.CurrencyRate);
                    cusDiscount = _priceFormatter.FormatPrice(-subscriptionDiscountInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, language);
                    displayDiscount = true;
                }

                //total
                var subscriptionTotalInCustomerCurrency = _currencyService.ConvertCurrency(subscription.SubscriptionTotal, subscription.CurrencyRate);
                cusTotal = _priceFormatter.FormatPrice(subscriptionTotalInCustomerCurrency, true, subscription.CustomerCurrencyCode, false, language);




                //subtotal
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Subscription.SubTotal", languageId), cusSubTotal));

                //discount (applied to subscription subtotal)
                if (displaySubTotalDiscount)
                {
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Subscription.SubTotalDiscount", languageId), cusSubTotalDiscount));
                }

 

                //payment method fee
                if (displayPaymentMethodFee)
                {
                    string paymentMethodFeeTitle = _localizationService.GetResource("Messages.Subscription.PaymentMethodAdditionalFee", languageId);
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, paymentMethodFeeTitle, cusPaymentMethodAdditionalFee));
                }

                //tax
                if (displayTax)
                {
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Subscription.Tax", languageId), cusTaxTotal));
                }
                if (displayTaxRates)
                {
                    foreach (var item in taxRates)
                    {
                        string taxRate = String.Format(_localizationService.GetResource("Messages.Subscription.TaxRateLine"), _priceFormatter.FormatTaxRate(item.Key));
                        string taxValue = _priceFormatter.FormatPrice(item.Value, true, subscription.CustomerCurrencyCode, false, language);
                        sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, taxRate, taxValue));
                    }
                }

                //discount
                if (displayDiscount)
                {
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Subscription.TotalDiscount", languageId), cusDiscount));
                }

               

                //reward points
                if (subscription.RedeemedRewardPointsEntry != null)
                {
                    string rpTitle = string.Format(_localizationService.GetResource("Messages.Subscription.RewardPoints", languageId), -subscription.RedeemedRewardPointsEntry.Points);
                    string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(subscription.RedeemedRewardPointsEntry.UsedAmount, subscription.CurrencyRate)), true, subscription.CustomerCurrencyCode, false, language);
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, rpTitle, rpAmount));
                }

                //total
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Subscription.SubscriptionTotal", languageId), cusTotal));
                #endregion

            }

            sb.AppendLine("</table>");
            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>HTML table of articles</returns>
        protected virtual string ArticleListToHtmlTable(int languageId)
        {
            string result;
            
            var sb = new StringBuilder();
            sb.AppendLine("<table bsubscription=\"0\" style=\"width:100%;\">");

            #region Articles
            sb.AppendLine(string.Format("<tr style=\"background-color:{0};text-align:center;\">", _templatesSettings.Color1));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Name", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Subscription.Article(s).Quantity", languageId)));
            sb.AppendLine("</tr>");

             
            #endregion
            
            sb.AppendLine("</table>");
            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Get store URL
        /// </summary>
        /// <param name="storeId">Store identifier; Pass 0 to load URL of the current store</param>
        /// <returns></returns>
        protected virtual string GetStoreUrl(int storeId = 0)
        {
            var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore;

            if (store == null)
                throw new Exception("No store could be loaded");

            return store.Url;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        public virtual void AddStoreTokens(IList<Token> tokens, Store store, EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            tokens.Add(new Token("Store.Name", store.GetLocalized(x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));
            tokens.Add(new Token("GooglePlus.URL", _storeInformationSettings.GooglePlusLink));

            //event notification
            _eventPublisher.EntityTokensAdded(store, tokens);
        }

        /// <summary>
        /// Add subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="contributorId">Contributor identifier</param>
        public virtual void AddSubscriptionTokens(IList<Token> tokens, Subscription subscription, int languageId, int contributorId = 0)
        {
            tokens.Add(new Token("Subscription.SubscriptionNumber", subscription.CustomSubscriptionNumber));

            tokens.Add(new Token("Subscription.CustomerFullName", string.Format("{0} {1}", subscription.BillingAddress.FirstName, subscription.BillingAddress.LastName)));
            tokens.Add(new Token("Subscription.CustomerEmail", subscription.BillingAddress.Email));


            tokens.Add(new Token("Subscription.BillingFirstName", subscription.BillingAddress.FirstName));
            tokens.Add(new Token("Subscription.BillingLastName", subscription.BillingAddress.LastName));
            tokens.Add(new Token("Subscription.BillingPhoneNumber", subscription.BillingAddress.PhoneNumber));
            tokens.Add(new Token("Subscription.BillingEmail", subscription.BillingAddress.Email));
            tokens.Add(new Token("Subscription.BillingFaxNumber", subscription.BillingAddress.FaxNumber));
            tokens.Add(new Token("Subscription.BillingCompany", subscription.BillingAddress.Company));
            tokens.Add(new Token("Subscription.BillingAddress1", subscription.BillingAddress.Address1));
            tokens.Add(new Token("Subscription.BillingAddress2", subscription.BillingAddress.Address2));
            tokens.Add(new Token("Subscription.BillingCity", subscription.BillingAddress.City));
            tokens.Add(new Token("Subscription.BillingStateProvince", subscription.BillingAddress.StateProvince != null ? subscription.BillingAddress.StateProvince.GetLocalized(x => x.Name) : string.Empty));
            tokens.Add(new Token("Subscription.BillingZipPostalCode", subscription.BillingAddress.ZipPostalCode));
            tokens.Add(new Token("Subscription.BillingCountry", subscription.BillingAddress.Country != null ? subscription.BillingAddress.Country.GetLocalized(x => x.Name) : string.Empty));
            tokens.Add(new Token("Subscription.BillingCustomAttributes", _addressAttributeFormatter.FormatAttributes(subscription.BillingAddress.CustomAttributes), true));

            tokens.Add(new Token("Subscription.Shippable", !string.IsNullOrEmpty(subscription.ShippingMethod)));
            tokens.Add(new Token("Subscription.ShippingMethod", subscription.ShippingMethod));
            tokens.Add(new Token("Subscription.ShippingFirstName", subscription.ShippingAddress != null ? subscription.ShippingAddress.FirstName : string.Empty));
            tokens.Add(new Token("Subscription.ShippingLastName", subscription.ShippingAddress != null ? subscription.ShippingAddress.LastName : string.Empty));
            tokens.Add(new Token("Subscription.ShippingPhoneNumber", subscription.ShippingAddress != null ? subscription.ShippingAddress.PhoneNumber : string.Empty));
            tokens.Add(new Token("Subscription.ShippingEmail", subscription.ShippingAddress != null ? subscription.ShippingAddress.Email : string.Empty));
            tokens.Add(new Token("Subscription.ShippingFaxNumber", subscription.ShippingAddress != null ? subscription.ShippingAddress.FaxNumber : string.Empty));
            tokens.Add(new Token("Subscription.ShippingCompany", subscription.ShippingAddress != null ? subscription.ShippingAddress.Company : string.Empty));
            tokens.Add(new Token("Subscription.ShippingAddress1", subscription.ShippingAddress != null ? subscription.ShippingAddress.Address1 : string.Empty));
            tokens.Add(new Token("Subscription.ShippingAddress2", subscription.ShippingAddress != null ? subscription.ShippingAddress.Address2 : string.Empty));
            tokens.Add(new Token("Subscription.ShippingCity", subscription.ShippingAddress != null ? subscription.ShippingAddress.City : string.Empty));
            tokens.Add(new Token("Subscription.ShippingStateProvince", subscription.ShippingAddress != null && subscription.ShippingAddress.StateProvince != null ? subscription.ShippingAddress.StateProvince.GetLocalized(x => x.Name) : string.Empty));
            tokens.Add(new Token("Subscription.ShippingZipPostalCode", subscription.ShippingAddress != null ? subscription.ShippingAddress.ZipPostalCode : string.Empty));
            tokens.Add(new Token("Subscription.ShippingCountry", subscription.ShippingAddress != null && subscription.ShippingAddress.Country != null ? subscription.ShippingAddress.Country.GetLocalized(x => x.Name) : string.Empty));
            tokens.Add(new Token("Subscription.ShippingCustomAttributes", _addressAttributeFormatter.FormatAttributes(subscription.ShippingAddress != null ? subscription.ShippingAddress.CustomAttributes : string.Empty), true));

            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(subscription.PaymentMethodSystemName);
            var paymentMethodName = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : subscription.PaymentMethodSystemName;
            tokens.Add(new Token("Subscription.PaymentMethod", paymentMethodName));
            tokens.Add(new Token("Subscription.VatNumber", subscription.VatNumber));
            var sbCustomValues = new StringBuilder();
            var customValues = subscription.DeserializeCustomValues();
            if (customValues != null)
            {
                foreach (var item in customValues)
                {
                    sbCustomValues.AppendFormat("{0}: {1}", HttpUtility.HtmlEncode(item.Key), HttpUtility.HtmlEncode(item.Value != null ? item.Value.ToString() : string.Empty));
                    sbCustomValues.Append("<br />");
                }
            }
            tokens.Add(new Token("Subscription.CustomValues", sbCustomValues.ToString(), true));
            


            tokens.Add(new Token("Subscription.Article(s)", ArticleListToHtmlTable(subscription, languageId, contributorId), true));

            var language = _languageService.GetLanguageById(languageId);
            if (language != null && !String.IsNullOrEmpty(language.LanguageCulture))
            {
                DateTime createdOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, TimeZoneInfo.Utc, _dateTimeHelper.GetCustomerTimeZone(subscription.Customer));
                tokens.Add(new Token("Subscription.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("Subscription.CreatedOn", subscription.CreatedOnUtc.ToString("D")));
            }

            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            tokens.Add(new Token("Subscription.SubscriptionURLForCustomer", string.Format("{0}subscriptiondetails/{1}", GetStoreUrl(subscription.StoreId), subscription.Id), true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        /// <summary>
        /// Add refunded subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Subscription</param>
        /// <param name="refundedAmount">Refunded amount of subscription</param>
        public virtual void AddSubscriptionRefundedTokens(IList<Token> tokens, Subscription subscription, decimal refundedAmount)
        {
            //should we convert it to customer currency?
            //most probably, no. It can cause some rounding or legal issues
            //furthermore, exchange rate could be changed
            //so let's display it the primary store currency

            var primaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            var refundedAmountStr = _priceFormatter.FormatPrice(refundedAmount, true, primaryStoreCurrencyCode, false, _workContext.WorkingLanguage);

            tokens.Add(new Token("Subscription.AmountRefunded", refundedAmountStr));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        

        /// <summary>
        /// Add subscription note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscriptionNote">Subscription note</param>
        public virtual void AddSubscriptionNoteTokens(IList<Token> tokens, SubscriptionNote subscriptionNote)
        {
            tokens.Add(new Token("Subscription.NewNoteText", subscriptionNote.FormatSubscriptionNoteText(), true));
            tokens.Add(new Token("Subscription.SubscriptionNoteAttachmentUrl", string.Format("{0}download/subscriptionnotefile/{1}", GetStoreUrl(subscriptionNote.Subscription.StoreId), subscriptionNote.Id), true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscriptionNote, tokens);
        }

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment)
        {
            tokens.Add(new Token("RecurringPayment.ID", recurringPayment.Id));
            tokens.Add(new Token("RecurringPayment.CancelAfterFailedPayment", 
                recurringPayment.LastPaymentFailed && _paymentSettings.CancelRecurringPaymentsAfterFailedPayment));
            if (recurringPayment.InitialSubscription != null)
                tokens.Add(new Token("RecurringPayment.RecurringPaymentType", _paymentService.GetRecurringPaymentType(recurringPayment.InitialSubscription.PaymentMethodSystemName).ToString()));

            //event notification
            _eventPublisher.EntityTokensAdded(recurringPayment, tokens);
        }

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="subscriptionItem">Subscription item</param>
        public virtual void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, SubscriptionItem subscriptionItem)
        {
            tokens.Add(new Token("ReturnRequest.CustomNumber", returnRequest.CustomNumber));
            tokens.Add(new Token("ReturnRequest.SubscriptionId", subscriptionItem.SubscriptionId));
            tokens.Add(new Token("ReturnRequest.Article.Quantity", returnRequest.Quantity));
            tokens.Add(new Token("ReturnRequest.Article.Name", subscriptionItem.Article.Name));
            tokens.Add(new Token("ReturnRequest.Reason", returnRequest.ReasonForReturn));
            tokens.Add(new Token("ReturnRequest.RequestedAction", returnRequest.RequestedAction));
            tokens.Add(new Token("ReturnRequest.CustomerComment", HtmlHelper.FormatText(returnRequest.CustomerComments, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.StaffNotes", HtmlHelper.FormatText(returnRequest.StaffNotes, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.Status", returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext)));

            //event notification
            _eventPublisher.EntityTokensAdded(returnRequest, tokens);
        }

        

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", customer.GetFullName()));
            tokens.Add(new Token("Customer.FirstName", customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)));
            tokens.Add(new Token("Customer.LastName", customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName)));
            tokens.Add(new Token("Customer.VatNumber", customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber)));
            tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId)).ToString()));

            var customAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes);
            tokens.Add(new Token("Customer.CustomAttributes", _customerAttributeFormatter.FormatAttributes(customAttributesXml), true));


            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var passwordRecoveryUrl = string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken), HttpUtility.UrlEncode(customer.Email));
            var accountActivationUrl = string.Format("{0}customer/activation?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken), HttpUtility.UrlEncode(customer.Email));
            var emailRevalidationUrl = string.Format("{0}customer/revalidateemail?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.EmailRevalidationToken), HttpUtility.UrlEncode(customer.Email));
            var wishlistUrl = string.Format("{0}wishlist/{1}", GetStoreUrl(), customer.CustomerGuid);

            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Customer.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        /// <summary>
        /// Add contributor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="contributor">Contributor</param>
        public virtual void AddContributorTokens(IList<Token> tokens, Contributor contributor)
        {
            tokens.Add(new Token("Contributor.Name", contributor.Name));
            tokens.Add(new Token("Contributor.Email", contributor.Email));

            //event notification
            _eventPublisher.EntityTokensAdded(contributor, tokens);
        }

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        public virtual void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));


            const string urlFormat = "{0}newsletter/subscriptionactivation/{1}/{2}";

            var activationUrl = String.Format(urlFormat, GetStoreUrl(), subscription.NewsLetterSubscriptionGuid, "true");
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deActivationUrl = String.Format(urlFormat, GetStoreUrl(), subscription.NewsLetterSubscriptionGuid, "false");
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deActivationUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        /// <summary>
        /// Add article review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="articleReview">Article review</param>
        public virtual void AddArticleReviewTokens(IList<Token> tokens, ArticleReview articleReview)
        {
            tokens.Add(new Token("ArticleReview.ArticleName", articleReview.Article.Name));

            //event notification
            _eventPublisher.EntityTokensAdded(articleReview, tokens);
        }

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        public virtual void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment)
        {
            tokens.Add(new Token("BlogComment.BlogPostTitle", blogComment.BlogPost.Title));

            //event notification
            _eventPublisher.EntityTokensAdded(blogComment, tokens);
        }

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        public virtual void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment)
        {
            tokens.Add(new Token("NewsComment.NewsTitle", newsComment.NewsItem.Title));

            //event notification
            _eventPublisher.EntityTokensAdded(newsComment, tokens);
        }

        /// <summary>
        /// Add article tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="article">Article</param>
        /// <param name="languageId">Language identifier</param>
        public virtual void AddArticleTokens(IList<Token> tokens, Article article, int languageId)
        {
            tokens.Add(new Token("Article.ID", article.Id));
            tokens.Add(new Token("Article.Name", article.GetLocalized(x => x.Name, languageId)));
            tokens.Add(new Token("Article.ShortDescription", article.GetLocalized(x => x.ShortDescription, languageId), true));
            tokens.Add(new Token("Article.SKU", article.Sku));
            tokens.Add(new Token("Article.StockQuantity", article.GetTotalStockQuantity()));

            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var articleUrl = string.Format("{0}{1}", GetStoreUrl(), article.GetSeName());
            tokens.Add(new Token("Article.ArticleURLForCustomer", articleUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(article, tokens);
        }

        /// <summary>
        /// Add article attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">Article attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        public virtual void AddAttributeCombinationTokens(IList<Token> tokens, ArticleAttributeCombination combination,  int languageId)
        {
            //attributes
            //we cannot inject IArticleAttributeFormatter into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var articleAttributeFormatter = EngineContext.Current.Resolve<IArticleAttributeFormatter>();
            string attributes = articleAttributeFormatter.FormatAttributes(combination.Article, 
                combination.AttributesXml, 
                _workContext.CurrentCustomer, 
                renderPrices: false);

            

            tokens.Add(new Token("AttributeCombination.Formatted", attributes, true));
            tokens.Add(new Token("AttributeCombination.SKU", combination.Article.FormatSku(combination.AttributesXml, _articleAttributeParser)));
            tokens.Add(new Token("AttributeCombination.StockQuantity", combination.StockQuantity));
            
            //event notification
            _eventPublisher.EntityTokensAdded(combination, tokens);
        }

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        public virtual void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic, 
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string topicUrl;
            if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
                topicUrl = string.Format("{0}boards/topic/{1}/{2}/page/{3}", GetStoreUrl(), forumTopic.Id, forumTopic.GetSeName(), friendlyForumTopicPageIndex.Value);
            else
                topicUrl = string.Format("{0}boards/topic/{1}/{2}", GetStoreUrl(), forumTopic.Id, forumTopic.GetSeName());
            if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
                topicUrl = string.Format("{0}#{1}", topicUrl, appendedPostIdentifierAnchor.Value);
            tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
            tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

            //event notification
            _eventPublisher.EntityTokensAdded(forumTopic, tokens);
        }

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        public virtual void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost)
        {
            tokens.Add(new Token("Forums.PostAuthor", forumPost.Customer.FormatUserName()));
            tokens.Add(new Token("Forums.PostBody", forumPost.FormatPostText(), true));

            //event notification
            _eventPublisher.EntityTokensAdded(forumPost, tokens);
        }

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        public virtual void AddForumTokens(IList<Token> tokens, Forum forum)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var forumUrl = string.Format("{0}boards/forum/{1}/{2}", GetStoreUrl(), forum.Id, forum.GetSeName());
            tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
            tokens.Add(new Token("Forums.ForumName", forum.Name));

            //event notification
            _eventPublisher.EntityTokensAdded(forum, tokens);
        }

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        public virtual void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage)
        {
            tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
            tokens.Add(new Token("PrivateMessage.Text",  privateMessage.FormatPrivateMessageText(), true));

            //event notification
            _eventPublisher.EntityTokensAdded(privateMessage, tokens);
        }
 

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>Collection of allowed (supported) message tokens for campaigns</returns>
        public virtual IEnumerable<string> GetListOfCampaignAllowedTokens()
        {
            var additionTokens = new CampaignAdditionTokensAddedEvent();
            _eventPublisher.Publish(additionTokens);

            var allowedTokens = GetListOfAllowedTokens(new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens }).ToList();
            allowedTokens.AddRange(additionTokens.AdditionTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        public virtual IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null)
        {
            var additionTokens = new AdditionTokensAddedEvent();
            _eventPublisher.Publish(additionTokens);

            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            allowedTokens.AddRange(additionTokens.AdditionTokens);

            return allowedTokens.Distinct();
        }

        #endregion
    }
}
