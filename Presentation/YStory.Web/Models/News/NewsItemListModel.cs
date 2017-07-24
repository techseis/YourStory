using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.News
{
    public partial class NewsItemListModel : BaseYStoryModel
    {
        public NewsItemListModel()
        {
            PagingFilteringContext = new NewsPagingFilteringModel();
            NewsItems = new List<NewsItemModel>();
        }

        public int WorkingLanguageId { get; set; }
        public NewsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<NewsItemModel> NewsItems { get; set; }
    }
}