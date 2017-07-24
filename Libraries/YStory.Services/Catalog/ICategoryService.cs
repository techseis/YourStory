using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ICategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteCategory(Category category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Category> GetAllCategories(string categoryName = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false, bool includeAllLevels = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false);
                
        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertCategory(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateCategory(Category category);
        
        /// <summary>
        /// Deletes a article category mapping
        /// </summary>
        /// <param name="articleCategory">Article category</param>
        void DeleteArticleCategory(ArticleCategory articleCategory);

        /// <summary>
        /// Gets article category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article a category mapping collection</returns>
        IPagedList<ArticleCategory> GetArticleCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a article category mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Article category mapping collection</returns>
        IList<ArticleCategory> GetArticleCategoriesByArticleId(int articleId, bool showHidden = false);
        /// <summary>
        /// Gets a article category mapping collection
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Article category mapping collection</returns>
        IList<ArticleCategory> GetArticleCategoriesByArticleId(int articleId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a article category mapping 
        /// </summary>
        /// <param name="articleCategoryId">Article category mapping identifier</param>
        /// <returns>Article category mapping</returns>
        ArticleCategory GetArticleCategoryById(int articleCategoryId);

        /// <summary>
        /// Inserts a article category mapping
        /// </summary>
        /// <param name="articleCategory">>Article category mapping</param>
        void InsertArticleCategory(ArticleCategory articleCategory);

        /// <summary>
        /// Updates the article category mapping 
        /// </summary>
        /// <param name="articleCategory">>Article category mapping</param>
        void UpdateArticleCategory(ArticleCategory articleCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        string[] GetNotExistingCategories(string[] categoryNames);

        /// <summary>
        /// Get category IDs for articles
        /// </summary>
        /// <param name="articleIds">Articles IDs</param>
        /// <returns>Category IDs for articles</returns>
        IDictionary<int, int[]> GetArticleCategoryIds(int[] articleIds);
    }
}
