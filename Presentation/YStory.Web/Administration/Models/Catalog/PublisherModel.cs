using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Validators.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    [Validator(typeof(PublisherValidator))]
    public partial class PublisherModel : BaseYStoryEntityModel, ILocalizedModel<PublisherLocalizedModel>
    {
        public PublisherModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<PublisherLocalizedModel>();
            AvailablePublisherTemplates = new List<SelectListItem>();

            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.PublisherTemplate")]
        public int PublisherTemplateId { get; set; }
        public IList<SelectListItem> AvailablePublisherTemplates { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [UIHint("Picture")]
        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Picture")]
        public int PictureId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.PageSize")]
        public int PageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.PriceRanges")]
        [AllowHtml]
        public string PriceRanges { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Published")]
        public bool Published { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Deleted")]
        public bool Deleted { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
        
        public IList<PublisherLocalizedModel> Locales { get; set; }


        //ACL (customer roles)
        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        
        //store mapping
        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }


        //discounts
        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Discounts")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }


        #region Nested classes

        public partial class PublisherArticleModel : BaseYStoryEntityModel
        {
            public int PublisherId { get; set; }

            public int ArticleId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Publishers.Articles.Fields.Article")]
            public string ArticleName { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Publishers.Articles.Fields.IsFeaturedArticle")]
            public bool IsFeaturedArticle { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Publishers.Articles.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }
        }

        public partial class AddPublisherArticleModel : BaseYStoryModel
        {
            public AddPublisherArticleModel()
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

            public int PublisherId { get; set; }

            public int[] SelectedArticleIds { get; set; }
        }

        #endregion
    }

    public partial class PublisherLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.Description")]
        [AllowHtml]
        public string Description {get;set;}

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Publishers.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}