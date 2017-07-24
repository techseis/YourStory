using YStory.Core.Domain.Subscriptions;
using YStory.Web.Models.Subscription;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the interface of the return request model factory
    /// </summary>
    public partial interface IReturnRequestModelFactory
    {
        /// <summary>
        /// Prepare the subscription item model
        /// </summary>
        /// <param name="subscriptionItem">Subscription item</param>
        /// <returns>Subscription item model</returns>
        SubmitReturnRequestModel.SubscriptionItemModel PrepareSubmitReturnRequestSubscriptionItemModel(SubscriptionItem subscriptionItem);

        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="subscription">Subscription</param>
        /// <returns>Submit return request model</returns>
        SubmitReturnRequestModel PrepareSubmitReturnRequestModel(SubmitReturnRequestModel model, Subscription subscription);

        /// <summary>
        /// Prepare the customer return requests model
        /// </summary>
        /// <returns>Customer return requests model</returns>
        CustomerReturnRequestsModel PrepareCustomerReturnRequestsModel();
    }
}
