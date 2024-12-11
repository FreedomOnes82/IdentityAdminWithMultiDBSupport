using Framework.Core.DataAccessors;
using Framework.Core.ORM;
using Framework.Core.UOW;
using Microsoft.Extensions.DependencyInjection;
using Framework.Core.Blazor.Admin.MySql.Logs.IDataLayer;
using Framework.Core.Blazor.Admin.MySql.Logs.DataLayer;
using Framework.Core.Blazor.Admin.MySql.Logs.Abstractions;
using Framework.Core.Blazor.Admin.MySql.Logs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.MySql.Logs
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
