using System.Web.Mvc;
using YStory.Web.Framework.Security;

namespace YStory.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [YStoryHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}
