namespace Framework.Core.Blazor.Admin.MySql.DatabaseScriptConfig
{
    public class DBProviderOptions
    {
        public string DbSchema { get; set; } = "[dbo]";

        public string ConnectionString { get; set; }
    }
}
