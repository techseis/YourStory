using System.Collections.Generic;
using System.Linq;
using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Subscriptions
{
    /// <summary>
    /// Place subscription result
    /// </summary>
    public partial class PlaceSubscriptionResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PlaceSubscriptionResult() 
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (!this.Errors.Any()); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
        
        /// <summary>
        /// Gets or sets the placed subscription
        /// </summary>
        public Subscription PlacedSubscription { get; set; }
    }
}
