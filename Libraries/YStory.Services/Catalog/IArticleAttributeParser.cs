using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article attribute parser interface
    /// </summary>
    public partial interface IArticleAttributeParser
    {
        #region Article attributes

        /// <summary>
        /// Gets selected article attribute mappings
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected article attribute mappings</returns>
        IList<ArticleAttributeMapping> ParseArticleAttributeMappings(string attributesXml);

        /// <summary>
        /// Get article attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeMappingId">Article attribute mapping identifier; pass 0 to load all values</param>
        /// <returns>Article attribute values</returns>
        IList<ArticleAttributeValue> ParseArticleAttributeValues(string attributesXml, int articleAttributeMappingId = 0);

        /// <summary>
        /// Gets selected article attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeMappingId">Article attribute mapping identifier</param>
        /// <returns>Article attribute values</returns>
        IList<string> ParseValues(string attributesXml, int articleAttributeMappingId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <param name="value">Value</param>
        /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToArticle to specify the quantity entered by the customer)</param>
        /// <returns>Updated result (XML format)</returns>
        string AddArticleAttribute(string attributesXml, ArticleAttributeMapping articleAttributeMapping, string value, int? quantity = null);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveArticleAttribute(string attributesXml, ArticleAttributeMapping articleAttributeMapping);

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributesXml1">The attributes of the first article</param>
        /// <param name="attributesXml2">The attributes of the second article</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreQuantity">A value indicating whether we should ignore the quantity of attribute value entered by the customer</param>
        /// <returns>Result</returns>
        bool AreArticleAttributesEqual(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="pam">Article attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>Result</returns>
        bool? IsConditionMet(ArticleAttributeMapping pam, string selectedAttributesXml);

        /// <summary>
        /// Finds a article attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Found article attribute combination</returns>
        ArticleAttributeCombination FindArticleAttributeCombination(Article article,
            string attributesXml, bool ignoreNonCombinableAttributes = true);

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Attribute combinations in XML format</returns>
        IList<string> GenerateAllCombinations(Article article, bool ignoreNonCombinableAttributes = false);

        #endregion

        #region Gift card attributes

        /// <summary>
        /// Add gift card attrbibutes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        /// <returns>Attributes</returns>
        string AddGiftCardAttribute(string attributesXml, string recipientName,
            string recipientEmail, string senderName, string senderEmail, string giftCardMessage);

        /// <summary>
        /// Get gift card attrbibutes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        void GetGiftCardAttribute(string attributesXml, out string recipientName,
            out string recipientEmail, out string senderName,
            out string senderEmail, out string giftCardMessage);

        #endregion
    }
}
