using System.Web.Mvc;
using System.Web.Routing;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Cms
{
    public partial class WidgetModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.ContentManagement.Widgets.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Widgets.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.ContentManagement.Widgets.Fields.IsActive")]
        public bool IsActive { get; set; }
        

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}