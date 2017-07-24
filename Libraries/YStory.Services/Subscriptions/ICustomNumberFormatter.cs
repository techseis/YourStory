using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Subscriptions
{
    public partial interface ICustomNumberFormatter
    {
        string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest);

        string GenerateSubscriptionCustomNumber(Subscription subscription);
    }
}