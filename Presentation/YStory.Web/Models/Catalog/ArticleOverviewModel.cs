using System;
using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Media;

namespace YStory.Web.Models.Catalog
{
    public partial class ArticleOverviewModel : BaseYStoryEntityModel
    {
        public ArticleOverviewModel()
        {
            ArticlePrice = new ArticlePriceModel();
            DefaultPictureModel = new PictureModel();
            SpecificationAttributeModels = new List<ArticleSpecificationModel>();
            ReviewOverviewModel = new ArticleReviewOverviewModel();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }

        public string Sku { get; set; }

        public ArticleType ArticleType { get; set; }

        public bool MarkAsNew { get; set; }

        //price
        public ArticlePriceModel ArticlePrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //specification attributes
        public IList<ArticleSpecificationModel> SpecificationAttributeModels { get; set; }
        //price
        public ArticleReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        public partial class ArticlePriceModel : BaseYStoryModel
        {
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }
            public bool DisableAddToCompareListButton { get; set; }

            public bool AvailableForPreSubscription { get; set; }
            public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
        }

		#endregion
    }
}