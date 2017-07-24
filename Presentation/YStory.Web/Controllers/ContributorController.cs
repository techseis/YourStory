using System;
using System.Web;
using System.Web.Mvc;
using YStory.Core;
using YStory.Core.Domain.Customers;
using YStory.Core.Domain.Localization;
using YStory.Core.Domain.Media;
using YStory.Core.Domain.Contributors;
using YStory.Services.Customers;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Services.Messages;
using YStory.Services.Seo;
using YStory.Services.Contributors;
using YStory.Web.Factories;
using YStory.Web.Framework.Controllers;
using YStory.Web.Framework.Security;
using YStory.Web.Framework.Security.Captcha;
using YStory.Web.Models.Contributors;

namespace YStory.Web.Controllers
{
    public partial class ContributorController : BasePublicController
    {
        #region Fields

        private readonly IContributorModelFactory _contributorModelFactory;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IContributorService _contributorService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;

        private readonly LocalizationSettings _localizationSettings;
        private readonly ContributorSettings _contributorSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Constructors

        public ContributorController(IContributorModelFactory contributorModelFactory,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IContributorService contributorService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            LocalizationSettings localizationSettings,
            ContributorSettings contributorSettings,
            CaptchaSettings captchaSettings)
        {
            this._contributorModelFactory = contributorModelFactory;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._contributorService = contributorService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;

            this._localizationSettings = localizationSettings;
            this._contributorSettings = contributorSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Utilites

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Contributor contributor)
        {
            var picture = _pictureService.GetPictureById(contributor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(contributor.Name));
        }

        #endregion

        #region Methods

        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ApplyContributor()
        {
            if (!_contributorSettings.AllowCustomersToApplyForContributorAccount)
                return RedirectToRoute("HomePage");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new ApplyContributorModel();
            model = _contributorModelFactory.PrepareApplyContributorModel(model, true, false);
            return View(model);
        }

        [HttpPost, ActionName("ApplyContributor")]
        [PublicAntiForgery]
        [CaptchaValidator]
        public virtual ActionResult ApplyContributorSubmit(ApplyContributorModel model, bool captchaValid, HttpPostedFileBase uploadedFile)
        {
            if (!_contributorSettings.AllowCustomersToApplyForContributorAccount)
                return RedirectToRoute("HomePage");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyContributorPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            int pictureId = 0;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var contributorPictureBinary = uploadedFile.GetPictureBits();
                    var picture = _pictureService.InsertPicture(contributorPictureBinary, contentType, null);

                    if (picture != null)
                        pictureId = picture.Id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Contributors.ApplyAccount.Picture.ErrorMessage"));
                }
            }

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);
                //disabled by default
                var contributor = new Contributor
                {
                    Name = model.Name,
                    Email = model.Email,
                    //some default settings
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = _contributorSettings.DefaultContributorPageSizeOptions,
                    PictureId = pictureId,
                    Description = description
                };
                _contributorService.InsertContributor(contributor);
                //search engine name (the same as contributor name)
                var seName = contributor.ValidateSeName(contributor.Name, contributor.Name, true);
                _urlRecordService.SaveSlug(contributor, seName, 0);

                //associate to the current customer
                //but a store owner will have to manually add this customer role to "Contributors" role
                //if he wants to grant access to admin area
                _workContext.CurrentCustomer.ContributorId = contributor.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                //update picture seo file name
                UpdatePictureSeoNames(contributor);

                //notify store owner here (email)
                _workflowMessageService.SendNewContributorAccountApplyStoreOwnerNotification(_workContext.CurrentCustomer,
                    contributor, _localizationSettings.DefaultAdminLanguageId);

                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Contributors.ApplyAccount.Submitted");
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _contributorModelFactory.PrepareApplyContributorModel(model, false, true);
            return View(model);
        }

        [YStoryHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentContributor == null || !_contributorSettings.AllowContributorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var model = new ContributorInfoModel();
            model = _contributorModelFactory.PrepareContributorInfoModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [PublicAntiForgery]
        [ValidateInput(false)]
        [FormValueRequired("save-info-button")]
        public virtual ActionResult Info(ContributorInfoModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentContributor == null || !_contributorSettings.AllowContributorsToEditInfo)
                return RedirectToRoute("CustomerInfo");
            
            Picture picture = null;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                 {
                    var contentType = uploadedFile.ContentType;
                    var contributorPictureBinary = uploadedFile.GetPictureBits();
                    picture = _pictureService.InsertPicture(contributorPictureBinary, contentType, null);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.ContributorInfo.Picture.ErrorMessage"));
                }
            }

            var contributor = _workContext.CurrentContributor;
            var prevPicture = _pictureService.GetPictureById(contributor.PictureId);

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);

                contributor.Name = model.Name;
                contributor.Email = model.Email;
                contributor.Description = description;

                if (picture != null)
                {
                    contributor.PictureId = picture.Id;

                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(contributor);

                _contributorService.UpdateContributor(contributor);

                //notifications
                if (_contributorSettings.NotifyStoreOwnerAboutContributorInformationChange)
                    _workflowMessageService.SendContributorInformationChangeNotification(contributor, _localizationSettings.DefaultAdminLanguageId);

                return RedirectToAction("Info");
            }

            //If we got this far, something failed, redisplay form
            model = _contributorModelFactory.PrepareContributorInfoModel(model, true);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [PublicAntiForgery]
        [ValidateInput(false)]
        [FormValueRequired("remove-picture")]
        public virtual ActionResult RemovePicture()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentContributor == null || !_contributorSettings.AllowContributorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var contributor = _workContext.CurrentContributor;
            var picture = _pictureService.GetPictureById(contributor.PictureId);

            if (picture != null)
                _pictureService.DeletePicture(picture);

            contributor.PictureId = 0;
            _contributorService.UpdateContributor(contributor);

            //notifications
            if (_contributorSettings.NotifyStoreOwnerAboutContributorInformationChange)
                _workflowMessageService.SendContributorInformationChangeNotification(contributor, _localizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }

        #endregion
    }
}
