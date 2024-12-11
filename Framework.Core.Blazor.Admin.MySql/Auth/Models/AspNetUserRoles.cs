using Framework.Core.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Models
{
    [DataTable("AspNetUserRoles")]
    public class AspNetUserRole
    {
        [DBField("UserId", true)]
        public string UserId { get; set; }
        [DBField("RoleId", true)]
        public string RoleId { get; set; }
    }
}
