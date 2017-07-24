using System.Collections.Generic;

namespace YStory.Web.Framework.Events
{
    /// <summary>
    /// Article search event
    /// </summary>
    public class ArticleSearchEvent
    {
        public string SearchTerm { get; set; }
        public bool SearchInDescriptions { get; set; }
        public IList<int> CategoryIds { get; set; }
        public int PublisherId { get; set; }
        public int WorkingLanguageId { get; set; }
        public int ContributorId { get; set; }
    }
}
