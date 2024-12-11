using Framework.Core.ORM;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Models
{
    [DataTable("AspNetRoles")]
    public class ApplicationRole : IdentityRole
    {
        [DBField("Id", true, false)]
        public override string Id { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
