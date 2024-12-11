using Blazor.Admin.Web.Components.Global;
using Blazor.Admin.Web.Locales;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blazor.Admin.Web.Models
{
    public class RoleModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[a-zA-Z0-9_ ]{3,50}$", ErrorMessage = "Must be composed of 3-50 digits, letters, spaces and underscores.")]
        public string RoleName { get; set; } = string.Empty;

        public List<System.Security.Claims.Claim> Claims { get; set; } = new List<System.Security.Claims.Claim>();
    }

    public class EditRoleModel : IValidatableObject
    {
        private readonly IStringLocalizer<Resource> _localizer;

        public EditRoleModel(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;
        }
        public string Id { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                yield return new ValidationResult(_localizer["Model.RoleName"] + _localizer["Error.Required"], new[] { nameof(RoleName) });
            }

            Regex emailRegex = new Regex(GlobalSettings.NameRegularExpression);
            if (!emailRegex.IsMatch(RoleName))
            {
                yield return new ValidationResult(_localizer["Model.RoleName"] + _localizer["Error.NameLimitation"], new[] { nameof(RoleName) });
            }
        }
    }
}
