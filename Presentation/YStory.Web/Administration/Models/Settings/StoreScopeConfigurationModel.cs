using System.Collections.Generic;
using YStory.Admin.Models.Stores;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class StoreScopeConfigurationModel : BaseYStoryModel
    {
        public StoreScopeConfigurationModel()
        {
            Stores = new List<StoreModel>();
        }

        public int StoreId { get; set; }
        public IList<StoreModel> Stores { get; set; }
    }
}