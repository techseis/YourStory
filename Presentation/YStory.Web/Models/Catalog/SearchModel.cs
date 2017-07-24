using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class SearchModel : BaseYStoryModel
    {
        public SearchModel()
        {
            this.PagingFilteringContext = new CatalogPagingFilteringModel();
            this.Articles = new List<ArticleOverviewModel>();

            this.AvailableCategories = new List<SelectListItem>();
            this.AvailablePublishers = new List<SelectListItem>();
            this.AvailableContributors = new List<SelectListItem>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [YStoryResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string q { get; set; }
        /// <summary>
        /// Category ID
        /// </summary>
        [YStoryResourceDisplayName("Search.Category")]
        public int cid { get; set; }
        [YStoryResourceDisplayName("Search.IncludeSubCategories")]
        public bool isc { get; set; }
        /// <summary>
        /// Publisher ID
        /// </summary>
        [YStoryResourceDisplayName("Search.Publisher")]
        public int mid { get; set; }
        /// <summary>
        /// Contributor ID
        /// </summary>
        [YStoryResourceDisplayName("Search.Contributor")]
        public int vid { get; set; }
        /// <summary>
        /// Price - From 
        /// </summary>
        [AllowHtml]
        public string pf { get; set; }
        /// <summary>
        /// Price - To
        /// </summary>
        [AllowHtml]
        public string pt { get; set; }
        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [YStoryResourceDisplayName("Search.SearchInDescriptions")]
        public bool sid { get; set; }
        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [YStoryResourceDisplayName("Search.AdvancedSearch")]
        public bool adv { get; set; }
        /// <summary>
        /// A value indicating whether "allow search by contributor" is enabled
        /// </summary>
        public bool asv { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }


        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ArticleOverviewModel> Articles { get; set; }

        #region Nested classes

        public class CategoryModel : BaseYStoryEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}