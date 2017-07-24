using Autofac;
using Autofac.Core;
using YStory.Core.Caching;
using YStory.Core.Configuration;
using YStory.Core.Infrastructure;
using YStory.Core.Infrastructure.DependencyManagement;
using YStory.Web.Controllers;
using YStory.Web.Factories;
using YStory.Web.Infrastructure.Installation;

namespace YStory.Web.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, YStoryConfig config)
        {
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();




            //controllers (we cache some data between HTTP requests)
            builder.RegisterType<ArticleController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
            builder.RegisterType<ShoppingCartController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));




            //factories (we cache presentation models between HTTP requests)
            builder.RegisterType<AddressModelFactory>().As<IAddressModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BlogModelFactory>().As<IBlogModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogModelFactory>().As<ICatalogModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CheckoutModelFactory>().As<ICheckoutModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CountryModelFactory>().As<ICountryModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomerModelFactory>().As<ICustomerModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ForumModelFactory>().As<IForumModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ExternalAuthenticationModelFactory>().As<IExternalAuthenticationModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NewsModelFactory>().As<INewsModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<NewsletterModelFactory>().As<INewsletterModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubscriptionModelFactory>().As<ISubscriptionModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PollModelFactory>().As<IPollModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<PrivateMessagesModelFactory>().As<IPrivateMessagesModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ArticleModelFactory>().As<IArticleModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<ProfileModelFactory>().As<IProfileModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ReturnRequestModelFactory>().As<IReturnRequestModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<ShoppingCartModelFactory>().As<IShoppingCartModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<TopicModelFactory>().As<ITopicModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<ContributorModelFactory>().As<IContributorModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WidgetModelFactory>().As<IWidgetModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Subscription of this dependency registrar implementation
        /// </summary>
        public int Subscription
        {
            get { return 2; }
        }
    }
}
