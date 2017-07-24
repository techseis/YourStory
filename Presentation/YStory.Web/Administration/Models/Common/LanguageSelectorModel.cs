using System.Collections.Generic;
using YStory.Admin.Models.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Common
{
    public partial class LanguageSelectorModel : BaseYStoryModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}