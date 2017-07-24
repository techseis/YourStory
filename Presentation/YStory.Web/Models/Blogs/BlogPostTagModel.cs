using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Blogs
{
    public partial class BlogPostTagModel : BaseYStoryModel
    {
        public string Name { get; set; }

        public int BlogPostCount { get; set; }
    }
}