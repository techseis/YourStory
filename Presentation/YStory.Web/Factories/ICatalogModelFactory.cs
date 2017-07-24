using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Contributors;
using YStory.Web.Models.Catalog;

namespace YStory.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Category model</returns>
        CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        string PrepareCategoryTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentArticleId">Current article identifier</param>
        /// <returns>Category navigation model</returns>
        CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId,
            int currentArticleId);

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        TopMenuModel PrepareTopMenuModel();

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        List<CategoryModel> PrepareHomepageCategoryModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null);

        #endregion

        #region Publishers

        /// <summary>
        /// Prepare publisher model
        /// </summary>
        /// <param name="publisher">Publisher identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Publisher model</returns>
        PublisherModel PreparePublisherModel(Publisher publisher, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare publisher template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Publisher template view path</returns>
        string PreparePublisherTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare publisher all models
        /// </summary>
        /// <returns>List of publisher models</returns>
        List<PublisherModel> PreparePublisherAllModels();

        /// <summary>
        /// Prepare publisher navigation model
        /// </summary>
        /// <param name="currentPublisherId">Current publisher identifier</param>
        /// <returns>Publisher navigation model</returns>
        PublisherNavigationModel PreparePublisherNavigationModel(int currentPublisherId);

        #endregion

        #region Contributors

        /// <summary>
        /// Prepare contributor model
        /// </summary>
        /// <param name="contributor">Contributor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Contributor model</returns>
        ContributorModel PrepareContributorModel(Contributor contributor, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare contributor all models
        /// </summary>
        /// <returns>List of contributor models</returns>
        List<ContributorModel> PrepareContributorAllModels();

        /// <summary>
        /// Prepare contributor navigation model
        /// </summary>
        /// <returns>Contributor navigation model</returns>
        ContributorNavigationModel PrepareContributorNavigationModel();

        #endregion

        #region Article tags

        /// <summary>
        /// Prepare popular article tags model
        /// </summary>
        /// <returns>Article tags model</returns>
        PopularArticleTagsModel PreparePopularArticleTagsModel();

        /// <summary>
        /// Prepare articles by tag model
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Articles by tag model</returns>
        ArticlesByTagModel PrepareArticlesByTagModel(ArticleTag articleTag,
            CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare article tags all model
        /// </summary>
        /// <returns>Popular article tags model</returns>
        PopularArticleTagsModel PrepareArticleTagsAllModel();

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Search model</returns>
        SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>Search box model</returns>
        SearchBoxModel PrepareSearchBoxModel();

        #endregion
    }
}
