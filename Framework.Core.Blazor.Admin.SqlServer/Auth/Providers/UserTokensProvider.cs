using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Models;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Providers;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Providers
{
    internal class UserTokensProvider:Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserTokensProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IEnumerable<UserToken>> GetTokensAsync(string userId) {
            var repository = GetRepository<AspNetUserToken>();
            var res = await repository.GetAllAsync();
            res = res.Where(x => x.UserId == userId);
            return res.Select(x => new UserToken { UserId = x.UserId });
            ////var command = "SELECT * " +
            ////                       $"FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserTokens] " +
            ////                       "WHERE UserId = @UserId;";
            //var command = "SELECT * " +
            //  $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetUserTokens " +
            //  "WHERE UserId = @UserId;";

            //await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            //return await sqlConnection.QueryAsync<UserToken>(command, new {
            //    UserId = userId
            //});
        }
    }
}
