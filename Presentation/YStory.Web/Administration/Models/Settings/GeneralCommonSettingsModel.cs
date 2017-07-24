using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;
using YStory.Web.Framework.Security.Captcha;

namespace YStory.Admin.Models.Settings
{
    public partial class GeneralCommonSettingsModel : BaseYStoryModel
    {
        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            FullTextSettings = new FullTextSettingsModel();
            DisplayDefaultMenuItemSettings = new DisplayDefaultMenuItemSettingsModel();
        }

        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public CaptchaSettingsModel CaptchaSettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }
        public DisplayDefaultMenuItemSettingsModel DisplayDefaultMenuItemSettings { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }


        #region Nested classes

        public partial class StoreInformationSettingsModel : BaseYStoryModel
        {
            public StoreInformationSettingsModel()
            {
                this.AvailableStoreThemes = new List<ThemeConfigurationModel>();
            }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreClosed")]
            public bool StoreClosed { get; set; }
            public bool StoreClosed_OverrideForStore { get; set; }
            
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            [AllowHtml]
            public string DefaultStoreTheme { get; set; }
            public bool DefaultStoreTheme_OverrideForStore { get; set; }
            public IList<ThemeConfigurationModel> AvailableStoreThemes { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }
            public bool AllowCustomerToSelectTheme_OverrideForStore { get; set; }

            [UIHint("Picture")]
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Logo")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }
            

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
            public bool DisplayEuCookieLawWarning { get; set; }
            public bool DisplayEuCookieLawWarning_OverrideForStore { get; set; }
            
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
            public string FacebookLink { get; set; }
            public bool FacebookLink_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
            public string TwitterLink { get; set; }
            public bool TwitterLink_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
            public string YoutubeLink { get; set; }
            public bool YoutubeLink_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GooglePlusLink")]
            public string GooglePlusLink { get; set; }
            public bool GooglePlusLink_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
            public bool SubjectFieldOnContactUsForm { get; set; }
            public bool SubjectFieldOnContactUsForm_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
            public bool UseSystemEmailForContactUsForm { get; set; }
            public bool UseSystemEmailForContactUsForm_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
            public bool SitemapEnabled { get; set; }
            public bool SitemapEnabled_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories")]
            public bool SitemapIncludeCategories { get; set; }
            public bool SitemapIncludeCategories_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludePublishers")]
            public bool SitemapIncludePublishers { get; set; }
            public bool SitemapIncludePublishers_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeArticles")]
            public bool SitemapIncludeArticles { get; set; }
            public bool SitemapIncludeArticles_OverrideForStore { get; set; }

            #region Nested classes

            public partial class ThemeConfigurationModel
            {
                public string ThemeName { get; set; }
                public string ThemeTitle { get; set; }
                public string PreviewImageUrl { get; set; }
                public string PreviewText { get; set; }
                public bool SupportRtl { get; set; }
                public bool Selected { get; set; }
            }

            #endregion
        }

        public partial class SeoSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            [NoTrim]
            public string PageTitleSeparator { get; set; }
            public bool PageTitleSeparator_OverrideForStore { get; set; }
            
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public int PageTitleSeoAdjustment { get; set; }
            public bool PageTitleSeoAdjustment_OverrideForStore { get; set; }
            public SelectList PageTitleSeoAdjustmentValues { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }
            public bool DefaultTitle_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }
            public bool DefaultMetaKeywords_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }
            public bool DefaultMetaDescription_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GenerateArticleMetaDescription")]
            [AllowHtml]
            public bool GenerateArticleMetaDescription { get; set; }
            public bool GenerateArticleMetaDescription_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }
            public bool ConvertNonWesternChars_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }
            public bool CanonicalUrlsEnabled_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.WwwRequirement")]
            public int WwwRequirement { get; set; }
            public bool WwwRequirement_OverrideForStore { get; set; }
            public SelectList WwwRequirementValues { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableJsBundling")]
            public bool EnableJsBundling { get; set; }
            public bool EnableJsBundling_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling")]
            public bool EnableCssBundling { get; set; }
            public bool EnableCssBundling_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
            public bool TwitterMetaTags { get; set; }
            public bool TwitterMetaTags_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
            public bool OpenGraphMetaTags { get; set; }
            public bool OpenGraphMetaTags_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CustomHeadTags")]
            [AllowHtml]
            public string CustomHeadTags { get; set; }
            public bool CustomHeadTags_OverrideForStore { get; set; }
        }

        public partial class SecuritySettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages")]
            public bool ForceSslForAllPages { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea")]
            public bool EnableXsrfProtectionForAdminArea { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore")]
            public bool EnableXsrfProtectionForPublicStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
            public bool HoneypotEnabled { get; set; }
        }

        public partial class CaptchaSettingsModel : BaseYStoryModel
        {
            public CaptchaSettingsModel()
            {
                this.AvailableReCaptchaVersions = new List<SelectListItem>();
            }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
            public bool Enabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
            public bool ShowOnLoginPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
            public bool ShowOnRegistrationPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
            public bool ShowOnContactUsPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
            public bool ShowOnEmailWishlistToFriendPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailArticleToFriendPage")]
            public bool ShowOnEmailArticleToFriendPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
            public bool ShowOnBlogCommentPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
            public bool ShowOnNewsCommentPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnArticleReviewPage")]
            public bool ShowOnArticleReviewPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyContributorPage")]
            public bool ShowOnApplyContributorPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
            [AllowHtml]
            public string ReCaptchaPublicKey { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
            [AllowHtml]
            public string ReCaptchaPrivateKey { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion")]
            public ReCaptchaVersion ReCaptchaVersion { get; set; }

            public IList<SelectListItem> AvailableReCaptchaVersions { get; set; }
        }

        public partial class PdfSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
            public bool LetterPageSizeEnabled { get; set; }
            public bool LetterPageSizeEnabled_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
            [UIHint("Picture")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisablePdfInvoicesForPendingSubscriptions")]
            public bool DisablePdfInvoicesForPendingSubscriptions { get; set; }
            public bool DisablePdfInvoicesForPendingSubscriptions_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1")]
            [AllowHtml]
            public string InvoiceFooterTextColumn1 { get; set; }
            public bool InvoiceFooterTextColumn1_OverrideForStore { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2")]
            [AllowHtml]
            public string InvoiceFooterTextColumn2 { get; set; }
            public bool InvoiceFooterTextColumn2_OverrideForStore { get; set; }

        }

        public partial class LocalizationSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage")]
            public bool AutomaticallyDetectLanguage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocalizedPropertiesOnStartup")]
            public bool LoadAllLocalizedPropertiesOnStartup { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllUrlRecordsOnStartup")]
            public bool LoadAllUrlRecordsOnStartup { get; set; }
        }

        public partial class FullTextSettingsModel : BaseYStoryModel
        {
            public bool Supported { get; set; }

            public bool Enabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public int SearchMode { get; set; }
            public SelectList SearchModeValues { get; set; }
        }
        
        public partial class DisplayDefaultMenuItemSettingsModel: BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem")]
            public bool DisplayHomePageMenuItem { get; set; }
            public bool DisplayHomePageMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewArticlesMenuItem")]
            public bool DisplayNewArticlesMenuItem { get; set; }
            public bool DisplayNewArticlesMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayArticleSearchMenuItem")]
            public bool DisplayArticleSearchMenuItem { get; set; }
            public bool DisplayArticleSearchMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem")]
            public bool DisplayCustomerInfoMenuItem { get; set; }
            public bool DisplayCustomerInfoMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
            public bool DisplayBlogMenuItem { get; set; }
            public bool DisplayBlogMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
            public bool DisplayForumsMenuItem { get; set; }
            public bool DisplayForumsMenuItem_OverrideForStore { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
            public bool DisplayContactUsMenuItem { get; set; }
            public bool DisplayContactUsMenuItem_OverrideForStore { get; set; }
        }
        
        #endregion
    }
}