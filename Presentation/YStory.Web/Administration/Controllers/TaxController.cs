﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using YStory.Admin.Extensions;
using YStory.Admin.Models.Tax;
using YStory.Core.Domain.Tax;
using YStory.Services.Configuration;
using YStory.Services.Security;
using YStory.Services.Tax;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class TaxController : BaseAdminController
	{
		#region Fields

        private readonly ITaxService _taxService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly TaxSettings _taxSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TaxController(ITaxService taxService,
            ITaxCategoryService taxCategoryService,
            TaxSettings taxSettings,
            ISettingService settingService,
            IPermissionService permissionService)
		{
            this._taxService = taxService;
            this._taxCategoryService = taxCategoryService;
            this._taxSettings = taxSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Tax Providers

        public virtual ActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();
            
            return View();
        }

        [HttpPost]
        public virtual ActionResult Providers(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var taxProvidersModel = _taxService.LoadAllTaxProviders()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var tpm in taxProvidersModel)
                tpm.IsPrimaryTaxProvider = tpm.SystemName.Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);
            var gridModel = new DataSourceResult
            {
                Data = taxProvidersModel,
                Total = taxProvidersModel.Count()
            };

            return Json(gridModel);
        }

        public virtual ActionResult ConfigureProvider(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
            if (taxProvider == null)
                //No tax provider found with the specified id
                return RedirectToAction("Providers");

            var model = taxProvider.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            taxProvider.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        public virtual ActionResult MarkAsPrimaryProvider(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
            {
                return RedirectToAction("Providers");
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
            if (taxProvider != null)
            {
                _taxSettings.ActiveTaxProviderSystemName = systemName;
                _settingService.SaveSetting(_taxSettings);
            }

            return RedirectToAction("Providers");
        }

        #endregion

        #region Tax Categories

        public virtual ActionResult Categories()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult Categories(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var categoriesModel = _taxCategoryService.GetAllTaxCategories()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = categoriesModel,
                Total = categoriesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult CategoryUpdate(TaxCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var taxCategory = _taxCategoryService.GetTaxCategoryById(model.Id);
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.UpdateTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult CategoryAdd([Bind(Exclude = "Id")] TaxCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var taxCategory = new TaxCategory();
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.InsertTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult CategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxCategory = _taxCategoryService.GetTaxCategoryById(id);
            if (taxCategory == null)
                throw new ArgumentException("No tax category found with the specified id");
            _taxCategoryService.DeleteTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        #endregion
    }
}
