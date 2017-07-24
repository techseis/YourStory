using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using YStory.Admin.Models.Settings;
using YStory.Admin.Validators.Catalog;
using YStory.Web.Framework;
using YStory.Web.Framework.Localization;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Catalog
{
    [Validator(typeof(ArticleValidator))]
    public partial class ArticleModel : BaseYStoryEntityModel, ILocalizedModel<ArticleLocalizedModel>
    {
        public ArticleModel()
        {
            Locales = new List<ArticleLocalizedModel>();
            ArticlePictureModels = new List<ArticlePictureModel>();
            CopyArticleModel = new CopyArticleModel();
            AddPictureModel = new ArticlePictureModel();
            AddSpecificationAttributeModel = new AddArticleSpecificationAttributeModel();
            ArticleEditorSettingsModel = new ArticleEditorSettingsModel();

            AvailableBasepriceUnits = new List<SelectListItem>();
            AvailableBasepriceBaseUnits = new List<SelectListItem>();
            AvailableArticleTemplates = new List<SelectListItem>();
            AvailableTaxCategories = new List<SelectListItem>();
            AvailableDeliveryDates = new List<SelectListItem>();
            AvailableArticleAvailabilityRanges = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableArticleAttributes = new List<SelectListItem>();
            ArticlesTypesSupportedByArticleTemplates = new Dictionary<int, IList<SelectListItem>>();

            AvailableContributors = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            SelectedPublisherIds = new List<int>();
            AvailablePublishers = new List<SelectListItem>();

            SelectedCategoryIds = new List<int>();
            AvailableCategories = new List<SelectListItem>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedDiscountIds = new List<int>();
            AvailableDiscounts = new List<SelectListItem>();
        }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ID")]
        public override int Id { get; set; }

        //picture thumbnail
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleType")]
        public int ArticleTypeId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleType")]
        public string ArticleTypeName { get; set; }


        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AssociatedToArticleName")]
        public int AssociatedToArticleId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AssociatedToArticleName")]
        public string AssociatedToArticleName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleTemplate")]
        public int ArticleTemplateId { get; set; }
        public IList<SelectListItem> AvailableArticleTemplates { get; set; }
        //<article type ID, list of supported article template IDs>
        public Dictionary<int, IList<SelectListItem>> ArticlesTypesSupportedByArticleTemplates { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleTags")]
        public string ArticleTags { get; set; }




        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Sku")]
        [AllowHtml]
        public string Sku { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.PublisherPartNumber")]
        [AllowHtml]
        public string PublisherPartNumber { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.GTIN")]
        [AllowHtml]
        public virtual string Gtin { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsGiftCard")]
        public bool IsGiftCard { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.GiftCardType")]
        public int GiftCardTypeId { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.OverriddenGiftCardAmount")]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenGiftCardAmount { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RequireOtherArticles")]
        public bool RequireOtherArticles { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RequiredArticleIds")]
        public string RequiredArticleIds { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AutomaticallyAddRequiredArticles")]
        public bool AutomaticallyAddRequiredArticles { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DownloadExpirationDays")]
        [UIHint("Int32Nullable")]
        public int? DownloadExpirationDays { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.HasSampleDownload")]
        public bool HasSampleDownload { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.SampleDownload")]
        [UIHint("Download")]
        public int SampleDownloadId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.HasUserAgreement")]
        public bool HasUserAgreement { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.UserAgreementText")]
        [AllowHtml]
        public string UserAgreementText { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsRecurring")]
        public bool IsRecurring { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RecurringCycleLength")]
        public int RecurringCycleLength { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RecurringCyclePeriod")]
        public int RecurringCyclePeriodId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RecurringTotalCycles")]
        public int RecurringTotalCycles { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsRental")]
        public bool IsRental { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RentalPriceLength")]
        public int RentalPriceLength { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.RentalPricePeriod")]
        public int RentalPricePeriodId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsShipEnabled")]
        public bool IsShipEnabled { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsFreeShipping")]
        public bool IsFreeShipping { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ShipSeparately")]
        public bool ShipSeparately { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AdditionalShippingCharge")]
        public decimal AdditionalShippingCharge { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DeliveryDate")]
        public int DeliveryDateId { get; set; }
        public IList<SelectListItem> AvailableDeliveryDates { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices")]
        public bool IsTelecommunicationsOrBroadcastingOrElectronicServices { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ManageInventoryMethod")]
        public int ManageInventoryMethodId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleAvailabilityRange")]
        public int ArticleAvailabilityRangeId { get; set; }
        public IList<SelectListItem> AvailableArticleAvailabilityRanges { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.UseMultipleWarehouses")]
        public bool UseMultipleWarehouses { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.StockQuantity")]
        public int StockQuantity { get; set; }
        public int LastStockQuantity { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.StockQuantity")]
        public string StockQuantityStr { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DisplayStockAvailability")]
        public bool DisplayStockAvailability { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DisplayStockQuantity")]
        public bool DisplayStockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MinStockQuantity")]
        public int MinStockQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.LowStockActivity")]
        public int LowStockActivityId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.NotifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BacksubscriptionMode")]
        public int BacksubscriptionModeId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AllowBackInStockSubscriptions")]
        public bool AllowBackInStockSubscriptions { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.SubscriptionMinimumQuantity")]
        public int SubscriptionMinimumQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.SubscriptionMaximumQuantity")]
        public int SubscriptionMaximumQuantity { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AllowedQuantities")]
        public string AllowedQuantities { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AllowAddingOnlyExistingAttributeCombinations")]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.NotReturnable")]
        public bool NotReturnable { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DisableBuyButton")]
        public bool DisableBuyButton { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DisableWishlistButton")]
        public bool DisableWishlistButton { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AvailableForPreSubscription")]
        public bool AvailableForPreSubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.PreSubscriptionAvailabilityStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? PreSubscriptionAvailabilityStartDateTimeUtc { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.CallForPrice")]
        public bool CallForPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Price")]
        public decimal Price { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ArticleCost")]
        public decimal ArticleCost { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.CustomerEntersPrice")]
        public bool CustomerEntersPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MinimumCustomerEnteredPrice")]
        public decimal MinimumCustomerEnteredPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MaximumCustomerEnteredPrice")]
        public decimal MaximumCustomerEnteredPrice { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BasepriceEnabled")]
        public bool BasepriceEnabled { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BasepriceAmount")]
        public decimal BasepriceAmount { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BasepriceUnit")]
        public int BasepriceUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceUnits { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BasepriceBaseAmount")]
        public decimal BasepriceBaseAmount { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.BasepriceBaseUnit")]
        public int BasepriceBaseUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceBaseUnits { get; set; }


        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MarkAsNew")]
        public bool MarkAsNew { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MarkAsNewStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MarkAsNewEndDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }


        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Weight")]
        public decimal Weight { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Length")]
        public decimal Length { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Width")]
        public decimal Width { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Height")]
        public decimal Height { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AvailableStartDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AvailableEndDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.DisplaySubscription")]
        public int DisplaySubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Published")]
        public bool Published { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.CreatedOn")]
        public DateTime? CreatedOn { get; set; }
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }


        public string PrimaryStoreCurrencyCode { get; set; }
        public string BaseDimensionIn { get; set; }
        public string BaseWeightIn { get; set; }

        public IList<ArticleLocalizedModel> Locales { get; set; }



        //ACL (customer roles)
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //categories
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Categories")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCategoryIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        //publishers
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Publishers")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedPublisherIds { get; set; }
        public IList<SelectListItem> AvailablePublishers { get; set; }

        //contributors
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Contributor")]
        public int ContributorId { get; set; }
        public IList<SelectListItem> AvailableContributors { get; set; }

        //discounts
        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Discounts")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        //contributor
        public bool IsLoggedInAsContributor { get; set; }

        //article attributes
        public IList<SelectListItem> AvailableArticleAttributes { get; set; }
        
        //pictures
        public ArticlePictureModel AddPictureModel { get; set; }
        public IList<ArticlePictureModel> ArticlePictureModels { get; set; }

        //add specification attribute model
        public AddArticleSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }

       
        //copy article
        public CopyArticleModel CopyArticleModel { get; set; }

        //editor settings
        public ArticleEditorSettingsModel ArticleEditorSettingsModel { get; set; }

        //stock quantity history
        public StockQuantityHistoryModel StockQuantityHistory { get; set; }

        #region Nested classes

        public partial class AddRequiredArticleModel : BaseYStoryModel
        {
            public AddRequiredArticleModel()
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
        }

        public partial class AddArticleSpecificationAttributeModel : BaseYStoryModel
        {
            public AddArticleSpecificationAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
                AvailableOptions = new List<SelectListItem>();
            }
            
            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.SpecificationAttribute")]
            public int SpecificationAttributeId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.AttributeType")]
            public int AttributeTypeId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.SpecificationAttributeOption")]
            public int SpecificationAttributeOptionId { get; set; }

            [AllowHtml]
            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.CustomValue")]
            public string CustomValue { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.AllowFiltering")]
            public bool AllowFiltering { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.ShowOnArticlePage")]
            public bool ShowOnArticlePage { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.SpecificationAttributes.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }

            public IList<SelectListItem> AvailableAttributes { get; set; }
            public IList<SelectListItem> AvailableOptions { get; set; }
        }
        
        public partial class ArticlePictureModel : BaseYStoryEntityModel
        {
            public int ArticleId { get; set; }

            [UIHint("Picture")]
            [YStoryResourceDisplayName("Admin.Catalog.Articles.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.Pictures.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }
        }

        public partial class RelatedArticleModel : BaseYStoryEntityModel
        {
            public int ArticleId2 { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.RelatedArticles.Fields.Article")]
            public string Article2Name { get; set; }
            
            [YStoryResourceDisplayName("Admin.Catalog.Articles.RelatedArticles.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }
        }
        public partial class AddRelatedArticleModel : BaseYStoryModel
        {
            public AddRelatedArticleModel()
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

            public int ArticleId { get; set; }

            public int[] SelectedArticleIds { get; set; }

            //contributor
            public bool IsLoggedInAsContributor { get; set; }
        }

        public partial class AssociatedArticleModel : BaseYStoryEntityModel
        {
            [YStoryResourceDisplayName("Admin.Catalog.Articles.AssociatedArticles.Fields.Article")]
            public string ArticleName { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.AssociatedArticles.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }
        }
        public partial class AddAssociatedArticleModel : BaseYStoryModel
        {
            public AddAssociatedArticleModel()
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

            public int ArticleId { get; set; }

            public int[] SelectedArticleIds { get; set; }

            //contributor
            public bool IsLoggedInAsContributor { get; set; }
        }

        public partial class CrossSellArticleModel : BaseYStoryEntityModel
        {
            public int ArticleId2 { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.CrossSells.Fields.Article")]
            public string Article2Name { get; set; }
        }
        public partial class AddCrossSellArticleModel : BaseYStoryModel
        {
            public AddCrossSellArticleModel()
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

            public int ArticleId { get; set; }

            public int[] SelectedArticleIds { get; set; }

            //contributor
            public bool IsLoggedInAsContributor { get; set; }
        }

    
         

        public partial class ArticleAttributeMappingModel : BaseYStoryEntityModel
        {
            public int ArticleId { get; set; }

            public int ArticleAttributeId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Fields.Attribute")]
            public string ArticleAttribute { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Fields.TextPrompt")]
            [AllowHtml]
            public string TextPrompt { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Fields.IsRequired")]
            public bool IsRequired { get; set; }

            public int AttributeControlTypeId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Fields.AttributeControlType")]
            public string AttributeControlType { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }

            public bool ShouldHaveValues { get; set; }
            public int TotalValues { get; set; }

            //validation fields
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules")]
            public bool ValidationRulesAllowed { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.MinLength")]
            [UIHint("Int32Nullable")]
            public int? ValidationMinLength { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.MaxLength")]
            [UIHint("Int32Nullable")]
            public int? ValidationMaxLength { get; set; }
            [AllowHtml]
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.FileAllowedExtensions")]
            public string ValidationFileAllowedExtensions { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.FileMaximumSize")]
            [UIHint("Int32Nullable")]
            public int? ValidationFileMaximumSize { get; set; }
            [AllowHtml]
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.ValidationRules.DefaultValue")]
            public string DefaultValue { get; set; }
            public string ValidationRulesString { get; set; }
            
            //condition
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Condition")]
            public bool ConditionAllowed { get; set; }
            public string ConditionString { get; set; }
        }
        public partial class ArticleAttributeValueListModel : BaseYStoryModel
        {
            public int ArticleId { get; set; }

            public string ArticleName { get; set; }

            public int ArticleAttributeMappingId { get; set; }

            public string ArticleAttributeName { get; set; }
        }
        [Validator(typeof(ArticleAttributeValueModelValidator))]
        public partial class ArticleAttributeValueModel : BaseYStoryEntityModel, ILocalizedModel<ArticleAttributeValueLocalizedModel>
        {
            public ArticleAttributeValueModel()
            {
                ArticlePictureModels = new List<ArticlePictureModel>();
                Locales = new List<ArticleAttributeValueLocalizedModel>();
            }

            public int ArticleAttributeMappingId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AttributeValueType")]
            public int AttributeValueTypeId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AttributeValueType")]
            public string AttributeValueTypeName { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AssociatedArticle")]
            public int AssociatedArticleId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.AssociatedArticle")]
            public string AssociatedArticleName { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Name")]
            [AllowHtml]
            public string Name { get; set; }
            
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.ColorSquaresRgb")]
            [AllowHtml]
            public string ColorSquaresRgb { get; set; }
            public bool DisplayColorSquaresRgb { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.ImageSquaresPicture")]
            [UIHint("Picture")]
            public int ImageSquaresPictureId { get; set; }
            public bool DisplayImageSquaresPicture { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.PriceAdjustment")]
            public decimal PriceAdjustment { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.PriceAdjustment")]
            //used only on the values list page
            public string PriceAdjustmentStr { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.WeightAdjustment")]
            public decimal WeightAdjustment { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.WeightAdjustment")]
            //used only on the values list page
            public string WeightAdjustmentStr { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Cost")]
            public decimal Cost { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.CustomerEntersQty")]
            public bool CustomerEntersQty { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Quantity")]
            public int Quantity { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.IsPreSelected")]
            public bool IsPreSelected { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.DisplaySubscription")]
            public int DisplaySubscription { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Picture")]
            public int PictureId { get; set; }
            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Picture")]
            public string PictureThumbnailUrl { get; set; }

            public IList<ArticlePictureModel> ArticlePictureModels { get; set; }
            public IList<ArticleAttributeValueLocalizedModel> Locales { get; set; }

            #region Nested classes

            public partial class AssociateArticleToAttributeValueModel : BaseYStoryModel
            {
                public AssociateArticleToAttributeValueModel()
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
        public partial class ArticleAttributeValueLocalizedModel : ILocalizedModelLocal
        {
            public int LanguageId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.Attributes.Values.Fields.Name")]
            [AllowHtml]
            public string Name { get; set; }
        }
        public partial class ArticleAttributeCombinationModel : BaseYStoryEntityModel
        {
            public int ArticleId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.Attributes")]
            [AllowHtml]
            public string AttributesXml { get; set; }

            [AllowHtml]
            public string Warnings { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.StockQuantity")]
            public int StockQuantity { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.AllowOutOfStockSubscriptions")]
            public bool AllowOutOfStockSubscriptions { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.Sku")]
            public string Sku { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.PublisherPartNumber")]
            public string PublisherPartNumber { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.Gtin")]
            public string Gtin { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.OverriddenPrice")]
            [UIHint("DecimalNullable")]
            public decimal? OverriddenPrice { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.ArticleAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow")]
            public int NotifyAdminForQuantityBelow { get; set; }

        }

        #region Stock quantity history

        public partial class StockQuantityHistoryModel : BaseYStoryEntityModel
        {
            [YStoryResourceDisplayName("Admin.Catalog.Articles.List.SearchWarehouse")]
            public int SearchWarehouseId { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.Warehouse")]
            [AllowHtml]
            public string WarehouseName { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.Combination")]
            [AllowHtml]
            public string AttributeCombination { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.QuantityAdjustment")]
            public int QuantityAdjustment { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.StockQuantity")]
            public int StockQuantity { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.Message")]
            [AllowHtml]
            public string Message { get; set; }

            [YStoryResourceDisplayName("Admin.Catalog.Articles.StockQuantityHistory.Fields.CreatedOn")]
            [UIHint("DecimalNullable")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion

        #endregion
    }

    public partial class ArticleLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [YStoryResourceDisplayName("Admin.Catalog.Articles.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}