using Framework.Core.Abstractions;
using Framework.Core.DataLayer;
using Framework.Core.Blazor.Admin.MySql.Logs.DataModels;
using Framework.Core.Blazor.Admin.MySql.Logs.IDataLayer;

namespace Framework.Core.Blazor.Admin.MySql.Logs.DataLayer
{
    public class AuditLogDAL : DALBase, IAuditLogDAL
    {
        public AuditLogDAL(IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider) : base(connectionAccessor, dataTablePrefixProvider)
        { 
        }

        public async Task<AuditLog> Create(AuditLog auditLog)
        {
            var res = GetRepository<AuditLog>();
            return await res.InsertAndReturnAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAll()
        {
            var res = GetRepository<AuditLog>();
            return await res.GetAllAsync();
        }

        public async Task<PagerResult<AuditLog>> SearchAsync(AuditLogFilter filter)
        {
            var repository = GetRepository<AuditLog>();
            var whereCmd = "1=1 ";
            if (string.IsNullOrEmpty(filter.UserName) == false)
                whereCmd += "AND UserName = @UserName ";
            if (filter.Status != 2)
                whereCmd += "AND Status=@Status ";
            if (string.IsNullOrEmpty(filter.Message) == false)
                whereCmd += "AND Message LIKE '%" + filter.Message + "%'";
            //if(filter.RangeFrom!=null&& filter.Status!='')
            whereCmd += "AND TimeStamp >= @RangeFrom AND TimeStamp <= @RangeTo";
            return await repository.SearchAndCountByFilterOpts(new FilterOptions
            {
                WhereClause = whereCmd,
                OrderByStatements = new List<string> { "CreatedDate DESC" },
                Pager = new PagerInfo
                {
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                }
            }, new
            {
                UserName = filter.UserName,
                Status = filter.Status,
                RangeFrom = filter.RangeFrom,
                RangeTo = filter.RangeTo
            });
        }

        public async Task<IEnumerable<AuditLog>> GetForExport(AuditLogFilter filter)
        {
            var repository = GetRepository<AuditLog>();
            var whereCmd = "1=1 ";
            if (string.IsNullOrEmpty(filter.UserName) == false)
                whereCmd += "AND UserName = @UserName ";
            if (filter.Status == 1 || filter.Status == 0)
                whereCmd += "AND Status=@Status ";
            if (string.IsNullOrEmpty(filter.Message) == false)
                whereCmd += "AND Message LIKE '%" + filter.Message + "%'";
            //if(filter.RangeFrom!=null&& filter.Status!='')
            whereCmd += "AND TimeStamp >= @RangeFrom AND TimeStamp <= @RangeTo";
            return await repository.SearchByFilterOpts(new FilterOptions
            {
                WhereClause = whereCmd,
                OrderByStatements = new List<string> { "CreatedDate DESC" },
            }, new
            {
                UserName = filter.UserName,
                Status = filter.Status,
                RangeFrom = filter.RangeFrom,
                RangeTo = filter.RangeTo
            });
        }
    }
}
