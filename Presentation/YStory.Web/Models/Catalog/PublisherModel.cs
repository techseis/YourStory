using System.Collections.Generic;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Media;

namespace YStory.Web.Models.Catalog
{
    public partial class PublisherModel : BaseYStoryEntityModel
    {
        public PublisherModel()
        {
            PictureModel = new PictureModel();
            FeaturedArticles = new List<ArticleOverviewModel>();
            Articles = new List<ArticleOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }


        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ArticleOverviewModel> FeaturedArticles { get; set; }
        public IList<ArticleOverviewModel> Articles { get; set; }
    }
}