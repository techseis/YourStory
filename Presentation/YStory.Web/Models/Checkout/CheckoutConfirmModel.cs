using System.Collections.Generic;
using YStory.Web.Framework.Mvc;

namespace YStory.Web.Models.Checkout
{
    public partial class CheckoutConfirmModel : BaseYStoryModel
    {
        public CheckoutConfirmModel()
        {
            Warnings = new List<string>();
        }

        public bool TermsOfServiceOnSubscriptionConfirmPage { get; set; }
        public string MinSubscriptionTotalWarning { get; set; }

        public IList<string> Warnings { get; set; }
    }
}