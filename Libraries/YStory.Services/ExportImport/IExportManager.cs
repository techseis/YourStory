using System.Collections.Generic;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export publisher list to xml
        /// </summary>
        /// <param name="publishers">Publishers</param>
        /// <returns>Result in XML format</returns>
        string ExportPublishersToXml(IList<Publisher> publishers);

        /// <summary>
        /// Export publishers to XLSX
        /// </summary>
        /// <param name="publishers">Manufactures</param>
        byte[] ExportPublishersToXlsx(IEnumerable<Publisher> publishers);

        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportCategoriesToXml();

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        byte[] ExportCategoriesToXlsx(IEnumerable<Category> categories);

        /// <summary>
        /// Export article list to xml
        /// </summary>
        /// <param name="articles">Articles</param>
        /// <returns>Result in XML format</returns>
        string ExportArticlesToXml(IList<Article> articles);

        /// <summary>
        /// Export articles to XLSX
        /// </summary>
        /// <param name="articles">Articles</param>
        byte[] ExportArticlesToXlsx(IEnumerable<Article> articles);

        /// <summary>
        /// Export subscription list to xml
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in XML format</returns>
        string ExportSubscriptionsToXml(IList<Subscription> subscriptions);

        /// <summary>
        /// Export subscriptions to XLSX
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        byte[] ExportSubscriptionsToXlsx(IList<Subscription> subscriptions);

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        byte[] ExportCustomersToXlsx(IList<Customer> customers);

        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        string ExportCustomersToXml(IList<Customer> customers);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportStatesToTxt(IList<StateProvince> states);
    }
}
