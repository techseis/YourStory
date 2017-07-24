using System;
using AutoMapper;
using YStory.Admin.Models.Blogs;
using YStory.Admin.Models.Catalog;
using YStory.Admin.Models.Cms;
using YStory.Admin.Models.Common;
using YStory.Admin.Models.Customers;
using YStory.Admin.Models.Directory;
using YStory.Admin.Models.ExternalAuthentication;
using YStory.Admin.Models.Forums;
using YStory.Admin.Models.Localization;
using YStory.Admin.Models.Logging;
using YStory.Admin.Models.Messages;
using YStory.Admin.Models.News;
using YStory.Admin.Models.Subscriptions;
using YStory.Admin.Models.Payments;
using YStory.Admin.Models.Plugins;
using YStory.Admin.Models.Polls;
using YStory.Admin.Models.Settings;
using YStory.Admin.Models.Stores;
using YStory.Admin.Models.Tax;
using YStory.Admin.Models.Templates;
using YStory.Admin.Models.Topics;
using YStory.Admin.Models.Contributors;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Common;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Forums;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Logging;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Messages;
using YStory.Core.Domain.News;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Domain.Polls;
using YStory.Core.Domain.Stores;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Topics;
using YStory.Core.Domain.Contributors;
using YStory.Core.Infrastructure.Mapper;
using YStory.Core.Plugins;
using YStory.Services.Authentication.External;
using YStory.Services.Cms;
using YStory.Services.Payments;
using YStory.Services.Seo;
using YStory.Services.Tax;
using YStory.Web.Framework.Security.Captcha;

