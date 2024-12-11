using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Framework.Core.Blazor.Admin.MySql.DatabaseScriptConfig
{
    /// <summary>
    /// Extension methods to add Dapper support to IdentityServer.
    /// </summary>
    public static class AdminServerDbScriptsExtensions
    {
        public static IServiceCollection AddIdentityDbUpDatabaseScripts(this IServiceCollection services, Action<DBProviderOptions> dbProviderOptionsAction = null)
        {
            var options = GetDefaultOptions();
            dbProviderOptionsAction?.Invoke(options);
            services.AddSingleton(options);
            services.TryAddTransient<IAdminMigration, Migrations>();
            return services;
        }
        public static DBProviderOptions GetDefaultOptions()
        {
            //config mssql
            var options = new DBProviderOptions();
            return options;
        }
    }
}
