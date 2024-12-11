using System.Security.Claims;

namespace Blazor.Admin.Web.Models
{
    public class CustomClaims
    {
        public string Category { get; set; } = string.Empty;
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsFromRole { get; set; } = false;
        public bool IsCategoryBase { get; set; } = false;
        public CustomClaims(string type, string value, string category = null, bool isCategoryBase = false)
        {
            ClaimType = type;
            ClaimValue = value;
            Category = category;
            IsCategoryBase = isCategoryBase;
        }

        public static List<CustomClaims> GetDefaultClaims()
        {
            return new List<CustomClaims>()
            {
                new CustomClaims("Permission", "View Users", "User Management",true),
                new CustomClaims("Permission", "Create Users", "User Management"),
                new CustomClaims("Permission", "Update Users", "User Management"),
                new CustomClaims("Permission", "Delete Users", "User Management"),
                new CustomClaims("Permission", "Manage User Claims", "User Management"),
                new CustomClaims("Permission", "View Roles", "Role Management",true),
                new CustomClaims("Permission", "Create Roles", "Role Management"),
                new CustomClaims("Permission", "Update Roles", "Role Management"),
                new CustomClaims("Permission", "Delete Roles", "Role Management"),
                new CustomClaims("Permission", "Manage Role Claims", "Role Management"),
                new CustomClaims("Permission", "View System Logs", "Log Management"),
                new CustomClaims("Permission", "View Audit Logs", "Log Management"),
            };
        }
    }
}
