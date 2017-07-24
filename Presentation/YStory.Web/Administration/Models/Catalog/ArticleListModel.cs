using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class ArticleListModel : BaseYStoryModel
    {
        public ArticleListModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailablePublishers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableContributors = new List<SelectListItem>();
            AvailableArticleTypes = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleName")]
        [AllowHtml]
        public string SearchArticleName { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchCategory")]
        public int SearchCategoryId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchPublisher")]
        public int SearchPublisherId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchStore")]
        public int SearchStoreId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchContributor")]
        public int SearchContributorId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchWarehouse")]
        public int SearchWarehouseId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleType")]
        public int SearchArticleTypeId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.GoDirectlyToSku")]
        [AllowHtml]
        public string GoDirectlyToSku { get; set; }

        public bool IsLoggedInAsContributor { get; set; }

        public bool AllowContributorsToImportArticles { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }
        public IList<SelectListItem> AvailableArticleTypes { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }
}