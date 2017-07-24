using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class CurrencyModel : BaseYStoryEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}