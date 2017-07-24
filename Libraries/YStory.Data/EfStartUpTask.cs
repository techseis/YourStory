using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Infrastructure;

namespace YStory.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IDataProvider>();
                if (provider == null)
                    throw new YStoryException("No IDataProvider found");
                provider.SetDatabaseInitializer();
            }
        }

        public int Subscription
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
