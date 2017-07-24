using System;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Blogs
{
    public partial class BlogCommentModel : BaseYStoryEntityModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
}