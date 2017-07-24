using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Services.Directory;
using YStory.Services.Localization;
 

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ArticleExtensions
    {
         
        
        /// <summary>
        /// Finds a related article item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="articleId2">The second article identifier</param>
        /// <returns>Related article</returns>
        public static RelatedArticle FindRelatedArticle(this IList<RelatedArticle> source,
            int articleId1, int articleId2)
        {
            foreach (RelatedArticle relatedArticle in source)
                if (relatedArticle.ArticleId1 == articleId1 && relatedArticle.ArticleId2 == articleId2)
                    return relatedArticle;
            return null;
        }

        /// <summary>
        /// Finds a cross-sell article item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="articleId2">The second article identifier</param>
        /// <returns>Cross-sell article</returns>
        public static CrossSellArticle FindCrossSellArticle(this IList<CrossSellArticle> source,
            int articleId1, int articleId2)
        {
            foreach (CrossSellArticle crossSellArticle in source)
                if (crossSellArticle.ArticleId1 == articleId1 && crossSellArticle.ArticleId2 == articleId2)
                    return crossSellArticle;
            return null;
        }

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Selected article attributes in XML format (if specified)</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="articleAttributeParser">Article attribute parser</param>
        /// <param name="dateRangeService">Date range service</param>
        /// <returns>The stock message</returns>
        public static string FormatStockMessage(this Article article, string attributesXml,
            ILocalizationService localizationService, IArticleAttributeParser articleAttributeParser)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            if (articleAttributeParser == null)
                throw new ArgumentNullException("articleAttributeParser");

            

            string stockMessage = string.Empty;

             
            return stockMessage;
        }
        
        /// <summary>
        /// Indicates whether a article tag exists
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <returns>Result</returns>
        public static bool ArticleTagExists(this Article article,
            int articleTagId)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            bool result = article.ArticleTags.ToList().Find(pt => pt.Id == articleTagId) != null;
            return result;
        }

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Result</returns>
        public static int[] ParseAllowedQuantities(this Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            var result = new List<int>();
             

            return result.ToArray();
        }

        /// <summary>
        /// Get total quantity
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="useReservedQuantity">
        /// A value indicating whether we should consider "Reserved Quantity" property 
        /// when "multiple warehouses" are used
        /// </param>
        /// <param name="warehouseId">
        /// Warehouse identifier. Used to limit result to certain warehouse.
        /// Used only with "multiple warehouses" enabled.
        /// </param>
        /// <returns>Result</returns>
        public static int GetTotalStockQuantity(this Article article, 
            bool useReservedQuantity = true, int warehouseId = 0)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            
            
            return 0;
        }

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        public static int GetRentalPeriods(this Article article,
            DateTime startDate, DateTime endDate)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (!article.IsRental)
                return 1;

            if (startDate.CompareTo(endDate) >= 0)
                return 1;

            int totalPeriods;
            switch (article.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                {
                    var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                    int configuredPeriodDays = article.RentalPriceLength;
                    totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent/configuredPeriodDays));
                }
                    break;
                case RentalPricePeriod.Weeks:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        int configuredPeriodDays = 7 * article.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }
                    break;
                case RentalPricePeriod.Months:
                    {
                        //Source: http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
                        var totalMonthsToRent = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
                        if (startDate.AddMonths(totalMonthsToRent) < endDate)
                        {
                            //several days added (not full month)
                            totalMonthsToRent++;
                        }
                        int configuredPeriodMonths = article.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
                    }
                    break;
                case RentalPricePeriod.Years:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        int configuredPeriodDays = 365 * article.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }
                    break;
                default:
                    throw new Exception("Not supported rental period");
            }

            return totalPeriods;
        }



        /// <summary>
        /// Gets SKU, Publisher part number and GTIN
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeParser">Article attribute service (used when attributes are specified)</param>
        /// <param name="sku">SKU</param>
        /// <param name="publisherPartNumber">Publisher part number</param>
        /// <param name="gtin">GTIN</param>
        private static void GetSkuMpnGtin(this Article article, string attributesXml, IArticleAttributeParser articleAttributeParser,
            out string sku, out string publisherPartNumber, out string gtin)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            sku = null;
            publisherPartNumber = null;
            gtin = null;

            

            if (String.IsNullOrEmpty(sku))
                sku = article.Sku;
            if (String.IsNullOrEmpty(publisherPartNumber))
                publisherPartNumber = article.PublisherPartNumber;
            if (String.IsNullOrEmpty(gtin))
                gtin = article.Gtin;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeParser">Article attribute service (used when attributes are specified)</param>
        /// <returns>SKU</returns>
        public static string FormatSku(this Article article, string attributesXml = null, IArticleAttributeParser articleAttributeParser = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            string sku;
            string publisherPartNumber;
            string gtin;

            article.GetSkuMpnGtin(attributesXml, articleAttributeParser,
                out sku, out publisherPartNumber, out gtin);

            return sku;
        }

        /// <summary>
        /// Formats publisher part number
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeParser">Article attribute service (used when attributes are specified)</param>
        /// <returns>Publisher part number</returns>
        public static string FormatMpn(this Article article, string attributesXml = null, IArticleAttributeParser articleAttributeParser = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            string sku;
            string publisherPartNumber;
            string gtin;

            article.GetSkuMpnGtin(attributesXml, articleAttributeParser,
                out sku, out publisherPartNumber, out gtin);

            return publisherPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeParser">Article attribute service (used when attributes are specified)</param>
        /// <returns>GTIN</returns>
        public static string FormatGtin(this Article article, string attributesXml = null, IArticleAttributeParser articleAttributeParser = null)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            string sku;
            string publisherPartNumber;
            string gtin;

            article.GetSkuMpnGtin(attributesXml, articleAttributeParser,
                out sku, out publisherPartNumber, out gtin);

            return gtin;
        }

        /// <summary>
        /// Formats start/end date for rental article
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public static string FormatRentalDate(this Article article, DateTime date)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (!article.IsRental)
                return null;

            return date.ToShortDateString();
        }

        /// <summary>
        /// Format base price (PAngV)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="articlePrice">Article price (in primary currency). Pass null if you want to use a default produce price</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="measureService">Measure service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <returns>Base price</returns>
        public static string FormatBasePrice(this Article article, decimal? articlePrice, ILocalizationService localizationService,
            IMeasureService measureService, ICurrencyService currencyService,
            IWorkContext workContext, IPriceFormatter priceFormatter)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            
            if (measureService == null)
                throw new ArgumentNullException("measureService");

            if (currencyService == null)
                throw new ArgumentNullException("currencyService");

            if (workContext == null)
                throw new ArgumentNullException("workContext");

            if (priceFormatter == null)
                throw new ArgumentNullException("priceFormatter");

           

            articlePrice = articlePrice.HasValue ? articlePrice.Value : article.Price;

            decimal basePrice = articlePrice.Value;
            decimal basePriceInCurrentCurrency = currencyService.ConvertFromPrimaryStoreCurrency(basePrice, workContext.WorkingCurrency);
            string basePriceStr = priceFormatter.FormatPrice(basePriceInCurrentCurrency, true, false);

            var result = string.Format(localizationService.GetResource("Articles.BasePrice"),
                basePriceStr);
            return result;
        }
    }
}
