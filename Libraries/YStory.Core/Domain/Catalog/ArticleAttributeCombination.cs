namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article attribute combination
    /// </summary>
    public partial class ArticleAttributeCombination : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow subscriptions when out of stock
        /// </summary>
        public bool AllowOutOfStockSubscriptions { get; set; }
        
        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the publisher part number
        /// </summary>
        public string PublisherPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>
        public string Gtin { get; set; }

        /// <summary>
        /// Gets or sets the attribute combination price. This way a store owner can override the default article price when this attribute combination is added to the cart. For example, you can give a discount this way.
        /// </summary>
        public decimal? OverriddenPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        public int NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }

    }
}