namespace YStory.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : IMapperConfiguration
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Mapper configuration action</returns>
        public Action<IMapperConfigurationExpression> GetConfiguration()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            Action<IMapperConfigurationExpression> action = cfg =>
            {
                //address
                cfg.CreateMap<Address, AddressModel>()
                    .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.FormattedCustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CompanyEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CompanyRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryName,
                        mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                    .ForMember(dest => dest.StateProvinceName,
                        mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null))
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressModel, Address>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Country, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

                //countries
                cfg.CreateMap<CountryModel, Country>()
                    .ForMember(dest => dest.StateProvinces, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                cfg.CreateMap<Country, CountryModel>()
                    .ForMember(dest => dest.NumberOfStates,
                        mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //state/provinces
                cfg.CreateMap<StateProvince, StateProvinceModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StateProvinceModel, StateProvince>()
                    .ForMember(dest => dest.Country, mo => mo.Ignore());

                //language
                cfg.CreateMap<Language, LanguageModel>()
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCurrencies, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Search, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LanguageModel, Language>()
                    .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //email account
                cfg.CreateMap<EmailAccount, EmailAccountModel>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore())
                    .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<EmailAccountModel, EmailAccount>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore());
                //message template
                cfg.CreateMap<MessageTemplate, MessageTemplateModel>()
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MessageTemplateModel, MessageTemplate>()
                    .ForMember(dest => dest.DelayPeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //queued email
                cfg.CreateMap<QueuedEmail, QueuedEmailModel>()
                    .ForMember(dest => dest.EmailAccountName,
                        mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.PriorityName, mo => mo.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<QueuedEmailModel, QueuedEmail>()
                    .ForMember(dest => dest.Priority, dt => dt.Ignore())
                    .ForMember(dest => dest.PriorityId, dt => dt.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFilePath, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFileName, mo => mo.Ignore());
                //campaign
                cfg.CreateMap<Campaign, CampaignModel>()
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.TestEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CampaignModel, Campaign>()
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
                //topcis
                cfg.CreateMap<Topic, TopicModel>()
                    .ForMember(dest => dest.AvailableTopicTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Url, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TopicModel, Topic>()
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());

                //category
                cfg.CreateMap<Category, CategoryModel>()
                    .ForMember(dest => dest.AvailableCategoryTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.Breadcrumb, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CategoryModel, Category>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //publisher
                cfg.CreateMap<Publisher, PublisherModel>()
                    .ForMember(dest => dest.AvailablePublisherTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<PublisherModel, Publisher>()
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                ;

                //contributors
                cfg.CreateMap<Contributor, ContributorModel>()
                    .ForMember(dest => dest.AssociatedCustomers, mo => mo.Ignore())
                    .ForMember(dest => dest.Address, mo => mo.Ignore())
                    .ForMember(dest => dest.AddContributorNoteMessage, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ContributorModel, Contributor>()
                    .ForMember(dest => dest.ContributorNotes, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore());

                //articles
                cfg.CreateMap<Article, ArticleModel>()
                    .ForMember(dest => dest.ArticlesTypesSupportedByArticleTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedToArticleId, mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedToArticleName, mo => mo.Ignore())
                    .ForMember(dest => dest.StockQuantityStr, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleTags, mo => mo.Ignore())
                    .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableContributors, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableArticleTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailablePublishers, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableArticleAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.AddPictureModel, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlePictureModels, mo => mo.Ignore())
                    .ForMember(dest => dest.AddSpecificationAttributeModel, mo => mo.Ignore())
                    .ForMember(dest => dest.CopyArticleModel, mo => mo.Ignore())
                    .ForMember(dest => dest.IsLoggedInAsContributor, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.BaseDimensionIn, mo => mo.Ignore())
                    .ForMember(dest => dest.BaseWeightIn, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCategoryIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedPublisherIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDeliveryDates, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableArticleAvailabilityRanges, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableWarehouses, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableBasepriceUnits, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableBasepriceBaseUnits, mo => mo.Ignore())
                    .ForMember(dest => dest.LastStockQuantity, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleEditorSettingsModel, mo => mo.Ignore())
                    .ForMember(dest => dest.StockQuantityHistory, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ArticleModel, Article>()
                    .ForMember(dest => dest.ArticleTags, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.ParentGroupedArticleId, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleType, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.ApprovedRatingSum, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedRatingSum, mo => mo.Ignore())
                    .ForMember(dest => dest.ApprovedTotalReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedTotalReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlePublishers, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlePictures, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSpecificationAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.RecurringCyclePeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.RentalPricePeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleAttributeMappings, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleAttributeCombinations, mo => mo.Ignore())
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //logs
                cfg.CreateMap<Log, LogModel>()
                    .ForMember(dest => dest.CustomerEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LogModel, Log>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                    .ForMember(dest => dest.Customer, mo => mo.Ignore());
                //ActivityLogType
                cfg.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                    .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
                cfg.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ActivityLog, ActivityLogModel>()
                    .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                    .ForMember(dest => dest.CustomerEmail, mo => mo.MapFrom(src => src.Customer.Email))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //currencies
                cfg.CreateMap<Currency, CurrencyModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryExchangeRateCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryStoreCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CurrencyModel, Currency>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                    .ForMember(dest => dest.RoundingType, mo => mo.Ignore());
                //measure weights
                cfg.CreateMap<MeasureWeight, MeasureWeightModel>()
                    .ForMember(dest => dest.IsPrimaryWeight, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MeasureWeightModel, MeasureWeight>();
                //measure dimensions
                cfg.CreateMap<MeasureDimension, MeasureDimensionModel>()
                    .ForMember(dest => dest.IsPrimaryDimension, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MeasureDimensionModel, MeasureDimension>();
                //tax providers
                cfg.CreateMap<ITaxProvider, TaxProviderModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.IsPrimaryTaxProvider, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //tax categories
                cfg.CreateMap<TaxCategory, TaxCategoryModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TaxCategoryModel, TaxCategory>();
                 
                //payment methods
                cfg.CreateMap<IPaymentMethod, PaymentMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplaySubscription, mo => mo.MapFrom(src => src.PluginDescriptor.DisplaySubscription))
                    .ForMember(dest => dest.RecurringPaymentType,
                        mo => mo.MapFrom(src => src.RecurringPaymentType.ToString()))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //external authentication methods
                cfg.CreateMap<IExternalAuthenticationMethod, AuthenticationMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplaySubscription, mo => mo.MapFrom(src => src.PluginDescriptor.DisplaySubscription))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //widgets
                cfg.CreateMap<IWidgetPlugin, WidgetModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplaySubscription, mo => mo.MapFrom(src => src.PluginDescriptor.DisplaySubscription))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //plugins
                cfg.CreateMap<PluginDescriptor, PluginModel>()
                    .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.CanChangeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.IsEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //newsLetter subscriptions
                cfg.CreateMap<NewsLetterSubscription, NewsLetterSubscriptionModel>()
                    .ForMember(dest => dest.StoreName, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<NewsLetterSubscriptionModel, NewsLetterSubscription>()
                    .ForMember(dest => dest.StoreId, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsLetterSubscriptionGuid, mo => mo.Ignore());
                //forums
                cfg.CreateMap<ForumGroup, ForumGroupModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumGroupModel, ForumGroup>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Forums, mo => mo.Ignore());
                cfg.CreateMap<Forum, ForumModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumGroups, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumModel, Forum>()
                    .ForMember(dest => dest.NumTopics, mo => mo.Ignore())
                    .ForMember(dest => dest.NumPosts, mo => mo.Ignore())
                    .ForMember(dest => dest.LastTopicId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostCustomerId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostTime, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumGroup, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
                //blogs
                cfg.CreateMap<BlogPost, BlogPostModel>()
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
                    .ForMember(dest => dest.ApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<BlogPostModel, BlogPost>()
                    .ForMember(dest => dest.BlogComments, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //news
                cfg.CreateMap<NewsItem, NewsItemModel>()
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
                    .ForMember(dest => dest.ApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<NewsItemModel, NewsItem>()
                    .ForMember(dest => dest.NewsComments, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //news
                cfg.CreateMap<Poll, PollModel>()
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<PollModel, Poll>()
                    .ForMember(dest => dest.PollAnswers, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore());
                //customer roles
                cfg.CreateMap<CustomerRole, CustomerRoleModel>()
                    .ForMember(dest => dest.PurchasedWithArticleName, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerRoleModel, CustomerRole>()
                    .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

                //article attributes
                cfg.CreateMap<ArticleAttribute, ArticleAttributeModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ArticleAttributeModel, ArticleAttribute>();
                //specification attributes
                cfg.CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeModel, SpecificationAttribute>()
                    .ForMember(dest => dest.SpecificationAttributeOptions, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                    .ForMember(dest => dest.NumberOfAssociatedArticles, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.EnableColorSquaresRgb, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>()
                    .ForMember(dest => dest.SpecificationAttribute, mo => mo.Ignore());
                //checkout attributes
                cfg.CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
                    .ForMember(dest => dest.AvailableTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionAllowed, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionModel, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionAttributeXml, mo => mo.Ignore())
                    .ForMember(dest => dest.CheckoutAttributeValues, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //customer attributes
                cfg.CreateMap<CustomerAttribute, CustomerAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerAttributeModel, CustomerAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomerAttributeValues, mo => mo.Ignore());
                //address attributes
                cfg.CreateMap<AddressAttribute, AddressAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressAttributeModel, AddressAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.AddressAttributeValues, mo => mo.Ignore());
               
                //stores
                cfg.CreateMap<Store, StoreModel>()
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StoreModel, Store>();

                //Settings
                cfg.CreateMap<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>()
                    .ForMember(dest => dest.AvailableReCaptchaVersions, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<GeneralCommonSettingsModel.CaptchaSettingsModel, CaptchaSettings>()
                    .ForMember(dest => dest.ReCaptchaTheme, mo => mo.Ignore())
                    .ForMember(dest => dest.ReCaptchaLanguage, mo => mo.Ignore());
                cfg.CreateMap<TaxSettings, TaxSettingsModel>()
                    .ForMember(dest => dest.DefaultTaxAddress, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxDisplayTypeValues, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOnValues, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatShopCountries, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.PricesIncludeTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToSelectTaxDisplayType_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxDisplayType_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxSuffix_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxRates_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.HideZeroTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.HideTaxInSubscriptionSummary_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForceTaxExclusionFromSubscriptionSubtotal_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultTaxCategoryId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOn_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOnPickupPointAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultTaxAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingIsTaxable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingPriceIncludesTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingTaxClassId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatShopCountryId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatAllowVatExemption_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatUseWebService_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatAssumeValid_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TaxSettingsModel, TaxSettings>()
                    .ForMember(dest => dest.ActiveTaxProviderSystemName, mo => mo.Ignore())
                    .ForMember(dest => dest.LogErrors, mo => mo.Ignore());
                cfg.CreateMap<NewsSettings, NewsSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutNewNewsComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowNewsOnMainPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MainPageNewsCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsArchivePageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowHeaderRssUrl_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsCommentsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<NewsSettingsModel, NewsSettings>();
                cfg.CreateMap<ForumSettings, ForumSettingsModel>()
                    .ForMember(dest => dest.ForumEditorValues, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RelativeDateTimeFormattingEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCustomersPostCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowGuestsToCreatePosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowGuestsToCreateTopics_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToEditPosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToDeletePosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowPostVoting_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaxVotesPerDay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToManageSubscriptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TopicsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PostsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumEditor_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SignaturesEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowPrivateMessages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowAlertForPM_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutPrivateMessages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsFeedEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsFeedCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumFeedsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumFeedCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchResultsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumSettingsModel, ForumSettings>()
                    .ForMember(dest => dest.TopicSubjectMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.StrippedTopicMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.PostMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.LatestCustomerPostsPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.PrivateMessagesPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumSubscriptionsPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.PMSubjectMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.PMTextMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.HomePageActiveDiscussionsTopicCount, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumSearchTermMinimumLength, mo => mo.Ignore());
                cfg.CreateMap<BlogSettings, BlogSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PostsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutNewBlogComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfTags_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowHeaderRssUrl_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.BlogCommentsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<BlogSettingsModel, BlogSettings>();
                cfg.CreateMap<ContributorSettings, ContributorSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.ContributorsBlockItemsToDisplay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowContributorOnArticleDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToContactContributors_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToApplyForContributorAccount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowSearchByContributor_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowContributorsToEditInfo_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyStoreOwnerAboutContributorInformationChange_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumArticleNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowContributorsToImportArticles_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<ContributorSettingsModel, ContributorSettings>()
                    .ForMember(dest => dest.DefaultContributorPageSizeOptions, mo => mo.Ignore());
               
                cfg.CreateMap<CatalogSettings, CatalogSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowViewUnpublishedArticlePage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayDiscontinuedMessageForUnpublishedArticles_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ShowSkuOnArticleDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowSkuOnCatalogPages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowPublisherPartNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowGtin_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowFreeShippingNotification_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowArticleSorting_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowArticleViewModeChanging_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticlesFromSubcategories_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCategoryArticleNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCategoryArticleNumberIncludingSubcategories_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.CategoryBreadcrumbEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowShareButton_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PageShareCode_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleReviewsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToReviewArticle_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleReviewPossibleOnlyAfterPurchasing_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyStoreOwnerAboutNewArticleReviews_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleReviewsPerStore_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAFriendEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToEmailAFriend_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RecentlyViewedArticlesNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RecentlyViewedArticlesEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewArticlesNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewArticlesEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CompareArticlesEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowBestsellersOnHomepage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfBestsellersOnHomepage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPageArticlesPerPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPagePageSizeOptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSearchAutoCompleteEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSearchAutoCompleteNumberOfArticles_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleImagesInSearchAutoComplete_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSearchTermMinimumLength_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlesAlsoPurchasedEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlesAlsoPurchasedNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfArticleTags_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlesByTagPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlesByTagAllowCustomersToSelectPageSize_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ArticlesByTagPageSizeOptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeShortDescriptionInCompareArticles_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeFullDescriptionInCompareArticles_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PublishersBlockItemsToDisplay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoFooter_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoArticleDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoArticleBoxes_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoSubscriptionDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleReviewsOnAccountPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleReviewsPageSizeOnAccountPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportImportArticleAttributes_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<CatalogSettingsModel, CatalogSettings>()
                    .ForMember(dest => dest.PublishBackArticleWhenCancellingSubscriptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultViewMode, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultArticleRatingValue, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeFeaturedArticlesInNormalLists, mo => mo.Ignore())
                    .ForMember(dest => dest.AjaxProcessAttributeChange, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumBackInStockSubscriptions, mo => mo.Ignore())
                    .ForMember(dest => dest.CompareArticlesNumber, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultCategoryPageSizeOptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultCategoryPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultPublisherPageSizeOptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultPublisherPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSortingEnumDisabled, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleSortingEnumDisplaySubscription, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportImportUseDropdownlistsForAssociatedEntities, mo => mo.Ignore());
                cfg.CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExchangeRate_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinimumRewardPointsToUse_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForRegistration_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForPurchases_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivationDelay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivatePointsImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayHowMuchWillBeEarned_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();
                cfg.CreateMap<SubscriptionSettings, SubscriptionSettingsModel>()
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.SubscriptionIdent, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.IsReSubscriptionAllowed_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinSubscriptionSubtotalAmount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinSubscriptionSubtotalAmountIncludingTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinSubscriptionTotalAmount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AutoUpdateSubscriptionTotalsOnEditingSubscription_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AnonymousCheckoutAllowed_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TermsOfServiceOnShoppingCartPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TermsOfServiceOnSubscriptionConfirmPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.OnePageCheckoutEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsAllowFiles_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfDaysReturnRequestAvailable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisableBillingAddressCheckoutStep_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisableSubscriptionCompletedPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToSubscriptionPlacedEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToSubscriptionPaidEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToSubscriptionCompletedEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestNumberMask_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomSubscriptionNumberMask_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportWithArticles_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<SubscriptionSettingsModel, SubscriptionSettings>()
                    .ForMember(dest => dest.GeneratePdfInvoiceInCustomerLanguage, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsFileMaximumSize, mo => mo.Ignore())
                    .ForMember(dest => dest.MinimumSubscriptionPlacementInterval, mo => mo.Ignore());
                cfg.CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayCartAfterAddingArticle_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayWishlistAfterAddingArticle_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumShoppingCartItems_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumWishlistItems_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MoveItemsFromWishlistToCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CartsSharedBetweenStores_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleImagesOnShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleImagesOnWishList_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowDiscountBox_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowGiftCardBox_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CrossSellsNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailWishlistEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToEmailWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniShoppingCartEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowArticleImagesInMiniShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniShoppingCartArticleNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCartItemEditing_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ShoppingCartSettingsModel, ShoppingCartSettings>()
                    .ForMember(dest => dest.RoundPricesDuringCalculation, mo => mo.Ignore())
                    .ForMember(dest => dest.RenderAssociatedAttributeValueQuantity, mo => mo.Ignore());
                cfg.CreateMap<MediaSettings, MediaSettingsModel>()
                    .ForMember(dest => dest.PicturesStoredIntoDatabase, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleDetailsPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ArticleThumbPictureSizeOnArticleDetailsPage_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedArticlePictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CategoryThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PublisherThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ContributorThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniCartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumImageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MultipleThumbDirectories_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultImageQuality_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ImportArticleImagesUsingHash_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultPictureZoomEnabled_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<MediaSettingsModel, MediaSettings>()
                    .ForMember(dest => dest.ImageSquarePictureSize, mo => mo.Ignore())
                    .ForMember(dest => dest.AutoCompleteSearchThumbPictureSize, mo => mo.Ignore());
                cfg.CreateMap<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerUserSettingsModel.CustomerSettingsModel, CustomerSettings>()
                    .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarMaximumSizeBytes, mo => mo.Ignore())
                    .ForMember(dest => dest.DownloadableArticlesValidateUser, mo => mo.Ignore())
                    .ForMember(dest => dest.OnlineCustomerMinutes, mo => mo.Ignore())
                    .ForMember(dest => dest.SuffixDeletedCustomers, mo => mo.Ignore())
                    .ForMember(dest => dest.DeleteGuestTaskOlderThanMinutes, mo => mo.Ignore());
                cfg.CreateMap<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerUserSettingsModel.AddressSettingsModel, AddressSettings>();
                cfg.CreateMap<ArticleEditorSettings, ArticleEditorSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ArticleEditorSettingsModel, ArticleEditorSettings>();

                //return request reasons
                cfg.CreateMap<ReturnRequestReason, ReturnRequestReasonModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ReturnRequestReasonModel, ReturnRequestReason>();
                //return request actions
                cfg.CreateMap<ReturnRequestAction, ReturnRequestActionModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ReturnRequestActionModel, ReturnRequestAction>();

                //category template
                cfg.CreateMap<CategoryTemplate, CategoryTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CategoryTemplateModel, CategoryTemplate>();
                //publisher template
                cfg.CreateMap<PublisherTemplate, PublisherTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<PublisherTemplateModel, PublisherTemplate>();
                //article template
                cfg.CreateMap<ArticleTemplate, ArticleTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ArticleTemplateModel, ArticleTemplate>();
                //topic template
                cfg.CreateMap<TopicTemplate, TopicTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TopicTemplateModel, TopicTemplate>();

            };
            return action;
        }

        /// <summary>
        /// Subscription of this mapper implementation
        /// </summary>
        public int Subscription
        {
            get { return 0; }
        }
    }
}