using System.Collections.Generic;
using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Publisher template service interface
    /// </summary>
    public partial interface IPublisherTemplateService
    {
        /// <summary>
        /// Delete publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        void DeletePublisherTemplate(PublisherTemplate publisherTemplate);

        /// <summary>
        /// Gets all publisher templates
        /// </summary>
        /// <returns>Publisher templates</returns>
        IList<PublisherTemplate> GetAllPublisherTemplates();

        /// <summary>
        /// Gets a publisher template
        /// </summary>
        /// <param name="publisherTemplateId">Publisher template identifier</param>
        /// <returns>Publisher template</returns>
        PublisherTemplate GetPublisherTemplateById(int publisherTemplateId);

        /// <summary>
        /// Inserts publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        void InsertPublisherTemplate(PublisherTemplate publisherTemplate);

        /// <summary>
        /// Updates the publisher template
        /// </summary>
        /// <param name="publisherTemplate">Publisher template</param>
        void UpdatePublisherTemplate(PublisherTemplate publisherTemplate);
    }
}
