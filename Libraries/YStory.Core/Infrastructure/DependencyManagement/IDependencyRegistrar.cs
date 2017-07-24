using Autofac;
using YStory.Core.Configuration;

namespace YStory.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Dependency registrar interface
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, YStoryConfig config);

        /// <summary>
        /// Subscription of this dependency registrar implementation
        /// </summary>
        int Subscription { get; }
    }
}
