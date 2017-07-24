using System;
using System.Collections.Generic;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Models.Common;

namespace YStory.Web.Models.Subscription
{
    public partial class CustomerRewardPointsModel : BaseYStoryModel
    {
        public CustomerRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
        }

        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public PagerModel PagerModel { get; set; }
        public int RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public int MinimumRewardPointsBalance { get; set; }
        public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial class RewardPointsHistoryModel : BaseYStoryEntityModel
        {
            [YStoryResourceDisplayName("RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [YStoryResourceDisplayName("RewardPoints.Fields.PointsBalance")]
            public string PointsBalance { get; set; }

            [YStoryResourceDisplayName("RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [YStoryResourceDisplayName("RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}