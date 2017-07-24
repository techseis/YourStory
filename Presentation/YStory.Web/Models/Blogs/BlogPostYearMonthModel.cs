using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Blogs
{
    public partial class BlogPostYearModel : BaseYStoryModel
    {
        public BlogPostYearModel()
        {
            Months = new List<BlogPostMonthModel>();
        }
        public int Year { get; set; }
        public IList<BlogPostMonthModel> Months { get; set; }
    }
    public partial class BlogPostMonthModel : BaseYStoryModel
    {
        public int Month { get; set; }

        public int BlogPostCount { get; set; }
    }
}