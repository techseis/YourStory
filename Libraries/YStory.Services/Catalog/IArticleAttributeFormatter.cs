using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article attribute formatter interface
    /// </summary>
    public partial interface IArticleAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(Article article, string attributesXml);

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
        string FormatAttributes(Article article, string attributesXml,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderArticleAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true);
    }
}
