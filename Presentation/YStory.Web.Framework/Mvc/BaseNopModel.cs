using System.Collections.Generic;
using System.Web.Mvc;

namespace YStory.Web.Framework.Mvc
{
    /// <summary>
    /// Base yourStory model
    /// </summary>
    [ModelBinder(typeof(YStoryModelBinder))]
    public partial class BaseYStoryModel
    {
        public BaseYStoryModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    /// <summary>
    /// Base yourStory entity model
    /// </summary>
    public partial class BaseYStoryEntityModel : BaseYStoryModel
    {
        public virtual int Id { get; set; }
    }
}
