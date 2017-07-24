using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class StoreThemeSelectorModel : BaseYStoryModel
    {
        public StoreThemeSelectorModel()
        {
            AvailableStoreThemes = new List<StoreThemeModel>();
        }

        public IList<StoreThemeModel> AvailableStoreThemes { get; set; }

        public StoreThemeModel CurrentStoreTheme { get; set; }
    }
}