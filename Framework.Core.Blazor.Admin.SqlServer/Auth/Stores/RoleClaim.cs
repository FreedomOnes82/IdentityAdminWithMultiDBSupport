namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Stores
{
    internal class RoleClaim
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
