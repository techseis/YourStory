using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Settings;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    [Validator(typeof(ReturnRequestReasonValidator))]
    public partial class ReturnRequestReasonModel : BaseYStoryEntityModel, ILocalizedModel<ReturnRequestReasonLocalizedModel>
    {
        public ReturnRequestReasonModel()
        {
            Locales = new List<ReturnRequestReasonLocalizedModel>();
        }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        public IList<ReturnRequestReasonLocalizedModel> Locales { get; set; }
    }

    public partial class ReturnRequestReasonLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestReasons.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}