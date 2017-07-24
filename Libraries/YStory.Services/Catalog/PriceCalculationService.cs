using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YStory.Core;
using YStory.Core.Caching;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Subscriptions;
using YStory.Services.Catalog.Cache;
using YStory.Services.Customers;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial class PriceCalculationService : IPriceCalculationService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IArticleAttributeParser _articleAttributeParser;
        private readonly IArticleService _articleService;
        private readonly ICacheManager _cacheManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public PriceCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IArticleAttributeParser articleAttributeParser, 
            IArticleService articleService,
            ICacheManager cacheManager,
            ShoppingCartSettings shoppingCartSettings, 
            CatalogSettings catalogSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._publisherService = publisherService;
            this._articleAttributeParser = articleAttributeParser;
            this._articleService = articleService;
            this._cacheManager = cacheManager;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }
        
        #endregion

        #region Nested classes

        [Serializable]
        protected class ArticlePriceForCaching
        {
            public ArticlePriceForCaching()
            {
               
            }

            public decimal Price { get; set; }
            public decimal AppliedDiscountAmount { get; set; }
          
        }
        #endregion

        #region Utilities


       

       

       

     

        #endregion

        #region Methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article,
            Customer customer,
            decimal additionalCharge = decimal.Zero,
            bool includeDiscounts = true,
            int quantity = 1)
        {
            decimal discountAmount;
         
            return GetFinalPrice(article, customer, additionalCharge, includeDiscounts,
                quantity, out discountAmount);
        }
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article, 
            Customer customer,
            decimal additionalCharge, 
            bool includeDiscounts,
            int quantity,
            out decimal discountAmount
           )
        {
            return GetFinalPrice(article, customer,
                additionalCharge, includeDiscounts, quantity,
                null, null,
                out discountAmount);
        }
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental articles)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental articles)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article,
            Customer customer,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount
           )
        {
            return GetFinalPrice(article, customer, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate, out discountAmount);
        }
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="customer">The customer</param>
        /// <param name="overriddenArticlePrice">Overridden article price. If specified, then it'll be used instead of a article price. For example, used with article attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental articles)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental articles)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article, 
            Customer customer,
            decimal? overriddenArticlePrice,
            decimal additionalCharge, 
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount
           )
        {
            if (article == null)
                throw new ArgumentNullException("article");

            discountAmount = decimal.Zero;
          

            var cacheKey = string.Format(PriceCacheEventConsumer.ARTICLE_PRICE_MODEL_KEY,
                article.Id,
                overriddenArticlePrice.HasValue ? overriddenArticlePrice.Value.ToString(CultureInfo.InvariantCulture) : null,
                additionalCharge.ToString(CultureInfo.InvariantCulture),
                includeDiscounts, 
                quantity,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
             var cacheTime = _catalogSettings.CacheArticlePrices ? 60 : 0;
            //we do not cache price for rental articles
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (article.IsRental)
                cacheTime = 0;
            var cachedPrice = _cacheManager.Get(cacheKey, cacheTime, () =>
            {
                var result = new ArticlePriceForCaching();

                //initial price
                decimal price = overriddenArticlePrice.HasValue ? overriddenArticlePrice.Value : article.Price;

                 

                //additional charge
                price = price + additionalCharge;

                //rental articles
                if (article.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price = price * article.GetRentalPeriods(rentalStartDate.Value, rentalEndDate.Value);

                

                if (price < decimal.Zero)
                    price = decimal.Zero;

                result.Price = price;
                return result;
            });

           
            return cachedPrice.Price;
        }



        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts = true)
        {
            decimal discountAmount;
           
            return GetUnitPrice(shoppingCartItem, includeDiscounts,
                out discountAmount);
        }
        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts,
            out decimal discountAmount)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            return GetUnitPrice(shoppingCartItem.Article,
                shoppingCartItem.Customer,
                shoppingCartItem.ShoppingCartType,
                shoppingCartItem.Quantity,
                shoppingCartItem.AttributesXml,
                shoppingCartItem.CustomerEnteredPrice,
                shoppingCartItem.RentalStartDateUtc,
                shoppingCartItem.RentalEndDateUtc,
                includeDiscounts,
                out discountAmount
                );
        }
        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Article atrributes (XML format)</param>
        /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
        /// <param name="rentalStartDate">Rental start date (null for not rental articles)</param>
        /// <param name="rentalEndDate">Rental end date (null for not rental articles)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(Article article,
            Customer customer, 
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts,
            out decimal discountAmount
           )
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (customer == null)
                throw new ArgumentNullException("customer");

            discountAmount = decimal.Zero;
           

            decimal finalPrice;

            var combination = _articleAttributeParser.FindArticleAttributeCombination(article, attributesXml);
            if (combination != null && combination.OverriddenPrice.HasValue)
            {
                finalPrice = GetFinalPrice(article,
                        customer,
                        combination.OverriddenPrice.Value,
                        decimal.Zero,
                        includeDiscounts,
                        quantity,
                        article.IsRental ? rentalStartDate : null,
                        article.IsRental ? rentalEndDate : null,
                        out discountAmount);
            }
            else
            {
                //summarize price of all attributes
                decimal attributesTotalPrice = decimal.Zero;
                var attributeValues = _articleAttributeParser.ParseArticleAttributeValues(attributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        attributesTotalPrice += GetArticleAttributeValuePriceAdjustment(attributeValue);
                    }
                }

                //get price of a article (with previously calculated price of all attributes)
                
                    int qty;
                    
                        qty = quantity;
                    
                    finalPrice = GetFinalPrice(article,
                        customer,
                        attributesTotalPrice,
                        includeDiscounts,
                        qty,
                        article.IsRental ? rentalStartDate : null,
                        article.IsRental ? rentalEndDate : null,
                        out discountAmount);
                
            }
            
            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                finalPrice = RoundingHelper.RoundPrice(finalPrice);

            return finalPrice;
        }
        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotal(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts = true)
        {
            decimal discountAmount;
            
            int? maximumDiscountQty;
            return GetSubTotal(shoppingCartItem, includeDiscounts, out discountAmount, out maximumDiscountQty);
        }
        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="maximumDiscountQty">Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotal(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts,
            out decimal discountAmount,
             
            out int? maximumDiscountQty)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            decimal subTotal;
            maximumDiscountQty = null;

            //unit price
            var unitPrice = GetUnitPrice(shoppingCartItem, includeDiscounts,
                out discountAmount);

                 
                subTotal = unitPrice * shoppingCartItem.Quantity;
            
            return subTotal;
        }


        /// <summary>
        /// Gets the article cost (one item)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>Article cost (one item)</returns>
        public virtual decimal GetArticleCost(Article article, string attributesXml)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            decimal cost = article.ArticleCost;
            var attributeValues = _articleAttributeParser.ParseArticleAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        {
                            //simple attribute
                            cost += attributeValue.Cost;
                        }
                        break;
                    case AttributeValueType.AssociatedToArticle:
                        {
                            //bundled article
                            var associatedArticle = _articleService.GetArticleById(attributeValue.AssociatedArticleId);
                            if (associatedArticle != null)
                                cost += associatedArticle.ArticleCost * attributeValue.Quantity;
                        }
                        break;
                    default:
                        break;
                }
            }

            return cost;
        }



        /// <summary>
        /// Get a price adjustment of a article attribute value
        /// </summary>
        /// <param name="value">Article attribute value</param>
        /// <returns>Price adjustment</returns>
        public virtual decimal GetArticleAttributeValuePriceAdjustment(ArticleAttributeValue value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var adjustment = decimal.Zero;
            switch (value.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    {
                        //simple attribute
                        adjustment = value.PriceAdjustment;
                    }
                    break;
                case AttributeValueType.AssociatedToArticle:
                    {
                        //bundled article
                        var associatedArticle = _articleService.GetArticleById(value.AssociatedArticleId);
                        if (associatedArticle != null)
                        {
                            adjustment = GetFinalPrice(associatedArticle, _workContext.CurrentCustomer, includeDiscounts: true) * value.Quantity;
                        }
                    }
                    break;
                default:
                    break;
            }

            return adjustment;
        }

        #endregion
    }
}
