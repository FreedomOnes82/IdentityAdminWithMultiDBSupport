using Framework.Core.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Models
{
    [DataTable("AspNetUserTokens")]
    public class AspNetUserToken
    {
        [DBField("UserId",true)]
        public string UserId { get; set; }
        [DBField("LoginProvider", true)]
        public string LoginProvider { get; set; }
        [DBField("Name", true)]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
