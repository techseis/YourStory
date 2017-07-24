
using YStory.Core.Configuration;

namespace YStory.Core.Domain.Common
{
    public class PdfSettings : ISettings
    {
        /// <summary>
        /// PDF logo picture identifier
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets whether letter page size is enabled
        /// </summary>
        public bool LetterPageSizeEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render subscription notes in PDf reports
        /// </summary>
        public bool RenderSubscriptionNotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disallow customers to print PDF invoices for pedning subscriptions
        /// </summary>
        public bool DisablePdfInvoicesForPendingSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the font file name that will be used
        /// </summary>
        public string FontFileName { get; set; }

        /// <summary>
        /// Gets or sets the text that will appear at the bottom of invoices (column 1)
        /// </summary>
        public string InvoiceFooterTextColumn1 { get; set; }

        /// <summary>
        /// Gets or sets the text that will appear at the bottom of invoices (column 1)
        /// </summary>
        public string InvoiceFooterTextColumn2 { get; set; }
    }
}