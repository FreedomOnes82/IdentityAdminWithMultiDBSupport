using Framework.Core.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Models
{
    [DataTable("AspNetUserLogins")]
    public class AspNetUserLogin
    {
        [DBField("LoginProvider", true)]
        public string LoginProvider { get; set; }
        [DBField("ProviderKey", true)]
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }

    }
}
