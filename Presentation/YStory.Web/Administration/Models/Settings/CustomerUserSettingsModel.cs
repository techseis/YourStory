﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YStory.Web.Framework;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Models.Settings
{
    public partial class CustomerUserSettingsModel : BaseYStoryModel
    {
        public CustomerUserSettingsModel()
        {
            CustomerSettings = new CustomerSettingsModel();
            AddressSettings = new AddressSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
            ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
        }
        public CustomerSettingsModel CustomerSettings { get; set; }
        public AddressSettingsModel AddressSettings { get; set; }
        public DateTimeSettingsModel DateTimeSettings { get; set; }
        public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

        #region Nested classes

        public partial class CustomerSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.UsernamesEnabled")]
            public bool UsernamesEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowUsersToChangeUsernames")]
            public bool AllowUsersToChangeUsernames { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CheckUsernameAvailabilityEnabled")]
            public bool CheckUsernameAvailabilityEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.UserRegistrationType")]
            public int UserRegistrationType { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowCustomersToUploadAvatars")]
            public bool AllowCustomersToUploadAvatars { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultAvatarEnabled")]
            public bool DefaultAvatarEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ShowCustomersLocation")]
            public bool ShowCustomersLocation { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ShowCustomersJoinDate")]
            public bool ShowCustomersJoinDate { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowViewingProfiles")]
            public bool AllowViewingProfiles { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.NotifyNewCustomerRegistration")]
            public bool NotifyNewCustomerRegistration { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.RequireRegistrationForDownloadableArticles")]
            public bool RequireRegistrationForDownloadableArticles { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.HideDownloadableArticlesTab")]
            public bool HideDownloadableArticlesTab { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.HideBackInStockSubscriptionsTab")]
            public bool HideBackInStockSubscriptionsTab { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CustomerNameFormat")]
            public int CustomerNameFormat { get; set; }
            
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.PasswordMinLength")]
            public int PasswordMinLength { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.UnduplicatedPasswordsNumber")]
            public int UnduplicatedPasswordsNumber { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.PasswordRecoveryLinkDaysValid")]
            public int PasswordRecoveryLinkDaysValid { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultPasswordFormat")]
            public int DefaultPasswordFormat { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.PasswordLifetime")]
            public int PasswordLifetime { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.FailedPasswordAllowedAttempts")]
            public int FailedPasswordAllowedAttempts { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.FailedPasswordLockoutMinutes")]
            public int FailedPasswordLockoutMinutes { get; set; }
            
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.NewsletterEnabled")]
            public bool NewsletterEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault")]
            public bool NewsletterTickedByDefault { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.HideNewsletterBlock")]
            public bool HideNewsletterBlock { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.NewsletterBlockAllowToUnsubscribe")]
            public bool NewsletterBlockAllowToUnsubscribe { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StoreLastVisitedPage")]
            public bool StoreLastVisitedPage { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.EnteringEmailTwice")]
            public bool EnteringEmailTwice { get; set; }


            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.GenderEnabled")]
            public bool GenderEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DateOfBirthEnabled")]
            public bool DateOfBirthEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DateOfBirthRequired")]
            public bool DateOfBirthRequired { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DateOfBirthMinimumAge")]
            [UIHint("Int32Nullable")]
            public int? DateOfBirthMinimumAge { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CityEnabled")]
            public bool CityEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CityRequired")]
            public bool CityRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CountryEnabled")]
            public bool CountryEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.CountryRequired")]
            public bool CountryRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.StateProvinceRequired")]
            public bool StateProvinceRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.FaxRequired")]
            public bool FaxRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AcceptPrivacyPolicyEnabled")]
            public bool AcceptPrivacyPolicyEnabled { get; set; }
        }

        public partial class AddressSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityEnabled")]
            public bool CityEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityRequired")]
            public bool CityRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxRequired")]
            public bool FaxRequired { get; set; }
        }

        public partial class DateTimeSettingsModel : BaseYStoryModel
        {
            public DateTimeSettingsModel()
            {
                AvailableTimeZones = new List<SelectListItem>();
            }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowCustomersToSetTimeZone")]
            public bool AllowCustomersToSetTimeZone { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultStoreTimeZone")]
            public string DefaultStoreTimeZoneId { get; set; }

            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultStoreTimeZone")]
            public IList<SelectListItem> AvailableTimeZones { get; set; }
        }

        public partial class ExternalAuthenticationSettingsModel : BaseYStoryModel
        {
            [YStoryResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationAutoRegisterEnabled")]
            public bool AutoRegisterEnabled { get; set; }
        }
        #endregion
    }
}