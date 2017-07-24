
using System.IO;

namespace YStory.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        /// <summary>
        /// Import articles from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportArticlesFromXlsx(Stream stream);

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        int ImportNewsletterSubscribersFromTxt(Stream stream);

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        int ImportStatesFromTxt(Stream stream);

        /// <summary>
        /// Import publishers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportPublishersFromXlsx(Stream stream);

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportCategoriesFromXlsx(Stream stream);
    }
}
