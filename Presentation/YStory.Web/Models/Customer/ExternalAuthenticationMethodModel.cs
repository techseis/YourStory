using System.Web.Routing;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Customer
{
    public partial class ExternalAuthenticationMethodModel : BaseYStoryModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}