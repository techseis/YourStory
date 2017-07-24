using YStory.Core.Domain.Localization;

namespace YStory.Core.Domain.Subscriptions
{
    /// <summary>
    /// Represents a return request action
    /// </summary>
    public partial class ReturnRequestAction : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }
    }
}
