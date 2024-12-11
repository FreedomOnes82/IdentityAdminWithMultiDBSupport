using Framework.Core.Abstractions;
using Framework.Core.Services;

namespace Framework.Core.Blazor.Admin.SqlServer.Logs.Services
{
    public class MyServiceBase : ServiceBase
    {
        public MyServiceBase(IDbConnectionAccessor dbConnectionAccessor,IDataTablePrefixProvider dataTablePrefixProvider) : base(dbConnectionAccessor, dataTablePrefixProvider,null)
        {
        }
    }
}
