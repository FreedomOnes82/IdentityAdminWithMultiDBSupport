namespace Framework.Core.Blazor.Admin.MySql.Auth.Stores
{
    internal class UserToken
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
