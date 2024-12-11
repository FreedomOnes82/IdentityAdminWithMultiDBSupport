using Framework.Core.Blazor.Admin.SqlServer.Logs.DataModels;

namespace Blazor.Admin.Web.Models
{

    public class SearchOptions
    {
        public DateTime DateFrom { get; set; } = DateTime.Today;
        public DateTime DateEnd { get; set; } = DateTime.Today;
        public List<AuditStatus> Status { get; set; } = new List<AuditStatus>();
        public List<string> Operator { get; set; } = new List<string>();
        public string Keywords { get; set; } = string.Empty;
    }
    public class LogStatus
    {
        public string Text { get; set; } = string.Empty;
        public AuditStatus Value { get; set; }
    }
    public class OperatorModel
    {
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
