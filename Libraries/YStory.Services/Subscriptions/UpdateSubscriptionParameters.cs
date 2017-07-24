using System.Collections.Generic;
using YStory.Core.Domain.Subscriptions;
 

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Parameters for the updating subscription totals
    /// </summary>
    public class UpdateSubscriptionParameters
    {
        public UpdateSubscriptionParameters()
        {
            Warnings = new List<string>();
       
        }

        /// <summary>
        /// The updated subscription
        /// </summary>
        public Subscription UpdatedSubscription { get; set; }

        /// <summary>
        /// The updated subscription item
        /// </summary>
        public SubscriptionItem UpdatedSubscriptionItem { get; set; }

        /// <summary>
        /// The price of item with tax
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// The price of item without tax
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The amount of discount with tax
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// The amount of discount without tax
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Subtotal of item with tax
        /// </summary>
        public decimal SubTotalInclTax { get; set; }

        /// <summary>
        /// Subtotal of item without tax
        /// </summary>
        public decimal SubTotalExclTax { get; set; }

        /// <summary>
        /// Warnings
        /// </summary>
        public List<string> Warnings { get; set; }

       
    }
}
