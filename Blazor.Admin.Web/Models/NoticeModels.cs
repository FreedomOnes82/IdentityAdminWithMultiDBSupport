namespace Blazor.Admin.Web.Models
{
    public class NoticeModel
    {
        public string Message { get; set; } = string.Empty;
        public NoticeStatus Status { get; set; }
        public bool Show { get; set; }
        public NoticeModel(string message, NoticeStatus status = NoticeStatus.Success)
        {
            Message = message;
            Status = status;
            Show = true;
        }
    }
    public enum NoticeStatus
    {
        Success,
        Falied,
        Warning
    }
}
