using System;
using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Home
{
    public partial class YourStoryNewsModel : BaseYStoryModel
    {
        public YourStoryNewsModel()
        {
            Items = new List<NewsDetailsModel>();
        }

        public List<NewsDetailsModel> Items { get; set; }
        public bool HasNewItems { get; set; }
        public bool HideAdvertisements { get; set; }

        public class NewsDetailsModel : BaseYStoryModel
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Summary { get; set; }
            public DateTimeOffset PublishDate { get; set; }
        }
    }
}