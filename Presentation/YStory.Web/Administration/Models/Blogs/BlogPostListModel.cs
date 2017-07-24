using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Blogs
{
    public partial class BlogPostListModel : BaseYStoryModel
    {
        public BlogPostListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}