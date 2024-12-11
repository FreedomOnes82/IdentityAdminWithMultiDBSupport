using Blazor.Admin.Web.Components.Global;
using Blazor.Admin.Web.Components.Layout;
using Blazor.Admin.Web.Models;

namespace Blazor.Admin.Web.Abstraction
{
    public interface IMainLayout
    {
        public Task NavigationTo(string url, string name, string iconClass = null, object parameters = null);
        public Task NewAuditlog(string message, bool isSuccess = true);
        public string CurrentRoute { get; set; }
        //public List<NoticeModel> NoticeLists { get; set; }
        public Task AddNotice(string message, NoticeStatus status = NoticeStatus.Success);

        //public bool RefreshRoles { get; set; }
        public bool ChangedRoleInfo { get; set; }

        public bool ChangedRoleClaims { get; set; }
        public bool AddNewAudit { get; set; }

    }
}
