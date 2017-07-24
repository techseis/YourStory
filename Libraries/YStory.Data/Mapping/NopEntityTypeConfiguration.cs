using System.Data.Entity.ModelConfiguration;

namespace YStory.Data.Mapping
{
    public abstract class YStoryEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected YStoryEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in subscription to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}