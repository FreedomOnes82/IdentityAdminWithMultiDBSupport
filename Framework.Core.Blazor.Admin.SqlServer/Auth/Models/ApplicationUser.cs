using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Framework.Core.ORM;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Stores;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Models
{
    [DataTable("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        [DBField("Id", true, false)]
        public override string Id { get; set; }
        //public int UserType { get; set; }
        //public bool IsActive { get; set; } = true;

        public List<Claim> Claims { get; set; }
        internal List<UserRole> Roles { get; set; }
        internal List<UserLoginInfo> Logins { get; set; }
        internal List<UserToken> Tokens { get; set; }
    }
}
