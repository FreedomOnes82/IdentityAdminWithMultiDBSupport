using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.MySql.Auth.Models;
using Framework.Core.Blazor.Admin.MySql.Auth.Providers;
using Framework.Core.Blazor.Admin.MySql.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.Blazor.Admin.MySql;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Providers
{
    internal class UserClaimsProvider:Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserClaimsProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user) {
            var repository = GetRepository<AspNetUserClaim>();
            var res = await repository.GetAllAsync();
            res = res.Where(x => x.UserId == user.Id);
            return res.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
            //var command = "SELECT ClaimType,ClaimValue" +
            //                      "FROM AspNetUserClaims " +
            //                      "WHERE UserId = @UserId;";

            //await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            //return (
            //        await sqlConnection.QueryAsync<UserClaim>(command, new { UserId = user.Id })
            //    )
            //    .Select(e => new Claim(e.ClaimType, e.ClaimValue))
            //    .ToList();
        }
    }
}
