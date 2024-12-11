using Blazor.Admin.Web.Components.Global;
using Blazor.Admin.Web.Locales;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blazor.Admin.Web.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class CreateUserModel : IValidatableObject
    {
        private readonly IStringLocalizer<Resource> _localizer;

        public CreateUserModel(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;
        }

        public string Email { get; set; } = "";

        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Email))
            {
                yield return new ValidationResult(_localizer["Model.Email"] + _localizer["Error.Required"], new[] { nameof(Email) });
            }

            Regex emailRegex = new Regex(GlobalSettings.EmailRegularExpression);
            if (!emailRegex.IsMatch(Email))
            {
                yield return new ValidationResult(_localizer["Error.EmailFormat"], new[] { nameof(Email) });
            }

            if (Password.Length < 8 || Password.Length > 100)
            {
                yield return new ValidationResult(_localizer["Model.Password"] + _localizer["Error.LengthLimitation"] + "8-100", new[] { nameof(Password) });
            }
            if (ConfirmPassword != Password)
            {
                yield return new ValidationResult(_localizer["Error.PwdNotMatch"], new[] { nameof(ConfirmPassword) });
            }
        }
    }

    public class EditUserModel : IValidatableObject
    {
        private readonly IStringLocalizer<Resource> _localizer;

        public EditUserModel(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;
        }
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = "";
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                yield return new ValidationResult(_localizer["Model.UserName"] + _localizer["Error.Required"], new[] { nameof(UserName) });
            }

            if (string.IsNullOrEmpty(Email))
            {
                yield return new ValidationResult(_localizer["Model.Email"] + _localizer["Error.Required"], new[] { nameof(Email) });
            }

            Regex userNameRegex = new Regex(GlobalSettings.NameRegularExpression);
            if (!userNameRegex.IsMatch(UserName))
            {
                yield return new ValidationResult(_localizer["Model.UserName"] + _localizer["Error.NameLimitation"], new[] { nameof(UserName) });
            }

            Regex emailRegex = new Regex(GlobalSettings.EmailRegularExpression);
            if (!emailRegex.IsMatch(Email))
            {
                yield return new ValidationResult(_localizer["Error.EmailFormat"], new[] { nameof(Email) });
            }
        }
    }

    public class ChangePwdModel : IValidatableObject
    {
        private readonly IStringLocalizer<Resource> _localizer;

        public ChangePwdModel(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;
        }

        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = "";

        [Display(Name = "New Password Confirm")]
        public string NewPasswordConfirm { get; set; } = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                yield return new ValidationResult(_localizer["Model.NewPassword"] + _localizer["Error.Required"], new[] { nameof(NewPassword) });
            }
            if (NewPassword.Length < 8 || NewPassword.Length > 100)
            {
                yield return new ValidationResult(_localizer["Model.Password"] + _localizer["Error.LengthLimitation"] + "8-100", new[] { nameof(NewPassword) });
            }
            if (NewPasswordConfirm != NewPassword)
            {
                yield return new ValidationResult(_localizer["Error.PwdNotMatch"], new[] { nameof(NewPasswordConfirm) });
            }
        }
    }

}
