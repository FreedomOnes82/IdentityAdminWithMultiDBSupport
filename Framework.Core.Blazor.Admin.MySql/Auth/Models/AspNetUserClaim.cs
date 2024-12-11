using Framework.Core.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Models
{
    [DataTable("AspNetUserClaims")]
    public class AspNetUserClaim
    {
        [DBField("Id", true, true)]
        public int Id { get; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
