using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Subscriptions
{
    public partial class BestsellersReportModel : BaseYStoryModel
    {
        public BestsellersReportModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableSubscriptionStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailablePublishers = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableContributors = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }


        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Store")]
        public int StoreId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.SubscriptionStatus")]
        public int SubscriptionStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Category")]
        public int CategoryId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Publisher")]
        public int PublisherId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.BillingCountry")]
        public int BillingCountryId { get; set; }
        [YStoryResourceDisplayName("Admin.SalesReport.Bestsellers.Contributor")]
        public int ContributorId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableSubscriptionStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }

        public bool IsLoggedInAsContributor { get; set; }
    }
}