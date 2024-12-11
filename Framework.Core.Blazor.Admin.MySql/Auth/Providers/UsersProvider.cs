using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.MySql.Auth.Models;
using Framework.Core.Blazor.Admin.MySql.Auth.Providers;
using Framework.Core.Blazor.Admin.MySql.Auth.Stores;
using Dapper;
using Framework.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;
using Framework.Core.Blazor.Admin.MySql;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Providers
{
    internal class UsersProvider : Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UsersProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            int rowsInserted = 0; ;
            var repository = GetRepository<ApplicationUser>();
            user.Id = Guid.NewGuid().ToString();
            await repository.InsertAsync(user);
            if (!string.IsNullOrEmpty(user.UserName))
            {
                var res = await FindByNameAsync(user.NormalizedUserName);
                rowsInserted = res != null ? 1 : 0;
            }
            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = nameof(CreateAsync),
                Description = $"User with email {user.Email} could not be inserted."
            });
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var repository = GetRepository<ApplicationUser>();
            await repository.DeleteAsync(user);
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(Guid userId)
        {
            var repository = GetRepository<ApplicationUser>();
            var res = await repository.GetByKeyAsync(new ApplicationUser { Id = userId.ToString() });
            return res;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            var repository = GetRepository<ApplicationUser>();
            var res = await repository.GetAllAsync();
            return res.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail)
        {
            var repository = GetRepository<ApplicationUser>();
            var res = await repository.GetAllAsync();
            return res.FirstOrDefault(x => x.NormalizedEmail == normalizedEmail);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            var isValidGuid = Guid.TryParse(user.Id, out var userGuid);
            if (!isValidGuid)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(CreateAsync),
                    Description = $"The user ID with email {user.Email} ID is incorrect."
                });
            }
            var repository = GetRepository<ApplicationUser>();

            await repository.UpdateAsync(user);

            var UcRepository = GetRepository<AspNetUserClaim>();
            var claims = (await UcRepository.GetAllAsync()).Where(x => x.UserId == user.Id);
            foreach (var claim in claims)
            {
                await UcRepository.DeleteAsync(claim);
            }
            if (user.Claims != null && user.Claims?.Count > 0)
            {
                foreach (var claim in user.Claims)
                    await UcRepository.InsertAsync(new AspNetUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            }
            var UlRepository = GetRepository<AspNetUserLogin>();
            var logins = (await UlRepository.GetAllAsync()).Where(x => x.UserId == user.Id);
            foreach (var login in logins)
            {
                await UlRepository.DeleteAsync(login);
            }
            if (user.Logins != null && user.Logins.Count > 0)
            {
                foreach (var login in user.Logins)
                    await UlRepository.InsertAsync(new AspNetUserLogin
                    {
                        UserId = user.Id,
                        LoginProvider = login.LoginProvider,
                        ProviderDisplayName = login.ProviderDisplayName,
                        ProviderKey = login.ProviderKey
                    });
            }
            var UrRepository = GetRepository<AspNetUserRole>();
            var roles = (await UrRepository.GetAllAsync()).Where(x => x.UserId == user.Id);
            foreach (var role in roles)
            {
                await UrRepository.DeleteAsync(role);
            }
            if (user.Roles != null && user.Roles.Count > 0)
            {
                foreach (var role in user.Roles)
                    await UrRepository.InsertAsync(new AspNetUserRole
                    {
                        RoleId = role.RoleId,
                        UserId = user.Id
                    });
            }
            var UtRepository = GetRepository<AspNetUserToken>();
            var tokens = (await UtRepository.GetAllAsync()).Where(x => x.UserId == user.Id);
            foreach (var token in tokens)
            {
                await UtRepository.DeleteAsync(token);
            }
            if (user.Tokens != null && user.Tokens.Count > 0)
            {
                foreach (var token in user.Tokens)
                    await UtRepository.InsertAsync(new AspNetUserToken
                    {
                        LoginProvider = token.LoginProvider,
                        Name = token.Name,
                        UserId = user.Id,
                        Value = token.Value
                    });
            }
            return IdentityResult.Success;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
            var userRepository = GetRepository<ApplicationUser>();
            var users = await userRepository.GetAllAsync();
            var roleRepository = GetRepository<ApplicationRole>();
            var roles = await roleRepository.GetAllAsync();
            var urRepository = GetRepository<AspNetUserRole>();
            var urs = await urRepository.GetAllAsync();
            var query = from u in users
                        join ur in urs on u.Id equals ur.UserId
                        join r in roles on ur.RoleId equals r.Id
                        where r.NormalizedName == roleName
                        select u;
            return query.ToList();
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim)
        {
            var userRepository = GetRepository<ApplicationUser>();
            var users = await userRepository.GetAllAsync();
            var claimRepository = GetRepository<AspNetUserClaim>();
            var claims = await claimRepository.GetAllAsync();
            var query = from u in users
                        join uc in claims on u.Id equals uc.UserId
                        where uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value
                        select u;
            return query.ToList();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var userRepository = GetRepository<ApplicationUser>();
            var users = await userRepository.GetAllAsync();
            return users;
        }
    }
}
