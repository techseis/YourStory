using System;
using System.IO;
using System.Linq;
using System.Web;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Media;
using YStory.Services.Catalog;

namespace YStory.Services.Media
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Download binary array</returns>
        public static byte[] GetDownloadBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            var binary = new byte[size];
            fs.Read(binary, 0, size);
            return binary;
        }

        /// <summary>
        /// Gets the picture binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Picture binary array</returns>
        public static byte[] GetPictureBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            var img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }

        /// <summary>
        /// Get article picture (for shopping cart and subscription details pages)
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="attributesXml">Atributes (in XML format)</param>
        /// <param name="pictureService">Picture service</param>
        /// <param name="articleAttributeParser">Article attribute service</param>
        /// <returns>Picture</returns>
        public static Picture GetArticlePicture(this Article article, string attributesXml,
            IPictureService pictureService,
            IArticleAttributeParser articleAttributeParser)
        {
            if (article == null)
                throw new ArgumentNullException("article");
            if (pictureService == null)
                throw new ArgumentNullException("pictureService");
            if (articleAttributeParser == null)
                throw new ArgumentNullException("articleAttributeParser");

            Picture picture = null;

            //first, let's see whether we have some attribute values with custom pictures
            var attributeValues = articleAttributeParser.ParseArticleAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                var attributePicture = pictureService.GetPictureById(attributeValue.PictureId);
                if (attributePicture != null)
                {
                    picture = attributePicture;
                    break;
                }
            }

            //now let's load the default article picture
            if (picture == null)
            {
                picture = pictureService.GetPicturesByArticleId(article.Id, 1).FirstOrDefault();
            }

            //let's check whether this article has some parent "grouped" article
            if (picture == null && !article.VisibleIndividually && article.ParentGroupedArticleId > 0)
            {
                picture = pictureService.GetPicturesByArticleId(article.ParentGroupedArticleId, 1).FirstOrDefault();
            }

            return picture;
        }
    }
}
