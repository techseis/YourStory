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
    [Validator(typeof(CategoryValidator))]
    public partial class CategoryModel : BaseYStoryEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<CategoryLocalizedModel>();
            AvailableCategoryTemplates = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
        public int CategoryTemplateId { get; set; }
        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.PriceRanges")]
        [AllowHtml]
        public string PriceRanges { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
        
        public IList<CategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        //ACL (customer roles)
        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        
        //store mapping
        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }


        public IList<SelectListItem> AvailableCategories { get; set; }


        //discounts
        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Discounts")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }


        #region Nested classes

        public partial class CategoryArticleModel : BaseYStoryEntityModel
        {
            public int CategoryId { get; set; }

            public int ArticleId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Categories.Articles.Fields.Article")]
            public string ArticleName { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Categories.Articles.Fields.IsFeaturedArticle")]
            public bool IsFeaturedArticle { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Categories.Articles.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }
        }

        public partial class AddCategoryArticleModel : BaseYStoryModel
        {
            public AddCategoryArticleModel()
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

            public int CategoryId { get; set; }

            public int[] SelectedArticleIds { get; set; }
        }

        #endregion
    }

    public partial class CategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description {get;set;}

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}