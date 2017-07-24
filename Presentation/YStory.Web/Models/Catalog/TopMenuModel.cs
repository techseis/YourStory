using System.Collections.Generic;
using System.Linq;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class TopMenuModel : BaseYStoryModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopMenuTopicModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopMenuTopicModel> Topics { get; set; }

        public bool BlogEnabled { get; set; }
        public bool NewArticlesEnabled { get; set; }
        public bool ForumEnabled { get; set; }

        public bool DisplayHomePageMenuItem { get; set; }
        public bool DisplayNewArticlesMenuItem { get; set; }
        public bool DisplayArticleSearchMenuItem { get; set; }
        public bool DisplayCustomerInfoMenuItem { get; set; }
        public bool DisplayBlogMenuItem { get; set; }
        public bool DisplayForumsMenuItem { get; set; }
        public bool DisplayContactUsMenuItem { get; set; }

        public bool HasOnlyCategories
        {
            get
            {
                return Categories.Any()
                       && !Topics.Any()
                       && !DisplayHomePageMenuItem
                       && !(DisplayNewArticlesMenuItem && NewArticlesEnabled)
                       && !DisplayArticleSearchMenuItem
                       && !DisplayCustomerInfoMenuItem
                       && !(DisplayBlogMenuItem && BlogEnabled)
                       && !(DisplayForumsMenuItem && ForumEnabled)
                       && !DisplayContactUsMenuItem;
            }
        }

        #region Nested classes

        public class TopMenuTopicModel : BaseYStoryEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }

        #endregion
    }
}