using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Models.Common;
using YStory.Admin.Validators.Contributors;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Contributors
{
    [Validator(typeof(ContributorValidator))]
    public partial class ContributorModel : BaseYStoryEntityModel, ILocalizedModel<ContributorLocalizedModel>
    {
        public ContributorModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Address = new AddressModel();

            Locales = new List<ContributorLocalizedModel>();
            AssociatedCustomers = new List<AssociatedCustomerInfo>();
        }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [UIHint("Picture")]
        [YStoryResourceDisplayName("Admin.Contributors.Fields.Picture")]
        public int PictureId { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        public AddressModel Address { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Active")]
        public bool Active { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }
        

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.PageSize")]
        public int PageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        public IList<ContributorLocalizedModel> Locales { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.AssociatedCustomerEmails")]
        public IList<AssociatedCustomerInfo> AssociatedCustomers { get; set; }



        //contributor notes
        [YStoryResourceDisplayName("Admin.Contributors.ContributorNotes.Fields.Note")]
        [AllowHtml]
        public string AddContributorNoteMessage { get; set; }




        #region Nested classes

        public class AssociatedCustomerInfo : BaseYStoryEntityModel
        {
            public string Email { get; set; }
        }


        public partial class ContributorNote : BaseYStoryEntityModel
        {
            public int ContributorId { get; set; }
            [YStoryResourceDisplayName("Admin.Contributors.ContributorNotes.Fields.Note")]
            public string Note { get; set; }
            [YStoryResourceDisplayName("Admin.Contributors.ContributorNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }
        #endregion

    }

    public partial class ContributorLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Contributors.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}