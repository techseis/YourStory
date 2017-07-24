using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class PublisherNavigationModel : BaseYStoryModel
    {
        public PublisherNavigationModel()
        {
            this.Publishers = new List<PublisherBriefInfoModel>();
        }

        public IList<PublisherBriefInfoModel> Publishers { get; set; }

        public int TotalPublishers { get; set; }
    }

    public partial class PublisherBriefInfoModel : BaseYStoryEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        
        public bool IsActive { get; set; }
    }
}