using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the interface of the article model factory
    /// </summary>
    public partial interface IArticleModelFactory
    {
        /// <summary>
        /// Get the article template view path
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>View path</returns>
        string PrepareArticleTemplateViewPath(Article article);

        /// <summary>
        /// Prepare the article overview models
        /// </summary>
        /// <param name="articles">Collection of articles</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="articleThumbPictureSize">Article thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of article overview model</returns>
        IEnumerable<ArticleOverviewModel> PrepareArticleOverviewModels(IEnumerable<Article> articles,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? articleThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        /// <summary>
        /// Prepare the article details model
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedArticle">Whether the article is associated</param>
        /// <returns>Article details model</returns>
        ArticleDetailsModel PrepareArticleDetailsModel(Article article, ShoppingCartItem updatecartitem = null, bool isAssociatedArticle = false);

        /// <summary>
        /// Prepare the article reviews model
        /// </summary>
        /// <param name="model">Article reviews model</param>
        /// <param name="article">Article</param>
        /// <returns>Article reviews model</returns>
        ArticleReviewsModel PrepareArticleReviewsModel(ArticleReviewsModel model, Article article);

        /// <summary>
        /// Prepare the customer article reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer article reviews model</returns>
        CustomerArticleReviewsModel PrepareCustomerArticleReviewsModel(int? page);

        /// <summary>
        /// Prepare the article email a friend model
        /// </summary>
        /// <param name="model">Article email a friend model</param>
        /// <param name="article">Article</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>article email a friend model</returns>
        ArticleEmailAFriendModel PrepareArticleEmailAFriendModel(ArticleEmailAFriendModel model, Article article, bool excludeProperties);

        /// <summary>
        /// Prepare the article specification models
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>List of article specification model</returns>
        IList<ArticleSpecificationModel> PrepareArticleSpecificationModel(Article article);
    }
}
