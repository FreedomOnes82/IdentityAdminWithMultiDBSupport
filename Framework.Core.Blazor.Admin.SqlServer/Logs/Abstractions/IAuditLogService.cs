using Framework.Core.Abstractions;
using Framework.Core.Blazor.Admin.SqlServer.Logs.DataModels;

namespace Framework.Core.Blazor.Admin.SqlServer.Logs.Abstractions
{
    public interface IAuditLogService
    {
        Task<AuditLog> InsertAsync(AuditLog model);
        Task<IEnumerable<AuditLog>> GetAll();
        Task<PagerResult<AuditLog>> SearchAsync(AuditLogFilter filter);
        Task<IEnumerable<AuditLog>> GetForExport(AuditLogFilter filter);
    }
}
