using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Models;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Providers;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;
using Microsoft.Graph;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Providers
{
    internal class RoleClaimsProvider:Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RoleClaimsProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<Claim>> GetClaimsAsync(string roleId)
        {
            var repository = GetRepository<AspNetRoleClaim>();
            var res = await repository.GetAllAsync();
            res = res.Where(x => x.RoleId == roleId);
            return res.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
            //var command = "SELECT ClaimType, ClaimValue " +
            //       "FROM AspNetRoleClaims " +
            //       "WHERE RoleId = @RoleId;";

            //IEnumerable<RoleClaim> roleClaims = new List<RoleClaim>();

            //await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            //return (
            //        await sqlConnection.QueryAsync<RoleClaim>(command, new { RoleId = roleId })
            //    )
            //    .Select(x => new Claim(x.ClaimType, x.ClaimValue))
            //    .ToList();
        }
    }
}
