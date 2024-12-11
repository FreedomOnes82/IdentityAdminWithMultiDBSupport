using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer
{
    public interface IDatabaseConnectionFactory
    {
        Task<SqlConnection> CreateConnectionAsync();
        string DbSchema { get; set; }
    }
}
