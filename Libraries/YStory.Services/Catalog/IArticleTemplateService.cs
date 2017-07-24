using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article template interface
    /// </summary>
    public partial interface IArticleTemplateService
    {
        /// <summary>
        /// Delete article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        void DeleteArticleTemplate(ArticleTemplate articleTemplate);

        /// <summary>
        /// Gets all article templates
        /// </summary>
        /// <returns>Article templates</returns>
        IList<ArticleTemplate> GetAllArticleTemplates();

        /// <summary>
        /// Gets a article template
        /// </summary>
        /// <param name="articleTemplateId">Article template identifier</param>
        /// <returns>Article template</returns>
        ArticleTemplate GetArticleTemplateById(int articleTemplateId);

        /// <summary>
        /// Inserts article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        void InsertArticleTemplate(ArticleTemplate articleTemplate);

        /// <summary>
        /// Updates the article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        void UpdateArticleTemplate(ArticleTemplate articleTemplate);
    }
}
