using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Media;

namespace YStory.Web.Models.ShoppingCart
{
    public partial class MiniShoppingCartModel : BaseYStoryModel
    {
        public MiniShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
        }

        public IList<ShoppingCartItemModel> Items { get; set; }
        public int TotalArticles { get; set; }
        public string SubTotal { get; set; }
        public bool DisplayShoppingCartButton { get; set; }
        public bool DisplayCheckoutButton { get; set; }
        public bool CurrentCustomerIsGuest { get; set; }
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool ShowArticleImages { get; set; }


        #region Nested Classes

        public partial class ShoppingCartItemModel : BaseYStoryEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
            }

            public int ArticleId { get; set; }

            public string ArticleName { get; set; }

            public string ArticleSeName { get; set; }

            public int Quantity { get; set; }

            public string UnitPrice { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }
        }

        #endregion
    }
}