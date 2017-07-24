using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Services.Events;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Article template service
    /// </summary>
    public partial class ArticleTemplateService : IArticleTemplateService
    {
        #region Fields

        private readonly IRepository<ArticleTemplate> _articleTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="articleTemplateRepository">Article template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ArticleTemplateService(IRepository<ArticleTemplate> articleTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._articleTemplateRepository = articleTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        public virtual void DeleteArticleTemplate(ArticleTemplate articleTemplate)
        {
            if (articleTemplate == null)
                throw new ArgumentNullException("articleTemplate");

            _articleTemplateRepository.Delete(articleTemplate);

            //event notification
            _eventPublisher.EntityDeleted(articleTemplate);
        }

        /// <summary>
        /// Gets all article templates
        /// </summary>
        /// <returns>Article templates</returns>
        public virtual IList<ArticleTemplate> GetAllArticleTemplates()
        {
            var query = from pt in _articleTemplateRepository.Table
                        orderby pt.DisplaySubscription, pt.Id
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        /// <summary>
        /// Gets a article template
        /// </summary>
        /// <param name="articleTemplateId">Article template identifier</param>
        /// <returns>Article template</returns>
        public virtual ArticleTemplate GetArticleTemplateById(int articleTemplateId)
        {
            if (articleTemplateId == 0)
                return null;

            return _articleTemplateRepository.GetById(articleTemplateId);
        }

        /// <summary>
        /// Inserts article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        public virtual void InsertArticleTemplate(ArticleTemplate articleTemplate)
        {
            if (articleTemplate == null)
                throw new ArgumentNullException("articleTemplate");

            _articleTemplateRepository.Insert(articleTemplate);

            //event notification
            _eventPublisher.EntityInserted(articleTemplate);
        }

        /// <summary>
        /// Updates the article template
        /// </summary>
        /// <param name="articleTemplate">Article template</param>
        public virtual void UpdateArticleTemplate(ArticleTemplate articleTemplate)
        {
            if (articleTemplate == null)
                throw new ArgumentNullException("articleTemplate");

            _articleTemplateRepository.Update(articleTemplate);

            //event notification
            _eventPublisher.EntityUpdated(articleTemplate);
        }
        
        #endregion
    }
}
