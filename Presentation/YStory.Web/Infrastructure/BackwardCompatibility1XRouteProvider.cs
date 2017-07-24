﻿using System.Web.Mvc;
using System.Web.Routing;
using YStory.Core.Configuration;
using YStory.Core.Infrastructure;
using YStory.Web.Framework.Mvc.Routes;

namespace YStory.Web.Infrastructure
{
    //Routes used for backward compatibility with 1.x versions of yourStory
    public partial class BackwardCompatibility1XRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var config = EngineContext.Current.Resolve<YStoryConfig>();
            if (!config.SupportPreviousYourstoryVersions)
                return;

            //all old aspx URLs
            routes.MapRoute("", "{oldfilename}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "GeneralRedirect" },
                            new[] { "YStory.Web.Controllers" });
            
            //articles
            routes.MapRoute("", "articles/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectArticle"},
                            new[] { "YStory.Web.Controllers" });
            
            //categories
            routes.MapRoute("", "category/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectCategory" },
                            new[] { "YStory.Web.Controllers" });

            //publishers
            routes.MapRoute("", "publisher/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectPublisher" },
                            new[] { "YStory.Web.Controllers" });

            //article tags
            routes.MapRoute("", "articletag/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectArticleTag" },
                            new[] { "YStory.Web.Controllers" });

            //news
            routes.MapRoute("", "news/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectNewsItem" },
                            new[] { "YStory.Web.Controllers" });

            //blog posts
            routes.MapRoute("", "blog/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectBlogPost" },
                            new[] { "YStory.Web.Controllers" });

            //topics
            routes.MapRoute("", "topic/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectTopic" },
                            new[] { "YStory.Web.Controllers" });

            //forums
            routes.MapRoute("", "boards/fg/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectForumGroup" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapRoute("", "boards/f/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectForum" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapRoute("", "boards/t/{id}.aspx",
                            new { controller = "BackwardCompatibility1X", action = "RedirectForumTopic" },
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
