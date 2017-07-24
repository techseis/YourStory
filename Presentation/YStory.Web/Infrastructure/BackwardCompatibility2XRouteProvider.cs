using System.Web.Mvc;
using System.Web.Routing;
using YStory.Core.Configuration;
using YStory.Core.Infrastructure;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc.Routes;

namespace YStory.Web.Infrastructure
{
    //Routes used for backward compatibility with 2.x versions of yourStory
    public partial class BackwardCompatibility2XRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var config = EngineContext.Current.Resolve<YStoryConfig>();
            if (!config.SupportPreviousYourstoryVersions)
                return;

            //articles
            routes.MapLocalizedRoute("", "p/{articleId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectArticleById", SeName = UrlParameter.Optional },
                new { articleId = @"\d+" },
                new[] { "YStory.Web.Controllers" });

            //categories
            routes.MapLocalizedRoute("", "c/{categoryId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectCategoryById", SeName = UrlParameter.Optional },
                new { categoryId = @"\d+" },
                new[] { "YStory.Web.Controllers" });

            //publishers
            routes.MapLocalizedRoute("", "m/{publisherId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectPublisherById", SeName = UrlParameter.Optional },
                new { publisherId = @"\d+" },
                new[] { "YStory.Web.Controllers" });

            //news
            routes.MapLocalizedRoute("", "news/{newsItemId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectNewsItemById", SeName = UrlParameter.Optional },
                new { newsItemId = @"\d+" },
                new[] { "YStory.Web.Controllers" });

            //blog
            routes.MapLocalizedRoute("", "blog/{blogPostId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectBlogPostById", SeName = UrlParameter.Optional },
                new { blogPostId = @"\d+" },
                new[] { "YStory.Web.Controllers" });

            //topic
            routes.MapLocalizedRoute("", "t/{SystemName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectTopicBySystemName" },
                new[] { "YStory.Web.Controllers" });

            //contributors
            routes.MapLocalizedRoute("", "contributor/{contributorId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectContributorById", SeName = UrlParameter.Optional },
                new { contributorId = @"\d+" },
                new[] { "YStory.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //register it after all other IRouteProvider are processed
                return -1000;
            }
        }
    }
}
