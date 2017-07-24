using System.Collections.Generic;
using System.Web.Optimization;

namespace YStory.Web.Framework.UI
{
    public partial class AsIsBundleOrder : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
