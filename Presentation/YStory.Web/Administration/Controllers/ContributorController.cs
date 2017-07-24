using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Extensions;
using YStory.Admin.Models.Contributors;
using YStory.Core.Domain.Directory;
using YStory.Core.Domain.Contributors;
using YStory.Services.Common;
using YStory.Services.Customers;
using YStory.Services.Directory;
using YStory.Services.Helpers;
using YStory.Services.Localization;
using YStory.Services.Logging;
using YStory.Services.Media;
using YStory.Services.Security;
using YStory.Services.Seo;
using YStory.Services.Contributors;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Kendoui;
using YStory.Web.Framework.Mvc;

namespace YStory.Admin.Controllers
{
    public partial class ContributorController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IContributorService _contributorService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ContributorSettings _contributorSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Constructors

        public ContributorController(ICustomerService customerService, 
            ILocalizationService localizationService,
            IContributorService contributorService, 
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IDateTimeHelper dateTimeHelper,
            ContributorSettings contributorSettings,
            ICustomerActivityService customerActivityService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService)
        {
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._contributorService = contributorService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._dateTimeHelper = dateTimeHelper;
            this._contributorSettings = contributorSettings;
            this._customerActivityService = customerActivityService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Contributor contributor)
        {
            var picture = _pictureService.GetPictureById(contributor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(contributor.Name));
        }

        [NonAction]
        protected virtual void UpdateLocales(Contributor contributor, ContributorModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(contributor,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(contributor,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(contributor,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(contributor,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(contributor,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = contributor.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(contributor, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareContributorModel(ContributorModel model, Contributor contributor, bool excludeProperties, bool prepareEntireAddressModel)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var address = _addressService.GetAddressById(contributor != null ? contributor.AddressId : 0);

            if (contributor != null)
            {
                if (!excludeProperties)
                {
                    if (address != null)
                    {
                        model.Address = address.ToModel();
                    }
                }

                //associated customer emails
                model.AssociatedCustomers = _customerService
                    .GetAllCustomers(contributorId: contributor.Id)
                    .Select(c => new ContributorModel.AssociatedCustomerInfo()
                    {
                        Id = c.Id,
                        Email = c.Email
                    })
                    .ToList();
            }

            if (prepareEntireAddressModel)
            {
                model.Address.CountryEnabled = true;
                model.Address.StateProvinceEnabled = true;
                model.Address.CityEnabled = true;
                model.Address.StreetAddressEnabled = true;
                model.Address.StreetAddress2Enabled = true;
                model.Address.ZipPostalCodeEnabled = true;
                model.Address.PhoneEnabled = true;
                model.Address.FaxEnabled = true;

                //address
                model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(showHidden: true))
                    model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (address != null && c.Id == address.CountryId) });

                var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                        model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (address != null && s.Id == address.StateProvinceId) });
                }
                else
                    model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            }
        }

        #endregion

        #region Contributors

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var model = new ContributorListModel();
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, ContributorListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedKendoGridJson();

            var contributors = _contributorService.GetAllContributors(model.SearchName, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = contributors.Select(x =>
                {
                    var contributorModel = x.ToModel();
                    PrepareContributorModel(contributorModel, x, false, false);
                    return contributorModel;
                }),
                Total = contributors.TotalCount,
            };

