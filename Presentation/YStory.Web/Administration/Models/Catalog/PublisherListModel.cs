using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    public partial class PublisherListModel : BaseYStoryModel
    {
        public PublisherListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.List.SearchPublisherName")]
        [AllowHtml]
        public string SearchPublisherName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}