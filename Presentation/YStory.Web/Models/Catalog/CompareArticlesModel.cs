using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class CompareArticlesModel : BaseYStoryEntityModel
    {
        public CompareArticlesModel()
        {
            Articles = new List<ArticleOverviewModel>();
        }
        public IList<ArticleOverviewModel> Articles { get; set; }

        public bool IncludeShortDescriptionInCompareArticles { get; set; }
        public bool IncludeFullDescriptionInCompareArticles { get; set; }
    }
}