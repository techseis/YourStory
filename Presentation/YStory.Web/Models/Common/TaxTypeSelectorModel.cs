using YStory.Core.Domain.Tax;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class TaxTypeSelectorModel : BaseYStoryModel
    {
        public TaxDisplayType CurrentTaxType { get; set; }
    }
}