using System;
using System.Linq;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Payments;
using YStory.Services.Events;

namespace YStory.Services.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        private readonly IRepository<Download> _downloadRepository;
        private readonly IEventPublisher _eventPubisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="downloadRepository">Download repository</param>
        /// <param name="eventPubisher"></param>
        public DownloadService(IRepository<Download> downloadRepository,
            IEventPublisher eventPubisher)
        {
            _downloadRepository = downloadRepository;
            _eventPubisher = eventPubisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;
            
            return _downloadRepository.GetById(downloadId);
        }

        /// <summary>
        /// Gets a download by GUID
        /// </summary>
        /// <param name="downloadGuid">Download GUID</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadByGuid(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var query = from o in _downloadRepository.Table
                        where o.DownloadGuid == downloadGuid
                        select o;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Delete(download);

            _eventPubisher.EntityDeleted(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Insert(download);

            _eventPubisher.EntityInserted(download);
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Update(download);

            _eventPubisher.EntityUpdated(download);
        }

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="subscriptionItem">Subscription item to check</param>
        /// <returns>True if download is allowed; otherwise, false.</returns>
        public virtual bool IsDownloadAllowed(SubscriptionItem subscriptionItem)
        {
            if (subscriptionItem == null)
                return false;

            var subscription = subscriptionItem.Subscription;
            if (subscription == null || subscription.Deleted)
                return false;

            //subscription status
            if (subscription.SubscriptionStatus == SubscriptionStatus.Cancelled)
                return false;

            var article = subscriptionItem.Article;
            if (article == null  )
                return false;

            //payment status
             

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="subscriptionItem">Subscription item to check</param>
        /// <returns>True if license download is allowed; otherwise, false.</returns>
        public virtual bool IsLicenseDownloadAllowed(SubscriptionItem subscriptionItem)
        {
            if (subscriptionItem == null)
                return false;

            return IsDownloadAllowed(subscriptionItem) &&
                subscriptionItem.LicenseDownloadId.HasValue &&
                subscriptionItem.LicenseDownloadId > 0;
        }

        #endregion
    }
}
