using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Catalog;
using YStory.Web.Models.Topics;

namespace YStory.Web.Models.Common
{
    public partial class SitemapModel : BaseYStoryModel
    {
        public SitemapModel()
        {
            Articles = new List<ArticleOverviewModel>();
            Categories = new List<CategorySimpleModel>();
            Publishers = new List<PublisherBriefInfoModel>();
            Topics = new List<TopicModel>();
        }
        public IList<ArticleOverviewModel> Articles { get; set; }
        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<PublisherBriefInfoModel> Publishers { get; set; }
        public IList<TopicModel> Topics { get; set; }

        public bool NewsEnabled { get; set; }
        public bool BlogEnabled { get; set; }
        public bool ForumEnabled { get; set; }
    }
}