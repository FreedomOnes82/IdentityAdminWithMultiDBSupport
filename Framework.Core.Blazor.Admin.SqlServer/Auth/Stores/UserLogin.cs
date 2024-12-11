namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Stores
{
    internal class UserLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }
    }
}
