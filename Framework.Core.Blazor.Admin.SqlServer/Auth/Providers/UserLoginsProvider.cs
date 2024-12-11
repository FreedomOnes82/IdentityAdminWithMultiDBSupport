using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Models;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Providers;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Providers
{
    internal class UserLoginsProvider: Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserLoginsProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider) 
            :base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            //var command = "SELECT * " +
            //                       $"FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] " +
            //                       "WHERE UserId = @UserId;";

            var repository = GetRepository<AspNetUserLogin>();
            var res = await repository.GetAllAsync();
            return res.Where(x => x.UserId == user.Id).Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
            //var command = "SELECT * " +
            //      $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetUserLogins " +
            //      "WHERE UserId = @UserId;";

            //await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            //return (
            //        await sqlConnection.QueryAsync<UserLogin>(command, new { UserId = user.Id })
            //    )
            //    .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
            //    .ToList();
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            ////string[] command =
            ////{
            ////    "SELECT UserId " +
            ////    $"FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] " +
            ////    "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;"
            ////};
            //string[] command =
            //{
            //    "SELECT UserId " +
            //      $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetUserLogins " +
            //      "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;"
            //};

            //await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            //var userId = await sqlConnection.QuerySingleOrDefaultAsync<Guid?>(command[0], new
            //{
            //    LoginProvider = loginProvider,
            //    ProviderKey = providerKey
            //});

            //if (userId == null)
            //{
            //    return null;
            //}

            ////command[0] = "SELECT * " +
            ////             $"FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
            ////             "WHERE Id = @Id;";
            //command[0] = "SELECT * " +
            //      $"FROM `{_databaseConnectionFactory.DbSchema}`.AspNetUsers " +
            //      "WHERE Id = @Id;";

            //return await sqlConnection.QuerySingleAsync<ApplicationUser>(command[0], new { Id = userId });
            var repository = GetRepository<AspNetUserLogin>();
            var res = await repository.GetAllAsync();
            var userId = res.FirstOrDefault(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey)?.UserId;
            if (userId == null)
            {
                return null;
            }
            var userRepository = GetRepository<ApplicationUser>();
            return await userRepository.GetByKeyAsync(new ApplicationUser { Id = userId });
        }
    }
}
