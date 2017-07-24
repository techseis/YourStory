using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class FooterModel : BaseYStoryModel
    {
        public FooterModel()
        {
            Topics = new List<FooterTopicModel>();
        }

        public string StoreName { get; set; }
        public bool WishlistEnabled { get; set; }
        public bool ShoppingCartEnabled { get; set; }
        public bool SitemapEnabled { get; set; }
        public bool NewsEnabled { get; set; }
        public bool BlogEnabled { get; set; }
        public bool CompareArticlesEnabled { get; set; }
        public bool ForumEnabled { get; set; }
        public bool RecentlyViewedArticlesEnabled { get; set; }
        public bool NewArticlesEnabled { get; set; }
        public bool AllowCustomersToApplyForContributorAccount { get; set; }
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool HidePoweredByYourStory { get; set; }

        public int WorkingLanguageId { get; set; }

        public IList<FooterTopicModel> Topics { get; set; }

        #region Nested classes

        public class FooterTopicModel : BaseYStoryEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }

            public bool IncludeInFooterColumn1 { get; set; }
            public bool IncludeInFooterColumn2 { get; set; }
            public bool IncludeInFooterColumn3 { get; set; }
        }

        #endregion
    }
}