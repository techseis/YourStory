using System;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Customers;
using YStory.Core.Infrastructure;

namespace YStory.Web.Framework.Controllers
{
    /// <summary>
    /// Attribute to ensure that users with "Contributor" customer role has appropriate contributor account associated (and active)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public class AdminContributorValidation : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool _ignore;

        public AdminContributorValidation(bool ignore = false)
        {
            this._ignore = ignore;
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (_ignore)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (!workContext.CurrentCustomer.IsContributor())
                return;

            //ensure that this user has active contributor record associated
            if (workContext.CurrentContributor == null)
                filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
