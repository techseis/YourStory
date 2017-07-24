using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Customer
{
    public partial class CustomerNavigationModel : BaseYStoryModel
    {
        public CustomerNavigationModel()
        {
            CustomerNavigationItems = new List<CustomerNavigationItemModel>();
        }

        public IList<CustomerNavigationItemModel> CustomerNavigationItems { get; set; }

        public CustomerNavigationEnum SelectedTab { get; set; }
    }

    public class CustomerNavigationItemModel
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public CustomerNavigationEnum Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum CustomerNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        Subscriptions = 20,
        ChangePassword = 30,
        Avatar = 40,
        ForumSubscriptions = 50,
        ArticleReviews = 60,
        ContributorInfo = 70
    }
}