using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class MediaSettingsModel : BaseYStoryModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }
        public bool AvatarPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.ArticleThumbPictureSize")]
        public int ArticleThumbPictureSize { get; set; }
        public bool ArticleThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.ArticleDetailsPictureSize")]
        public int ArticleDetailsPictureSize { get; set; }
        public bool ArticleDetailsPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.ArticleThumbPictureSizeOnArticleDetailsPage")]
        public int ArticleThumbPictureSizeOnArticleDetailsPage { get; set; }
        public bool ArticleThumbPictureSizeOnArticleDetailsPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedArticlePictureSize")]
        public int AssociatedArticlePictureSize { get; set; }
        public bool AssociatedArticlePictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }
        public bool CategoryThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.PublisherThumbPictureSize")]
        public int PublisherThumbPictureSize { get; set; }
        public bool PublisherThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.ContributorThumbPictureSize")]
        public int ContributorThumbPictureSize { get; set; }
        public bool ContributorThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }
        public bool CartThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }
        public bool MiniCartThumbPictureSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }
        public bool MaximumImageSize_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.MultipleThumbDirectories")]
        public bool MultipleThumbDirectories { get; set; }
        public bool MultipleThumbDirectories_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.DefaultImageQuality")]
        public int DefaultImageQuality { get; set; }
        public bool DefaultImageQuality_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.ImportArticleImagesUsingHash")]
        public bool ImportArticleImagesUsingHash { get; set; }
        public bool ImportArticleImagesUsingHash_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }
        public bool DefaultPictureZoomEnabled_OverrideForStore { get; set; }
    }
}