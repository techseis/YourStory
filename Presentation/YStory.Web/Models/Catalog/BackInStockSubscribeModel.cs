using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class BackInStockSubscribeModel : BaseYStoryModel
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleSeName { get; set; }

        public bool IsCurrentCustomerRegistered { get; set; }
        public bool SubscriptionAllowed { get; set; }
        public bool AlreadySubscribed { get; set; }

        public int MaximumBackInStockSubscriptions { get; set; }
        public int CurrentNumberOfBackInStockSubscriptions { get; set; }
    }
}