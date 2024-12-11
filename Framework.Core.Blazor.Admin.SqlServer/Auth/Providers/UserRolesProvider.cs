using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Models;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Providers;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Providers
{
    internal class UserRolesProvider : Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IEnumerable<UserRole>> GetRolesAsync(ApplicationUser user)
        {
            ////var command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
            ////                       $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles AS r " +
            ////                       $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] AS ur ON ur.RoleId = r.Id " +
            ////                       "WHERE ur.UserId = @UserId;";

            //var command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
            //  $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetRoles AS r " +
            //  $"INNER JOIN `{_databaseConnectionFactory.DbSchema}`.AspNetUserRoles AS ur ON ur.RoleId = r.Id " +
            //  "WHERE ur.UserId = @UserId;";
            var roleRepository = GetRepository<ApplicationRole>();
            var roles = await roleRepository.GetAllAsync();
            var urRepository = GetRepository<AspNetUserRole>();
            var urs = await urRepository.GetAllAsync();
            var userRoles = from role in roles
                            join ur in urs on role.Id equals ur.RoleId
                            where ur.UserId == user.Id
                            select new UserRole { RoleId = role.Id, RoleName = role.Name };
            return userRoles;
            //var command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
            //  $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetRoles AS r " +
            //  $"INNER JOIN `{_databaseConnectionFactory.DbSchema}`.AspNetUserRoles AS ur ON ur.RoleId = r.Id " +
            //  "WHERE ur.UserId = @UserId;";

            //await using var sqlConnection = _dbConnectionAccessor.CreateConnection();
            //return await sqlConnection.QueryAsync<UserRole>(command, new
            //{
            //    UserId = user.Id
            //});
        }
    }

}
