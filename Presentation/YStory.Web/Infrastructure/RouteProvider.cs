using System.Web.Mvc;
using System.Web.Routing;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc.Routes;

namespace YStory.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //We resubscriptioned our routes so the most used ones are on top. It can improve performance.

            //home page
            routes.MapLocalizedRoute("HomePage",
                            "",
                            new { controller = "Home", action = "Index" },
                            new[] { "YStory.Web.Controllers" });

            //widgets
            //we have this route for performance optimization because named routes are MUCH faster than usual Html.Action(...)
            //and this route is highly used
            routes.MapRoute("WidgetsByZone",
                            "widgetsbyzone/",
                            new { controller = "Widget", action = "WidgetsByZone" },
                            new[] { "YStory.Web.Controllers" });

            //login
            routes.MapLocalizedRoute("Login",
                            "login/",
                            new { controller = "Customer", action = "Login" },
                            new[] { "YStory.Web.Controllers" });
            //register
            routes.MapLocalizedRoute("Register",
                            "register/",
                            new { controller = "Customer", action = "Register" },
                            new[] { "YStory.Web.Controllers" });
            //logout
            routes.MapLocalizedRoute("Logout",
                            "logout/",
                            new { controller = "Customer", action = "Logout" },
                            new[] { "YStory.Web.Controllers" });

            //shopping cart
            routes.MapLocalizedRoute("ShoppingCart",
                            "cart/",
                            new { controller = "ShoppingCart", action = "Cart" },
                            new[] { "YStory.Web.Controllers" });
            //estimate shipping
            routes.MapLocalizedRoute("EstimateShipping",
                            "cart/estimateshipping",
                            new {controller = "ShoppingCart", action = "GetEstimateShipping"},
                            new[] {"YStory.Web.Controllers"});
            //wishlist
            routes.MapLocalizedRoute("Wishlist",
                            "wishlist/{customerGuid}",
                            new { controller = "ShoppingCart", action = "Wishlist", customerGuid = UrlParameter.Optional },
                            new[] { "YStory.Web.Controllers" });

            //customer account links
            routes.MapLocalizedRoute("CustomerInfo",
                            "customer/info",
                            new { controller = "Customer", action = "Info" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerAddresses",
                            "customer/addresses",
                            new { controller = "Customer", action = "Addresses" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerSubscriptions",
                            "subscription/history",
                            new { controller = "Subscription", action = "CustomerSubscriptions" },
                            new[] { "YStory.Web.Controllers" });

            //contact us
            routes.MapLocalizedRoute("ContactUs",
                            "contactus",
                            new { controller = "Common", action = "ContactUs" },
                            new[] { "YStory.Web.Controllers" });
            //sitemap
            routes.MapLocalizedRoute("Sitemap",
                            "sitemap",
                            new { controller = "Common", action = "Sitemap" },
                            new[] { "YStory.Web.Controllers" });

            //article search
            routes.MapLocalizedRoute("ArticleSearch",
                            "search/",
                            new { controller = "Catalog", action = "Search" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ArticleSearchAutoComplete",
                            "catalog/searchtermautocomplete",
                            new { controller = "Catalog", action = "SearchTermAutoComplete" },
                            new[] { "YStory.Web.Controllers" });

            //change currency (AJAX link)
            routes.MapLocalizedRoute("ChangeCurrency",
                            "changecurrency/{customercurrency}",
                            new { controller = "Common", action = "SetCurrency" },
                            new { customercurrency = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //change language (AJAX link)
            routes.MapLocalizedRoute("ChangeLanguage",
                            "changelanguage/{langid}",
                            new { controller = "Common", action = "SetLanguage" },
                            new { langid = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //change tax (AJAX link)
            routes.MapLocalizedRoute("ChangeTaxType",
                            "changetaxtype/{customertaxtype}",
                            new { controller = "Common", action = "SetTaxType" },
                            new { customertaxtype = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //recently viewed articles
            routes.MapLocalizedRoute("RecentlyViewedArticles",
                            "recentlyviewedarticles/",
                            new { controller = "Article", action = "RecentlyViewedArticles" },
                            new[] { "YStory.Web.Controllers" });
            //new articles
            routes.MapLocalizedRoute("NewArticles",
                            "newarticles/",
                            new { controller = "Article", action = "NewArticles" },
                            new[] { "YStory.Web.Controllers" });
            //blog
            routes.MapLocalizedRoute("Blog",
                            "blog",
                            new { controller = "Blog", action = "List" },
                            new[] { "YStory.Web.Controllers" });
            //news
            routes.MapLocalizedRoute("NewsArchive",
                            "news",
                            new { controller = "News", action = "List" },
                            new[] { "YStory.Web.Controllers" });

            //forum
            routes.MapLocalizedRoute("Boards",
                            "boards",
                            new { controller = "Boards", action = "Index" },
                            new[] { "YStory.Web.Controllers" });

            //compare articles
            routes.MapLocalizedRoute("CompareArticles",
                            "comparearticles/",
                            new { controller = "Article", action = "CompareArticles" },
                            new[] { "YStory.Web.Controllers" });

            //article tags
            routes.MapLocalizedRoute("ArticleTagsAll",
                            "articletag/all/",
                            new { controller = "Catalog", action = "ArticleTagsAll" },
                            new[] { "YStory.Web.Controllers" });

            //publishers
            routes.MapLocalizedRoute("PublisherList",
                            "publisher/all/",
                            new { controller = "Catalog", action = "PublisherAll" },
                            new[] { "YStory.Web.Controllers" });
            //contributors
            routes.MapLocalizedRoute("ContributorList",
                            "contributor/all/",
                            new { controller = "Catalog", action = "ContributorAll" },
                            new[] { "YStory.Web.Controllers" });


            //add article to cart (without any attributes and options). used on catalog pages.
            routes.MapLocalizedRoute("AddArticleToCart-Catalog",
                            "addarticletocart/catalog/{articleId}/{shoppingCartTypeId}/{quantity}",
                            new { controller = "ShoppingCart", action = "AddArticleToCart_Catalog" },
                            new { articleId = @"\d+", shoppingCartTypeId = @"\d+", quantity = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //add article to cart (with attributes and options). used on the article details pages.
            routes.MapLocalizedRoute("AddArticleToCart-Details",
                            "addarticletocart/details/{articleId}/{shoppingCartTypeId}",
                            new { controller = "ShoppingCart", action = "AddArticleToCart_Details" },
                            new { articleId = @"\d+", shoppingCartTypeId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //article tags
            routes.MapLocalizedRoute("ArticlesByTag",
                            "articletag/{articleTagId}/{SeName}",
                            new { controller = "Catalog", action = "ArticlesByTag", SeName = UrlParameter.Optional },
                            new { articleTagId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //comparing articles
            routes.MapLocalizedRoute("AddArticleToCompare",
                            "comparearticles/add/{articleId}",
                            new { controller = "Article", action = "AddArticleToCompareList" },
                            new { articleId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //article email a friend
            routes.MapLocalizedRoute("ArticleEmailAFriend",
                            "articleemailafriend/{articleId}",
                            new { controller = "Article", action = "ArticleEmailAFriend" },
                            new { articleId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //reviews
            routes.MapLocalizedRoute("ArticleReviews",
                            "articlereviews/{articleId}",
                            new { controller = "Article", action = "ArticleReviews" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerArticleReviews",
                            "customer/articlereviews",
                            new { controller = "Article", action = "CustomerArticleReviews" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerArticleReviewsPaged",
                            "customer/articlereviews/page/{page}",
                            new { controller = "Article", action = "CustomerArticleReviews" },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //back in stock notifications
            routes.MapLocalizedRoute("BackInStockSubscribePopup",
                            "backinstocksubscribe/{articleId}",
                            new { controller = "BackInStockSubscription", action = "SubscribePopup" },
                            new { articleId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("BackInStockSubscribeSend",
                            "backinstocksubscribesend/{articleId}",
                            new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" },
                            new { articleId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //downloads
            routes.MapRoute("GetSampleDownload",
                            "download/sample/{articleid}",
                            new { controller = "Download", action = "Sample" },
                            new { articleid = @"\d+" },
                            new[] { "YStory.Web.Controllers" });



            //checkout pages
            routes.MapLocalizedRoute("Checkout",
                            "checkout/",
                            new { controller = "Checkout", action = "Index" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutOnePage",
                            "onepagecheckout/",
                            new { controller = "Checkout", action = "OnePageCheckout" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutShippingAddress",
                            "checkout/shippingaddress",
                            new { controller = "Checkout", action = "ShippingAddress" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutSelectShippingAddress",
                            "checkout/selectshippingaddress",
                            new { controller = "Checkout", action = "SelectShippingAddress" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutBillingAddress",
                            "checkout/billingaddress",
                            new { controller = "Checkout", action = "BillingAddress" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutSelectBillingAddress",
                            "checkout/selectbillingaddress",
                            new { controller = "Checkout", action = "SelectBillingAddress" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutShippingMethod",
                            "checkout/shippingmethod",
                            new { controller = "Checkout", action = "ShippingMethod" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutPaymentMethod",
                            "checkout/paymentmethod",
                            new { controller = "Checkout", action = "PaymentMethod" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "Checkout", action = "PaymentInfo" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "Checkout", action = "Confirm" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CheckoutCompleted",
                            "checkout/completed/{subscriptionId}",
                            new { controller = "Checkout", action = "Completed", subscriptionId = UrlParameter.Optional },
                            new { subscriptionId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //subscribe newsletters
            routes.MapLocalizedRoute("SubscribeNewsletter",
                            "subscribenewsletter",
                            new { controller = "Newsletter", action = "SubscribeNewsletter" },
                            new[] { "YStory.Web.Controllers" });

            //email wishlist
            routes.MapLocalizedRoute("EmailWishlist",
                            "emailwishlist",
                            new { controller = "ShoppingCart", action = "EmailWishlist" },
                            new[] { "YStory.Web.Controllers" });

            //login page for checkout as guest
            routes.MapLocalizedRoute("LoginCheckoutAsGuest",
                            "login/checkoutasguest",
                            new { controller = "Customer", action = "Login", checkoutAsGuest = true },
                            new[] { "YStory.Web.Controllers" });
            //register result page
            routes.MapLocalizedRoute("RegisterResult",
                            "registerresult/{resultId}",
                            new { controller = "Customer", action = "RegisterResult" },
                            new { resultId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //check username availability
            routes.MapLocalizedRoute("CheckUsernameAvailability",
                            "customer/checkusernameavailability",
                            new { controller = "Customer", action = "CheckUsernameAvailability" },
                            new[] { "YStory.Web.Controllers" });

            //passwordrecovery
            routes.MapLocalizedRoute("PasswordRecovery",
                            "passwordrecovery",
                            new { controller = "Customer", action = "PasswordRecovery" },
                            new[] { "YStory.Web.Controllers" });
            //password recovery confirmation
            routes.MapLocalizedRoute("PasswordRecoveryConfirm",
                            "passwordrecovery/confirm",
                            new { controller = "Customer", action = "PasswordRecoveryConfirm" },                            
                            new[] { "YStory.Web.Controllers" });

            //topics
            routes.MapLocalizedRoute("TopicPopup",
                            "t-popup/{SystemName}",
                            new { controller = "Topic", action = "TopicDetailsPopup" },
                            new[] { "YStory.Web.Controllers" });
            
            //blog
            routes.MapLocalizedRoute("BlogByTag",
                            "blog/tag/{tag}",
                            new { controller = "Blog", action = "BlogByTag" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("BlogByMonth",
                            "blog/month/{month}",
                            new { controller = "Blog", action = "BlogByMonth" },
                            new[] { "YStory.Web.Controllers" });
            //blog RSS
            routes.MapLocalizedRoute("BlogRSS",
                            "blog/rss/{languageId}",
                            new { controller = "Blog", action = "ListRss" },
                            new { languageId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //news RSS
            routes.MapLocalizedRoute("NewsRSS",
                            "news/rss/{languageId}",
                            new { controller = "News", action = "ListRss" },
                            new { languageId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //set review helpfulness (AJAX link)
            routes.MapRoute("SetArticleReviewHelpfulness",
                            "setarticlereviewhelpfulness",
                            new { controller = "Article", action = "SetArticleReviewHelpfulness" },
                            new[] { "YStory.Web.Controllers" });

            //customer account links
            routes.MapLocalizedRoute("CustomerReturnRequests",
                            "returnrequest/history",
                            new { controller = "ReturnRequest", action = "CustomerReturnRequests" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerDownloadableArticles",
                            "customer/downloadablearticles",
                            new { controller = "Customer", action = "DownloadableArticles" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerBackInStockSubscriptions",
                            "backinstocksubscriptions/manage",
                            new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerBackInStockSubscriptionsPaged",
                            "backinstocksubscriptions/manage/{page}",
                            new { controller = "BackInStockSubscription", action = "CustomerSubscriptions", page = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerRewardPoints",
                            "rewardpoints/history",
                            new { controller = "Subscription", action = "CustomerRewardPoints" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerRewardPointsPaged",
                            "rewardpoints/history/page/{page}",
                            new { controller = "Subscription", action = "CustomerRewardPoints" },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerChangePassword",
                            "customer/changepassword",
                            new { controller = "Customer", action = "ChangePassword" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerAvatar",
                            "customer/avatar",
                            new { controller = "Customer", action = "Avatar" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("AccountActivation",
                            "customer/activation",
                            new { controller = "Customer", action = "AccountActivation" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("EmailRevalidation",
                            "customer/revalidateemail",
                            new { controller = "Customer", action = "EmailRevalidation" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerForumSubscriptions",
                            "boards/forumsubscriptions",
                            new { controller = "Boards", action = "CustomerForumSubscriptions" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerForumSubscriptionsPaged",
                            "boards/forumsubscriptions/{page}",
                            new { controller = "Boards", action = "CustomerForumSubscriptions", page = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerAddressEdit",
                            "customer/addressedit/{addressId}",
                            new { controller = "Customer", action = "AddressEdit" },
                            new { addressId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerAddressAdd",
                            "customer/addressadd",
                            new { controller = "Customer", action = "AddressAdd" },
                            new[] { "YStory.Web.Controllers" });
            //customer profile page
            routes.MapLocalizedRoute("CustomerProfile",
                            "profile/{id}",
                            new { controller = "Profile", action = "Index" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("CustomerProfilePaged",
                            "profile/{id}/page/{page}",
                            new { controller = "Profile", action = "Index" },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //subscriptions
            routes.MapLocalizedRoute("SubscriptionDetails",
                            "subscriptiondetails/{subscriptionId}",
                            new { controller = "Subscription", action = "Details" },
                            new { subscriptionId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ShipmentDetails",
                            "subscriptiondetails/shipment/{shipmentId}",
                            new { controller = "Subscription", action = "ShipmentDetails" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ReturnRequest",
                            "returnrequest/{subscriptionId}",
                            new { controller = "ReturnRequest", action = "ReturnRequest" },
                            new { subscriptionId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ReSubscription",
                            "resubscription/{subscriptionId}",
                            new { controller = "Subscription", action = "ReSubscription" },
                            new { subscriptionId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("GetSubscriptionPdfInvoice",
                            "subscriptiondetails/pdf/{subscriptionId}",
                            new { controller = "Subscription", action = "GetPdfInvoice" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PrintSubscriptionDetails",
                            "subscriptiondetails/print/{subscriptionId}",
                            new { controller = "Subscription", action = "PrintSubscriptionDetails" },
                            new[] { "YStory.Web.Controllers" });
            //subscription downloads
            routes.MapRoute("GetDownload",
                            "download/getdownload/{subscriptionItemId}/{agree}",
                            new { controller = "Download", action = "GetDownload", agree = UrlParameter.Optional },
                            new { subscriptionItemId = new GuidConstraint(false) },
                            new[] { "YStory.Web.Controllers" });
            routes.MapRoute("GetLicense",
                            "download/getlicense/{subscriptionItemId}/",
                            new { controller = "Download", action = "GetLicense" },
                            new { subscriptionItemId = new GuidConstraint(false) },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("DownloadUserAgreement",
                            "customer/useragreement/{subscriptionItemId}",
                            new { controller = "Customer", action = "UserAgreement" },
                            new { subscriptionItemId = new GuidConstraint(false) },
                            new[] { "YStory.Web.Controllers" });
            routes.MapRoute("GetSubscriptionNoteFile",
                            "download/subscriptionnotefile/{subscriptionnoteid}",
                            new { controller = "Download", action = "GetSubscriptionNoteFile" },
                            new { subscriptionnoteid = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //contact contributor
            routes.MapLocalizedRoute("ContactContributor",
                            "contactcontributor/{contributorId}",
                            new { controller = "Common", action = "ContactContributor" },
                            new[] { "YStory.Web.Controllers" });
            //apply for contributor account
            routes.MapLocalizedRoute("ApplyContributorAccount",
                            "contributor/apply",
                            new { controller = "Contributor", action = "ApplyContributor" },
                            new[] { "YStory.Web.Controllers" });
            //contributor info
            routes.MapLocalizedRoute("CustomerContributorInfo",
                            "customer/contributorinfo",
                            new { controller = "Contributor", action = "Info" },
                            new[] { "YStory.Web.Controllers" });

            //poll vote AJAX link
            routes.MapLocalizedRoute("PollVote",
                            "poll/vote",
                            new { controller = "Poll", action = "Vote" },
                            new[] { "YStory.Web.Controllers" });

            //comparing articles
            routes.MapLocalizedRoute("RemoveArticleFromCompareList",
                            "comparearticles/remove/{articleId}",
                            new { controller = "Article", action = "RemoveArticleFromCompareList" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ClearCompareList",
                            "clearcomparelist/",
                            new { controller = "Article", action = "ClearCompareList" },
                            new[] { "YStory.Web.Controllers" });

            //new RSS
            routes.MapLocalizedRoute("NewArticlesRSS",
                            "newarticles/rss",
                            new { controller = "Article", action = "NewArticlesRss" },
                            new[] { "YStory.Web.Controllers" });
            
            //get state list by country ID  (AJAX link)
            routes.MapRoute("GetStatesByCountryId",
                            "country/getstatesbycountryid/",
                            new { controller = "Country", action = "GetStatesByCountryId" },
                            new[] { "YStory.Web.Controllers" });

            //EU Cookie law accept button handler (AJAX link)
            routes.MapRoute("EuCookieLawAccept",
                            "eucookielawaccept",
                            new { controller = "Common", action = "EuCookieLawAccept" },
                            new[] { "YStory.Web.Controllers" });

            //authenticate topic AJAX link
            routes.MapLocalizedRoute("TopicAuthenticate",
                            "topic/authenticate",
                            new { controller = "Topic", action = "Authenticate" },
                            new[] { "YStory.Web.Controllers" });

            //article attributes with "upload file" type
            routes.MapLocalizedRoute("UploadFileArticleAttribute",
                            "uploadfilearticleattribute/{attributeId}",
                            new { controller = "ShoppingCart", action = "UploadFileArticleAttribute" },
                            new { attributeId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //checkout attributes with "upload file" type
            routes.MapLocalizedRoute("UploadFileCheckoutAttribute",
                            "uploadfilecheckoutattribute/{attributeId}",
                            new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" },
                            new { attributeId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            //return request with "upload file" tsupport
            routes.MapLocalizedRoute("UploadFileReturnRequest",
                            "uploadfilereturnrequest",
                            new { controller = "ReturnRequest", action = "UploadFileReturnRequest" },
                            new[] { "YStory.Web.Controllers" });

            //forums
            routes.MapLocalizedRoute("ActiveDiscussions",
                            "boards/activediscussions",
                            new { controller = "Boards", action = "ActiveDiscussions" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ActiveDiscussionsPaged",
                            "boards/activediscussions/page/{page}",
                            new { controller = "Boards", action = "ActiveDiscussions", page = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ActiveDiscussionsRSS",
                            "boards/activediscussionsrss",
                            new { controller = "Boards", action = "ActiveDiscussionsRSS" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PostEdit",
                            "boards/postedit/{id}",
                            new { controller = "Boards", action = "PostEdit" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PostDelete",
                            "boards/postdelete/{id}",
                            new { controller = "Boards", action = "PostDelete" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PostCreate",
                            "boards/postcreate/{id}",
                            new { controller = "Boards", action = "PostCreate" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PostCreateQuote",
                            "boards/postcreate/{id}/{quote}",
                            new { controller = "Boards", action = "PostCreate" },
                            new { id = @"\d+", quote = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicEdit",
                            "boards/topicedit/{id}",
                            new { controller = "Boards", action = "TopicEdit" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicDelete",
                            "boards/topicdelete/{id}",
                            new { controller = "Boards", action = "TopicDelete" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicCreate",
                            "boards/topiccreate/{id}",
                            new { controller = "Boards", action = "TopicCreate" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicMove",
                            "boards/topicmove/{id}",
                            new { controller = "Boards", action = "TopicMove" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicWatch",
                            "boards/topicwatch/{id}",
                            new { controller = "Boards", action = "TopicWatch" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicSlug",
                            "boards/topic/{id}/{slug}",
                            new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("TopicSlugPaged",
                            "boards/topic/{id}/{slug}/page/{page}",
                            new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional, page = UrlParameter.Optional },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ForumWatch",
                            "boards/forumwatch/{id}",
                            new { controller = "Boards", action = "ForumWatch" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ForumRSS",
                            "boards/forumrss/{id}",
                            new { controller = "Boards", action = "ForumRSS" },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ForumSlug",
                            "boards/forum/{id}/{slug}",
                            new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ForumSlugPaged",
                            "boards/forum/{id}/{slug}/page/{page}",
                            new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional, page = UrlParameter.Optional },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ForumGroupSlug",
                            "boards/forumgroup/{id}/{slug}",
                            new { controller = "Boards", action = "ForumGroup", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("Search",
                            "boards/search",
                            new { controller = "Boards", action = "Search" },
                            new[] { "YStory.Web.Controllers" });

            //private messages
            routes.MapLocalizedRoute("PrivateMessages",
                            "privatemessages/{tab}",
                            new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesPaged",
                            "privatemessages/{tab}/page/{page}",
                            new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesInbox",
                            "inboxupdate",
                            new { controller = "PrivateMessages", action = "InboxUpdate" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesSent",
                            "sentupdate",
                            new { controller = "PrivateMessages", action = "SentUpdate" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("SendPM",
                            "sendpm/{toCustomerId}",
                            new { controller = "PrivateMessages", action = "SendPM" },
                            new { toCustomerId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("SendPMReply",
                            "sendpm/{toCustomerId}/{replyToMessageId}",
                            new { controller = "PrivateMessages", action = "SendPM" },
                            new { toCustomerId = @"\d+", replyToMessageId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("ViewPM",
                            "viewpm/{privateMessageId}",
                            new { controller = "PrivateMessages", action = "ViewPM" },
                            new { privateMessageId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("DeletePM",
                            "deletepm/{privateMessageId}",
                            new { controller = "PrivateMessages", action = "DeletePM" },
                            new { privateMessageId = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //activate newsletters
            routes.MapLocalizedRoute("NewsletterActivation",
                            "newsletter/subscriptionactivation/{token}/{active}",
                            new { controller = "Newsletter", action = "SubscriptionActivation" },
                            new { token = new GuidConstraint(false) },
                            new[] { "YStory.Web.Controllers" });

            //robots.txt
            routes.MapRoute("robots.txt",
                            "robots.txt",
                            new { controller = "Common", action = "RobotsTextFile" },
                            new[] { "YStory.Web.Controllers" });

            //sitemap (XML)
            routes.MapLocalizedRoute("sitemap.xml",
                            "sitemap.xml",
                            new { controller = "Common", action = "SitemapXml" },
                            new[] { "YStory.Web.Controllers" });
            routes.MapLocalizedRoute("sitemap-indexed.xml",
                            "sitemap-{Id}.xml",
                            new { controller = "Common", action = "SitemapXml" },
                            new { Id = @"\d+" },
                            new[] { "YStory.Web.Controllers" });

            //store closed
            routes.MapLocalizedRoute("StoreClosed",
                            "storeclosed",
                            new { controller = "Common", action = "StoreClosed" },
                            new[] { "YStory.Web.Controllers" });

            //install
            routes.MapRoute("Installation",
                            "install",
                            new { controller = "Install", action = "Index" },
                            new[] { "YStory.Web.Controllers" });
            
            //page not found
            routes.MapLocalizedRoute("PageNotFound",
                            "page-not-found",
                            new { controller = "Common", action = "PageNotFound" },
                            new[] { "YStory.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
