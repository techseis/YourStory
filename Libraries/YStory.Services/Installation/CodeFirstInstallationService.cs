using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain;
using YStory.Core.Domain.Affiliates;
using YStory.Core.Domain.Blogs;
using YStory.Core.Domain.Catalog;
using YStory.Core.Domain.Cms;
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
using YStory.Core.Domain.Payments;
using YStory.Core.Domain.Polls;
using YStory.Core.Domain.Security;
using YStory.Core.Domain.Seo;
using YStory.Core.Domain.Stores;
using YStory.Core.Domain.Tasks;
using YStory.Core.Domain.Tax;
using YStory.Core.Domain.Topics;
using YStory.Core.Domain.Contributors;
using YStory.Core.Infrastructure;
using YStory.Services.Common;
using YStory.Services.Configuration;
using YStory.Services.Customers;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Seo;

namespace YStory.Services.Installation
{
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<ArticleAttribute> _articleAttributeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<RelatedArticle> _relatedArticleRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ArticleTag> _articleTagRepository;
        private readonly IRepository<ArticleTemplate> _articleTemplateRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<PublisherTemplate> _publisherTemplateRepository;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Contributor> _contributorRepository;
        private readonly IRepository<Affiliate> _affiliateRepository;
        private readonly IRepository<Subscription> _subscriptionRepository;
        private readonly IRepository<SubscriptionItem> _subscriptionItemRepository;
        private readonly IRepository<SubscriptionNote> _subscriptionNoteRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<Store> storeRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<Language> languageRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<ArticleAttribute> articleAttributeRepository,
            IRepository<Category> categoryRepository,
            IRepository<Publisher> publisherRepository,
            IRepository<Article> articleRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<RelatedArticle> relatedArticleRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Topic> topicRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<Poll> pollRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ArticleTag> articleTagRepository,
            IRepository<ArticleTemplate> articleTemplateRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<PublisherTemplate> publisherTemplateRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<Address> addressRepository,
            IRepository<Contributor> contributorRepository,
            IRepository<Affiliate> affiliateRepository,
            IRepository<Subscription> subscriptionRepository,
            IRepository<SubscriptionItem> subscriptionItemRepository,
            IRepository<SubscriptionNote> subscriptionNoteRepository,
            IRepository<SearchTerm> searchTermRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._storeRepository = storeRepository;
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
            this._taxCategoryRepository = taxCategoryRepository;
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._customerRepository = customerRepository;
            this._customerPasswordRepository = customerPasswordRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._specificationAttributeRepository = specificationAttributeRepository;
            this._checkoutAttributeRepository = checkoutAttributeRepository;
            this._articleAttributeRepository = articleAttributeRepository;
            this._categoryRepository = categoryRepository;
            this._publisherRepository = publisherRepository;
            this._articleRepository = articleRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._relatedArticleRepository = relatedArticleRepository;
            this._emailAccountRepository = emailAccountRepository;
            this._messageTemplateRepository = messageTemplateRepository;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;
            this._blogPostRepository = blogPostRepository;
            this._topicRepository = topicRepository;
            this._newsItemRepository = newsItemRepository;
            this._pollRepository = pollRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._activityLogRepository = activityLogRepository;
            this._articleTagRepository = articleTagRepository;
            this._articleTemplateRepository = articleTemplateRepository;
            this._categoryTemplateRepository = categoryTemplateRepository;
            this._publisherTemplateRepository = publisherTemplateRepository;
            this._topicTemplateRepository = topicTemplateRepository;
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._returnRequestReasonRepository = returnRequestReasonRepository;
            this._returnRequestActionRepository = returnRequestActionRepository;
            this._addressRepository = addressRepository;
            this._contributorRepository = contributorRepository;
            this._affiliateRepository = affiliateRepository;
            this._subscriptionRepository = subscriptionRepository;
            this._subscriptionItemRepository = subscriptionItemRepository;
            this._subscriptionNoteRepository = subscriptionNoteRepository;
            this._searchTermRepository = searchTermRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplaySubscription = 1,
                    //should we set some default company info?
                    CompanyName = "Your company name",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                    CompanyVat = null,
                },
            };

            _storeRepository.Insert(stores);
        }

        protected virtual void InstallMeasures()
        {
            var measureDimensions = new List<MeasureDimension>
            {
                new MeasureDimension
                {
                    Name = "inch(es)",
                    SystemKeyword = "inches",
                    Ratio = 1M,
                    DisplaySubscription = 1,
                },
                new MeasureDimension
                {
                    Name = "feet",
                    SystemKeyword = "feet",
                    Ratio = 0.08333333M,
                    DisplaySubscription = 2,
                },
                new MeasureDimension
                {
                    Name = "meter(s)",
                    SystemKeyword = "meters",
                    Ratio = 0.0254M,
                    DisplaySubscription = 3,
                },
                new MeasureDimension
                {
                    Name = "millimetre(s)",
                    SystemKeyword = "millimetres",
                    Ratio = 25.4M,
                    DisplaySubscription = 4,
                }
            };

            _measureDimensionRepository.Insert(measureDimensions);

            var measureWeights = new List<MeasureWeight>
            {
                new MeasureWeight
                {
                    Name = "ounce(s)",
                    SystemKeyword = "ounce",
                    Ratio = 16M,
                    DisplaySubscription = 1,
                },
                new MeasureWeight
                {
                    Name = "lb(s)",
                    SystemKeyword = "lb",
                    Ratio = 1M,
                    DisplaySubscription = 2,
                },
                new MeasureWeight
                {
                    Name = "kg(s)",
                    SystemKeyword = "kg",
                    Ratio = 0.45359237M,
                    DisplaySubscription = 3,
                },
                new MeasureWeight
                {
                    Name = "gram(s)",
                    SystemKeyword = "grams",
                    Ratio = 453.59237M,
                    DisplaySubscription = 4,
                }
            };

            _measureWeightRepository.Insert(measureWeights);
        }

        protected virtual void InstallTaxCategories()
        {
            var taxCategories = new List<TaxCategory>
                               {
                                   new TaxCategory
                                       {
                                           Name = "Tax1",
                                           DisplaySubscription = 1,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Tax2",
                                           DisplaySubscription = 5,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Tax3",
                                           DisplaySubscription = 10,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Tax4",
                                           DisplaySubscription = 15,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Tax5",
                                           DisplaySubscription = 20,
                                       },
                               };
            _taxCategoryRepository.Insert(taxCategories);

        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplaySubscription = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = "",
                    Published = true,
                    DisplaySubscription = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 1.36M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.82M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 1.32M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.93M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.95M,
                    DisplayLocale = "",
                    //CustomFormatting = "ˆ0.00",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = true,
                    DisplaySubscription = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.75M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 116.64M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 59.75M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 9.08M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding1
                },
                new Currency
                {
                    Name = "Romanian Leu",
                    CurrencyCode = "RON",
                    Rate = 4.28M,
                    DisplayLocale = "ro-RO",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 11,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Indian Rupee",
                    CurrencyCode = "INR",
                    Rate = 68.17M,
                    DisplayLocale = "en-IN",
                    CustomFormatting = "",
                    Published = false,
                    DisplaySubscription = 12,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
            };
            _currencyRepository.Insert(currencies);
        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                SubjectToVat = false,
                DisplaySubscription = 1,
                Published = true,
            };
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplaySubscription = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplaySubscription = 1,
            });
            var cCanada = new Country
            {
                Name = "Canada",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                SubjectToVat = false,
                DisplaySubscription = 100,
                Published = true,
            };
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplaySubscription = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Yukon Territory",
                Abbreviation = "YT",
                Published = true,
                DisplaySubscription = 1,
            });
            var countries = new List<Country>
                                {
                                    cUsa,
                                    cCanada,
                                    //other countries
                                    new Country
                                    {
                                        Name = "Argentina",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AR",
                                        ThreeLetterIsoCode = "ARG",
                                        NumericIsoCode = 32,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Armenia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AM",
                                        ThreeLetterIsoCode = "ARM",
                                        NumericIsoCode = 51,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Aruba",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AW",
                                        ThreeLetterIsoCode = "ABW",
                                        NumericIsoCode = 533,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Australia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AU",
                                        ThreeLetterIsoCode = "AUS",
                                        NumericIsoCode = 36,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Austria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AT",
                                        ThreeLetterIsoCode = "AUT",
                                        NumericIsoCode = 40,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Azerbaijan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AZ",
                                        ThreeLetterIsoCode = "AZE",
                                        NumericIsoCode = 31,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahamas",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BS",
                                        ThreeLetterIsoCode = "BHS",
                                        NumericIsoCode = 44,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bangladesh",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BD",
                                        ThreeLetterIsoCode = "BGD",
                                        NumericIsoCode = 50,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belarus",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BY",
                                        ThreeLetterIsoCode = "BLR",
                                        NumericIsoCode = 112,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belgium",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BE",
                                        ThreeLetterIsoCode = "BEL",
                                        NumericIsoCode = 56,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belize",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BZ",
                                        ThreeLetterIsoCode = "BLZ",
                                        NumericIsoCode = 84,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bermuda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BM",
                                        ThreeLetterIsoCode = "BMU",
                                        NumericIsoCode = 60,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bolivia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BO",
                                        ThreeLetterIsoCode = "BOL",
                                        NumericIsoCode = 68,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bosnia and Herzegowina",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BA",
                                        ThreeLetterIsoCode = "BIH",
                                        NumericIsoCode = 70,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brazil",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BR",
                                        ThreeLetterIsoCode = "BRA",
                                        NumericIsoCode = 76,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bulgaria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BG",
                                        ThreeLetterIsoCode = "BGR",
                                        NumericIsoCode = 100,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cayman Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KY",
                                        ThreeLetterIsoCode = "CYM",
                                        NumericIsoCode = 136,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chile",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CL",
                                        ThreeLetterIsoCode = "CHL",
                                        NumericIsoCode = 152,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "China",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CN",
                                        ThreeLetterIsoCode = "CHN",
                                        NumericIsoCode = 156,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Colombia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CO",
                                        ThreeLetterIsoCode = "COL",
                                        NumericIsoCode = 170,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Costa Rica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CR",
                                        ThreeLetterIsoCode = "CRI",
                                        NumericIsoCode = 188,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Croatia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HR",
                                        ThreeLetterIsoCode = "HRV",
                                        NumericIsoCode = 191,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cuba",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CU",
                                        ThreeLetterIsoCode = "CUB",
                                        NumericIsoCode = 192,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cyprus",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CY",
                                        ThreeLetterIsoCode = "CYP",
                                        NumericIsoCode = 196,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Czech Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CZ",
                                        ThreeLetterIsoCode = "CZE",
                                        NumericIsoCode = 203,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Denmark",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DK",
                                        ThreeLetterIsoCode = "DNK",
                                        NumericIsoCode = 208,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominican Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DO",
                                        ThreeLetterIsoCode = "DOM",
                                        NumericIsoCode = 214,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "East Timor",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TL",
                                        ThreeLetterIsoCode = "TLS",
                                        NumericIsoCode = 626,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ecuador",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EC",
                                        ThreeLetterIsoCode = "ECU",
                                        NumericIsoCode = 218,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Egypt",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EG",
                                        ThreeLetterIsoCode = "EGY",
                                        NumericIsoCode = 818,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Finland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FI",
                                        ThreeLetterIsoCode = "FIN",
                                        NumericIsoCode = 246,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "France",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FR",
                                        ThreeLetterIsoCode = "FRA",
                                        NumericIsoCode = 250,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Georgia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GE",
                                        ThreeLetterIsoCode = "GEO",
                                        NumericIsoCode = 268,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Germany",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DE",
                                        ThreeLetterIsoCode = "DEU",
                                        NumericIsoCode = 276,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gibraltar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GI",
                                        ThreeLetterIsoCode = "GIB",
                                        NumericIsoCode = 292,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greece",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GR",
                                        ThreeLetterIsoCode = "GRC",
                                        NumericIsoCode = 300,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guatemala",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GT",
                                        ThreeLetterIsoCode = "GTM",
                                        NumericIsoCode = 320,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hong Kong",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HK",
                                        ThreeLetterIsoCode = "HKG",
                                        NumericIsoCode = 344,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hungary",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HU",
                                        ThreeLetterIsoCode = "HUN",
                                        NumericIsoCode = 348,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "India",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IN",
                                        ThreeLetterIsoCode = "IND",
                                        NumericIsoCode = 356,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Indonesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ID",
                                        ThreeLetterIsoCode = "IDN",
                                        NumericIsoCode = 360,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ireland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IE",
                                        ThreeLetterIsoCode = "IRL",
                                        NumericIsoCode = 372,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Israel",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IL",
                                        ThreeLetterIsoCode = "ISR",
                                        NumericIsoCode = 376,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Italy",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IT",
                                        ThreeLetterIsoCode = "ITA",
                                        NumericIsoCode = 380,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jamaica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JM",
                                        ThreeLetterIsoCode = "JAM",
                                        NumericIsoCode = 388,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Japan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JP",
                                        ThreeLetterIsoCode = "JPN",
                                        NumericIsoCode = 392,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jordan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JO",
                                        ThreeLetterIsoCode = "JOR",
                                        NumericIsoCode = 400,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kazakhstan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KZ",
                                        ThreeLetterIsoCode = "KAZ",
                                        NumericIsoCode = 398,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea, Democratic People's Republic of",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KP",
                                        ThreeLetterIsoCode = "PRK",
                                        NumericIsoCode = 408,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kuwait",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KW",
                                        ThreeLetterIsoCode = "KWT",
                                        NumericIsoCode = 414,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malaysia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MY",
                                        ThreeLetterIsoCode = "MYS",
                                        NumericIsoCode = 458,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mexico",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MX",
                                        ThreeLetterIsoCode = "MEX",
                                        NumericIsoCode = 484,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NL",
                                        ThreeLetterIsoCode = "NLD",
                                        NumericIsoCode = 528,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Zealand",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NZ",
                                        ThreeLetterIsoCode = "NZL",
                                        NumericIsoCode = 554,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norway",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NO",
                                        ThreeLetterIsoCode = "NOR",
                                        NumericIsoCode = 578,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pakistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PK",
                                        ThreeLetterIsoCode = "PAK",
                                        NumericIsoCode = 586,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palestine",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PS",
                                        ThreeLetterIsoCode = "PSE",
                                        NumericIsoCode = 275,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Paraguay",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PY",
                                        ThreeLetterIsoCode = "PRY",
                                        NumericIsoCode = 600,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Peru",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PE",
                                        ThreeLetterIsoCode = "PER",
                                        NumericIsoCode = 604,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Philippines",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PH",
                                        ThreeLetterIsoCode = "PHL",
                                        NumericIsoCode = 608,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Poland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PL",
                                        ThreeLetterIsoCode = "POL",
                                        NumericIsoCode = 616,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Portugal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PT",
                                        ThreeLetterIsoCode = "PRT",
                                        NumericIsoCode = 620,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Puerto Rico",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PR",
                                        ThreeLetterIsoCode = "PRI",
                                        NumericIsoCode = 630,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Qatar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "QA",
                                        ThreeLetterIsoCode = "QAT",
                                        NumericIsoCode = 634,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Romania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RO",
                                        ThreeLetterIsoCode = "ROM",
                                        NumericIsoCode = 642,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Russian Federation",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RU",
                                        ThreeLetterIsoCode = "RUS",
                                        NumericIsoCode = 643,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saudi Arabia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SA",
                                        ThreeLetterIsoCode = "SAU",
                                        NumericIsoCode = 682,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Singapore",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SG",
                                        ThreeLetterIsoCode = "SGP",
                                        NumericIsoCode = 702,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovakia (Slovak Republic)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SK",
                                        ThreeLetterIsoCode = "SVK",
                                        NumericIsoCode = 703,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovenia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SI",
                                        ThreeLetterIsoCode = "SVN",
                                        NumericIsoCode = 705,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Africa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZA",
                                        ThreeLetterIsoCode = "ZAF",
                                        NumericIsoCode = 710,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Spain",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ES",
                                        ThreeLetterIsoCode = "ESP",
                                        NumericIsoCode = 724,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sweden",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SE",
                                        ThreeLetterIsoCode = "SWE",
                                        NumericIsoCode = 752,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Switzerland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CH",
                                        ThreeLetterIsoCode = "CHE",
                                        NumericIsoCode = 756,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Taiwan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TW",
                                        ThreeLetterIsoCode = "TWN",
                                        NumericIsoCode = 158,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Thailand",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TH",
                                        ThreeLetterIsoCode = "THA",
                                        NumericIsoCode = 764,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkey",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TR",
                                        ThreeLetterIsoCode = "TUR",
                                        NumericIsoCode = 792,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ukraine",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UA",
                                        ThreeLetterIsoCode = "UKR",
                                        NumericIsoCode = 804,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Arab Emirates",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AE",
                                        ThreeLetterIsoCode = "ARE",
                                        NumericIsoCode = 784,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Kingdom",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GB",
                                        ThreeLetterIsoCode = "GBR",
                                        NumericIsoCode = 826,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United States minor outlying islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UM",
                                        ThreeLetterIsoCode = "UMI",
                                        NumericIsoCode = 581,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uruguay",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UY",
                                        ThreeLetterIsoCode = "URY",
                                        NumericIsoCode = 858,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uzbekistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UZ",
                                        ThreeLetterIsoCode = "UZB",
                                        NumericIsoCode = 860,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Venezuela",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VE",
                                        ThreeLetterIsoCode = "VEN",
                                        NumericIsoCode = 862,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Serbia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RS",
                                        ThreeLetterIsoCode = "SRB",
                                        NumericIsoCode = 688,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Afghanistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AF",
                                        ThreeLetterIsoCode = "AFG",
                                        NumericIsoCode = 4,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Albania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AL",
                                        ThreeLetterIsoCode = "ALB",
                                        NumericIsoCode = 8,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Algeria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DZ",
                                        ThreeLetterIsoCode = "DZA",
                                        NumericIsoCode = 12,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "American Samoa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AS",
                                        ThreeLetterIsoCode = "ASM",
                                        NumericIsoCode = 16,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Andorra",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AD",
                                        ThreeLetterIsoCode = "AND",
                                        NumericIsoCode = 20,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Angola",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AO",
                                        ThreeLetterIsoCode = "AGO",
                                        NumericIsoCode = 24,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Anguilla",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AI",
                                        ThreeLetterIsoCode = "AIA",
                                        NumericIsoCode = 660,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antarctica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AQ",
                                        ThreeLetterIsoCode = "ATA",
                                        NumericIsoCode = 10,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antigua and Barbuda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AG",
                                        ThreeLetterIsoCode = "ATG",
                                        NumericIsoCode = 28,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahrain",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BH",
                                        ThreeLetterIsoCode = "BHR",
                                        NumericIsoCode = 48,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Barbados",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BB",
                                        ThreeLetterIsoCode = "BRB",
                                        NumericIsoCode = 52,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Benin",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BJ",
                                        ThreeLetterIsoCode = "BEN",
                                        NumericIsoCode = 204,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bhutan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BT",
                                        ThreeLetterIsoCode = "BTN",
                                        NumericIsoCode = 64,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Botswana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BW",
                                        ThreeLetterIsoCode = "BWA",
                                        NumericIsoCode = 72,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bouvet Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BV",
                                        ThreeLetterIsoCode = "BVT",
                                        NumericIsoCode = 74,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "British Indian Ocean Territory",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IO",
                                        ThreeLetterIsoCode = "IOT",
                                        NumericIsoCode = 86,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brunei Darussalam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BN",
                                        ThreeLetterIsoCode = "BRN",
                                        NumericIsoCode = 96,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burkina Faso",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BF",
                                        ThreeLetterIsoCode = "BFA",
                                        NumericIsoCode = 854,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burundi",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BI",
                                        ThreeLetterIsoCode = "BDI",
                                        NumericIsoCode = 108,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cambodia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KH",
                                        ThreeLetterIsoCode = "KHM",
                                        NumericIsoCode = 116,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cameroon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CM",
                                        ThreeLetterIsoCode = "CMR",
                                        NumericIsoCode = 120,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cape Verde",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CV",
                                        ThreeLetterIsoCode = "CPV",
                                        NumericIsoCode = 132,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Central African Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CF",
                                        ThreeLetterIsoCode = "CAF",
                                        NumericIsoCode = 140,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chad",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TD",
                                        ThreeLetterIsoCode = "TCD",
                                        NumericIsoCode = 148,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Christmas Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CX",
                                        ThreeLetterIsoCode = "CXR",
                                        NumericIsoCode = 162,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cocos (Keeling) Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CC",
                                        ThreeLetterIsoCode = "CCK",
                                        NumericIsoCode = 166,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Comoros",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KM",
                                        ThreeLetterIsoCode = "COM",
                                        NumericIsoCode = 174,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CG",
                                        ThreeLetterIsoCode = "COG",
                                        NumericIsoCode = 178,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo (Democratic Republic of the)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CD",
                                        ThreeLetterIsoCode = "COD",
                                        NumericIsoCode = 180,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cook Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CK",
                                        ThreeLetterIsoCode = "COK",
                                        NumericIsoCode = 184,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cote D'Ivoire",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CI",
                                        ThreeLetterIsoCode = "CIV",
                                        NumericIsoCode = 384,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Djibouti",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DJ",
                                        ThreeLetterIsoCode = "DJI",
                                        NumericIsoCode = 262,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DM",
                                        ThreeLetterIsoCode = "DMA",
                                        NumericIsoCode = 212,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "El Salvador",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SV",
                                        ThreeLetterIsoCode = "SLV",
                                        NumericIsoCode = 222,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Equatorial Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GQ",
                                        ThreeLetterIsoCode = "GNQ",
                                        NumericIsoCode = 226,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Eritrea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ER",
                                        ThreeLetterIsoCode = "ERI",
                                        NumericIsoCode = 232,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Estonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EE",
                                        ThreeLetterIsoCode = "EST",
                                        NumericIsoCode = 233,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ethiopia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ET",
                                        ThreeLetterIsoCode = "ETH",
                                        NumericIsoCode = 231,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Falkland Islands (Malvinas)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FK",
                                        ThreeLetterIsoCode = "FLK",
                                        NumericIsoCode = 238,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Faroe Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FO",
                                        ThreeLetterIsoCode = "FRO",
                                        NumericIsoCode = 234,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Fiji",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FJ",
                                        ThreeLetterIsoCode = "FJI",
                                        NumericIsoCode = 242,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Guiana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GF",
                                        ThreeLetterIsoCode = "GUF",
                                        NumericIsoCode = 254,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Polynesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PF",
                                        ThreeLetterIsoCode = "PYF",
                                        NumericIsoCode = 258,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Southern Territories",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TF",
                                        ThreeLetterIsoCode = "ATF",
                                        NumericIsoCode = 260,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gabon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GA",
                                        ThreeLetterIsoCode = "GAB",
                                        NumericIsoCode = 266,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gambia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GM",
                                        ThreeLetterIsoCode = "GMB",
                                        NumericIsoCode = 270,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ghana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GH",
                                        ThreeLetterIsoCode = "GHA",
                                        NumericIsoCode = 288,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greenland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GL",
                                        ThreeLetterIsoCode = "GRL",
                                        NumericIsoCode = 304,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Grenada",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GD",
                                        ThreeLetterIsoCode = "GRD",
                                        NumericIsoCode = 308,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guadeloupe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GP",
                                        ThreeLetterIsoCode = "GLP",
                                        NumericIsoCode = 312,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GU",
                                        ThreeLetterIsoCode = "GUM",
                                        NumericIsoCode = 316,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GN",
                                        ThreeLetterIsoCode = "GIN",
                                        NumericIsoCode = 324,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea-bissau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GW",
                                        ThreeLetterIsoCode = "GNB",
                                        NumericIsoCode = 624,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guyana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GY",
                                        ThreeLetterIsoCode = "GUY",
                                        NumericIsoCode = 328,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Haiti",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HT",
                                        ThreeLetterIsoCode = "HTI",
                                        NumericIsoCode = 332,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Heard and Mc Donald Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HM",
                                        ThreeLetterIsoCode = "HMD",
                                        NumericIsoCode = 334,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Honduras",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HN",
                                        ThreeLetterIsoCode = "HND",
                                        NumericIsoCode = 340,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iceland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IS",
                                        ThreeLetterIsoCode = "ISL",
                                        NumericIsoCode = 352,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iran (Islamic Republic of)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IR",
                                        ThreeLetterIsoCode = "IRN",
                                        NumericIsoCode = 364,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iraq",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IQ",
                                        ThreeLetterIsoCode = "IRQ",
                                        NumericIsoCode = 368,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kenya",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KE",
                                        ThreeLetterIsoCode = "KEN",
                                        NumericIsoCode = 404,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kiribati",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KI",
                                        ThreeLetterIsoCode = "KIR",
                                        NumericIsoCode = 296,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KR",
                                        ThreeLetterIsoCode = "KOR",
                                        NumericIsoCode = 410,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kyrgyzstan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KG",
                                        ThreeLetterIsoCode = "KGZ",
                                        NumericIsoCode = 417,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lao People's Democratic Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LA",
                                        ThreeLetterIsoCode = "LAO",
                                        NumericIsoCode = 418,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Latvia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LV",
                                        ThreeLetterIsoCode = "LVA",
                                        NumericIsoCode = 428,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lebanon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LB",
                                        ThreeLetterIsoCode = "LBN",
                                        NumericIsoCode = 422,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lesotho",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LS",
                                        ThreeLetterIsoCode = "LSO",
                                        NumericIsoCode = 426,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liberia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LR",
                                        ThreeLetterIsoCode = "LBR",
                                        NumericIsoCode = 430,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Libyan Arab Jamahiriya",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LY",
                                        ThreeLetterIsoCode = "LBY",
                                        NumericIsoCode = 434,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liechtenstein",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LI",
                                        ThreeLetterIsoCode = "LIE",
                                        NumericIsoCode = 438,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lithuania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LT",
                                        ThreeLetterIsoCode = "LTU",
                                        NumericIsoCode = 440,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Luxembourg",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LU",
                                        ThreeLetterIsoCode = "LUX",
                                        NumericIsoCode = 442,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MO",
                                        ThreeLetterIsoCode = "MAC",
                                        NumericIsoCode = 446,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macedonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MK",
                                        ThreeLetterIsoCode = "MKD",
                                        NumericIsoCode = 807,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Madagascar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MG",
                                        ThreeLetterIsoCode = "MDG",
                                        NumericIsoCode = 450,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malawi",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MW",
                                        ThreeLetterIsoCode = "MWI",
                                        NumericIsoCode = 454,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Maldives",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MV",
                                        ThreeLetterIsoCode = "MDV",
                                        NumericIsoCode = 462,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mali",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ML",
                                        ThreeLetterIsoCode = "MLI",
                                        NumericIsoCode = 466,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malta",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MT",
                                        ThreeLetterIsoCode = "MLT",
                                        NumericIsoCode = 470,
                                        SubjectToVat = true,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Marshall Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MH",
                                        ThreeLetterIsoCode = "MHL",
                                        NumericIsoCode = 584,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Martinique",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MQ",
                                        ThreeLetterIsoCode = "MTQ",
                                        NumericIsoCode = 474,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MR",
                                        ThreeLetterIsoCode = "MRT",
                                        NumericIsoCode = 478,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritius",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MU",
                                        ThreeLetterIsoCode = "MUS",
                                        NumericIsoCode = 480,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mayotte",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "YT",
                                        ThreeLetterIsoCode = "MYT",
                                        NumericIsoCode = 175,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Micronesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FM",
                                        ThreeLetterIsoCode = "FSM",
                                        NumericIsoCode = 583,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Moldova",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MD",
                                        ThreeLetterIsoCode = "MDA",
                                        NumericIsoCode = 498,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Monaco",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MC",
                                        ThreeLetterIsoCode = "MCO",
                                        NumericIsoCode = 492,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mongolia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MN",
                                        ThreeLetterIsoCode = "MNG",
                                        NumericIsoCode = 496,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montenegro",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ME",
                                        ThreeLetterIsoCode = "MNE",
                                        NumericIsoCode = 499,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montserrat",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MS",
                                        ThreeLetterIsoCode = "MSR",
                                        NumericIsoCode = 500,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Morocco",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MA",
                                        ThreeLetterIsoCode = "MAR",
                                        NumericIsoCode = 504,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mozambique",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MZ",
                                        ThreeLetterIsoCode = "MOZ",
                                        NumericIsoCode = 508,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Myanmar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MM",
                                        ThreeLetterIsoCode = "MMR",
                                        NumericIsoCode = 104,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Namibia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NA",
                                        ThreeLetterIsoCode = "NAM",
                                        NumericIsoCode = 516,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nauru",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NR",
                                        ThreeLetterIsoCode = "NRU",
                                        NumericIsoCode = 520,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nepal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NP",
                                        ThreeLetterIsoCode = "NPL",
                                        NumericIsoCode = 524,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands Antilles",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AN",
                                        ThreeLetterIsoCode = "ANT",
                                        NumericIsoCode = 530,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Caledonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NC",
                                        ThreeLetterIsoCode = "NCL",
                                        NumericIsoCode = 540,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nicaragua",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NI",
                                        ThreeLetterIsoCode = "NIC",
                                        NumericIsoCode = 558,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niger",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NE",
                                        ThreeLetterIsoCode = "NER",
                                        NumericIsoCode = 562,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nigeria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NG",
                                        ThreeLetterIsoCode = "NGA",
                                        NumericIsoCode = 566,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niue",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NU",
                                        ThreeLetterIsoCode = "NIU",
                                        NumericIsoCode = 570,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norfolk Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NF",
                                        ThreeLetterIsoCode = "NFK",
                                        NumericIsoCode = 574,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Northern Mariana Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MP",
                                        ThreeLetterIsoCode = "MNP",
                                        NumericIsoCode = 580,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Oman",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "OM",
                                        ThreeLetterIsoCode = "OMN",
                                        NumericIsoCode = 512,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PW",
                                        ThreeLetterIsoCode = "PLW",
                                        NumericIsoCode = 585,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Panama",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PA",
                                        ThreeLetterIsoCode = "PAN",
                                        NumericIsoCode = 591,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Papua New Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PG",
                                        ThreeLetterIsoCode = "PNG",
                                        NumericIsoCode = 598,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pitcairn",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PN",
                                        ThreeLetterIsoCode = "PCN",
                                        NumericIsoCode = 612,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Reunion",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RE",
                                        ThreeLetterIsoCode = "REU",
                                        NumericIsoCode = 638,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Rwanda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RW",
                                        ThreeLetterIsoCode = "RWA",
                                        NumericIsoCode = 646,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Kitts and Nevis",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KN",
                                        ThreeLetterIsoCode = "KNA",
                                        NumericIsoCode = 659,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Lucia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LC",
                                        ThreeLetterIsoCode = "LCA",
                                        NumericIsoCode = 662,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Vincent and the Grenadines",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VC",
                                        ThreeLetterIsoCode = "VCT",
                                        NumericIsoCode = 670,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Samoa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "WS",
                                        ThreeLetterIsoCode = "WSM",
                                        NumericIsoCode = 882,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "San Marino",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SM",
                                        ThreeLetterIsoCode = "SMR",
                                        NumericIsoCode = 674,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sao Tome and Principe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ST",
                                        ThreeLetterIsoCode = "STP",
                                        NumericIsoCode = 678,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Senegal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SN",
                                        ThreeLetterIsoCode = "SEN",
                                        NumericIsoCode = 686,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Seychelles",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SC",
                                        ThreeLetterIsoCode = "SYC",
                                        NumericIsoCode = 690,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sierra Leone",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SL",
                                        ThreeLetterIsoCode = "SLE",
                                        NumericIsoCode = 694,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Solomon Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SB",
                                        ThreeLetterIsoCode = "SLB",
                                        NumericIsoCode = 90,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Somalia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SO",
                                        ThreeLetterIsoCode = "SOM",
                                        NumericIsoCode = 706,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Georgia & South Sandwich Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GS",
                                        ThreeLetterIsoCode = "SGS",
                                        NumericIsoCode = 239,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Sudan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SS",
                                        ThreeLetterIsoCode = "SSD",
                                        NumericIsoCode = 728,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sri Lanka",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LK",
                                        ThreeLetterIsoCode = "LKA",
                                        NumericIsoCode = 144,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Helena",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SH",
                                        ThreeLetterIsoCode = "SHN",
                                        NumericIsoCode = 654,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Pierre and Miquelon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PM",
                                        ThreeLetterIsoCode = "SPM",
                                        NumericIsoCode = 666,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sudan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SD",
                                        ThreeLetterIsoCode = "SDN",
                                        NumericIsoCode = 736,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Suriname",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SR",
                                        ThreeLetterIsoCode = "SUR",
                                        NumericIsoCode = 740,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Svalbard and Jan Mayen Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SJ",
                                        ThreeLetterIsoCode = "SJM",
                                        NumericIsoCode = 744,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Swaziland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SZ",
                                        ThreeLetterIsoCode = "SWZ",
                                        NumericIsoCode = 748,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Syrian Arab Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SY",
                                        ThreeLetterIsoCode = "SYR",
                                        NumericIsoCode = 760,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tajikistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TJ",
                                        ThreeLetterIsoCode = "TJK",
                                        NumericIsoCode = 762,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tanzania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TZ",
                                        ThreeLetterIsoCode = "TZA",
                                        NumericIsoCode = 834,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Togo",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TG",
                                        ThreeLetterIsoCode = "TGO",
                                        NumericIsoCode = 768,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tokelau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TK",
                                        ThreeLetterIsoCode = "TKL",
                                        NumericIsoCode = 772,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tonga",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TO",
                                        ThreeLetterIsoCode = "TON",
                                        NumericIsoCode = 776,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Trinidad and Tobago",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TT",
                                        ThreeLetterIsoCode = "TTO",
                                        NumericIsoCode = 780,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tunisia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TN",
                                        ThreeLetterIsoCode = "TUN",
                                        NumericIsoCode = 788,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkmenistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TM",
                                        ThreeLetterIsoCode = "TKM",
                                        NumericIsoCode = 795,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turks and Caicos Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TC",
                                        ThreeLetterIsoCode = "TCA",
                                        NumericIsoCode = 796,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tuvalu",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TV",
                                        ThreeLetterIsoCode = "TUV",
                                        NumericIsoCode = 798,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uganda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UG",
                                        ThreeLetterIsoCode = "UGA",
                                        NumericIsoCode = 800,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vanuatu",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VU",
                                        ThreeLetterIsoCode = "VUT",
                                        NumericIsoCode = 548,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vatican City State (Holy See)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VA",
                                        ThreeLetterIsoCode = "VAT",
                                        NumericIsoCode = 336,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Viet Nam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VN",
                                        ThreeLetterIsoCode = "VNM",
                                        NumericIsoCode = 704,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (British)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VG",
                                        ThreeLetterIsoCode = "VGB",
                                        NumericIsoCode = 92,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (U.S.)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VI",
                                        ThreeLetterIsoCode = "VIR",
                                        NumericIsoCode = 850,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Wallis and Futuna Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "WF",
                                        ThreeLetterIsoCode = "WLF",
                                        NumericIsoCode = 876,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Western Sahara",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EH",
                                        ThreeLetterIsoCode = "ESH",
                                        NumericIsoCode = 732,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Yemen",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "YE",
                                        ThreeLetterIsoCode = "YEM",
                                        NumericIsoCode = 887,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zambia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZM",
                                        ThreeLetterIsoCode = "ZMB",
                                        NumericIsoCode = 894,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zimbabwe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZW",
                                        ThreeLetterIsoCode = "ZWE",
                                        NumericIsoCode = 716,
                                        SubjectToVat = false,
                                        DisplaySubscription = 100,
                                        Published = true
                                    },
                                };
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.ForumModerators,
            };
            var crRegistered = new CustomerRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered,
            };
            var crGuests = new CustomerRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };
            var crContributors = new CustomerRole
            {
                Name = "Contributors",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Contributors,
            };
            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crForumModerators,
                                    crRegistered,
                                    crGuests,
                                    crContributors
                                };
            _customerRoleRepository.Insert(customerRoles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //admin user
            var adminUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = defaultUserEmail,
                FaxNumber = "",
                Company = "YStory Solutions Ltd",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.BillingAddress = defaultAdminUserAddress;
            adminUser.ShippingAddress = defaultAdminUserAddress;

            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crForumModerators);
            adminUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Smith");

            //set hashed admin password
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));

            //second user
            var secondUserEmail = "steve_gates@yourStory.com";
            var secondUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = secondUserEmail,
                FaxNumber = "",
                Company = "Steve Company",
                Address1 = "750 Bel Air Rd.",
                Address2 = "",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow,
            };
            secondUser.Addresses.Add(defaultSecondUserAddress);
            secondUser.BillingAddress = defaultSecondUserAddress;
            secondUser.ShippingAddress = defaultSecondUserAddress;

            secondUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(secondUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.FirstName, defaultSecondUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.LastName, defaultSecondUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@yourStory.com";
            var thirdUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultThirdUserAddress = new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = thirdUserEmail,
                FaxNumber = "",
                Company = "Holmes Company",
                Address1 = "221B Baker Street",
                Address2 = "",
                City = "London",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow,
            };
            thirdUser.Addresses.Add(defaultThirdUserAddress);
            thirdUser.BillingAddress = defaultThirdUserAddress;
            thirdUser.ShippingAddress = defaultThirdUserAddress;

            thirdUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(thirdUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.FirstName, defaultThirdUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.LastName, defaultThirdUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@yourStory.com";
            var fourthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = fourthUserEmail,
                FaxNumber = "",
                Company = "Pan Company",
                Address1 = "St Katharine’s West 16",
                Address2 = "",
                City = "St Andrews",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fourthUser.Addresses.Add(defaultFourthUserAddress);
            fourthUser.BillingAddress = defaultFourthUserAddress;
            fourthUser.ShippingAddress = defaultFourthUserAddress;

            fourthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(fourthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.FirstName, defaultFourthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.LastName, defaultFourthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fourthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@yourStory.com";
            var fifthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = fifthUserEmail,
                FaxNumber = "",
                Company = "Brenda Company",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = "",
                City = "Ketchikan",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fifthUser.Addresses.Add(defaultFifthUserAddress);
            fifthUser.BillingAddress = defaultFifthUserAddress;
            fifthUser.ShippingAddress = defaultFifthUserAddress;

            fifthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(fifthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.FirstName, defaultFifthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.LastName, defaultFifthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fifthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@yourStory.com";
            var sixthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = sixthUserEmail,
                FaxNumber = "",
                Company = "Terces Company",
                Address1 = "201 1st Avenue South",
                Address2 = "",
                City = "Saskatoon",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Saskatchewan"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "CAN"),
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow,
            };
            sixthUser.Addresses.Add(defaultSixthUserAddress);
            sixthUser.BillingAddress = defaultSixthUserAddress;
            sixthUser.ShippingAddress = defaultSixthUserAddress;

            sixthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(sixthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.FirstName, defaultSixthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.LastName, defaultSixthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = sixthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //search engine (crawler) built-in user
            var searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);


            //built-in user for background tasks
            var backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            backgroundTaskUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void InstallSubscriptions()
        {
             
 
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditDiscount")),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditSpecAttribute")),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewArticleAttribute")),
                Comment = "Added a new article attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteGiftCard")),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSearchTerms()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 34,
                Keyword = "computer",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 30,
                Keyword = "camera",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 27,
                Keyword = "jewelry",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 26,
                Keyword = "shoes",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 19,
                Keyword = "jeans",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 10,
                Keyword = "gift",
                StoreId = defaultStore.Id
            });
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Store name",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                               };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BlogCommentNotification,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BackInStockNotification,
                    Subject = "%Store.Name%. Back in stock notification",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%,{0}<br />{0}Article <a target=\"_blank\" href=\"%BackInStockSubscription.ArticleUrl%\">%BackInStockSubscription.ArticleName%</a> is in stock.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailValidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailRevalidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%!{0}<br />{0}To validate your new email address <a href=\"%Customer.EmailRevalidationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.PrivateMessageNotification,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}You have received a new private message.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerPasswordRecoveryMessage,
                    Subject = "%Store.Name%. Password recovery",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerWelcomeMessage,
                    Subject = "Welcome to %Store.Name%",
                    Body = string.Format("We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{0}<br />{0}<br />{0}You can now take part in the various services we have to offer you. Some of these services include:{0}<br />{0}<br />{0}Permanent Cart - Any articles added to your online cart remain there until you remove them, or check them out.{0}<br />{0}Address Book - We can now deliver your articles to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{0}<br />{0}Subscription History - View your history of purchases that you have made with us.{0}<br />{0}Articles Reviews - Share your opinions on articles with our other customers.{0}<br />{0}<br />{0}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}<br />{0}<br />{0}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumPostMessage,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}<br />{0}<br />{0}Post author: %Forums.PostAuthor%{0}<br />{0}Post body: %Forums.PostBody%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumTopicMessage,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.GiftCardNotification,
                    Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                    Body = string.Format("<p>{0}You have received a gift card for %Store.Name%{0}</p>{0}<p>{0}Dear %GiftCard.RecipientName%,{0}<br />{0}<br />{0}%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href=\"%Store.URL%\"> %Store.Name%</a>{0}</p>{0}<p>{0}You gift card code is %GiftCard.CouponCode%{0}</p>{0}<p>{0}%GiftCard.Message%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerRegisteredNotification,
                    Subject = "%Store.Name%. New customer registration",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new customer registered with your store. Below are the customer's details:{0}<br />{0}Full name: %Customer.FullName%{0}<br />{0}Email: %Customer.Email%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Customer.FullName% has just submitted a new return request. Details are below:{0}<br />{0}Request ID: %ReturnRequest.CustomNumber%{0}<br />{0}Article: %ReturnRequest.Article.Quantity% x Article: %ReturnRequest.Article.Name%{0}<br />{0}Reason for return: %ReturnRequest.Reason%{0}<br />{0}Requested action: %ReturnRequest.RequestedAction%{0}<br />{0}Customer comments:{0}<br />{0}%ReturnRequest.CustomerComment%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestCustomerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%!{0}<br />{0}You have just submitted a new return request. Details are below:{0}<br />{0}Request ID: %ReturnRequest.CustomNumber%{0}<br />{0}Article: %ReturnRequest.Article.Quantity% x Article: %ReturnRequest.Article.Name%{0}<br />{0}Reason for return: %ReturnRequest.Reason%{0}<br />{0}Requested action: %ReturnRequest.RequestedAction%{0}<br />{0}Customer comments:{0}<br />{0}%ReturnRequest.CustomerComment%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsCommentNotification,
                    Subject = "%Store.Name%. New news comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification,
                    Subject = "%Store.Name%. New VAT number is submitted.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:{0}<br />{0}VAT number: %Customer.VatNumber%{0}<br />{0}VAT number status: %Customer.VatNumberStatus%{0}<br />{0}Received name: %VatValidationResult.Name%{0}<br />{0}Received address: %VatValidationResult.Address%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionCancelledCustomerNotification,
                    Subject = "%Store.Name%. Your subscription cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Your subscription has been cancelled. Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a target=\"_blank\" href=\"%Subscription.SubscriptionURLForCustomer%\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionCompletedCustomerNotification,
                    Subject = "%Store.Name%. Your subscription completed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Your subscription has been completed. Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a target=\"_blank\" href=\"%Subscription.SubscriptionURLForCustomer%\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification,
                    Subject = "Your subscription from %Store.Name% has been delivered.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Good news! You subscription has been delivered.{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% Delivered Articles:{0}<br />{0}<br />{0}%Shipment.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPlacedCustomerNotification,
                    Subject = "Subscription receipt from %Store.Name%.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a target=\"_blank\" href=\"%Subscription.SubscriptionURLForCustomer%\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPlacedStoreOwnerNotification,
                    Subject = "%Store.Name%. Purchase Receipt for Subscription #%Subscription.SubscriptionNumber%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Subscription.CustomerFullName% (%Subscription.CustomerEmail%) has just placed an subscription from your store. Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentSentCustomerNotification,
                    Subject = "Your subscription from %Store.Name% has been shipped.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%!,{0}<br />{0}Good news! You subscription has been shipped.{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% Shipped Articles:{0}<br />{0}<br />{0}%Shipment.Article(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ArticleReviewNotification,
                    Subject = "%Store.Name%. New article review.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new article review has been written for article \"%ArticleReview.ArticleName%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Article.Name%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Article.Name% (ID: %Article.ID%) low quantity.{0}<br />{0}<br />{0}Quantity: %Article.StockQuantity%{0}<br />{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Article.Name%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Article.Name% (ID: %Article.ID%) low quantity.{0}<br />{0}%AttributeCombination.Formatted%{0}<br />{0}Quantity: %AttributeCombination.StockQuantity%{0}<br />{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification,
                    Subject = "%Store.Name%. Return request status was changed.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%,{0}<br />{0}Your return request #%ReturnRequest.CustomNumber% status has been changed.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.EmailAFriendMessage,
                    Subject = "%Store.Name%. Referred Item",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{0}<br />{0}<br />{0}<b><a target=\"_blank\" href=\"%Article.ArticleURLForCustomer%\">%Article.Name%</a></b>{0}<br />{0}%Article.ShortDescription%{0}<br />{0}<br />{0}For more info click <a target=\"_blank\" href=\"%Article.ArticleURLForCustomer%\">here</a>{0}<br />{0}<br />{0}<br />{0}%EmailAFriend.PersonalMessage%{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.WishlistToFriendMessage,
                    Subject = "%Store.Name%. Wishlist",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you.{0}<br />{0}<br />{0}<br />{0}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForCustomer%\">here</a>{0}<br />{0}<br />{0}<br />{0}%Wishlist.PersonalMessage%{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewSubscriptionNoteAddedCustomerNotification,
                    Subject = "%Store.Name%. New subscription note has been added",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%,{0}<br />{0}New subscription note has been added to your account:{0}<br />{0}\"%Subscription.NewNoteText%\".{0}<br />{0}<a target=\"_blank\" href=\"%Subscription.SubscriptionURLForCustomer%\">%Subscription.SubscriptionURLForCustomer%</a>{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%if (%RecurringPayment.CancelAfterFailedPayment%) The last payment for the recurring payment ID=%RecurringPayment.ID% failed, so it was cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) %Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%,{0}<br />{0}%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn't go through for this recurring payment (<a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>){0}<br />{0}So your subscription has been canceled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification,
                    Subject = "%Store.Name%. Last recurring payment failed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%,{0}<br />{0}It appears your credit card didn't go through for this recurring payment (<a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>){0}<br /> %if (%RecurringPayment.RecurringPaymentType% == \"Manual\") {0}You can recharge balance and manually retry payment or cancel it on the subscription history page. endif% %if (%RecurringPayment.RecurringPaymentType% == \"Automatic\") {0}You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the subscription history page. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPlacedContributorNotification,
                    Subject = "%Store.Name%. Subscription placed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Customer.FullName% (%Customer.Email%) has just placed an subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}%Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionRefundedCustomerNotification,
                    Subject = "%Store.Name%. Subscription #%Subscription.SubscriptionNumber% refunded",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Subscription #%Subscription.SubscriptionNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.{0}<br />{0}<br />{0}Amount refunded: %Subscription.AmountRefunded%{0}<br />{0}<br />{0}Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br /{0}>Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionRefundedStoreOwnerNotification,
                    Subject = "%Store.Name%. Subscription #%Subscription.SubscriptionNumber% refunded",
                    Body = string.Format("%Store.Name%. Subscription #%Subscription.SubscriptionNumber% refunded', N'{0}<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Subscription #%Subscription.SubscriptionNumber% has been just refunded{0}<br />{0}<br />{0}Amount refunded: %Subscription.AmountRefunded%{0}<br />{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPaidStoreOwnerNotification,
                    Subject = "%Store.Name%. Subscription #%Subscription.SubscriptionNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Subscription #%Subscription.SubscriptionNumber% has been just paid{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPaidCustomerNotification,
                    Subject = "%Store.Name%. Subscription #%Subscription.SubscriptionNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Subscription.CustomerFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Subscription #%Subscription.SubscriptionNumber% has been just paid. Below is the summary of the subscription.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Subscription Details: <a href=\"%Subscription.SubscriptionURLForCustomer%\" target=\"_blank\">%Subscription.SubscriptionURLForCustomer%</a>{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Subscription.BillingFirstName% %Subscription.BillingLastName%{0}<br />{0}%Subscription.BillingAddress1%{0}<br />{0}%Subscription.BillingCity% %Subscription.BillingZipPostalCode%{0}<br />{0}%Subscription.BillingStateProvince% %Subscription.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Subscription.Shippable%) Shipping Address{0}<br />{0}%Subscription.ShippingFirstName% %Subscription.ShippingLastName%{0}<br />{0}%Subscription.ShippingAddress1%{0}<br />{0}%Subscription.ShippingCity% %Subscription.ShippingZipPostalCode%{0}<br />{0}%Subscription.ShippingStateProvince% %Subscription.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Subscription.ShippingMethod%{0}<br />{0}<br />{0} endif% %Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.SubscriptionPaidContributorNotification,
                    Subject = "%Store.Name%. Subscription #%Subscription.SubscriptionNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Subscription #%Subscription.SubscriptionNumber% has been just paid.{0}<br />{0}<br />{0}Subscription Number: %Subscription.SubscriptionNumber%{0}<br />{0}Date Subscriptioned: %Subscription.CreatedOn%{0}<br />{0}<br />{0}%Subscription.Article(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewContributorAccountApplyStoreOwnerNotification,
                    Subject = "%Store.Name%. New contributor account submitted.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Customer.FullName% (%Customer.Email%) has just submitted for a contributor account. Details are below:{0}<br />{0}Contributor name: %Contributor.Name%{0}<br />{0}Contributor email: %Contributor.Email%{0}<br />{0}<br />{0}You can activate it in admin area.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContributorInformationChangeNotification,
                    Subject = "%Store.Name%. Contributor information change.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Contributor %Contributor.Name% (%Contributor.Email%) has just changed information about itself.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = string.Format("<p>{0}%ContactUs.Body%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactContributorMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = string.Format("<p>{0}%ContactUs.Body%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallTopics()
        {
            var defaultTopicTemplate =
                _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template");
            if (defaultTopicTemplate == null)
                throw new Exception("Topic template cannot be loaded");

            var topics = new List<Topic>
                               {
                                   new Topic
                                       {
                                           SystemName = "AboutUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplaySubscription = 20,
                                           Published = true,
                                           Title = "About us",
                                           Body = "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "CheckoutAsGuestOrRegister",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your subscription history and status</li></ul>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ConditionsOfUse",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplaySubscription = 15,
                                           Published = true,
                                           Title = "Conditions of Use",
                                           Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ContactUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ForumWelcomeMessage",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "Forums",
                                           Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "HomePageText",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "Welcome to our store",
                                           Body = "<p>Online shopping is the process consumers go through to purchase articles or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://www.yourstory.com/documentation.aspx\">Documentation</a>, or post in the <a href=\"http://www.yourstory.com/boards/\">Forums</a> at <a href=\"http://www.yourstory.com\">yourStory.com</a></p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "LoginRegistrationInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "About login / registration",
                                           Body = "<p>Put your login / registration information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PrivacyInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplaySubscription = 10,
                                           Published = true,
                                           Title = "Privacy notice",
                                           Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PageNotFound",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ShippingInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplaySubscription = 5,
                                           Published = true,
                                           Title = "Shipping & returns",
                                           Body = "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ApplyContributor",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplaySubscription = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p>Put your apply contributor instructions here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                               };
            _topicRepository.Insert(topics);


            //search engine names
            foreach (var topic in topics)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = topic.Id,
                    EntityName = "Topic",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = topic.ValidateSeName("", !String.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName, true)
                });
            }

        }

        protected virtual void InstallSettings(bool installSampleData)
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                RenderSubscriptionNotes = true,
                FontFileName = "FreeSerif.ttf",
                InvoiceFooterTextColumn1 = null,
                InvoiceFooterTextColumn2 = null,
            });

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProceduresIfSupported = true,
                UseStoredProcedureForLoadingCategories = false,
                SitemapEnabled = true,
                SitemapIncludeCategories = true,
                SitemapIncludePublishers = true,
                SitemapIncludeArticles = false,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "Your store",
                DefaultMetaKeywords = "",
                DefaultMetaDescription = "",
                GenerateArticleMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedarticles",
                    "newarticles",
                    "comparearticles",
                    "clearcomparelist",
                    "setarticlereviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "cart",
                    "wishlist",
                    "emailwishlist",
                    "checkout",
                    "onepagecheckout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = ""
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                UseRichEditorInMessageTemplates = false,
                UseIsoDateTimeConverterInJson = true
            });


            settingService.SaveSetting(new ArticleEditorSettings
            {
                Weight = true,
                Dimensions = true,
                ArticleAttributes = true,
                SpecificationAttributes =true
            });

            settingService.SaveSetting(new CatalogSettings
            {
                AllowViewUnpublishedArticlePage = true,
                DisplayDiscontinuedMessageForUnpublishedArticles = true,
                PublishBackArticleWhenCancellingSubscriptions = false,
                ShowSkuOnArticleDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                ShowPublisherPartNumber = false,
                ShowGtin = false,
                ShowFreeShippingNotification = true,
                AllowArticleSorting = true,
                AllowArticleViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowArticlesFromSubcategories = false,
                ShowCategoryArticleNumber = false,
                ShowCategoryArticleNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script type=\"text/javascript\" src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                ArticleReviewsMustBeApproved = false,
                DefaultArticleRatingValue = 5,
                AllowAnonymousUsersToReviewArticle = false,
                ArticleReviewPossibleOnlyAfterPurchasing = false,
                NotifyStoreOwnerAboutNewArticleReviews = false,
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedArticlesNumber = 3,
                RecentlyViewedArticlesEnabled = true,
                NewArticlesNumber = 6,
                NewArticlesEnabled = true,
                CompareArticlesEnabled = true,
                CompareArticlesNumber = 4,
                ArticleSearchAutoCompleteEnabled = true,
                ArticleSearchAutoCompleteNumberOfArticles = 10,
                ArticleSearchTermMinimumLength = 3,
                ShowArticleImagesInSearchAutoComplete = false,
                ShowBestsellersOnHomepage = false,
                NumberOfBestsellersOnHomepage = 4,
                SearchPageArticlesPerPage = 6,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                ArticlesAlsoPurchasedEnabled = true,
                ArticlesAlsoPurchasedNumber = 4,
                AjaxProcessAttributeChange = true,
                NumberOfArticleTags = 15,
                ArticlesByTagPageSize = 6,
                IncludeShortDescriptionInCompareArticles = false,
                IncludeFullDescriptionInCompareArticles = false,
                IncludeFeaturedArticlesInNormalLists = false,
                IgnoreFeaturedArticles = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                CacheArticlePrices = false,
                ArticlesByTagAllowCustomersToSelectPageSize = true,
                ArticlesByTagPageSizeOptions = "6, 3, 9, 18",
                MaximumBackInStockSubscriptions = 200,
                PublishersBlockItemsToDisplay = 2,
                DisplayTaxShippingInfoFooter = false,
                DisplayTaxShippingInfoArticleDetailsPage = false,
                DisplayTaxShippingInfoArticleBoxes = false,
                DisplayTaxShippingInfoShoppingCart = false,
                DisplayTaxShippingInfoWishlist = false,
                DisplayTaxShippingInfoSubscriptionDetailsPage = false,
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                DefaultPublisherPageSizeOptions = "6, 3, 9",
                DefaultPublisherPageSize = 6,
                ShowArticleReviewsTabOnAccountPage = true,
                ArticleReviewsPageSizeOnAccountPage = 10,
                ExportImportArticleAttributes = true,
                ExportImportUseDropdownlistsForAssociatedEntities = true
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new CustomerSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA1",
                PasswordMinLength = 6,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowCustomersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                HideDownloadableArticlesTab = false,
                HideBackInStockSubscriptionsTab = false,
                DownloadableArticlesValidateUser = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                NewsletterEnabled = true,
                NewsletterTickedByDefault = true,
                HideNewsletterBlock = false,
                NewsletterBlockAllowToUnsubscribe = false,
                OnlineCustomerMinutes = 20,
                StoreLastVisitedPage = false,
                SuffixDeletedCustomers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableArticles = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true,
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ArticleThumbPictureSize = 415,
                ArticleDetailsPictureSize = 550,
                ArticleThumbPictureSizeOnArticleDetailsPage = 100,
                AssociatedArticlePictureSize = 220,
                CategoryThumbPictureSize = 450,
                PublisherThumbPictureSize = 420,
                ContributorThumbPictureSize = 450,
                CartThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportArticleImagesUsingHash = true,
                AzureCacheControlHeader = string.Empty
            });

            settingService.SaveSetting(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowCustomerToSelectTheme = false,
                DisplayMiniProfilerInPublicStore = false,
                DisplayMiniProfilerForAdminOnly = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/yourStory",
                TwitterLink = "https://twitter.com/yourStory",
                YoutubeLink = "http://www.youtube.com/user/yourStory",
                GooglePlusLink = "https://plus.google.com/+yourstory",
                HidePoweredByYourStory = false
            });

            settingService.SaveSetting(new ExternalAuthenticationSettings
            {
                AutoRegisterEnabled = true,
                RequireEmailValidation = false
            });

            settingService.SaveSetting(new RewardPointsSettings
            {
                Enabled = true,
                ExchangeRate = 1,
                PointsForRegistration = 0,
                PointsForPurchases_Amount = 10,
                PointsForPurchases_Points = 1,
                ActivationDelay = 0,
                ActivationDelayPeriodId = 0,
                DisplayHowMuchWillBeEarned = true,
                PointsAccumulatedForAllStores = true,
                PageSize = 10
            });

            settingService.SaveSetting(new CurrencySettings
            {
                DisplayCurrencyLabel = false,
                PrimaryStoreCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                ActiveExchangeRateProviderSystemName = "CurrencyExchange.MoneyConverter",
                AutoUpdateEnabled = false
            });

            settingService.SaveSetting(new MeasureSettings
            {
                BaseDimensionId = _measureDimensionRepository.Table.Single(m => m.SystemKeyword == "inches").Id,
                BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == "lb").Id,
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6",
            });

            settingService.SaveSetting(new ShoppingCartSettings
            {
                DisplayCartAfterAddingArticle = false,
                DisplayWishlistAfterAddingArticle = false,
                MaximumShoppingCartItems = 1000,
                MaximumWishlistItems = 1000,
                AllowOutOfStockItemsToBeAddedToWishlist = false,
                MoveItemsFromWishlistToCart = true,
                CartsSharedBetweenStores = false,
                ShowArticleImagesOnShoppingCart = true,
                ShowArticleImagesOnWishList = true,
                ShowDiscountBox = true,
                ShowGiftCardBox = true,
                CrossSellsNumber = 4,
                EmailWishlistEnabled = true,
                AllowAnonymousUsersToEmailWishlist = false,
                MiniShoppingCartEnabled = true,
                ShowArticleImagesInMiniShoppingCart = true,
                MiniShoppingCartArticleNumber = 5,
                RoundPricesDuringCalculation = true,
                AllowCartItemEditing = true,
                RenderAssociatedAttributeValueQuantity = true
            });

            settingService.SaveSetting(new SubscriptionSettings
            {
                ReturnRequestNumberMask = "{ID}",
                IsReSubscriptionAllowed = true,
                MinSubscriptionSubtotalAmount = 0,
                MinSubscriptionSubtotalAmountIncludingTax = false,
                MinSubscriptionTotalAmount = 0,
                AutoUpdateSubscriptionTotalsOnEditingSubscription = false,
                AnonymousCheckoutAllowed = true,
                TermsOfServiceOnShoppingCartPage = true,
                TermsOfServiceOnSubscriptionConfirmPage = false,
                OnePageCheckoutEnabled = true,
                OnePageCheckoutDisplaySubscriptionTotalsOnPaymentInfoTab = false,
                DisableBillingAddressCheckoutStep = false,
                DisableSubscriptionCompletedPage = false,
                AttachPdfInvoiceToSubscriptionPlacedEmail = false,
                AttachPdfInvoiceToSubscriptionCompletedEmail = false,
                GeneratePdfInvoiceInCustomerLanguage = true,
                AttachPdfInvoiceToSubscriptionPaidEmail = false,
                ReturnRequestsEnabled = true,
                ReturnRequestsAllowFiles = false,
                ReturnRequestsFileMaximumSize = 2048,
                NumberOfDaysReturnRequestAvailable = 365,
                MinimumSubscriptionPlacementInterval = 30,
                ActivateGiftCardsAfterCompletingSubscription = false,
                DeactivateGiftCardsAfterCancellingSubscription = false,
                DeactivateGiftCardsAfterDeletingSubscription = false,
                CompleteSubscriptionWhenDelivered = true,
                CustomSubscriptionNumberMask = "{ID}",
                ExportWithArticles = true
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = false,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                EnableXsrfProtectionForAdminArea = true,
                EnableXsrfProtectionForPublicStore = true,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput"
            });

             

            settingService.SaveSetting(new PaymentSettings
            {
                ActivePaymentMethodSystemNames = new List<string>
                    {
                        "Payments.CheckMoneySubscription",
                        "Payments.Manual",
                        "Payments.PayInStore",
                        "Payments.PurchaseSubscription",
                    },
                AllowRePostingPayments = true,
                BypassPaymentMethodSelectionIfOnlyOne = true,
                ShowPaymentMethodDescriptions = true,
                SkipPaymentInfoStepForRedirectionPaymentMethods = false,
                CancelRecurringPaymentsAfterFailedPayment = false
            });

            settingService.SaveSetting(new TaxSettings
            {
                TaxBasedOn = TaxBasedOn.BillingAddress,
                TaxBasedOnPickupPointAddress = false,
                TaxDisplayType = TaxDisplayType.ExcludingTax,
                ActiveTaxProviderSystemName = "Tax.FixedOrByCountryStateZip",
                DefaultTaxAddressId = 0,
                DisplayTaxSuffix = false,
                DisplayTaxRates = false,
                PricesIncludeTax = false,
                AllowCustomersToSelectTaxDisplayType = false,
                ForceTaxExclusionFromSubscriptionSubtotal = false,
                DefaultTaxCategoryId = 0,
                HideZeroTax = false,
                HideTaxInSubscriptionSummary = false,
                ShippingIsTaxable = false,
                ShippingPriceIncludesTax = false,
                ShippingTaxClassId = 0,
                PaymentMethodAdditionalFeeIsTaxable = false,
                PaymentMethodAdditionalFeeIncludesTax = false,
                PaymentMethodAdditionalFeeTaxClassId = 0,
                EuVatEnabled = false,
                EuVatShopCountryId = 0,
                EuVatAllowVatExemption = true,
                EuVatUseWebService = false,
                EuVatAssumeValid = false,
                EuVatEmailAdminWhenNewVatSubmitted = false,
                LogErrors = false
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = "",
                AllowCustomersToSetTimeZone = false
            });

            settingService.SaveSetting(new BlogSettings
            {
                Enabled = true,
                PostsPageSize = 10,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewBlogComments = false,
                NumberOfTags = 15,
                ShowHeaderRssUrl = false,
                BlogCommentsMustBeApproved = false,
                ShowBlogCommentsPerStore = false
            });
            settingService.SaveSetting(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowNewsCommentsPerStore = false
            });

            settingService.SaveSetting(new ForumSettings
            {
                ForumsEnabled = false,
                RelativeDateTimeFormattingEnabled = true,
                AllowCustomersToDeletePosts = false,
                AllowCustomersToEditPosts = false,
                AllowCustomersToManageSubscriptions = false,
                AllowGuestsToCreatePosts = false,
                AllowGuestsToCreateTopics = false,
                AllowPostVoting = true,
                MaxVotesPerDay = 30,
                TopicSubjectMaxLength = 450,
                PostMaxLength = 4000,
                StrippedTopicMaxLength = 45,
                TopicsPageSize = 10,
                PostsPageSize = 10,
                SearchResultsPageSize = 10,
                ActiveDiscussionsPageSize = 50,
                LatestCustomerPostsPageSize = 10,
                ShowCustomersPostCount = true,
                ForumEditor = EditorType.BBCodeEditor,
                SignaturesEnabled = true,
                AllowPrivateMessages = false,
                ShowAlertForPM = false,
                PrivateMessagesPageSize = 10,
                ForumSubscriptionsPageSize = 10,
                NotifyAboutPrivateMessages = false,
                PMSubjectMaxLength = 450,
                PMTextMaxLength = 4000,
                HomePageActiveDiscussionsTopicCount = 5,
                ActiveDiscussionsFeedEnabled = false,
                ActiveDiscussionsFeedCount = 25,
                ForumFeedsEnabled = false,
                ForumFeedCount = 10,
                ForumSearchTermMinimumLength = 3,
            });

            settingService.SaveSetting(new ContributorSettings
            {
                DefaultContributorPageSizeOptions = "6, 3, 9",
                ContributorsBlockItemsToDisplay = 0,
                ShowContributorOnArticleDetailsPage = true,
                AllowCustomersToContactContributors = true,
                AllowCustomersToApplyForContributorAccount = true,
                AllowContributorsToEditInfo = false,
                NotifyStoreOwnerAboutContributorInformationChange = true,
                MaximumArticleNumber = 3000,
                AllowContributorsToImportArticles = true
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" },
            });

            settingService.SaveSetting(new DisplayDefaultMenuItemSettings
            {
                DisplayHomePageMenuItem = !installSampleData,
                DisplayNewArticlesMenuItem = !installSampleData,
                DisplayArticleSearchMenuItem = !installSampleData,
                DisplayCustomerInfoMenuItem = !installSampleData,
                DisplayBlogMenuItem = !installSampleData,
                DisplayForumsMenuItem = !installSampleData,
                DisplayContactUsMenuItem = !installSampleData
            });
        }

        protected virtual void InstallCheckoutAttributes()
        {
            var ca1 = new CheckoutAttribute
            {
                Name = "Gift wrapping",
                IsRequired = true,
                ShippableArticleRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplaySubscription = 1,
            };
            ca1.CheckoutAttributeValues.Add(new CheckoutAttributeValue
            {
                Name = "No",
                PriceAdjustment = 0,
                DisplaySubscription = 1,
                IsPreSelected = true,
            });
            ca1.CheckoutAttributeValues.Add(new CheckoutAttributeValue
            {
                Name = "Yes",
                PriceAdjustment = 10,
                DisplaySubscription = 2,
            });
            var checkoutAttributes = new List<CheckoutAttribute>
                                {
                                    ca1,
                                };
            _checkoutAttributeRepository.Insert(checkoutAttributes);
        }

        protected virtual void InstallSpecificationAttributes()
        {
            var sa1 = new SpecificationAttribute
            {
                Name = "Screensize",
                DisplaySubscription = 1,
            };
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "13.0''",
                DisplaySubscription = 2,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "13.3''",
                DisplaySubscription = 3,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "14.0''",
                DisplaySubscription = 4,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "15.0''",
                DisplaySubscription = 4,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "15.6''",
                DisplaySubscription = 5,
            });
            var sa2 = new SpecificationAttribute
            {
                Name = "CPU Type",
                DisplaySubscription = 2,
            };
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Intel Core i5",
                DisplaySubscription = 1,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Intel Core i7",
                DisplaySubscription = 2,
            });
            var sa3 = new SpecificationAttribute
            {
                Name = "Memory",
                DisplaySubscription = 3,
            };
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "4 GB",
                DisplaySubscription = 1,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "8 GB",
                DisplaySubscription = 2,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "16 GB",
                DisplaySubscription = 3,
            });
            var sa4 = new SpecificationAttribute
            {
                Name = "Hardrive",
                DisplaySubscription = 5,
            };
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "128 GB",
                DisplaySubscription = 7,
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "500 GB",
                DisplaySubscription = 4,
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "1 TB",
                DisplaySubscription = 3,
            });
            var sa5 = new SpecificationAttribute
            {
                Name = "Color",
                DisplaySubscription = 1,
            };
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Grey",
                DisplaySubscription = 2,
                ColorSquaresRgb = "#8a97a8"
            });
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Red",
                DisplaySubscription = 3,
                ColorSquaresRgb = "#8a374a"
            });
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Blue",
                DisplaySubscription = 4,
                ColorSquaresRgb = "#47476f"
            });
            var specificationAttributes = new List<SpecificationAttribute>
                                {
                                    sa1,
                                    sa2,
                                    sa3,
                                    sa4,
                                    sa5
                                };
            _specificationAttributeRepository.Insert(specificationAttributes);
        }

        protected virtual void InstallArticleAttributes()
        {
            var articleAttributes = new List<ArticleAttribute>
            {
                new ArticleAttribute
                {
                    Name = "Color",
                },
                new ArticleAttribute
                {
                    Name = "Print",
                },
                new ArticleAttribute
                {
                    Name = "Custom Text",
                },
                new ArticleAttribute
                {
                    Name = "HDD",
                },
                new ArticleAttribute
                {
                    Name = "OS",
                },
                new ArticleAttribute
                {
                    Name = "Processor",
                },
                new ArticleAttribute
                {
                    Name = "RAM",
                },
                new ArticleAttribute
                {
                    Name = "Size",
                },
                new ArticleAttribute
                {
                    Name = "Software",
                },
            };
            _articleAttributeRepository.Insert(articleAttributes);
        }

        protected virtual void InstallCategories()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = CommonHelper.MapPath("~/content/samples/");



            var categoryTemplateInGridAndLines = _categoryTemplateRepository
                .Table.FirstOrDefault(pt => pt.Name == "Articles in Grid or Lines");
            if (categoryTemplateInGridAndLines == null)
                throw new Exception("Category template cannot be loaded");


            //categories
            var allCategories = new List<Category>();
            var categoryComputers = new Category
            {
                Name = "Actors",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Actors.png"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Computers")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryComputers);
            _categoryRepository.Insert(categoryComputers);


            var categoryDesktops = new Category
            {
                Name = "Artists",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Artists.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Desktops")).Id,
                PriceRanges = "-1000;1000-1200;1200-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDesktops);
            _categoryRepository.Insert(categoryDesktops);


            var categoryNotebooks = new Category
            {
                Name = "Entrepreneurs",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Entrepreneurs.png"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Notebooks")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryNotebooks);
            _categoryRepository.Insert(categoryNotebooks);


            var categorySoftware = new Category
            {
                Name = "Humanitarians",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Humanitarians.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Software")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categorySoftware);
            _categoryRepository.Insert(categorySoftware);


            var categoryElectronics = new Category
            {
                Name = "Writers",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Writers.png"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Electronics")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomePage = true,
                DisplaySubscription = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryElectronics);
            _categoryRepository.Insert(categoryElectronics);


            var categoryCameraPhoto = new Category
            {
                Name = "Military",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Military.png"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Camera, photo")).Id,
                PriceRanges = "-500;500-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCameraPhoto);
            _categoryRepository.Insert(categoryCameraPhoto);


            var categoryCellPhones = new Category
            {
                Name = "Musicians",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Musicians.png"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Cell phones")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCellPhones);
            _categoryRepository.Insert(categoryCellPhones);


            var categoryOthers = new Category
            {
                Name = "Poets",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Poets.png"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Accessories")).Id,
                IncludeInTopMenu = true,
                PriceRanges = "-100;100-;",
                Published = true,
                DisplaySubscription = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryOthers);
            _categoryRepository.Insert(categoryOthers);


            var categoryApparel = new Category
            {
                Name = "Politicians",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Politicians.png"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Tax1")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomePage = true,
                DisplaySubscription = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryApparel);
            _categoryRepository.Insert(categoryApparel);


            var categoryShoes = new Category
            {
                Name = "Scientists",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Scientists.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Shoes")).Id,
                PriceRanges = "-500;500-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryShoes);
            _categoryRepository.Insert(categoryShoes);


            var categoryClothing = new Category
            {
                Name = "Sports",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Sports.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Clothing")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplaySubscription = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryClothing);
            _categoryRepository.Insert(categoryClothing);


            var categoryAccessories = new Category
            {
                Name = "Religious",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Religious.png"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Apparel Accessories")).Id,
                IncludeInTopMenu = true,
                PriceRanges = "-100;100-;",
                Published = true,
                DisplaySubscription = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryAccessories);
            _categoryRepository.Insert(categoryAccessories);


            



            //search engine names
            foreach (var category in allCategories)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = "Category",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = category.ValidateSeName("", category.Name, true)
                });
            }
        }

        protected virtual void InstallPublishers()
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = CommonHelper.MapPath("~/content/samples/");

            var publisherTemplateInGridAndLines =
                _publisherTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Articles in Grid or Lines");
            if (publisherTemplateInGridAndLines == null)
                throw new Exception("Publisher template cannot be loaded");

            var allPublishers = new List<Publisher>();
            var publisherAsus = new Publisher
            {
                Name = "FirstPost",
                PublisherTemplateId = publisherTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "firstpost.png"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("FirstPost")).Id,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _publisherRepository.Insert(publisherAsus);
            allPublishers.Add(publisherAsus);


            var publisherHp = new Publisher
            {
                Name = "Guardian",
                PublisherTemplateId = publisherTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Guardian.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Guardian")).Id,
                DisplaySubscription = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _publisherRepository.Insert(publisherHp);
            allPublishers.Add(publisherHp);


            var publisherNike = new Publisher
            {
                Name = "Republic",
                PublisherTemplateId = publisherTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "Republic.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Republic")).Id,
                DisplaySubscription = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _publisherRepository.Insert(publisherNike);
            allPublishers.Add(publisherNike);

            //search engine names
            foreach (var publisher in allPublishers)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = publisher.Id,
                    EntityName = "Publisher",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = publisher.ValidateSeName("", publisher.Name, true)
                });
            }
        }

        protected virtual void InstallArticles(string defaultUserEmail)
        {
            var articleTemplateSimple = _articleTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple article");
            if (articleTemplateSimple == null)
                throw new Exception("Simple article template could not be loaded");
            var articleTemplateGrouped = _articleTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped article (with variants)");
            if (articleTemplateGrouped == null)
                throw new Exception("Grouped article template could not be loaded");

            

            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");


            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = CommonHelper.MapPath("~/content/samples/");

            //downloads
            var downloadService = EngineContext.Current.Resolve<IDownloadService>();
            var sampleDownloadsPath = CommonHelper.MapPath("~/content/samples/");

            //articles
            var allArticles = new List<Article>();

            #region Actors


            var articleBuildComputer = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News1",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "build-your-own-computer",
                AllowCustomerReviews = true,
                Price = 1200M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                ShowOnHomePage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                 
            };
            allArticles.Add(articleBuildComputer);
            articleBuildComputer.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleBuildComputer.Name)),
                DisplaySubscription = 1,
            });
            articleBuildComputer.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "article_Desktops_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleBuildComputer.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleBuildComputer);





            var articleDigitalStorm = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News2",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "compaq-presario-sr1519x-pentium-4-desktop-pc-with-cdrw",
                AllowCustomerReviews = true,
                Price = 1259M,
              
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Actors"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleDigitalStorm);
            articleDigitalStorm.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleDigitalStorm.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleDigitalStorm);





            var articleLenovoIdeaCentre = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News3",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "hp-iq506-touchsmart-desktop-pc",
                AllowCustomerReviews = true,
                Price = 500M,
              
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Actors"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleLenovoIdeaCentre);
            articleLenovoIdeaCentre.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleLenovoIdeaCentre.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleLenovoIdeaCentre);




            #endregion

            #region Artists

            var articleAppleMacBookPro = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News4",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "asus-eee-pc-1000ha-10-inch-netbook",
                AllowCustomerReviews = true,
                Price = 1800M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "FirstPost"),
                        DisplaySubscription = 2,
                    }
                },
                
            };
            allArticles.Add(articleAppleMacBookPro);
            articleAppleMacBookPro.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleAppleMacBookPro.Name)),
                DisplaySubscription = 1,
            });
            articleAppleMacBookPro.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleAppleMacBookPro.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleAppleMacBookPro);





            var articleAsusN551JK = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News5",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "asus-eee-pc-900ha-89-inch-netbook-black",
                AllowCustomerReviews = true,
                Price = 1500M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
                
                
            };
            allArticles.Add(articleAsusN551JK);
            articleAsusN551JK.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleAsusN551JK.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleAsusN551JK);





            var articleSamsungSeries = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News6",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "hp-pavilion-artist-edition-dv2890nr-141-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1590M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
              
                Published = true,
                //ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
               
            };
            allArticles.Add(articleSamsungSeries);
            articleSamsungSeries.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleSamsungSeries.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleSamsungSeries);





            var articleHpSpectre = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News7",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "hp-pavilion-elite-m9150f-desktop-pc",
                AllowCustomerReviews = true,
                Price = 1350M,
             
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "Guardian"),
                        DisplaySubscription = 3,
                    }
                },
                
            };
            allArticles.Add(articleHpSpectre);
            articleHpSpectre.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHpSpectre.Name)),
                DisplaySubscription = 1,
            });
            articleHpSpectre.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHpSpectre.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleHpSpectre);



            var articleHpEnvy = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News8",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "hp-pavilion-g60-230us-160-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1460M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "Guardian"),
                        DisplaySubscription = 4,
                    }
                },
                
                 
            };
            allArticles.Add(articleHpEnvy);
            articleHpEnvy.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHpEnvy.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleHpEnvy);





            var articleLenovoThinkpad = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News9",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "toshiba-satellite-a305-s6908-154-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1360M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
              
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Artists"),
                        DisplaySubscription = 1,
                    }
                },
                 
            };
            allArticles.Add(articleLenovoThinkpad);
            articleLenovoThinkpad.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleLenovoThinkpad.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleLenovoThinkpad);

            #endregion

            #region Entrepreneurs


            var articleAdobePhotoshop = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News10",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "adobe-photoshop-elements-7",
                AllowCustomerReviews = true,
                Price = 75M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Entrepreneurs"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleAdobePhotoshop);
            articleAdobePhotoshop.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleAdobePhotoshop.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleAdobePhotoshop);






            var articleWindows8Pro = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News11",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "corel-paint-shop-pro-photo-x2",
                AllowCustomerReviews = true,
                Price = 65M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                 
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Entrepreneurs"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleWindows8Pro);
            articleWindows8Pro.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleWindows8Pro.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleWindows8Pro);





            var articleSoundForge = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News12",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "major-league-baseball-2k9",
                IsRecurring = true,
                RecurringCycleLength = 30,
                RecurringCyclePeriod = RecurringArticleCyclePeriod.Months,
                RecurringTotalCycles = 12,
                AllowCustomerReviews = true,
                Price = 54.99M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                 
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Entrepreneurs"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleSoundForge);
            articleSoundForge.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleSoundForge.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleSoundForge);




            #endregion

            #region Humanitarians


            //this one is a grouped article with two associated ones
            var articleNikonD5500DSLR = new Article
            {
                ArticleType = ArticleType.GroupedArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News13",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateGrouped.Id,
                //SeName = "canon-digital-slr-camera",
                AllowCustomerReviews = true,
                Published = true,
                Price = 670M,
                 
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
              
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Humanitarians"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleNikonD5500DSLR);
            articleNikonD5500DSLR.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleNikonD5500DSLR.Name)),
                DisplaySubscription = 1,
            });
            articleNikonD5500DSLR.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleNikonD5500DSLR.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleNikonD5500DSLR);
            
             
            var articleLeica = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News14",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "canon-vixia-hf100-camcsubscription",
                AllowCustomerReviews = true,
                Price = 530M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Humanitarians"),
                        DisplaySubscription = 3,
                    }
                }
            };
            allArticles.Add(articleLeica);
            articleLeica.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleLeica.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleLeica);






            var articleAppleICam = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News15",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "panasonic-hdc-sdt750k-high-definition-3d-camcsubscription",
                AllowCustomerReviews = true,
                Price = 1300M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                 
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Humanitarians"),
                        DisplaySubscription = 2,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "FirstPost"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleAppleICam);
            articleAppleICam.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleAppleICam.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleAppleICam);




            #endregion

            #region Writers

            var articleHtcOne = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News16",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "blackberry-bold-9000-phone-black-att",
                AllowCustomerReviews = true,
                Price = 245M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                ShowOnHomePage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Writers"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleHtcOne);
            articleHtcOne.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHtcOne.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleHtcOne);






            var articleHtcOneMini = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News17",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "samsung-rugby-a837-phone-black-att",
                AllowCustomerReviews = true,
                Price = 100M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Writers"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleHtcOneMini);
            articleHtcOneMini.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHtcOneMini.Name)),
                DisplaySubscription = 1,
            });
            articleHtcOneMini.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleHtcOneMini.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleHtcOneMini);

             

            var articleNokiaLumia = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News18",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "sony-dcr-sr85-1mp-60gb-hard-drive-handycam-camcsubscription",
                AllowCustomerReviews = true,
                Price = 349M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
              
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Writers"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleNokiaLumia);
            articleNokiaLumia.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleNokiaLumia.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleNokiaLumia);


            #endregion

            #region Sports



            var articleBeatsPill = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News19",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "acer-aspire-one-89-mini-notebook-case-black",
                AllowCustomerReviews = true,
                Price = 79.99M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                 
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Sports"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleBeatsPill);
            articleBeatsPill.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleBeatsPill.Name)),
                DisplaySubscription = 1,
            });
            articleBeatsPill.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleBeatsPill.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleBeatsPill);





            var articleUniversalTabletCover = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News20",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "apc-back-ups-rs-800va-ups-800-va-ups-battery-lead-acid-br800blk",
                AllowCustomerReviews = true,
                Price = 39M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Sports"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleUniversalTabletCover);
            articleUniversalTabletCover.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleUniversalTabletCover.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleUniversalTabletCover);




            var articlePortableSoundSpeakers = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News21",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "microsoft-bluetooth-notebook-mouse-5000-macwindows",
                AllowCustomerReviews = true,
                Price = 37M,
              
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Sports"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articlePortableSoundSpeakers);
            articlePortableSoundSpeakers.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articlePortableSoundSpeakers.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articlePortableSoundSpeakers);


            #endregion

            #region Musicians


            var articleNikeFloral = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News22",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "adidas-womens-supernova-csh-7-running-shoe",
                AllowCustomerReviews = true,
                Price = 40M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Musicians"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "Republic"),
                        DisplaySubscription = 2,
                    }
                },
                
            };
            allArticles.Add(articleNikeFloral);
            articleNikeFloral.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleNikeFloral.Name)),
                DisplaySubscription = 1,
            });
            articleNikeFloral.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleNikeFloral.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleNikeFloral);

         


            var articleAdidas = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News23",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "etnies-mens-digit-sneaker",
                AllowCustomerReviews = true,
                Price = 27.56M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
 
                Published = true,
                //ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Musicians"),
                        DisplaySubscription = 1,
                    }
                },
               
            };
            allArticles.Add(articleAdidas);
            articleAdidas.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleAdidas.Name)),
                DisplaySubscription = 1,
            });
            articleAdidas.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleAdidas.Name)),
                DisplaySubscription = 2,
            });
            articleAdidas.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleAdidas.Name)),
                DisplaySubscription = 3,
            });


            _articleRepository.Insert(articleAdidas);

            



            var articleNikeZoom = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News24",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "v-blue-juniors-cuffed-denim-short-with-rhinestones",
                AllowCustomerReviews = true,
                Price = 30M,
               
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
               
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Musicians"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "Republic"),
                        DisplaySubscription = 2,
                    }
                },
                
            };

            allArticles.Add(articleNikeZoom);
            articleNikeZoom.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleNikeZoom.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleNikeZoom);


            #endregion

            #region Politicians


            var articleObeyHat = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News25",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "indiana-jones-shapeable-wool-hat",
                AllowCustomerReviews = true,
                Price = 30M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Politicians"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleObeyHat);
            articleObeyHat.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleObeyHat.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleObeyHat);







            var articleBelt = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News26",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "nike-golf-casual-belt",
                AllowCustomerReviews = true,
                Price = 45M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Politicians"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleBelt);
            articleBelt.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleBelt.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleBelt);






            var articleSunglasses = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News27",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "ray-ban-aviator-sunglasses-rb-3025",
                AllowCustomerReviews = true,
                Price = 25M,
                
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,
                
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Politicians"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleSunglasses);
            articleSunglasses.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleSunglasses.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleSunglasses);

            #endregion

            #region Scientists

            var articleNikeTailwind = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News28",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "50s-rockabilly-polka-dot-top-jr-plus-size",
                AllowCustomerReviews = true,
                Published = true,
                Price = 15M,

                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,

                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                 
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Scientists"),
                        DisplaySubscription = 1,
                    }
                },
                ArticlePublishers =
                {
                    new ArticlePublisher
                    {
                        Publisher = _publisherRepository.Table.Single(c => c.Name == "Republic"),
                        DisplaySubscription = 2,
                    }
                }
            };
            allArticles.Add(articleNikeTailwind);
            articleNikeTailwind.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleNikeTailwind.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleNikeTailwind);




            var articleOversizedWomenTShirt = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News29",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "arrow-mens-wrinkle-free-pinpoint-solid-long-sleeve",
                AllowCustomerReviews = true,
                Price = 24M,

                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,

                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,

                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Scientists"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleOversizedWomenTShirt);
            articleOversizedWomenTShirt.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleOversizedWomenTShirt.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleOversizedWomenTShirt);




            var articleCustomTShirt = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News30",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "custom-t-shirt",
                AllowCustomerReviews = true,
                Price = 15M,

                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,

                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ArticleAttributeMappings =
                {
                    new ArticleAttributeMapping
                    {
                        ArticleAttribute = _articleAttributeRepository.Table.Single(x => x.Name == "Custom Text"),
                        TextPrompt = "Enter your text:",
                        AttributeControlType = AttributeControlType.TextBox,
                        IsRequired = true,
                    }
                },
                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Scientists"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleCustomTShirt);
            articleCustomTShirt.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(articleCustomTShirt.Name)),
                DisplaySubscription = 1,
            });
            _articleRepository.Insert(articleCustomTShirt);






            var articleLeviJeans = new Article
            {
                ArticleType = ArticleType.SimpleArticle,
                VisibleIndividually = true,
                Name = "Sachin Tendulkar, 'God of cricket', returns on celluloid",
                Sku = "News31",
                ShortDescription = "Regarded as one of the greatest batsmen ever, Sachin Ramesh Tendulkar is the mainstay of Indian batting line-up for more than two decades. He is the world's leading run-scorer in both Test (14,692) and ODI (18,111) cricket. In 2011, Tendulkar finally achieved his dream of winning the Cricket World Cup at the Wankhede stadium in Mumbai. It took six World Cup appearances for the 'Little Master' to win the coveted trophy. Tendulkar is the leading century maker in both Test and ODI and has so far scored 99 (51 Test + 48 ODI) international centuries.",
                FullDescription = "<p>Sachin Tendulkar is not an icon or a hero, he is an emotion.This line from a new film on the life and career of the legendary cricketer very much sums up his stature among his fans.He made millions teary - eyed in 2013 when he announced that his life between 22 yards for 24 years had come to an end.For his fans, the world as they knew it changed forever.Slowly, and rather reluctantly, they accepted that the god of cricket had decided to hang up his boots. Three years later,               the maestro is back - this time on cinema screens with his docudrama titled Sachin: A Billion Dreams. The title seems befitting for a man who had the ability to make a billion people happy or sad just with a stroke. People closely followed every run he scored and every word he spoke for almost a quarter of a century.</p> ",
                ArticleTemplateId = articleTemplateSimple.Id,
                //SeName = "levis-skinny-511-jeans",
                AllowCustomerReviews = true,
                Price = 43.5M,
                OldPrice = 55M,

                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Tax1").Id,

                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,

                ArticleCategories =
                {
                    new ArticleCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Scientists"),
                        DisplaySubscription = 1,
                    }
                }
            };
            allArticles.Add(articleLeviJeans);

            articleLeviJeans.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleLeviJeans.Name)),
                DisplaySubscription = 1,
            });
            articleLeviJeans.ArticlePictures.Add(new ArticlePicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "sachin.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(articleLeviJeans.Name)),
                DisplaySubscription = 2,
            });
            _articleRepository.Insert(articleLeviJeans);


            #endregion

            //search engine names
            foreach (var article in allArticles)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = article.Id,
                    EntityName = "Article",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = article.ValidateSeName("", article.Name, true)
                });
            }


            #region Related Articles

            //related articles
            var relatedArticles = new List<RelatedArticle>
            {
               
                new RelatedArticle
                {
                     ArticleId1 = articleAsusN551JK.Id,
                     ArticleId2 = articleLenovoThinkpad.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAsusN551JK.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAsusN551JK.Id,
                     ArticleId2 = articleSamsungSeries.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAsusN551JK.Id,
                     ArticleId2 = articleHpSpectre.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoThinkpad.Id,
                     ArticleId2 = articleAsusN551JK.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoThinkpad.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoThinkpad.Id,
                     ArticleId2 = articleSamsungSeries.Id,
                },
                 new RelatedArticle
                {
                     ArticleId1 = articleLenovoThinkpad.Id,
                     ArticleId2 = articleHpEnvy.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAppleMacBookPro.Id,
                     ArticleId2 = articleLenovoThinkpad.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAppleMacBookPro.Id,
                     ArticleId2 = articleSamsungSeries.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAppleMacBookPro.Id,
                     ArticleId2 = articleAsusN551JK.Id,
                },
                 new RelatedArticle
                {
                     ArticleId1 = articleAppleMacBookPro.Id,
                     ArticleId2 = articleHpSpectre.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpSpectre.Id,
                     ArticleId2 = articleLenovoThinkpad.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpSpectre.Id,
                     ArticleId2 = articleSamsungSeries.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpSpectre.Id,
                     ArticleId2 = articleAsusN551JK.Id,
                },
                 new RelatedArticle
                {
                     ArticleId1 = articleHpSpectre.Id,
                     ArticleId2 = articleHpEnvy.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpEnvy.Id,
                     ArticleId2 = articleAsusN551JK.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpEnvy.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpEnvy.Id,
                     ArticleId2 = articleHpSpectre.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHpEnvy.Id,
                     ArticleId2 = articleSamsungSeries.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleSamsungSeries.Id,
                     ArticleId2 = articleAsusN551JK.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleSamsungSeries.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleSamsungSeries.Id,
                     ArticleId2 = articleHpEnvy.Id,
                },
                 new RelatedArticle
                {
                     ArticleId1 = articleSamsungSeries.Id,
                     ArticleId2 = articleHpSpectre.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeica.Id,
                     ArticleId2 = articleHtcOneMini.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeica.Id,
                     ArticleId2 = articleNikonD5500DSLR.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeica.Id,
                     ArticleId2 = articleAppleICam.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeica.Id,
                     ArticleId2 = articleNokiaLumia.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOne.Id,
                     ArticleId2 = articleHtcOneMini.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOne.Id,
                     ArticleId2 = articleNokiaLumia.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOne.Id,
                     ArticleId2 = articleBeatsPill.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOne.Id,
                     ArticleId2 = articlePortableSoundSpeakers.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOneMini.Id,
                     ArticleId2 = articleHtcOne.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOneMini.Id,
                     ArticleId2 = articleNokiaLumia.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOneMini.Id,
                     ArticleId2 = articleBeatsPill.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleHtcOneMini.Id,
                     ArticleId2 = articlePortableSoundSpeakers.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleNokiaLumia.Id,
                     ArticleId2 = articleHtcOne.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleNokiaLumia.Id,
                     ArticleId2 = articleHtcOneMini.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleNokiaLumia.Id,
                     ArticleId2 = articleBeatsPill.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleNokiaLumia.Id,
                     ArticleId2 = articlePortableSoundSpeakers.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAdidas.Id,
                     ArticleId2 = articleLeviJeans.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAdidas.Id,
                     ArticleId2 = articleNikeFloral.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAdidas.Id,
                     ArticleId2 = articleNikeZoom.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleAdidas.Id,
                     ArticleId2 = articleNikeTailwind.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeviJeans.Id,
                     ArticleId2 = articleAdidas.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeviJeans.Id,
                     ArticleId2 = articleNikeFloral.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeviJeans.Id,
                     ArticleId2 = articleNikeZoom.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLeviJeans.Id,
                     ArticleId2 = articleNikeTailwind.Id,
                },

                new RelatedArticle
                {
                     ArticleId1 = articleCustomTShirt.Id,
                     ArticleId2 = articleLeviJeans.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleCustomTShirt.Id,
                     ArticleId2 = articleNikeTailwind.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleCustomTShirt.Id,
                     ArticleId2 = articleOversizedWomenTShirt.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleCustomTShirt.Id,
                     ArticleId2 = articleObeyHat.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleDigitalStorm.Id,
                     ArticleId2 = articleBuildComputer.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleDigitalStorm.Id,
                     ArticleId2 = articleLenovoIdeaCentre.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleDigitalStorm.Id,
                     ArticleId2 = articleLenovoThinkpad.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleDigitalStorm.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },


                new RelatedArticle
                {
                     ArticleId1 = articleLenovoIdeaCentre.Id,
                     ArticleId2 = articleBuildComputer.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoIdeaCentre.Id,
                     ArticleId2 = articleDigitalStorm.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoIdeaCentre.Id,
                     ArticleId2 = articleLenovoThinkpad.Id,
                },
                new RelatedArticle
                {
                     ArticleId1 = articleLenovoIdeaCentre.Id,
                     ArticleId2 = articleAppleMacBookPro.Id,
                },
            };
            _relatedArticleRepository.Insert(relatedArticles);

            #endregion

            #region Article Tags

            //article tags
            
            AddArticleTag(articleNikeTailwind, "cool");
            AddArticleTag(articleNikeTailwind, "Tax1");
            AddArticleTag(articleNikeTailwind, "shirt");
            AddArticleTag(articleBeatsPill, "computer");
            AddArticleTag(articleBeatsPill, "cool");
            AddArticleTag(articleNikeFloral, "cool");
            AddArticleTag(articleNikeFloral, "shoes");
            AddArticleTag(articleNikeFloral, "Tax1");
            AddArticleTag(articleAdobePhotoshop, "computer");
            AddArticleTag(articleAdobePhotoshop, "awesome");
            AddArticleTag(articleUniversalTabletCover, "computer");
            AddArticleTag(articleUniversalTabletCover, "cool");
            AddArticleTag(articleOversizedWomenTShirt, "cool");
            AddArticleTag(articleOversizedWomenTShirt, "Tax1");
            AddArticleTag(articleOversizedWomenTShirt, "shirt");
            AddArticleTag(articleAppleMacBookPro, "compact");
            AddArticleTag(articleAppleMacBookPro, "awesome");
            AddArticleTag(articleAppleMacBookPro, "computer");
            AddArticleTag(articleAsusN551JK, "compact");
            AddArticleTag(articleAsusN551JK, "awesome");
            AddArticleTag(articleAsusN551JK, "computer");
         
            AddArticleTag(articleHtcOne, "cell");
            AddArticleTag(articleHtcOne, "compact");
            AddArticleTag(articleHtcOne, "awesome");
            AddArticleTag(articleBuildComputer, "awesome");
            AddArticleTag(articleBuildComputer, "computer");
            AddArticleTag(articleNikonD5500DSLR, "cool");
            AddArticleTag(articleNikonD5500DSLR, "camera");
            AddArticleTag(articleLeica, "camera");
            AddArticleTag(articleLeica, "cool");
            AddArticleTag(articleDigitalStorm, "cool");
            AddArticleTag(articleDigitalStorm, "computer");
            AddArticleTag(articleWindows8Pro, "awesome");
            AddArticleTag(articleWindows8Pro, "computer");
            AddArticleTag(articleCustomTShirt, "cool");
            AddArticleTag(articleCustomTShirt, "shirt");
            AddArticleTag(articleCustomTShirt, "Tax1");
            
            AddArticleTag(articleAdidas, "cool");
            AddArticleTag(articleAdidas, "shoes");
            AddArticleTag(articleAdidas, "Tax1");
            AddArticleTag(articleLenovoIdeaCentre, "awesome");
            AddArticleTag(articleLenovoIdeaCentre, "computer");
            AddArticleTag(articleSamsungSeries, "nice");
            AddArticleTag(articleSamsungSeries, "computer");
            AddArticleTag(articleSamsungSeries, "compact");
            AddArticleTag(articleHpSpectre, "nice");
            AddArticleTag(articleHpSpectre, "computer");
            AddArticleTag(articleHpEnvy, "computer");
            AddArticleTag(articleHpEnvy, "cool");
            AddArticleTag(articleHpEnvy, "compact");
            AddArticleTag(articleObeyHat, "Tax1");
            AddArticleTag(articleObeyHat, "cool");
            AddArticleTag(articleLeviJeans, "cool");
            AddArticleTag(articleLeviJeans, "jeans");
            AddArticleTag(articleLeviJeans, "Tax1");
            AddArticleTag(articleSoundForge, "game");
            AddArticleTag(articleSoundForge, "computer");
            AddArticleTag(articleSoundForge, "cool");
            
            AddArticleTag(articleSunglasses, "Tax1");
            AddArticleTag(articleSunglasses, "cool");
            AddArticleTag(articleHtcOneMini, "awesome");
            AddArticleTag(articleHtcOneMini, "compact");
            AddArticleTag(articleHtcOneMini, "cell");
            
            AddArticleTag(articleNokiaLumia, "awesome");
            AddArticleTag(articleNokiaLumia, "cool");
            AddArticleTag(articleNokiaLumia, "camera");
         
            AddArticleTag(articleLenovoThinkpad, "awesome");
            AddArticleTag(articleLenovoThinkpad, "computer");
            AddArticleTag(articleLenovoThinkpad, "compact");
            AddArticleTag(articleNikeZoom, "jeans");
            AddArticleTag(articleNikeZoom, "cool");
            AddArticleTag(articleNikeZoom, "Tax1");
          


            #endregion

            #region  Reviews

            //reviews
            var random = new Random();
            foreach (var article in allArticles)
            {
                if (article.ArticleType != ArticleType.SimpleArticle)
                    continue;

                //only 3 of 4 articles will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);
                article.ArticleReviews.Add(new ArticleReview
                {
                    CustomerId = defaultCustomer.Id,
                    ArticleId = article.Id,
                    StoreId = defaultStore.Id,
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = string.Format("This sample review is for the {0}. I've been waiting for this article to be available. It is priced just right.", article.Name),
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });
                article.ApprovedRatingSum = rating;
                article.ApprovedTotalReviews = article.ArticleReviews.Count;

            }
            _articleRepository.Update(allArticles);

            #endregion

             
        }

        protected virtual void InstallForums()
        {
            var forumGroup = new ForumGroup
            {
                Name = "General",
                DisplaySubscription = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            _forumGroupRepository.Insert(forumGroup);

            var newArticlesForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "New Articles",
                Description = "Discuss new articles and industry trends",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplaySubscription = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(newArticlesForum);

            var mobileDevicesForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "Mobile Devices Forum",
                Description = "Discuss the mobile phone market",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplaySubscription = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(mobileDevicesForum);

            var packagingShippingForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "Packaging & Shipping",
                Description = "Discuss packaging & shipping",
                NumTopics = 0,
                NumPosts = 0,
                LastPostTime = null,
                DisplaySubscription = 20,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(packagingShippingForum);
        }

        protected virtual void InstallBlogPosts(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var blogPosts = new List<BlogPost>
                                {
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "How a blog can help your growing e-Commerce business",
                                             BodyOverview = "<p>When you start an online business, your main aim is to sell the articles, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p>",
                                             Body = "<p>When you start an online business, your main aim is to sell the articles, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p><h3>1) Blog is useful in educating your customers</h3><p>Blogging is one of the best way by which you can educate your customers about your articles/services that you offer. This helps you as a business owner to bring more value to your brand. When you provide useful information to the customers about your articles, they are more likely to buy articles from you. You can use your blog for providing tutorials in regard to the use of your articles.</p><p><strong>For example:</strong> If you have an online store that offers computer parts. You can write tutorials about how to build a computer or how to make your computer&rsquo;s performance better. While talking about these things, you can mention articles in the tutorials and provide link to your articles within the blog post from your website. Your potential customers might get different ideas of using your article and will likely to buy articles from your online store.</p><h3>2) Blog helps your business in Search Engine Optimization (SEO)</h3><p>Blog posts create more internal links to your website which helps a lot in SEO. Blog is a great way to have quality content on your website related to your articles/services which is indexed by all major search engines like Google, Bing and Yahoo. The more original content you write in your blog post, the better ranking you will get in search engines. SEO is an on-going process and posting blog posts regularly keeps your site active all the time which is beneficial when it comes to search engine optimization.</p><p><strong>For example:</strong> Let&rsquo;s say you sell &ldquo;Sony Television Model XYZ&rdquo; and you regularly publish blog posts about your article. Now, whenever someone searches for &ldquo;Sony Television Model XYZ&rdquo;, Google will crawl on your website knowing that you have something to do with this particular article. Hence, your website will show up on the search result page whenever this item is being searched.</p><h3>3) Blog helps in boosting your sales by convincing the potential customers to buy</h3><p>If you own an online business, there are so many ways you can share different stories with your audience in regard your articles/services that you offer. Talk about how you started your business, share stories that educate your audience about what&rsquo;s new in your industry, share stories about how your article/service was beneficial to someone or share anything that you think your audience might find interesting (it does not have to be related to your article). This kind of blogging shows that you are an expert in your industry and interested in educating your audience. It sets you apart in the competitive market. This gives you an opportunity to showcase your expertise by educating the visitors and it can turn your audience into buyers.</p><p><strong>Fun Fact:</strong> Did you know that 92% of companies who decided to blog acquired customers through their blog?</p><p><a href=\"http://www.yourstory.com/\">yourStory</a> is great e-Commerce solution that also offers a variety of CMS features including blog. A store owner has full access for managing the blog posts and related comments.</p>",
                                             Tags = "e-commerce, blog, moey",
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Why your online store needs a wish list",
                                             BodyOverview = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save articles in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p>",
                                             Body = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save articles in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p><p>Does every e-Commerce store needs a wish list? The answer to this question in most cases is yes, because of the following reasons:</p><p><strong>Understanding the needs of your customers</strong> - A wish list is a great way to know what is in your customer&rsquo;s mind. Try to think the purchase history as a small portion of the customer&rsquo;s preferences. But, the wish list is like a wide open door that can give any online business a lot of valuable information about their customer and what they like or desire.</p><p><strong>Shoppers like to share their wish list with friends and family</strong> - Providing your customers a way to email their wish list to their friends and family is a pleasant way to make online shopping enjoyable for the shoppers. It is always a good idea to make the wish list sharable by a unique link so that it can be easily shared though different channels like email or on social media sites.</p><p><strong>Wish list can be a great marketing tool</strong> &ndash; Another way to look at wish list is a great marketing tool because it is extremely targeted and the recipients are always motivated to use it. For example: when your younger brother tells you that his wish list is on a certain e-Commerce store. What is the first thing you are going to do? You are most likely to visit the e-Commerce store, check out the wish list and end up buying something for your younger brother.</p><p>So, how a wish list is a marketing tool? The reason is quite simple, it introduce your online store to new customers just how it is explained in the above example.</p><p><strong>Encourage customers to return to the store site</strong> &ndash; Having a feature of wish list on the store site can increase the return traffic because it encourages customers to come back and buy later. Allowing the customers to save the wish list to their online accounts gives them a reason return to the store site and login to the account at any time to view or edit the wish list items.</p><p><strong>Wish list can be used for gifts for different occasions like weddings or birthdays. So, what kind of benefits a gift-giver gets from a wish list?</strong></p><ul><li>It gives them a surety that they didn&rsquo;t buy a wrong gift</li><li>It guarantees that the recipient will like the gift</li><li>It avoids any awkward moments when the recipient unwraps the gift and as a gift-giver you got something that the recipient do not want</li></ul><p><strong>Wish list is a great feature to have on a store site &ndash; So, what kind of benefits a business owner gets from a wish list</strong></p><ul><li>It is a great way to advertise an online store as many people do prefer to shop where their friend or family shop online</li><li>It allows the current customers to return to the store site and open doors for the new customers</li><li>It allows store admins to track what&rsquo;s in customers wish list and run promotions accordingly to target specific customer segments</li></ul><p><a href=\"http://www.yourstory.com/\">yourStory</a> offers the feature of wish list that allows customers to create a list of articles that they desire or planning to buy in future.</p>",
                                             Tags = "e-commerce, yourStory, sample tag, money",
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            _blogPostRepository.Insert(blogPosts);

            //search engine names
            foreach (var blogPost in blogPosts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = blogPost.Id,
                    EntityName = "BlogPost",
                    LanguageId = blogPost.LanguageId,
                    IsActive = true,
                    Slug = blogPost.ValidateSeName("", blogPost.Title, true)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var blogPost in blogPosts)
            {
                blogPost.BlogComments.Add(new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentText = "This is a sample comment for this blog post",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
            _blogPostRepository.Update(blogPosts);
        }

        protected virtual void InstallNews(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var news = new List<NewsItem>
                                {
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "About yourStory",
                                         Short = "It's stable and highly usable. From downloads to documentation, www.yourStory.com offers a comprehensive base of information, resources, and support to the yourStory community.",
                                         Full = "<p>For full feature list go to <a href=\"http://www.yourStory.com\">yourStory.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow,
                                    },
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "yourStory new release!",
                                         Short = "yourStory includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! yourStory is a fully customizable shopping cart",
                                         Full = "<p>yourStory includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                    },
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "New online store is open!",
                                         Short = "The new yourStory store is open now! We are very excited to offer our new range of articles. We will be constantly adding to our range so please register on our site.",
                                         Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new articles.</p><p>All shipping is worldwide and will leave the same day an subscription is placed! Happy Shopping and spread the word!!</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow.AddSeconds(2),
                                    },

                                };
            _newsItemRepository.Insert(news);

            //search engine names
            foreach (var newsItem in news)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = newsItem.Id,
                    EntityName = "NewsItem",
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = newsItem.ValidateSeName("", newsItem.Title, true)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var newsItem in news)
            {
                newsItem.NewsComments.Add(new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentTitle = "Sample comment title",
                    CommentText = "This is a sample comment...",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
            _newsItemRepository.Update(news);
        }

        protected virtual void InstallPolls()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var poll1 = new Poll
            {
                Language = defaultLanguage,
                Name = "Do you like yourStory?",
                SystemKeyword = "",
                Published = true,
                ShowOnHomePage = true,
                DisplaySubscription = 1,
            };
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Excellent",
                DisplaySubscription = 1,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Good",
                DisplaySubscription = 2,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Poor",
                DisplaySubscription = 3,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Very bad",
                DisplaySubscription = 4,
            });
            _pollRepository.Insert(poll1);
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomer",
                    Enabled = true,
                    Name = "Add a new customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttribute",
                    Enabled = true,
                    Name = "Add a new customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttributeValue",
                    Enabled = true,
                    Name = "Add a new customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerRole",
                    Enabled = true,
                    Name = "Add a new customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewPublisher",
                    Enabled = true,
                    Name = "Add a new publisher"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewArticle",
                    Enabled = true,
                    Name = "Add a new article"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewArticleAttribute",
                    Enabled = true,
                    Name = "Add a new article attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewContributor",
                    Enabled = true,
                    Name = "Add a new contributor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomer",
                    Enabled = true,
                    Name = "Delete a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttribute",
                    Enabled = true,
                    Name = "Delete a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttributeValue",
                    Enabled = true,
                    Name = "Delete a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerRole",
                    Enabled = true,
                    Name = "Delete a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePublisher",
                    Enabled = true,
                    Name = "Delete a publisher"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSubscription",
                    Enabled = true,
                    Name = "Delete an subscription"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteArticle",
                    Enabled = true,
                    Name = "Delete a article"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteArticleAttribute",
                    Enabled = true,
                    Name = "Delete a article attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteArticleReview",
                    Enabled = true,
                    Name = "Delete a article review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteContributor",
                    Enabled = true,
                    Name = "Delete a contributor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomer",
                    Enabled = true,
                    Name = "Edit a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttribute",
                    Enabled = true,
                    Name = "Edit a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttributeValue",
                    Enabled = true,
                    Name = "Edit a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerRole",
                    Enabled = true,
                    Name = "Edit a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPublisher",
                    Enabled = true,
                    Name = "Edit a publisher"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSubscription",
                    Enabled = true,
                    Name = "Edit an subscription"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditArticle",
                    Enabled = true,
                    Name = "Edit a article"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditArticleAttribute",
                    Enabled = true,
                    Name = "Edit a article attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditArticleReview",
                    Enabled = true,
                    Name = "Edit a article review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditContributor",
                    Enabled = true,
                    Name = "Edit a contributor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "Customer impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "Customer impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportPublishers",
                    Enabled = true,
                    Name = "Publishers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportArticles",
                    Enabled = true,
                    Name = "Articles were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewPublisher",
                    Enabled = false,
                    Name = "Public store. View a publisher"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewArticle",
                    Enabled = false,
                    Name = "Public store. View a article"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PlaceSubscription",
                    Enabled = false,
                    Name = "Public store. Place an subscription"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddArticleReview",
                    Enabled = false,
                    Name = "Public store. Add article review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                }
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallArticleTemplates()
        {
            var articleTemplates = new List<ArticleTemplate>
                               {
                                   new ArticleTemplate
                                       {
                                           Name = "Simple article",
                                           ViewPath = "ArticleTemplate.Simple",
                                           DisplaySubscription = 10,
                                           IgnoredArticleTypes = ((int)ArticleType.GroupedArticle).ToString()
                                       },
                                   new ArticleTemplate
                                       {
                                           Name = "Grouped article (with variants)",
                                           ViewPath = "ArticleTemplate.Grouped",
                                           DisplaySubscription = 100,
                                           IgnoredArticleTypes = ((int)ArticleType.SimpleArticle).ToString()
                                       }
                               };
            _articleTemplateRepository.Insert(articleTemplates);
        }

        protected virtual void InstallCategoryTemplates()
        {
            var categoryTemplates = new List<CategoryTemplate>
                               {
                                   new CategoryTemplate
                                       {
                                           Name = "Articles in Grid or Lines",
                                           ViewPath = "CategoryTemplate.ArticlesInGridOrLines",
                                           DisplaySubscription = 1
                                       },
                               };
            _categoryTemplateRepository.Insert(categoryTemplates);
        }

        protected virtual void InstallPublisherTemplates()
        {
            var publisherTemplates = new List<PublisherTemplate>
                               {
                                   new PublisherTemplate
                                       {
                                           Name = "Articles in Grid or Lines",
                                           ViewPath = "PublisherTemplate.ArticlesInGridOrLines",
                                           DisplaySubscription = 1
                                       },
                               };
            _publisherTemplateRepository.Insert(publisherTemplates);
        }

        protected virtual void InstallTopicTemplates()
        {
            var topicTemplates = new List<TopicTemplate>
                               {
                                   new TopicTemplate
                                       {
                                           Name = "Default template",
                                           ViewPath = "TopicDetails",
                                           DisplaySubscription = 1
                                       },
                               };
            _topicTemplateRepository.Insert(topicTemplates);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "YStory.Services.Messages.QueuedMessagesSendTask, YStory.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "YStory.Services.Common.KeepAliveTask, YStory.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "YStory.Services.Customers.DeleteGuestsTask, YStory.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "YStory.Services.Caching.ClearCacheTask, YStory.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "YStory.Services.Logging.ClearLogTask, YStory.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "YStory.Services.Directory.UpdateExchangeRateTask, YStory.Services",
                    Enabled = true,
                    StopOnError = false,
                },
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        protected virtual void InstallReturnRequestReasons()
        {
            var returnRequestReasons = new List<ReturnRequestReason>
                                {
                                    new ReturnRequestReason
                                        {
                                            Name = "Received Wrong Article",
                                            DisplaySubscription = 1
                                        },
                                    new ReturnRequestReason
                                        {
                                            Name = "Wrong Article Subscriptioned",
                                            DisplaySubscription = 2
                                        },
                                    new ReturnRequestReason
                                        {
                                            Name = "There Was A Problem With The Article",
                                            DisplaySubscription = 3
                                        }
                                };
            _returnRequestReasonRepository.Insert(returnRequestReasons);
        }

        protected virtual void InstallReturnRequestActions()
        {
            var returnRequestActions = new List<ReturnRequestAction>
                                {
                                    new ReturnRequestAction
                                        {
                                            Name = "Repair",
                                            DisplaySubscription = 1
                                        },
                                    new ReturnRequestAction
                                        {
                                            Name = "Replacement",
                                            DisplaySubscription = 2
                                        },
                                    new ReturnRequestAction
                                        {
                                            Name = "Store Credit",
                                            DisplaySubscription = 3
                                        }
                                };
            _returnRequestActionRepository.Insert(returnRequestActions);
        }

        protected virtual void InstallWarehouses()
        {
            var warehouse1address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            _addressRepository.Insert(warehouse1address);
            var warehouse2address = new Address
            {
                Address1 = "300 South Spring Stree",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90013",
                CreatedOnUtc = DateTime.UtcNow,
            };
            _addressRepository.Insert(warehouse2address);
           
        }

        protected virtual void InstallContributors()
        {
            var contributors = new List<Contributor>
            {
                new Contributor
                {
                    Name = "Contributor 1",
                    Email = "contributor1email@gmail.com",
                    Description = "Some description...",
                    AdminComment = "",
                    PictureId = 0,
                    Active = true,
                    DisplaySubscription = 1,
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18",
                },
                new Contributor
                {
                    Name = "Contributor 2",
                    Email = "contributor2email@gmail.com",
                    Description = "Some description...",
                    AdminComment = "",
                    PictureId = 0,
                    Active = true,
                    DisplaySubscription = 2,
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18",
                }
            };

            _contributorRepository.Insert(contributors);

            //search engine names
            foreach (var contributor in contributors)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = contributor.Id,
                    EntityName = "Contributor",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = contributor.ValidateSeName("", contributor.Name, true)
                });
            }
        }

        protected virtual void InstallAffiliates()
        {
            var affiliateAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "affiliate_email@gmail.com",
                Company = "Company name here...",
                City = "New York",
                Address1 = "21 West 52nd Street",
                ZipPostalCode = "10021",
                PhoneNumber = "123456789",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                CreatedOnUtc = DateTime.UtcNow,
            };
            _addressRepository.Insert(affiliateAddress);
            var affilate = new Affiliate
            {
                Active = true,
                Address = affiliateAddress
            };
            _affiliateRepository.Insert(affilate);
        }

        private void AddArticleTag(Article article, string tag)
        {
            var articleTag = _articleTagRepository.Table.FirstOrDefault(pt => pt.Name == tag);
            if (articleTag == null)
            {
                articleTag = new ArticleTag
                {
                    Name = tag,
                };
            }
            article.ArticleTags.Add(articleTag);
            _articleRepository.Update(article);
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
            InstallMeasures();
            InstallTaxCategories();
            InstallLanguages();
            InstallCurrencies();
            InstallCountriesAndStates();
            
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings(installSampleData);
            InstallTopicTemplates();
            InstallTopics();
            InstallLocaleResources();
            InstallActivityLogTypes();
            InstallArticleTemplates();
            InstallCategoryTemplates();
            InstallPublisherTemplates();
            InstallScheduleTasks();
            InstallReturnRequestReasons();
            InstallReturnRequestActions();

            if (installSampleData)
            {
                InstallCheckoutAttributes();
                InstallSpecificationAttributes();
                InstallArticleAttributes();
                InstallCategories();
                InstallPublishers();
                InstallArticles(defaultUserEmail);
                InstallForums();
                 
                InstallBlogPosts(defaultUserEmail);
                InstallNews(defaultUserEmail);
                InstallPolls();
                InstallWarehouses();
                InstallContributors();
                InstallAffiliates();
                InstallSubscriptions();
                InstallActivityLog(defaultUserEmail);
                InstallSearchTerms();
            }
        }

        #endregion
    }
}