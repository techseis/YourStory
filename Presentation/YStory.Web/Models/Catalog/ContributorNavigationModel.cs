using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Catalog
{
    public partial class ContributorNavigationModel : BaseYStoryModel
    {
        public ContributorNavigationModel()
        {
            this.Contributors = new List<ContributorBriefInfoModel>();
        }

        public IList<ContributorBriefInfoModel> Contributors { get; set; }

        public int TotalContributors { get; set; }
    }

    public partial class ContributorBriefInfoModel : BaseYStoryEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
    }
}