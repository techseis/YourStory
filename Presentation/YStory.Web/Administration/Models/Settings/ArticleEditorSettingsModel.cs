using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class ArticleEditorSettingsModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Id")]
        public bool Id { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleType")]
        public bool ArticleType { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleTemplate")]
        public bool ArticleTemplate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AdminComment")]
        public bool AdminComment { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Contributor")]
        public bool Contributor { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Stores")]
        public bool Stores { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ACL")]
        public bool ACL { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DisplaySubscription")]
        public bool DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleTags")]
        public bool ArticleTags { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.PublisherPartNumber")]
        public bool PublisherPartNumber { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.GTIN")]
        public bool GTIN { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleCost")]
        public bool ArticleCost { get; set; }

     

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Discounts")]
        public bool Discounts { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DisableBuyButton")]
        public bool DisableBuyButton { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DisableWishlistButton")]
        public bool DisableWishlistButton { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AvailableForPreSubscription")]
        public bool AvailableForPreSubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.CallForPrice")]
        public bool CallForPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.OldPrice")]
        public bool OldPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.CustomerEntersPrice")]
        public bool CustomerEntersPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.PAngV")]
        public bool PAngV { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.RequireOtherArticlesAddedToTheCart")]
        public bool RequireOtherArticlesAddedToTheCart { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.IsGiftCard")]
        public bool IsGiftCard { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DownloadableArticle")]
        public bool DownloadableArticle { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.RecurringArticle")]
        public bool RecurringArticle { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.IsRental")]
        public bool IsRental { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.FreeShipping")]
        public bool FreeShipping { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ShipSeparately")]
        public bool ShipSeparately { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AdditionalShippingCharge")]
        public bool AdditionalShippingCharge { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DeliveryDate")]
        public bool DeliveryDate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.TelecommunicationsBroadcastingElectronicServices")]
        public bool TelecommunicationsBroadcastingElectronicServices { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleAvailabilityRange")]
        public bool ArticleAvailabilityRange { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.UseMultipleWarehouses")]
        public bool UseMultipleWarehouses { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Warehouse")]
        public bool Warehouse { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DisplayStockAvailability")]
        public bool DisplayStockAvailability { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.DisplayStockQuantity")]
        public bool DisplayStockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MinimumStockQuantity")]
        public bool MinimumStockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.LowStockActivity")]
        public bool LowStockActivity { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.NotifyAdminForQuantityBelow")]
        public bool NotifyAdminForQuantityBelow { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Backsubscriptions")]
        public bool Backsubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AllowBackInStockSubscriptions")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MinimumCartQuantity")]
        public bool MinimumCartQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MaximumCartQuantity")]
        public bool MaximumCartQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AllowedQuantities")]
        public bool AllowedQuantities { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AllowAddingOnlyExistingAttributeCombinations")]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.NotReturnable")]
        public bool NotReturnable { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Weight")]
        public bool Weight { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Dimensions")]
        public bool Dimensions { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AvailableStartDate")]
        public bool AvailableStartDate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.AvailableEndDate")]
        public bool AvailableEndDate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MarkAsNew")]
        public bool MarkAsNew { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MarkAsNewStartDate")]
        public bool MarkAsNewStartDate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.MarkAsNewEndDate")]
        public bool MarkAsNewEndDate { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Published")]
        public bool Published { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.CreatedOn")]
        public bool CreatedOn { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.UpdatedOn")]
        public bool UpdatedOn { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.RelatedArticles")]
        public bool RelatedArticles { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.CrossSellsArticles")]
        public bool CrossSellsArticles { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Seo")]
        public bool Seo { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.PurchasedWithSubscriptions")]
        public bool PurchasedWithSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.OneColumnArticlePage")]
        public bool OneColumnArticlePage { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.ArticleAttributes")]
        public bool ArticleAttributes { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.SpecificationAttributes")]
        public bool SpecificationAttributes { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.Publishers")]
        public bool Publishers { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ArticleEditor.StockQuantityHistory")]
        public bool StockQuantityHistory { get; set; }
    }
}