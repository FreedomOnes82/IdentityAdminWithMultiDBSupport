namespace Framework.Core.Blazor.Admin.MySql.Auth.Stores
{
    internal class UserClaim
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
