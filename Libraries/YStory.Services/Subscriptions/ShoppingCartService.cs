using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Catalog;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Events;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Security;
using YStory.Services.Stores;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IArticleService _articleService;
        private readonly ILocalizationService _localizationService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IArticleAttributeService _articleAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="articleService">Article settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="articleAttributeParser">Article attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="dateRangeService">Date range service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="articleAttributeService">Article attribute service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public ShoppingCartService(IRepository<ShoppingCartItem> sciRepository,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IArticleService articleService,
            ILocalizationService localizationService,
            IArticleAttributeParser articleAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPriceFormatter priceFormatter,
            ICustomerService customerService,
            ShoppingCartSettings shoppingCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService, 
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            IArticleAttributeService articleAttributeService,
            IDateTimeHelper dateTimeHelper)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._articleService = articleService;
            this._localizationService = localizationService;
            this._articleAttributeParser = articleAttributeParser;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._priceFormatter = priceFormatter;
            this._customerService = customerService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._genericAttributeService = genericAttributeService;
            this._articleAttributeService = articleAttributeService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true, 
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            var customer = shoppingCartItem.Customer;
            var storeId = shoppingCartItem.StoreId;

            //reset checkout data
            if (resetCheckoutData)
            {
                _customerService.ResetCheckoutData(shoppingCartItem.Customer, shoppingCartItem.StoreId);
            }

            //delete item
            _sciRepository.Delete(shoppingCartItem);

            //reset "HasShoppingCartItems" property used for performance optimization
            customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
            _customerService.UpdateCustomer(customer);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore wishlist)
                shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
            {
                var cart = customer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(storeId)
                    .ToList();

                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, storeId);
                checkoutAttributesXml = _checkoutAttributeParser.EnsureOnlyActiveAttributes(checkoutAttributesXml, cart);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CheckoutAttributes, checkoutAttributesXml, storeId);
            }

            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            var query = from sci in _sciRepository.Table
                           where sci.UpdatedOnUtc < olderThanUtc
                           select sci;

            var cartItems = query.ToList();
            foreach (var cartItem in cartItems)
                _sciRepository.Delete(cartItem);
            return cartItems.Count;
        }

        /// <summary>
        /// Validates required articles (articles which require some other articles to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="automaticallyAddRequiredArticlesIfEnabled">Automatically add required articles if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredArticleWarnings(Customer customer,
            ShoppingCartType shoppingCartType, Article article,
            int storeId, bool automaticallyAddRequiredArticlesIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (article == null)
                throw new ArgumentNullException("article");

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            var warnings = new List<string>();

            if (article.RequireOtherArticles)
            {
                var requiredArticles = new List<Article>();
                foreach (var id in article.ParseRequiredArticleIds())
                {
                    var rp = _articleService.GetArticleById(id);
                    if (rp != null)
                        requiredArticles.Add(rp);
                }
                
                foreach (var rp in requiredArticles)
                {
                    //ensure that article is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.ArticleId == rp.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {

                        if (article.AutomaticallyAddRequiredArticles)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredArticlesIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredArticlesIfEnabled' to prevent circular references
                                var addToCartWarnings = AddToCart(customer: customer,
                                    article: rp, 
                                    shoppingCartType: shoppingCartType,
                                    storeId: storeId,
                                    automaticallyAddRequiredArticlesIfEnabled: false);
                                if (addToCartWarnings.Any())
                                {
                                    //a article wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only generic error
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredArticleWarning"), rp.GetLocalized(x => x.Name)));
                                }
                            }
                            else
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredArticleWarning"), rp.GetLocalized(x => x.Name)));
                            }
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredArticleWarning"), rp.GetLocalized(x => x.Name)));
                        }
                    }
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Validates a article for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Article article, string attributesXml, decimal customerEnteredPrice,
            int quantity)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (article == null)
                throw new ArgumentNullException("article");

            var warnings = new List<string>();

            //deleted
            if (article.Deleted)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ArticleDeleted"));
                return warnings;
            }

            //published
            if (!article.Published)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ArticleUnpublished"));
            }

            //we can add only simple articles
            if (article.ArticleType != ArticleType.SimpleArticle)
            {
                warnings.Add("This is not simple article");
            }
            
            //ACL
            if (!_aclService.Authorize(article, customer))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ArticleUnpublished"));
            }

            //Store mapping
            if (!_storeMappingService.Authorize(article, _storeContext.CurrentStore.Id))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ArticleUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && article.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && article.DisableWishlistButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && article.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Articles.CallForPrice"));
            }

              
            var allowedQuantities = article.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

             

            //availability dates
            bool availableStartDateError = false;
            if (article.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(article.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }
            if (article.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(article.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemAttributeWarnings(Customer customer, 
            ShoppingCartType shoppingCartType,
            Article article, 
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = _articleAttributeParser.ParseArticleAttributeMappings(attributesXml);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
            }
            foreach (var attribute in attributes1)
            {
                if (attribute.Article != null)
                {
                    if (attribute.Article.Id != article.Id)
                    {
                        warnings.Add("Attribute error");
                    }
                }
                else
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }
            }

            //validate required article attributes (whether they're chosen/selected/entered)
            var attributes2 = _articleAttributeService.GetArticleAttributeMappingsByArticleId(article.Id);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
            }
            //validate conditional attributes only (if specified)
            attributes2 = attributes2.Where(x =>
            {
                var conditionMet = _articleAttributeParser.IsConditionMet(x, attributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToList();
            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected article attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id == a2.Id)
                        {
                            var attributeValuesStr = _articleAttributeParser.ParseValues(attributesXml, a1.Id);
                            foreach (string str1 in attributeValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        var notFoundWarning = !string.IsNullOrEmpty(a2.TextPrompt) ?
                            a2.TextPrompt : 
                            string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.ArticleAttribute.GetLocalized(a => a.Name));
                        
                        warnings.Add(notFoundWarning);
                    }
                }

                if (a2.AttributeControlType == AttributeControlType.ReadonlyCheckboxes)
                {
                    //customers cannot edit read-only attributes
                    var allowedReadOnlyValueIds = _articleAttributeService.GetArticleAttributeValues(a2.Id)
                        .Where(x => x.IsPreSelected)
                        .Select(x => x.Id)
                        .ToArray();

                    var selectedReadOnlyValueIds = _articleAttributeParser.ParseArticleAttributeValues(attributesXml)
                        .Where(x => x.ArticleAttributeMappingId == a2.Id)
                        .Select(x => x.Id)
                        .ToArray();

                    if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
                    {
                        warnings.Add("You cannot change read-only values");
                    }
                }
            }

            //validation rules
            foreach (var pam in attributes2)
            {
                if (!pam.ValidationRulesAllowed())
                    continue;
                
                //minimum length
                if (pam.ValidationMinLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _articleAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), pam.ArticleAttribute.GetLocalized(a => a.Name), pam.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (pam.ValidationMaxLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _articleAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMaxLength.Value < enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), pam.ArticleAttribute.GetLocalized(a => a.Name), pam.ValidationMaxLength.Value));
                        }
                    }
                }
            }

            if (warnings.Any())
                return warnings;

            //validate bundled articles
            var attributeValues = _articleAttributeParser.ParseArticleAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToArticle)
                {
                    if (ignoreNonCombinableAttributes && attributeValue.ArticleAttributeMapping.IsNonCombinable())
                        continue;

                    //associated article (bundle)
                    var associatedArticle = _articleService.GetArticleById(attributeValue.AssociatedArticleId);
                    if (associatedArticle != null)
                    {
                        var totalQty = quantity * attributeValue.Quantity;
                        var associatedArticleWarnings = GetShoppingCartItemWarnings(customer,
                            shoppingCartType, associatedArticle, _storeContext.CurrentStore.Id,
                            "", decimal.Zero, null, null, totalQty, false);
                        foreach (var associatedArticleWarning in associatedArticleWarnings)
                        {
                            var attributeName = attributeValue.ArticleAttributeMapping.ArticleAttribute.GetLocalized(a => a.Name);
                            var attributeValueName = attributeValue.GetLocalized(a => a.Name);
                            warnings.Add(string.Format(
                                _localizationService.GetResource("ShoppingCart.AssociatedAttributeWarning"), 
                                attributeName, attributeValueName, associatedArticleWarning));
                        }
                    }
                    else
                    {
                        warnings.Add(string.Format("Associated article cannot be loaded - {0}", attributeValue.AssociatedArticleId));
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            Article article, string attributesXml)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var warnings = new List<string>();

            //gift cards
            if (article.IsGiftCard)
            {
                string giftCardRecipientName;
                string giftCardRecipientEmail;
                string giftCardSenderName;
                string giftCardSenderEmail;
                string giftCardMessage;
                _articleAttributeParser.GetGiftCardAttribute(attributesXml,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientNameError"));

                

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.SenderNameError"));
 
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental articles
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRentalArticleWarnings(Article article,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");
            
            var warnings = new List<string>();

            if (!article.IsRental)
                return warnings;

            if (!rentalStartDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterStartDate"));
                return warnings;
            }
            if (!rentalEndDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterEndDate"));
                return warnings;
            }
            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateLessEndDate"));
                return warnings;
            }

            //allowed start date should be the future date
            //we should compare rental start date with a store local time
            //but we what if a store works in distinct timezones? how we should handle it? skip it for now
            //we also ignore hours (anyway not supported yet)
            //today
            DateTime nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
            var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
            DateTime todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
            //dates are entered in store timezone (e.g. like in hotels)
            DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
            //but we what if dates should be entered in a customer timezone?
            //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            if (todayDtUtc.CompareTo(startDateUtc) > 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateShouldBeFuture"));
                return warnings;
            }

            return warnings;
        }


        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredArticlesIfEnabled">Automatically add required articles if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a article for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate article attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredArticleWarnings">A value indicating whether we should validate required articles (articles which require other articles to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Article article, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredArticlesIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredArticleWarnings = true,
            bool getRentalWarnings = true)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var warnings = new List<string>();
            
            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(customer, shoppingCartType, article, attributesXml, customerEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetShoppingCartItemAttributeWarnings(customer, shoppingCartType, article, quantity, attributesXml));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, article, attributesXml));

            //required articles
            if (getRequiredArticleWarnings)
                warnings.AddRange(GetRequiredArticleWarnings(customer, shoppingCartType, article, storeId, automaticallyAddRequiredArticlesIfEnabled));

            //rental articles
            if (getRentalWarnings)
                warnings.AddRange(GetRentalArticleWarnings(article, rentalStartDate, rentalEndDate));
            
            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartWarnings(IList<ShoppingCartItem> shoppingCart, 
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartArticles = false;
            bool hasRecurringArticles = false;

            foreach (var sci in shoppingCart)
            {
                var article = sci.Article;
                if (article == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CannotLoadArticle"), sci.ArticleId));
                    return warnings;
                }

                if (article.IsRecurring)
                    hasRecurringArticles = true;
                else
                    hasStandartArticles = true;
            }

            //don't mix standard and recurring articles
            if (hasStandartArticles && hasRecurringArticles)
                warnings.Add(_localizationService.GetResource("ShoppingCart.CannotMixStandardAndAutoshipArticles"));

            //recurring cart validation
            if (hasRecurringArticles)
            {
                int cycleLength;
                RecurringArticleCyclePeriod cyclePeriod;
                int totalCycles;
                string cyclesError = shoppingCart.GetRecurringCycleInfo(_localizationService,
                    out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (validateCheckoutAttributes)
            {
                //selected attributes
                var attributes1 = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributesXml);

                //existing checkout attributes
                var attributes2 = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id);

                //validate conditional attributes only (if specified)
                attributes2 = attributes2.Where(x =>
                {
                    var conditionMet = _checkoutAttributeParser.IsConditionMet(x, checkoutAttributesXml);
                    return !conditionMet.HasValue || conditionMet.Value;
                }).ToList();

                foreach (var a2 in attributes2)
                {
                    if (a2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var a1 in attributes1)
                        {
                            if (a1.Id == a2.Id)
                            {
                                var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                                foreach (string str1 in attributeValuesStr)
                                    if (!String.IsNullOrEmpty(str1.Trim()))
                                    {
                                        found = true;
                                        break;
                                    }
                            }
                        }

                        //if not found
                        if (!found)
                        {
                            if (!string.IsNullOrEmpty(a2.GetLocalized(a => a.TextPrompt)))
                                warnings.Add(a2.GetLocalized(a => a.TextPrompt));
                            else
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.GetLocalized(a => a.Name)));
                        }
                    }
                }

                //now validation rules

                //minimum length
                foreach (var ca in attributes2)
                {
                    if (ca.ValidationMinLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMinLength.Value > enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMinLength.Value));
                            }
                        }
                    }

                    //maximum length
                    if (ca.ValidationMaxLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMaxLength.Value < enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMaxLength.Value));
                            }
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Found shopping cart item</returns>
        public virtual ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Article article,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, 
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (article == null)
                throw new ArgumentNullException("article");

            foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == shoppingCartType))
            {
                if (sci.ArticleId == article.Id)
                {
                    //attributes
                    bool attributesEqual = _articleAttributeParser.AreArticleAttributesEqual(sci.AttributesXml, attributesXml, false, false);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.Article.IsGiftCard)
                    {
                        string giftCardRecipientName1;
                        string giftCardRecipientEmail1;
                        string giftCardSenderName1;
                        string giftCardSenderEmail1;
                        string giftCardMessage1;
                        _articleAttributeParser.GetGiftCardAttribute(attributesXml,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2;
                        string giftCardRecipientEmail2;
                        string giftCardSenderName2;
                        string giftCardSenderEmail2;
                        string giftCardMessage2;
                        _articleAttributeParser.GetGiftCardAttribute(sci.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for articles which require customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    

                    //rental articles
                    bool rentalInfoEqual = true;
                    if (sci.Article.IsRental)
                    {
                        rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                    }

                    //found?
                    if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a article to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="article">Article</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredArticlesIfEnabled">Automatically add required articles if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> AddToCart(Customer customer, Article article,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredArticlesIfEnabled = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (article == null)
                throw new ArgumentNullException("article");

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartType.ShoppingCart && !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart, customer))
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }
            if (shoppingCartType == ShoppingCartType.Wishlist && !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist, customer))
            {
                warnings.Add("Wishlist is disabled");
                return warnings;
            }
            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            _customerService.ResetCheckoutData(customer, storeId);

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            var shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, article, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, article,
                    storeId, attributesXml, 
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, automaticallyAddRequiredArticlesIfEnabled));

                if (!warnings.Any())
                {
                    shoppingCartItem.AttributesXml = attributesXml;
                    shoppingCartItem.Quantity = newQuantity;
                    shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityUpdated(shoppingCartItem);
                }
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, article,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate, 
                    quantity, automaticallyAddRequiredArticlesIfEnabled));
                if (!warnings.Any())
                {
                    //maximum items validation
                    switch (shoppingCartType)
                    {
                        case ShoppingCartType.ShoppingCart:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                                    return warnings;
                                }
                            }
                            break;
                        case ShoppingCartType.Wishlist:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                                    return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    DateTime now = DateTime.UtcNow;
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ShoppingCartType = shoppingCartType,
                        StoreId = storeId,
                        Article = article,
                        AttributesXml = attributesXml,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        RentalStartDateUtc = rentalStartDate,
                        RentalEndDateUtc = rentalEndDate,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    };
                    customer.ShoppingCartItems.Add(shoppingCartItem);
                    _customerService.UpdateCustomer(customer);


                    //updated "HasShoppingCartItems" property used for performance optimization
                    customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityInserted(shoppingCartItem);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">New customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(Customer customer,
            int shoppingCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, 
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var shoppingCartItem = customer.ShoppingCartItems.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
                    _customerService.ResetCheckoutData(customer, shoppingCartItem.StoreId);
                }
                if (quantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartItem.ShoppingCartType,
                        shoppingCartItem.Article, shoppingCartItem.StoreId,
                        attributesXml, customerEnteredPrice, 
                        rentalStartDate, rentalEndDate, quantity, false));
                    if (!warnings.Any())
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = quantity;
                        shoppingCartItem.AttributesXml = attributesXml;
                        shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
                        shoppingCartItem.RentalStartDateUtc = rentalStartDate;
                        shoppingCartItem.RentalEndDateUtc = rentalEndDate;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _customerService.UpdateCustomer(customer);

                        //event notification
                        _eventPublisher.EntityUpdated(shoppingCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        public virtual void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            if (fromCustomer == null)
                throw new ArgumentNullException("fromCustomer");
            if (toCustomer == null)
                throw new ArgumentNullException("toCustomer");

            if (fromCustomer.Id == toCustomer.Id)
                return; //the same customer

            //shopping cart items
            var fromCart = fromCustomer.ShoppingCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                AddToCart(toCustomer, sci.Article, sci.ShoppingCartType, sci.StoreId, 
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            }
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                DeleteShoppingCartItem(sci);
            }

            //copy discount and gift card coupon codes
            if (includeCouponCodes)
            {
                //discount
                foreach (var code in fromCustomer.ParseAppliedDiscountCouponCodes())
                    toCustomer.ApplyDiscountCouponCode(code);

                //gift card
                foreach (var code in fromCustomer.ParseAppliedGiftCardCouponCodes())
                    toCustomer.ApplyGiftCardCouponCode(code);

                //save customer
                _customerService.UpdateCustomer(toCustomer);
                 
            }
        }

        #endregion
    }
}
