using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using YStory.Core;
using YStory.Core.Configuration;
using YStory.Core.Data;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Media;
using YStory.Data;
using YStory.Services.Configuration;
using YStory.Services.Events;
using YStory.Services.Logging;

namespace YStory.Services.Media
{
    /// <summary>
    /// Picture service for Windows Azure
    /// </summary>
    public partial class AzurePictureService : PictureService
    {
        #region Fields
        
        private static CloudStorageAccount _storageAccount = null;
        private static CloudBlobClient blobClient = null;
        private static CloudBlobContainer container_thumb = null;

        private readonly MediaSettings _mediaSettings;
        private readonly YStoryConfig _config;

        #endregion

        #region Ctor

        public AzurePictureService(IRepository<Picture> pictureRepository,
            IRepository<ArticlePicture> articlePictureRepository,
            ISettingService settingService,
            IWebHelper webHelper,
            ILogger logger,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            YStoryConfig config,
            IDataProvider dataProvider)
            : base(pictureRepository,
                articlePictureRepository,
                settingService,
                webHelper,
                logger,
                dbContext,
                eventPublisher,
                mediaSettings,
                dataProvider)
        {
            this._mediaSettings = mediaSettings;
            this._config = config;

            if (String.IsNullOrEmpty(_config.AzureBlobStorageConnectionString))
                throw new Exception("Azure connection string for BLOB is not specified");
            if (String.IsNullOrEmpty(_config.AzureBlobStorageContainerName))
                throw new Exception("Azure container name for BLOB is not specified");
            if (String.IsNullOrEmpty(_config.AzureBlobStorageEndPoint))
                throw new Exception("Azure end point for BLOB is not specified");

            _storageAccount = CloudStorageAccount.Parse(_config.AzureBlobStorageConnectionString);
            if (_storageAccount == null)
                throw new Exception("Azure connection string for BLOB is not wrong");

            //should we do it for each HTTP request?
            blobClient = _storageAccount.CreateCloudBlobClient();
            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            //container.SetPermissions(containerPermissions);
            container_thumb = blobClient.GetContainerReference(_config.AzureBlobStorageContainerName);
            container_thumb.CreateIfNotExists();
            container_thumb.SetPermissions(containerPermissions);
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected override void DeletePictureThumbs(Picture picture)
        {
            string filter = string.Format("{0}", picture.Id.ToString("0000000"));
            var files = container_thumb.ListBlobs(prefix: filter, useFlatBlobListing: false);
            foreach (var ff in files)
            {
                CloudBlockBlob blockBlob = (CloudBlockBlob)ff;
                blockBlob.Delete();
            }
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbLocalPath(string thumbFileName)
        {
            var thumbFilePath = _config.AzureBlobStorageEndPoint + _config.AzureBlobStorageContainerName + "/" + thumbFileName;
            return thumbFilePath;
        }

        /// <summary>
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            var url = _config.AzureBlobStorageEndPoint + _config.AzureBlobStorageContainerName + "/";

            url = url + thumbFileName;
            return url;
        }

        /// <summary>
        /// Get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        protected override bool GeneratedThumbExists(string thumbFilePath, string thumbFileName)
        {
            try
            {
                CloudBlockBlob blockBlob = container_thumb.GetBlockBlobReference(thumbFileName);
                return blockBlob.Exists();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        protected override void SaveThumb(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            CloudBlockBlob blockBlob = container_thumb.GetBlockBlobReference(thumbFileName);
            
            //set mime type
            if (!String.IsNullOrEmpty(mimeType))
                blockBlob.Properties.ContentType = mimeType;

            //set cache control
            if (!string.IsNullOrEmpty(_mediaSettings.AzureCacheControlHeader))
                blockBlob.Properties.CacheControl = _mediaSettings.AzureCacheControlHeader;

            blockBlob.UploadFromByteArray(binary, 0, binary.Length);
        }

        #endregion
    }
}
