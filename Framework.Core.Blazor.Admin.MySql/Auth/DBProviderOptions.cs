namespace Framework.Core.Blazor.Admin.MySql.Auth
{
    public class DBProviderOptions
    {
        public string DbSchema { get; set; } = "dbo";

        public string ConnectionString { get; set; }
    }
}
