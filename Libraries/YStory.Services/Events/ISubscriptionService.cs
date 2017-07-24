using System.Collections.Generic;

namespace YStory.Services.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public interface ISubscriptionService1
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}
