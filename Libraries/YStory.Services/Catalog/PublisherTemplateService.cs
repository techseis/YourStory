using System;
using System.Collections.Generic;
using System.Linq;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Services.Events;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Publisher template service
    /// </summary>
    public partial class PublisherTemplateService : IPublisherTemplateService
    {
        #region Fields

        private readonly IRepository<PublisherTemplate> _publisherTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="publisherTemplateRepository">Publisher template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public PublisherTemplateService(IRepository<PublisherTemplate> publisherTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._publisherTemplateRepository = publisherTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        public virtual void DeletePublisherTemplate(PublisherTemplate publisherTemplate)
        {
            if (publisherTemplate == null)
                throw new ArgumentNullException("publisherTemplate");

            _publisherTemplateRepository.Delete(publisherTemplate);

            //event notification
            _eventPublisher.EntityDeleted(publisherTemplate);
        }

        /// <summary>
        /// Gets all publisher templates
        /// </summary>
        /// <returns>Publisher templates</returns>
        public virtual IList<PublisherTemplate> GetAllPublisherTemplates()
        {
            var query = from pt in _publisherTemplateRepository.Table
                        orderby pt.DisplaySubscription, pt.Id
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        /// <summary>
        /// Gets a publisher template
        /// </summary>
        /// <param name="publisherTemplateId">Publisher template identifier</param>
        /// <returns>Publisher template</returns>
        public virtual PublisherTemplate GetPublisherTemplateById(int publisherTemplateId)
        {
            if (publisherTemplateId == 0)
                return null;

            return _publisherTemplateRepository.GetById(publisherTemplateId);
        }

        /// <summary>
        /// Inserts publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        public virtual void InsertPublisherTemplate(PublisherTemplate publisherTemplate)
        {
            if (publisherTemplate == null)
                throw new ArgumentNullException("publisherTemplate");

            _publisherTemplateRepository.Insert(publisherTemplate);

            //event notification
            _eventPublisher.EntityInserted(publisherTemplate);
        }

        /// <summary>
        /// Updates the publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        public virtual void UpdatePublisherTemplate(PublisherTemplate publisherTemplate)
        {
            if (publisherTemplate == null)
                throw new ArgumentNullException("publisherTemplate");

            _publisherTemplateRepository.Update(publisherTemplate);

            //event notification
            _eventPublisher.EntityUpdated(publisherTemplate);
        }
        
        #endregion
    }
}
