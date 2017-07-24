using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Customers;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Customers
{
    [Validator(typeof(CustomerRoleValidator))]
    public partial class CustomerRoleModel : BaseYStoryEntityModel
    {
        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.FreeShipping")]
        [AllowHtml]
        public bool FreeShipping { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.TaxExempt")]
        public bool TaxExempt { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.Active")]
        public bool Active { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.SystemName")]
        public string SystemName { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.EnablePasswordLifetime")]
        public bool EnablePasswordLifetime { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithArticle")]
        public int PurchasedWithArticleId { get; set; }

        [YStoryResourceDisplayName("Admin.Customers.CustomerRoles.Fields.PurchasedWithArticle")]
        public string PurchasedWithArticleName { get; set; }


        #region Nested classes

        public partial class AssociateArticleToCustomerRoleModel : BaseYStoryModel
        {
            public AssociateArticleToCustomerRoleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailablePublishers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableContributors = new List<SelectListItem>();
                AvailableArticleTypes = new List<SelectListItem>();
            }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleName")]
            [AllowHtml]
            public string SearchArticleName { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchPublisher")]
            public int SearchPublisherId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchContributor")]
            public int SearchContributorId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchArticleType")]
            public int SearchArticleTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailablePublishers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableContributors { get; set; }
            public IList<SelectListItem> AvailableArticleTypes { get; set; }

            //contributor
            public bool IsLoggedInAsContributor { get; set; }


            public int AssociatedToArticleId { get; set; }
        }
        #endregion
    }
}