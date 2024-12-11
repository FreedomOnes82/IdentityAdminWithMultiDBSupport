﻿namespace Framework.Core.Blazor.Admin.SqlServer.DatabaseScriptConfig
{
    public class DBProviderOptions
    {
        public string DbSchema { get; set; } = "[dbo]";

        public string ConnectionString { get; set; }
    }
}