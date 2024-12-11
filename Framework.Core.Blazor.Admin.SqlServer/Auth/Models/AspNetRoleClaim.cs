using Framework.Core.ORM;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Models
{
    [DataTable("AspNetRoleClaims")]
    public class AspNetRoleClaim
    {
        [DBField("Id", true, true)]
        public int Id { get; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
