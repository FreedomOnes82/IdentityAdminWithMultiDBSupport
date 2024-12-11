using Framework.Core.Abstractions;
using Microsoft.Graph;
using Framework.Core.Blazor.Admin.SqlServer.Logs.Abstractions;
using Framework.Core.Blazor.Admin.SqlServer.Logs.DataModels;
using Framework.Core.Blazor.Admin.SqlServer.Logs.IDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer.Logs.Services
{
    public class AuditLogService:MyServiceBase,IAuditLogService
    {
        private readonly IAuditLogDAL _auditLogDAL;

        public AuditLogService(IDbConnectionAccessor dbConnectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider,IAuditLogDAL auditLogDAL)
            : base(dbConnectionAccessor, dataTablePrefixProvider)
        {
            _auditLogDAL = auditLogDAL;
        }


        public async Task<AuditLog> InsertAsync(AuditLog model)
        {
            return await _auditLogDAL.Create(model);
        }
        public async Task<IEnumerable<AuditLog>> GetAll()
        {
            return await _auditLogDAL.GetAll();
        }

        public async Task<PagerResult<AuditLog>> SearchAsync(AuditLogFilter filter)
        {
            //var dal = _dalFactory.CreateDAL<IAuditLogDAL>(_unitOfWorkFactory.CreateUnitOfWork());
            var data = await _auditLogDAL.SearchAsync(new AuditLogFilter
            {
                PageSize = filter.PageSize,
                PageIndex = filter.PageIndex,
                UserName = filter.UserName,
                Message = filter.Message,
                Status = filter.Status,
                RangeFrom = filter.RangeFrom,
                RangeTo = filter.RangeTo,
            });
            return new PagerResult<AuditLog>
            {
                Results = data.Results,
                TotalCount = data.TotalCount
            };
        }

        public async Task<IEnumerable<AuditLog>> GetForExport(AuditLogFilter filter)
        {
            var data = await _auditLogDAL.GetForExport(new AuditLogFilter
            {
                PageSize = filter.PageSize,
                PageIndex = filter.PageIndex,
                UserName = filter.UserName,
                Message = filter.Message,
                Status = filter.Status,
                RangeFrom = filter.RangeFrom,
                RangeTo = filter.RangeTo,
            });
            return data;
        }
    }
}
