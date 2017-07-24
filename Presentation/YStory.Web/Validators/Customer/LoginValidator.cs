using FluentValidation;
using YStory.Core.Domain.Customers;
using YStory.Services.Localization;
using YStory.Web.Framework.Validators;
using YStory.Web.Models.Customer;

namespace YStory.Web.Validators.Customer
{
    public partial class LoginValidator : BaseYStoryValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
    }
}