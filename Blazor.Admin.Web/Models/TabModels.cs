using Microsoft.AspNetCore.Components;

namespace Blazor.Admin.Web.Models
{

    public class TabModel
    {
        public Func<string> HeaderFun { get; set; } = () => string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public RenderFragment Content { get; set; }
        public object? Parameters { get; set; }
    }
}
