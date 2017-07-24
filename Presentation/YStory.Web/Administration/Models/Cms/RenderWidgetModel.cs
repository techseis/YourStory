using System.Web.Routing;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Cms
{
    public partial class RenderWidgetModel : BaseYStoryModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}