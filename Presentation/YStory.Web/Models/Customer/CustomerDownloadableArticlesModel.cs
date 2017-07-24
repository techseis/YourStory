using System;
using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Customer
{
    public partial class CustomerDownloadableArticlesModel : BaseYStoryModel
    {
        public CustomerDownloadableArticlesModel()
        {
            Items = new List<DownloadableArticlesModel>();
        }

        public IList<DownloadableArticlesModel> Items { get; set; }

        #region Nested classes
        public partial class DownloadableArticlesModel : BaseYStoryModel
        {
            public Guid SubscriptionItemGuid { get; set; }

            public int SubscriptionId { get; set; }
            public string CustomSubscriptionNumber { get; set; }

            public int ArticleId { get; set; }
            public string ArticleName { get; set; }
            public string ArticleSeName { get; set; }
            public string ArticleAttributes { get; set; }

            public int DownloadId { get; set; }
            public int LicenseId { get; set; }

            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }

    public partial class UserAgreementModel : BaseYStoryModel
    {
        public Guid SubscriptionItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}