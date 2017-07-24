﻿using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Common
{
    public partial class UrlRecordModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [YStoryResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [YStoryResourceDisplayName("Admin.System.SeNames.IsActive")]
        public bool IsActive { get; set; }

        [YStoryResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }

        [YStoryResourceDisplayName("Admin.System.SeNames.Details")]
        public string DetailsUrl { get; set; }
    }
}