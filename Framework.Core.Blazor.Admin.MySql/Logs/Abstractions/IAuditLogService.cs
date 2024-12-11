using Framework.Core.Abstractions;
using Framework.Core.Blazor.Admin.MySql.Logs.DataModels;

namespace Framework.Core.Blazor.Admin.MySql.Logs.Abstractions
{
    public interface IAuditLogService
    {
        Task<AuditLog> InsertAsync(AuditLog model);
        Task<IEnumerable<AuditLog>> GetAll();
        Task<PagerResult<AuditLog>> SearchAsync(AuditLogFilter filter);
        Task<IEnumerable<AuditLog>> GetForExport(AuditLogFilter filter);
    }
}
