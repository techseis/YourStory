
using YStory.Core.Configuration;

namespace YStory.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        /// <summary>
        /// Picture size of customer avatars (if enabled)
        /// </summary>
        public int AvatarPictureSize { get; set; }
        /// <summary>
        /// Picture size of article picture thumbs displayed on catalog pages (e.g. category details page)
        /// </summary>
        public int ArticleThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of the main article picture displayed on the article details page
        /// </summary>
        public int ArticleDetailsPictureSize { get; set; }
        /// <summary>
        /// Picture size of the article picture thumbs displayed on the article details page
        /// </summary>
        public int ArticleThumbPictureSizeOnArticleDetailsPage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AssociatedArticlePictureSize { get; set; }
        /// <summary>
        /// Picture size of category pictures
        /// </summary>
        public int CategoryThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of publisher pictures
        /// </summary>
        public int PublisherThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of contributor pictures
        /// </summary>
        public int ContributorThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of article pictures on the shopping cart page
        /// </summary>
        public int CartThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of article pictures for minishipping cart box
        /// </summary>
        public int MiniCartThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of article pictures for autocomplete search box
        /// </summary>
        public int AutoCompleteSearchThumbPictureSize { get; set; }
        /// <summary>
        /// Picture size of image squares on a article details page (used with "image squares" attribute type
        /// </summary>
        public int ImageSquarePictureSize { get; set; }
        /// <summary>
        /// A value indicating whether picture zoom is enabled
        /// </summary>
        public bool DefaultPictureZoomEnabled { get; set; }
        /// <summary>
        /// Maximum allowed picture size. If a larger picture is uploaded, then it'll be resized
        /// </summary>
        public int MaximumImageSize { get; set; }
        /// <summary>
        /// Gets or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether we should use fast HASHBYTES (hash sum) database function to compare pictures when importing articles
        /// </summary>
        public bool ImportArticleImagesUsingHash { get; set; }

        /// <summary>
        /// Gets or sets Azure CacheControl header (e.g. "max-age=3600, public")
        /// </summary>
        /// <remarks>
        ///max-age=[seconds]     — specifies the maximum amount of time that a representation will be considered fresh. Similar to Expires, this directive is relative to the time of the request, rather than absolute. [seconds] is the number of seconds from the time of the request you wish the representation to be fresh for.
        ///s-maxage=[seconds]    — similar to max-age, except that it only applies to shared (e.g., proxy) caches.
        ///public                — marks authenticated responses as cacheable; normally, if HTTP authentication is required, responses are automatically private.
        ///private               — allows caches that are specific to one user (e.g., in a browser) to store the response; shared caches (e.g., in a proxy) may not.
        ///no-cache              — forces caches to submit the request to the origin server for validation before releasing a cached copy, every time. This is useful to assure that authentication is respected (in combination with public), or to maintain rigid freshness, without sacrificing all of the benefits of caching.
        ///no-store              — instructs caches not to keep a copy of the representation under any conditions.
        ///must-revalidate       — tells caches that they must obey any freshness information you give them about a representation. HTTP allows caches to serve stale representations under special conditions; by specifying this header, you’re telling the cache that you want it to strictly follow your rules.
        ///proxy-revalidate      — similar to must-revalidate, except that it only applies to proxy caches.
        /// </remarks>
        public string AzureCacheControlHeader { get; set; }
    }
}