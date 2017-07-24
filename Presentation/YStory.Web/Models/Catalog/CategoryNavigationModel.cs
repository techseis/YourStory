using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class CategoryNavigationModel : BaseYStoryModel
    {
        public CategoryNavigationModel()
        {
            Categories = new List<CategorySimpleModel>();
        }

        public int CurrentCategoryId { get; set; }
        public List<CategorySimpleModel> Categories { get; set; }
    }
}