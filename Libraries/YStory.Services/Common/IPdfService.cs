using System.Collections.Generic;
using System.IO;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Subscriptions;
 

namespace YStory.Services.Common
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface IPdfService
    {
        /// <summary>
        /// Print an subscription to PDF
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an subscription</param>
        /// <param name="contributorId">Contributor identifier to limit articles; 0 to to print all articles. If specified, then totals won't be printed</param>
        /// <returns>A path of generated file</returns>
        string PrintSubscriptionToPdf(Subscription subscription, int languageId = 0, int contributorId = 0);

        /// <summary>
        /// Print subscriptions to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an subscription</param>
        /// <param name="contributorId">Contributor identifier to limit articles; 0 to to print all articles. If specified, then totals won't be printed</param>
        void PrintSubscriptionsToPdf(Stream stream, IList<Subscription> subscriptions, int languageId = 0, int contributorId = 0);

  

        
        /// <summary>
        /// Print articles to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="articles">Articles</param>
        void PrintArticlesToPdf(Stream stream, IList<Article> articles);
    }
}