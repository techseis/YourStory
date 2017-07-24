using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class SearchBoxModel : BaseYStoryModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
    }
}