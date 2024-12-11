using Framework.Core.ORM;
using Framework.Core.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.Blazor.Admin.MySql.Logs.DataModels
{
    [DataTable("AuditLogs")]
    public class AuditLog
    {
        //public int ID { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }

        [Key]
        [DBField("Id", true, true)]
        public int ID { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;
        [MaxLength(50)]
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    public class AuditLogFilter : PagerInfo
    {
        public string? UserName { get; set; } = string.Empty;
        public int? Status { get; set; }
        public DateTime RangeFrom { get; set; }
        public DateTime RangeTo { get; set; }
        public string? Message { get; set; } = string.Empty;
    }

    public enum AuditStatus
    {
        Failed=0,
        Success=1
    }
}
