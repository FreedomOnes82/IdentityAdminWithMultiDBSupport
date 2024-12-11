using Framework.Core.DataAccessors;
using Framework.Core.ORM;
using Framework.Core.UOW;
using Microsoft.Extensions.DependencyInjection;
using Framework.Core.Blazor.Admin.SqlServer.Logs.IDataLayer;
using Framework.Core.Blazor.Admin.SqlServer.Logs.DataLayer;
using Framework.Core.Blazor.Admin.SqlServer.Logs.Abstractions;
using Framework.Core.Blazor.Admin.SqlServer.Logs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer.Logs
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuditLogDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IAuditLogDAL,AuditLogDAL>();
            services.AddTransient<IAuditLogService, AuditLogService>();
        }
    }
}
