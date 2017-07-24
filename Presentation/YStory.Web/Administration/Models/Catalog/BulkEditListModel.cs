using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class BulkEditListModel : BaseYStoryModel
    {
        public BulkEditListModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailablePublishers = new List<SelectListItem>();
            AvailableArticleTypes = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.BulkEdit.List.SearchArticleName")]
        [AllowHtml]
        public string SearchArticleName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.BulkEdit.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.BulkEdit.List.SearchPublisher")]
        public int SearchPublisherId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleType")]
        public int SearchArticleTypeId { get; set; }
        public IList<SelectListItem> AvailableArticleTypes { get; set; }
        

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }
    }
}