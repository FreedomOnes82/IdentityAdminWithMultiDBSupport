using Framework.Core.Abstractions;
using Framework.Core.Blazor.Admin.MySql.Logs.DataModels;

namespace Framework.Core.Blazor.Admin.MySql.Logs.IDataLayer
{
    public interface IAuditLogDAL : IDALBase
    {
        Task<AuditLog> Create(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAll();
        Task<PagerResult<AuditLog>> SearchAsync(AuditLogFilter filter);
        Task<IEnumerable<AuditLog>> GetForExport(AuditLogFilter filter);
    }
}