            return Json(gridModel);
        }

        //create
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();


            var model = new ContributorModel();
            PrepareContributorModel(model, null, false, true);
            //locales
            AddLocales(_languageService, model.Locales);
            //default values
            model.PageSize = 6;
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _contributorSettings.DefaultContributorPageSizeOptions;

            //default value
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Create(ContributorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var contributor = model.ToEntity();
                _contributorService.InsertContributor(contributor);

                //activity log
                _customerActivityService.InsertActivity("AddNewContributor", _localizationService.GetResource("ActivityLog.AddNewContributor"), contributor.Id);

                //search engine name
                model.SeName = contributor.ValidateSeName(model.SeName, contributor.Name, true);
                _urlRecordService.SaveSlug(contributor, model.SeName, 0);

                //address
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                _addressService.InsertAddress(address);
                contributor.AddressId = address.Id;
                _contributorService.UpdateContributor(contributor);

                //locales
                UpdateLocales(contributor, model);
                //update picture seo file name
                UpdatePictureSeoNames(contributor);

                SuccessNotification(_localizationService.GetResource("Admin.Contributors.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = contributor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareContributorModel(model, null, true, true);
            return View(model);
        }


        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var contributor = _contributorService.GetContributorById(id);
            if (contributor == null || contributor.Deleted)
                //No contributor found with the specified id
                return RedirectToAction("List");

            var model = contributor.ToModel();
            PrepareContributorModel(model, contributor, false, true);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = contributor.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = contributor.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = contributor.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = contributor.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = contributor.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = contributor.GetSeName(languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ContributorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var contributor = _contributorService.GetContributorById(model.Id);
            if (contributor == null || contributor.Deleted)
                //No contributor found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = contributor.PictureId;
                contributor = model.ToEntity(contributor);
                _contributorService.UpdateContributor(contributor);

                //activity log
                _customerActivityService.InsertActivity("EditContributor", _localizationService.GetResource("ActivityLog.EditContributor"), contributor.Id);

                //search engine name
                model.SeName = contributor.ValidateSeName(model.SeName, contributor.Name, true);
                _urlRecordService.SaveSlug(contributor, model.SeName, 0);

                //address
                var address = _addressService.GetAddressById(contributor.AddressId);
                if (address == null)
                {
                    address = model.Address.ToEntity();
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.InsertAddress(address);
                    contributor.AddressId = address.Id;
                    _contributorService.UpdateContributor(contributor);
                }
                else
                {
                    address = model.Address.ToEntity(address);
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.UpdateAddress(address);
                }


                //locales
                UpdateLocales(contributor, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != contributor.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(contributor);

                SuccessNotification(_localizationService.GetResource("Admin.Contributors.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = contributor.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareContributorModel(model, contributor, true, true);

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var contributor = _contributorService.GetContributorById(id);
            if (contributor == null)
                //No contributor found with the specified id
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = _customerService.GetAllCustomers(contributorId: contributor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.ContributorId = 0;
                _customerService.UpdateCustomer(customer);
            }

            //delete a contributor
            _contributorService.DeleteContributor(contributor);

            //activity log
            _customerActivityService.InsertActivity("DeleteContributor", _localizationService.GetResource("ActivityLog.DeleteContributor"), contributor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Contributors.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Contributor notes

        [HttpPost]
        public virtual ActionResult ContributorNotesSelect(int contributorId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedKendoGridJson();

            var contributor = _contributorService.GetContributorById(contributorId);
            if (contributor == null)
                throw new ArgumentException("No contributor found with the specified id");

            var contributorNoteModels = new List<ContributorModel.ContributorNote>();
            foreach (var contributorNote in contributor.ContributorNotes
                .OrderByDescending(vn => vn.CreatedOnUtc))
            {
                contributorNoteModels.Add(new ContributorModel.ContributorNote
                {
                    Id = contributorNote.Id,
                    ContributorId = contributorNote.ContributorId,
                    Note = contributorNote.FormatContributorNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(contributorNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = contributorNoteModels,
                Total = contributorNoteModels.Count
            };

            return Json(gridModel);
        }

        [ValidateInput(false)]
        public virtual ActionResult ContributorNoteAdd(int contributorId, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var contributor = _contributorService.GetContributorById(contributorId);
            if (contributor == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var contributorNote = new ContributorNote
            {
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
            };
            contributor.ContributorNotes.Add(contributorNote);
            _contributorService.UpdateContributor(contributor);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult ContributorNoteDelete(int id, int contributorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributors))
                return AccessDeniedView();

            var contributor = _contributorService.GetContributorById(contributorId);
            if (contributor == null)
                throw new ArgumentException("No contributor found with the specified id");

            var contributorNote = contributor.ContributorNotes.FirstOrDefault(vn => vn.Id == id);
            if (contributorNote == null)
                throw new ArgumentException("No contributor note found with the specified id");
            _contributorService.DeleteContributorNote(contributorNote);

            return new NullJsonResult();
        }

        #endregion

    }
}
