using Framework.Core.Abstractions;
using System.Data.Common;

namespace Blazor.Admin.Web
{
    public class MySqlDataAccessor : IDbConnectionAccessor
    {
        private IConfiguration? _configuration = default(IConfiguration);

        public MySqlDataAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        private MySqlDataAccessor(string connStr)
        {
            _connStr = connStr;
        }

        private string _connStr = string.Empty;
        public static IDbConnectionAccessor CreateDataAccessor(string connectionString)
        {
            return new MySqlDataAccessor(connectionString);
        }

        public DbConnection CreateConnection()
        {
            var dbConn = new MySql.Data.MySqlClient.MySqlConnection(string.IsNullOrEmpty(_connStr) ? _configuration.GetConnectionString("DefaultConnection") : _connStr);
            //dbConn.Open();
            return dbConn;
        }
    }
}
