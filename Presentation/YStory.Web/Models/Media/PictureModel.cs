using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Media
{
    public partial class PictureModel : BaseYStoryModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}