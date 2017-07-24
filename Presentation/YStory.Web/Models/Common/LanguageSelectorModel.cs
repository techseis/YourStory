using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Common
{
    public partial class LanguageSelectorModel : BaseYStoryModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}