using Framework.Core.DataAccessors;
using Framework.Core.ORM;
using Framework.Core.UOW;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseSqlServerDataAccessor(this IServiceCollection services)
        {
            services.AddTransient<Abstractions.IDbConnectionAccessor, SqlServerDataAccessor>();
        }

        public static void UseSqlServerDataAccessor(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<Abstractions.IDbConnectionAccessor>(SqlServerDataAccessor.CreateDataAccessor(connectionString));
        }

        public static void UseDefaultDataTablePrefixProvider(this IServiceCollection services)
        {
            services.AddTransient<Abstractions.IDataTablePrefixProvider, DefaultDataTablePrefixProvider>();
        }

        public static void UseUpdateCommandGroupProvider(this IServiceCollection services) 
        {
            services.AddTransient<Abstractions.IUpdateCommandGroupProvider, UpdateCommandGroupProviderBase>();
        }
    }
}
