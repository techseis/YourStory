using YStory.Core.Domain.Subscriptions;
using YStory.Web.Models.Subscription;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the interface of the subscription model factory
    /// </summary>
    public partial interface ISubscriptionModelFactory
    {
        /// <summary>
        /// Prepare the customer subscription list model
        /// </summary>
        /// <returns>Customer subscription list model</returns>
        CustomerSubscriptionListModel PrepareCustomerSubscriptionListModel();

        /// <summary>
        /// Prepare the subscription details model
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>Subscription details model</returns>
        SubscriptionDetailsModel PrepareSubscriptionDetailsModel(Subscription subscription);

       

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page);
    }
}
