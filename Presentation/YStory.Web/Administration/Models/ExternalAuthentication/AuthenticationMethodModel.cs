using System.Web.Mvc;
using System.Web.Routing;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.ExternalAuthentication
{
    public partial class AuthenticationMethodModel : BaseYStoryModel
    {
        [YStoryResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.IsActive")]
        public bool IsActive { get; set; }



        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}