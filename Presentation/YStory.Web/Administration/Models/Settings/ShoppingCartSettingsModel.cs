using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class ShoppingCartSettingsModel : BaseYStoryModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }



        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingArticle")]
        public bool DisplayCartAfterAddingArticle { get; set; }
        public bool DisplayCartAfterAddingArticle_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingArticle")]
        public bool DisplayWishlistAfterAddingArticle { get; set; }
        public bool DisplayWishlistAfterAddingArticle_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumShoppingCartItems")]
        public int MaximumShoppingCartItems { get; set; }
        public bool MaximumShoppingCartItems_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumWishlistItems")]
        public int MaximumWishlistItems { get; set; }
        public bool MaximumWishlistItems_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist")]
        public bool AllowOutOfStockItemsToBeAddedToWishlist { get; set; }
        public bool AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart")]
        public bool MoveItemsFromWishlistToCart { get; set; }
        public bool MoveItemsFromWishlistToCart_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CartsSharedBetweenStores")]
        public bool CartsSharedBetweenStores { get; set; }
        public bool CartsSharedBetweenStores_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowArticleImagesOnShoppingCart")]
        public bool ShowArticleImagesOnShoppingCart { get; set; }
        public bool ShowArticleImagesOnShoppingCart_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowArticleImagesOnWishList")]
        public bool ShowArticleImagesOnWishList { get; set; }
        public bool ShowArticleImagesOnWishList_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowDiscountBox")]
        public bool ShowDiscountBox { get; set; }
        public bool ShowDiscountBox_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowGiftCardBox")]
        public bool ShowGiftCardBox { get; set; }
        public bool ShowGiftCardBox_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CrossSellsNumber")]
        public int CrossSellsNumber { get; set; }
        public bool CrossSellsNumber_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.EmailWishlistEnabled")]
        public bool EmailWishlistEnabled { get; set; }
        public bool EmailWishlistEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist")]
        public bool AllowAnonymousUsersToEmailWishlist { get; set; }
        public bool AllowAnonymousUsersToEmailWishlist_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartEnabled")]
        public bool MiniShoppingCartEnabled { get; set; }
        public bool MiniShoppingCartEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowArticleImagesInMiniShoppingCart")]
        public bool ShowArticleImagesInMiniShoppingCart { get; set; }
        public bool ShowArticleImagesInMiniShoppingCart_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartArticleNumber")]
        public int MiniShoppingCartArticleNumber { get; set; }
        public bool MiniShoppingCartArticleNumber_OverrideForStore { get; set; }
        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing")]
        public bool AllowCartItemEditing { get; set; }
        public bool AllowCartItemEditing_OverrideForStore { get; set; }
    }
}