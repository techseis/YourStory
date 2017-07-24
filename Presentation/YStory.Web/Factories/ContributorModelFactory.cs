using System;
using YStory.Core;
using YStory.Core.Domain.Media;
using YStory.Services.Localization;
using YStory.Services.Media;
using YStory.Web.Framework.Security.Captcha;
using YStory.Web.Models.Contributors;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the contributor model factory
    /// </summary>
    public partial class ContributorModelFactory : IContributorModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        
        private readonly CaptchaSettings _captchaSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Constructors

        public ContributorModelFactory(IWorkContext workContext,
            ILocalizationService localizationService,
            IPictureService pictureService,
            CaptchaSettings captchaSettings,
            MediaSettings mediaSettings)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            
            this._captchaSettings = captchaSettings;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the apply contributor model
        /// </summary>
        /// <param name="model">The apply contributor model</param>
        /// <param name="validateContributor">Whether to validate that the customer is already a contributor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>The apply contributor model</returns>
        public virtual ApplyContributorModel PrepareApplyContributorModel(ApplyContributorModel model, bool validateContributor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (validateContributor && _workContext.CurrentCustomer.ContributorId > 0)
            {
                //already applied for contributor account
                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Contributors.ApplyAccount.AlreadyApplied");
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyContributorPage;

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the contributor info model
        /// </summary>
        /// <param name="model">Contributor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contributor info model</returns>
        public virtual ContributorInfoModel PrepareContributorInfoModel(ContributorInfoModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var contributor = _workContext.CurrentContributor;
            if (!excludeProperties)
            {
                model.Description = contributor.Description;
                model.Email = contributor.Email;
                model.Name = contributor.Name;
            }

            var picture = _pictureService.GetPictureById(contributor.PictureId);
            var pictureSize = _mediaSettings.AvatarPictureSize;
            model.PictureUrl = picture != null ? _pictureService.GetPictureUrl(picture, pictureSize) : string.Empty;

            return model;
        }
        
        #endregion
    }
}
