using System.Collections.Generic;
using System.Web.Mvc;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Boards
{
    public partial class TopicMoveModel : BaseYStoryEntityModel
    {
        public TopicMoveModel()
        {
            ForumList = new List<SelectListItem>();
        }

        public int ForumSelected { get; set; }
        public string TopicSeName { get; set; }

        public IEnumerable<SelectListItem> ForumList { get; set; }
    }
}