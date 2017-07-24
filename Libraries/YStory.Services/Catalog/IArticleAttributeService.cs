using System.Collections.Generic;
using YStory.Core;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article attribute service interface
    /// </summary>
    public partial interface IArticleAttributeService
    {
        #region Article attributes

        /// <summary>
        /// Deletes a article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        void DeleteArticleAttribute(ArticleAttribute articleAttribute);

        /// <summary>
        /// Gets all article attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Article attributes</returns>
        IPagedList<ArticleAttribute> GetAllArticleAttributes(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a article attribute 
        /// </summary>
        /// <param name="articleAttributeId">Article attribute identifier</param>
        /// <returns>Article attribute </returns>
        ArticleAttribute GetArticleAttributeById(int articleAttributeId);

        /// <summary>
        /// Inserts a article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        void InsertArticleAttribute(ArticleAttribute articleAttribute);

        /// <summary>
        /// Updates the article attribute
        /// </summary>
        /// <param name="articleAttribute">Article attribute</param>
        void UpdateArticleAttribute(ArticleAttribute articleAttribute);

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        int[] GetNotExistingAttributes(int[] attributeId);

        #endregion

        #region Article attributes mappings

        /// <summary>
        /// Deletes a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        void DeleteArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping);

        /// <summary>
        /// Gets article attribute mappings by article identifier
        /// </summary>
        /// <param name="articleId">The article identifier</param>
        /// <returns>Article attribute mapping collection</returns>
        IList<ArticleAttributeMapping> GetArticleAttributeMappingsByArticleId(int articleId);

        /// <summary>
        /// Gets a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMappingId">Article attribute mapping identifier</param>
        /// <returns>Article attribute mapping</returns>
        ArticleAttributeMapping GetArticleAttributeMappingById(int articleAttributeMappingId);

        /// <summary>
        /// Inserts a article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">The article attribute mapping</param>
        void InsertArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping);

        /// <summary>
        /// Updates the article attribute mapping
        /// </summary>
        /// <param name="articleAttributeMapping">The article attribute mapping</param>
        void UpdateArticleAttributeMapping(ArticleAttributeMapping articleAttributeMapping);

        #endregion

        #region Article attribute values

        /// <summary>
        /// Deletes a article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">Article attribute value</param>
        void DeleteArticleAttributeValue(ArticleAttributeValue articleAttributeValue);

        /// <summary>
        /// Gets article attribute values by article attribute mapping identifier
        /// </summary>
        /// <param name="articleAttributeMappingId">The article attribute mapping identifier</param>
        /// <returns>Article attribute values</returns>
        IList<ArticleAttributeValue> GetArticleAttributeValues(int articleAttributeMappingId);

        /// <summary>
        /// Gets a article attribute value
        /// </summary>
        /// <param name="articleAttributeValueId">Article attribute value identifier</param>
        /// <returns>Article attribute value</returns>
        ArticleAttributeValue GetArticleAttributeValueById(int articleAttributeValueId);

        /// <summary>
        /// Inserts a article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">The article attribute value</param>
        void InsertArticleAttributeValue(ArticleAttributeValue articleAttributeValue);

        /// <summary>
        /// Updates the article attribute value
        /// </summary>
        /// <param name="articleAttributeValue">The article attribute value</param>
        void UpdateArticleAttributeValue(ArticleAttributeValue articleAttributeValue);

        #endregion

        #region Predefined article attribute values

        /// <summary>
        /// Deletes a predefined article attribute value
        /// </summary>
        /// <param name="ppav">Predefined article attribute value</param>
        void DeletePredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav);

        /// <summary>
        /// Gets predefined article attribute values by article attribute identifier
        /// </summary>
        /// <param name="articleAttributeId">The article attribute identifier</param>
        /// <returns>Article attribute mapping collection</returns>
        IList<PredefinedArticleAttributeValue> GetPredefinedArticleAttributeValues(int articleAttributeId);

        /// <summary>
        /// Gets a predefined article attribute value
        /// </summary>
        /// <param name="id">Predefined article attribute value identifier</param>
        /// <returns>Predefined article attribute value</returns>
        PredefinedArticleAttributeValue GetPredefinedArticleAttributeValueById(int id);

        /// <summary>
        /// Inserts a predefined article attribute value
        /// </summary>
        /// <param name="ppav">The predefined article attribute value</param>
        void InsertPredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav);

        /// <summary>
        /// Updates the predefined article attribute value
        /// </summary>
        /// <param name="ppav">The predefined article attribute value</param>
        void UpdatePredefinedArticleAttributeValue(PredefinedArticleAttributeValue ppav);

        #endregion

        #region Article attribute combinations

        /// <summary>
        /// Deletes a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        void DeleteArticleAttributeCombination(ArticleAttributeCombination combination);

        /// <summary>
        /// Gets all article attribute combinations
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        /// <returns>Article attribute combinations</returns>
        IList<ArticleAttributeCombination> GetAllArticleAttributeCombinations(int articleId);

        /// <summary>
        /// Gets a article attribute combination
        /// </summary>
        /// <param name="articleAttributeCombinationId">Article attribute combination identifier</param>
        /// <returns>Article attribute combination</returns>
        ArticleAttributeCombination GetArticleAttributeCombinationById(int articleAttributeCombinationId);

        /// <summary>
        /// Gets a article attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Article attribute combination</returns>
        ArticleAttributeCombination GetArticleAttributeCombinationBySku(string sku);

        /// <summary>
        /// Inserts a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        void InsertArticleAttributeCombination(ArticleAttributeCombination combination);

        /// <summary>
        /// Updates a article attribute combination
        /// </summary>
        /// <param name="combination">Article attribute combination</param>
        void UpdateArticleAttributeCombination(ArticleAttributeCombination combination);

        #endregion
       
    }
}
