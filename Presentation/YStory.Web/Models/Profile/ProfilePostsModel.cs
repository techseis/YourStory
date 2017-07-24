using System.Collections.Generic;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Profile
{
    public partial class ProfilePostsModel
    {
        public IList<PostsModel> Posts { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}