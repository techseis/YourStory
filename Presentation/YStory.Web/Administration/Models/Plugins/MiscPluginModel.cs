using System.Web.Routing;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Plugins
{
    public partial class MiscPluginModel : BaseYStoryModel
    {
        public string FriendlyName { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}