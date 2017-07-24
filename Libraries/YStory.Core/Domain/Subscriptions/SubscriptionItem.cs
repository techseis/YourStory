using System;
using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents an subscription item
    /// </summary>
    public partial class SubscriptionItem : BaseEntity
    {
       

        /// <summary>
        /// Gets or sets the subscription item identifier
        /// </summary>
        public Guid SubscriptionItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (incl tax)
        /// </summary>
        public decimal UnitPriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (excl tax)
        /// </summary>
        public decimal UnitPriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (incl tax)
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (excl tax)
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (incl tax)
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (excl tax)
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the original cost of this subscription item (when an subscription was placed), qty 1
        /// </summary>
        public decimal OriginalArticleCost { get; set; }

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public string AttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the article attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }
        
        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable article)
        /// </summary>
        public int? LicenseDownloadId { get; set; }

        /// <summary>
        /// Gets or sets the total weight of one item
        /// It's nullable for compatibility with the previous version of yourStory where was no such property
        /// </summary>
        public decimal? ItemWeight { get; set; }

        /// <summary>
        /// Gets or sets the rental article start date (null if it's not a rental article)
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental article end date (null if it's not a rental article)
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        /// Gets the subscription
        /// </summary>
        public virtual Subscription Subscription { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }

        
    }
}
