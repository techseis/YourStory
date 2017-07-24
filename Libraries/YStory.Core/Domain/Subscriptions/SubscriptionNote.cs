using System;

namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents an subscription note
    /// </summary>
    public partial class SubscriptionNote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the subscription identifier
        /// </summary>
        public int SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the attached file (download) identifier
        /// </summary>
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer can see a note
        /// </summary>
        public bool DisplayToCustomer { get; set; }

        /// <summary>
        /// Gets or sets the date and time of subscription note creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the subscription
        /// </summary>
        public virtual Subscription Subscription { get; set; }
    }

}
