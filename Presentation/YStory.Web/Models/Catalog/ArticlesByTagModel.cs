using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class ArticlesByTagModel : BaseYStoryEntityModel
    {
        public ArticlesByTagModel()
        {
            Articles = new List<ArticleOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }
        
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ArticleOverviewModel> Articles { get; set; }
    }
}