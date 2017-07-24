using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class SubscriptionSettingsModel : BaseYStoryModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.IsReSubscriptionAllowed")]
        public bool IsReSubscriptionAllowed { get; set; }
        public bool IsReSubscriptionAllowed_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.MinSubscriptionSubtotalAmount")]
        public decimal MinSubscriptionSubtotalAmount { get; set; }
        public bool MinSubscriptionSubtotalAmount_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.MinSubscriptionSubtotalAmountIncludingTax")]
        public bool MinSubscriptionSubtotalAmountIncludingTax { get; set; }
        public bool MinSubscriptionSubtotalAmountIncludingTax_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.MinSubscriptionTotalAmount")]
        public decimal MinSubscriptionTotalAmount { get; set; }
        public bool MinSubscriptionTotalAmount_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.AutoUpdateSubscriptionTotalsOnEditingSubscription")]
        public bool AutoUpdateSubscriptionTotalsOnEditingSubscription { get; set; }
        public bool AutoUpdateSubscriptionTotalsOnEditingSubscription_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.AnonymousCheckoutAllowed")]
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool AnonymousCheckoutAllowed_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.TermsOfServiceOnShoppingCartPage")]
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        public bool TermsOfServiceOnShoppingCartPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.TermsOfServiceOnSubscriptionConfirmPage")]
        public bool TermsOfServiceOnSubscriptionConfirmPage { get; set; }
        public bool TermsOfServiceOnSubscriptionConfirmPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.OnePageCheckoutEnabled")]
        public bool OnePageCheckoutEnabled { get; set; }
        public bool OnePageCheckoutEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab")]
        public bool OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab { get; set; }
        public bool OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.DisableBillingAddressCheckoutStep")]
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisableBillingAddressCheckoutStep_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.DisableSubscriptionCompletedPage")]
        public bool DisableSubscriptionCompletedPage { get; set; }
        public bool DisableSubscriptionCompletedPage_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.AttachPdfInvoiceToSubscriptionPlacedEmail")]
        public bool AttachPdfInvoiceToSubscriptionPlacedEmail { get; set; }
        public bool AttachPdfInvoiceToSubscriptionPlacedEmail_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.AttachPdfInvoiceToSubscriptionPaidEmail")]
        public bool AttachPdfInvoiceToSubscriptionPaidEmail { get; set; }
        public bool AttachPdfInvoiceToSubscriptionPaidEmail_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.AttachPdfInvoiceToSubscriptionCompletedEmail")]
        public bool AttachPdfInvoiceToSubscriptionCompletedEmail { get; set; }
        public bool AttachPdfInvoiceToSubscriptionCompletedEmail_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestsEnabled")]
        public bool ReturnRequestsEnabled { get; set; }
        public bool ReturnRequestsEnabled_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestsAllowFiles")]
        public bool ReturnRequestsAllowFiles { get; set; }
        public bool ReturnRequestsAllowFiles_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ReturnRequestNumberMask")]
        public string ReturnRequestNumberMask { get; set; }
        public bool ReturnRequestNumberMask_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.NumberOfDaysReturnRequestAvailable")]
        public int NumberOfDaysReturnRequestAvailable { get; set; }
        public bool NumberOfDaysReturnRequestAvailable_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ActivateGiftCardsAfterCompletingSubscription")]
        public bool ActivateGiftCardsAfterCompletingSubscription { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.DeactivateGiftCardsAfterCancellingSubscription")]
        public bool DeactivateGiftCardsAfterCancellingSubscription { get; set; }
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.DeactivateGiftCardsAfterDeletingSubscription")]
        public bool DeactivateGiftCardsAfterDeletingSubscription { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.CompleteSubscriptionWhenDelivered")]
        public bool CompleteSubscriptionWhenDelivered { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.SubscriptionIdent")]
        public int? SubscriptionIdent { get; set; }
        
        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.CustomSubscriptionNumberMask")]
        public string CustomSubscriptionNumberMask { get; set; }
        public bool CustomSubscriptionNumberMask_OverrideForStore { get; set; }

        [YStoryResourceDisplayName("Admin.Configuration.Settings.Subscription.ExportWithArticles")]
        public bool ExportWithArticles { get; set; }
        public bool ExportWithArticles_OverrideForStore { get; set; }
    }
}