using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Newsletter
{
    public partial class NewsletterBoxModel : BaseYStoryModel
    {
        public string NewsletterEmail { get; set; }
        public bool AllowToUnsubscribe { get; set; }
    }
}