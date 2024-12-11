namespace Blazor.Admin.Web.Models
{

    public enum LogLevel
    {
        Info,
        Warn,
        Error,
        Verbose,
        Debug,
        Fatal
    }

    public class SystemLogSearchOptions
    {
        public DateTime DateFrom { get; set; } = DateTime.Today;
        public DateTime DateEnd { get; set; } = DateTime.Today;
        public List<LogLevel> Levels { get; set; } = new List<LogLevel>();
        public string Keywords { get; set; } = string.Empty;
    }

    public class SystemLogModel
    {
        public string Message { get; set; } = string.Empty;
        public LogLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LevelModal
    {
        public string Text { get; set; } = string.Empty;
        public LogLevel Value { get; set; }
    }
}
